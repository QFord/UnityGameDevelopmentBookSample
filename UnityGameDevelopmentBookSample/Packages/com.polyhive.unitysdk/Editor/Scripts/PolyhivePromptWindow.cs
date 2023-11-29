using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using GLTFast;
using GLTFast.Export;
using System.IO;
using System;

[Serializable]
public class TextureJobParams
{
    public int seed;
    public string negative_prompt;
    public bool material_maps;
    public bool preserve_uvs;
    public bool zy_symmetry;
    // TODO: REMOVE BEFORE PUBLISHING
    // public string owner;
}

[Serializable]
public class TextureJob
{
    public string job_name;
    public string version_id;
    public string text_prompt;
    public TextureJobParams job_params;
}

public enum FileUploadStatusCode 
{
    SUCCESS = 200,
    EXCEEDS_FILE_SIZE_LIMIT = 410,
    EXCEEDS_STORAGE_LIMIT = 411,
    INVALID_FILE_TYPE = 412,
    EXCEEDS_FACE_COUNT_LIMIT = 413,
    EXCEEDS_SUBMESH_COUNT_LIMIT = 414,
    INTERNAL_SERVER_ERROR = 500,
}

public class PolyhivePromptWindow : EditorWindow 
{
    string jobName = "";
    string textPrompt = "";
    string negativePrompt = "";
    int seed = -1;
    bool preserveUVs = false;
    bool generateMaterialMaps = true;
    bool zySymmetry = false;
    GameObject selectedGameObject;

    const int MAX_SEED_VALUE = Int32.MaxValue;

