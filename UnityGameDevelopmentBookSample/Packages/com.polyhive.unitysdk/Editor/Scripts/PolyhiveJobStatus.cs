using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
class JobStatusList
{
    public List<JobStatus> jobs;
}

[System.Serializable]
class JobStatus
{
    public string job_id;
    public string job_name;
    public string queued_time;
    public string started_time;
    public string completed_time;
    public string job_status;
    public string thumbnail_path;
}

[System.Serializable]
class DownloadRequestMetadata
{
    public string url;
    public string file_name;
    public string extension;
}

public class PolyhiveJobStatus: EditorWindow
{
    private List<string> columnNames = new List<string>() { "Download", "Job Name", "Status", "Queued Time", "Started Time", "Completed Time" };
    private List<JobStatus>[] jobStatuses = new List<JobStatus>[2];
    private Vector2 scrollPosition = Vector2.zero;
    const int WIDTH = 180;

    private Dictionary<UnityWebRequest, string> webRequestInfo = new Dictionary<UnityWebRequest, string>();

    // TODO: make atomic
    private int curJobStatusIndex = 0;

    [MenuItem("Window/Polyhive/Polyhive Job Status")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PolyhiveJobStatus), false, "Polyhive Job Status");
    }

    private void OnEnable()
    {
        jobStatuses[0] = new List<JobStatus>();
        jobStatuses[1] = new List<JobStatus>();
    }

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("PolyhiveJobStatus", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh"))
        {
            RefreshJobStatuses();
        }


        GUILayout.BeginHorizontal();
        for (int i = 0; i < columnNames.Count; i++)
        {
            GUILayout.Label(columnNames[i], EditorStyles.boldLabel, GUILayout.Width(WIDTH));
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        for (int i = 0; i < jobStatuses[curJobStatusIndex].Count; i++)
        {
            string jobStatus = jobStatuses[curJobStatusIndex][i].job_status;

            GUILayout.BeginHorizontal();

            if(jobStatus == "completed")
            {
                // For some reason adding this button in the above if statement causes alignment to break,
                // so we add it here instead
                if(GUILayout.Button("Download", GUILayout.Width(WIDTH)))
                {
                    IssueDownloadRequest(jobStatuses[curJobStatusIndex][i].job_id);
                }
            } else
            {
                GUILayout.Label("", GUILayout.Width(WIDTH));
            }

            GUILayout.Label(jobStatuses[curJobStatusIndex][i].job_name, GUILayout.Width(WIDTH));
            GUILayout.Label(jobStatus, GUILayout.Width(WIDTH));
            GUILayout.Label(jobStatuses[curJobStatusIndex][i].queued_time, GUILayout.Width(WIDTH));
            GUILayout.Label(jobStatuses[curJobStatusIndex][i].started_time, GUILayout.Width(WIDTH));
            GUILayout.Label(jobStatuses[curJobStatusIndex][i].completed_time, GUILayout.Width(WIDTH));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.EndScrollView();
    }

    private void IssueDownloadRequest(string jobId)
    {
        if (EditorPrefs.HasKey("PolyhiveApiKey"))
        {
            string apiKey = EditorPrefs.GetString("PolyhiveApiKey");
            string endPoint = PolyhiveSettings.GetEndpoint();
            UnityWebRequest downloadMetadataRequest = UnityWebRequest.Get(endPoint + "content/retextureResult/" + jobId);

            downloadMetadataRequest.SetRequestHeader("Content-Type", "application/json");
            downloadMetadataRequest.SetRequestHeader("x-api-key", apiKey);
            downloadMetadataRequest.downloadHandler = new DownloadHandlerBuffer();
            downloadMetadataRequest.SendWebRequest().completed += OnDownloadMetadataRequestCompleted;
        }
        else
        {
            Debug.LogError("Failed to fetch job status: API key not set. Please save the API Key in Polyhive Settings (Window/Polyhive/Polyhive Settings)");
        }
    }

    private void OnDownloadMetadataRequestCompleted(AsyncOperation operation)
    {
        UnityWebRequest request = ((UnityWebRequestAsyncOperation)operation).webRequest;

        if (request.result == UnityWebRequest.Result.Success)
        {
            var downloadHandler = request.downloadHandler;
            if (downloadHandler != null)
            {
                DownloadRequestMetadata metadata = JsonUtility.FromJson<DownloadRequestMetadata>(downloadHandler.text);
                string downloadUrl = metadata.url;

                UnityWebRequest downloadMetadataRequest = UnityWebRequest.Get(downloadUrl);
                webRequestInfo[downloadMetadataRequest] = metadata.file_name + "." + metadata.extension;
                downloadMetadataRequest.SendWebRequest().completed += OnDownloadRequestCompleted;
            }
            else
            {
                Debug.LogError("Failed to fetch job status: downloadHandler is null");
            }
        }
        else if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Failed to fetch job status: " + request.error);
        }
        else
        {
            Debug.LogError("Failed to fetch job status: " + request.error);
        }
    }

    private void OnDownloadRequestCompleted(AsyncOperation operation)
    {
        UnityWebRequest request = ((UnityWebRequestAsyncOperation)operation).webRequest;

        if (request.result == UnityWebRequest.Result.Success)
        {
            string file_name = webRequestInfo[request];

            byte[] downloadedData = request.downloadHandler.data;

            string polyhiveDirectory = Path.Combine(Application.dataPath, "Polyhive");
            if(!Directory.Exists(polyhiveDirectory)) {
                Directory.CreateDirectory(polyhiveDirectory);
                Debug.Log("Created Polyhive folder in Assets directory");
            }

            int counter = 1;
            string fullPath = Path.Combine(polyhiveDirectory, file_name);
            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string fileExtension = Path.GetExtension(fullPath);

            while(File.Exists(fullPath))
            {
                string newFileName = $"{fileNameOnly}({counter++}){fileExtension}";
                fullPath = Path.Combine(polyhiveDirectory, newFileName);
            }

            File.WriteAllBytes(fullPath, downloadedData);
            AssetDatabase.Refresh();

            string downloadedFileName = Path.GetFileName(fullPath);
            Debug.Log("Successfully downloaded " + downloadedFileName);
        }
        else if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Failed to download asset: " + request.error);
        }
        else
        {
            Debug.LogError("Failed to download asset: " + request.error);
        }
    }

    private void RefreshJobStatuses()
    {
        if (EditorPrefs.HasKey("PolyhiveApiKey"))
        {
            string apiKey = EditorPrefs.GetString("PolyhiveApiKey");
            string endPoint = PolyhiveSettings.GetEndpoint();
            UnityWebRequest jobInfoRequest = UnityWebRequest.Get(endPoint + "content/fetchTextureJobs");
            
            jobInfoRequest.SetRequestHeader("Content-Type", "application/json");
            jobInfoRequest.SetRequestHeader("x-api-key", apiKey);
            jobInfoRequest.downloadHandler = new DownloadHandlerBuffer();
            jobInfoRequest.SendWebRequest().completed += OnJobStatusFetchCompleted;
        } else
        {
            Debug.LogError("Failed to fetch job status: API key not set. Please save the API Key in Polyhive Settings (Window/Polyhive/Polyhive Settings)");
        }
    }

    private void OnJobStatusFetchCompleted(AsyncOperation operation)
    {
        UnityWebRequest request = ((UnityWebRequestAsyncOperation)operation).webRequest;

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successfully fetched job status");

            var downloadHandler = request.downloadHandler;
            string json = "{\"jobs\":" + downloadHandler.text + "}";
            if (downloadHandler != null)
            {
                JobStatusList jobStatusList = JsonUtility.FromJson<JobStatusList>(json);
                jobStatuses[(curJobStatusIndex + 1) % 2] = jobStatusList.jobs;
                curJobStatusIndex = (curJobStatusIndex + 1) % 2;
            } else
            {
                Debug.LogError("Failed to fetch job status: downloadHandler is null");
            }
        }
        else if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            if (request.responseCode == 401)
            {
                Debug.LogError("Failed to fetch Polyhive texture jobs - Invalid API key");
            }
            else
            {
                Debug.LogError("Failed to fetch Polyhive texture jobs. Response code: " + request.responseCode);
            }
        }
        else
        {
            Debug.LogError("Failed to fetch Polyhive texture jobs - Unknown error");
        }
    }
}