    [MenuItem("Window/Polyhive/Polyhive Prompt Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PolyhivePromptWindow), false, "Polyhive Prompt Window");
    }

    private bool ValidateInput()
    {
        if (jobName == null || jobName.Length == 0)
        {
            Debug.LogError("Job name is required");
            return false;
        } else if (selectedGameObject == null)
        {
            Debug.LogError("A game object must be selected");
            return false;
        } else if (textPrompt == null || textPrompt.Length == 0)
        {
            Debug.LogError("Text prompt is required");
            return false;
        }

        return true;
    }

    async private void OnGUI()
    {
        GUILayout.Label("Polyhive", EditorStyles.boldLabel);

        jobName = EditorGUILayout.TextField("Job Name", jobName);
        textPrompt = EditorGUILayout.TextField("Prompt", textPrompt);
        negativePrompt = EditorGUILayout.TextField("Negative Prompt", negativePrompt);
        seed = EditorGUILayout.IntField("Seed", seed);
        preserveUVs = EditorGUILayout.Toggle("Preserve UVs", preserveUVs);
        generateMaterialMaps = EditorGUILayout.Toggle("Generate Material Maps", generateMaterialMaps);
        zySymmetry = EditorGUILayout.Toggle("ZY Symmetry", zySymmetry);
        selectedGameObject = EditorGUILayout.ObjectField("Select a Game Object", selectedGameObject, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Submit Texture Job"))
        {
            if (!EditorPrefs.HasKey("PolyhiveApiKey"))
            {
                Debug.LogError("Polyhive API Key has not been set! Please enter your API key in the Polyhive Settings window (Window/Polyhive/Polyhive Settings");
                return;
            }
            string apiKey = EditorPrefs.GetString("PolyhiveApiKey");

            if (!ValidateInput())
            {
                return;
            }

            var exportSettings = new ExportSettings
            {
                Format = GltfFormat.Binary,
                FileConflictResolution = FileConflictResolution.Overwrite,
                ComponentMask = ~(ComponentType.Camera | ComponentType.Animation | ComponentType.Light),
            };

            var gameObjectExportSettings = new GameObjectExportSettings
            {
                OnlyActiveInHierarchy = false,
                DisabledComponents = true,
            };

            GameObjectExport exporter = new GameObjectExport(exportSettings, gameObjectExportSettings);

            GameObject[] gameObjects = new GameObject[1];
            gameObjects[0] = selectedGameObject;
            exporter.AddScene(gameObjects);

            System.IO.Stream stream = new System.IO.MemoryStream();
            bool success = await exporter.SaveToStreamAndDispose(stream);

            if(!success)
            { 
                Debug.LogError("Failed to export");
            } else
            {
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, (int)stream.Length);

                string endPoint = PolyhiveSettings.GetEndpoint();
                UnityWebRequest request = new UnityWebRequest(endPoint + "content/upload", "POST");
                request.uploadHandler = new UploadHandlerRaw(data);
                request.downloadHandler = new DownloadHandlerBuffer();

                string fileName = selectedGameObject.name + ".glb";
                request.SetRequestHeader("x-api-key", apiKey);
                request.SetRequestHeader("file-name", fileName);
                request.SetRequestHeader("file-size", stream.Length.ToString());

                request.SendWebRequest().completed += OnUploadRequestCompleted;
            }

            stream.Dispose();
        }
    }

    private void OnUploadRequestCompleted(AsyncOperation operation)
    {
        UnityWebRequest request = ((UnityWebRequestAsyncOperation)operation).webRequest;
        request.uploadHandler.Dispose();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var downloadHandler = request.downloadHandler;
            if(downloadHandler != null)
            {
                if (!EditorPrefs.HasKey("PolyhiveApiKey"))
                {
                    Debug.LogError("Polyhive API Key has not been set! Please enter your API key in the Polyhive Settings window (Window/Polyhive/Polyhive Settings");
                    return;
                }
                string apiKey = EditorPrefs.GetString("PolyhiveApiKey");

                string assetId = downloadHandler.text;

                string endPoint = PolyhiveSettings.GetEndpoint();
                UnityWebRequest textureRequest = new UnityWebRequest(endPoint + "content/retexture", "POST");
                textureRequest.SetRequestHeader("Content-Type", "application/json");
                textureRequest.SetRequestHeader("x-api-key", apiKey);
                textureRequest.downloadHandler = new DownloadHandlerBuffer();

                int seedValue = seed;
                if (seedValue == -1)
                {
                    seedValue = UnityEngine.Random.Range(0, MAX_SEED_VALUE);
                }
                else if (seedValue < -1)
                {
                    seedValue *= -1;
                }

                TextureJob textureJob = new TextureJob();
                textureJob.job_name = jobName;
                textureJob.version_id = assetId;
                textureJob.text_prompt = textPrompt;
                TextureJobParams textureJobParams = new TextureJobParams();
                textureJobParams.seed = seedValue;
                textureJobParams.negative_prompt = negativePrompt;
                textureJobParams.material_maps = generateMaterialMaps;
                textureJobParams.preserve_uvs = preserveUVs;
                textureJobParams.zy_symmetry = zySymmetry;
                // textureJobParams.owner = "";
                // if (EditorPrefs.HasKey("PolyhiveOwnerFlag"))
                // {
                //     textureJobParams.owner = EditorPrefs.GetString("PolyhiveOwnerFlag");
                // }
                textureJob.job_params = textureJobParams;

                string json = JsonUtility.ToJson(textureJob);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                textureRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);

                textureRequest.SendWebRequest().completed += OnTextureRequestCompleted;
            }
            else
            {
                Debug.LogError("Invalid upload response recieved - No download handler");
            }
        } else if(request.result == UnityWebRequest.Result.ProtocolError)
        {
            try
            {
                FileUploadStatusCode statusCode = (FileUploadStatusCode)request.responseCode;
                Debug.LogError("Failed to upload file - " + statusCode.ToString());
            } catch (Exception e)
            {
                Debug.LogError("Failed to parse response code: " + e.Message);
            }

        } else if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Failed to upload file - Connection error");
        } else
        {
            Debug.LogError("Failed to upload file - Unknown error");
        }

    }

    private void OnTextureRequestCompleted(AsyncOperation operation)
    {
        UnityWebRequest request = ((UnityWebRequestAsyncOperation)operation).webRequest;
        request.uploadHandler.Dispose();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successfully submitted Polyhive texture job request");
        } else if(request.result == UnityWebRequest.Result.ProtocolError)
        {
            if (request.responseCode == 401)
            {
                Debug.LogError("Failed to submit Polyhive texture job request - Invalid API key");
            }
            else
            {
                Debug.LogError("Failed to submit Polyhive texture job request - " + request.responseCode);
            }
        } else
        {
            Debug.LogError("Failed to submit Polyhive texture job request - Unknown error");
        }
    }
}
