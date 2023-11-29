using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PolyhiveSettings : EditorWindow
{
    string apiKey = "";

    [MenuItem("Window/Polyhive/Polyhive Settings")]
    public static void ShowWindow()
    {
        PolyhiveSettings settings = (PolyhiveSettings)EditorWindow.GetWindow(typeof(PolyhiveSettings), false, "Polyhive Settings");
        if (EditorPrefs.HasKey("PolyhiveApiKey"))
        {
            settings.apiKey = EditorPrefs.GetString("PolyhiveApiKey");
        }
        else
        {
            settings.apiKey = "";
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Polyhive Settings", EditorStyles.boldLabel);

        apiKey = EditorGUILayout.TextField("API Key", apiKey);

        if(GUILayout.Button("Save"))
        {
            SaveApiKey();
        }
    }

    private void SaveApiKey()
    {
        EditorPrefs.SetString("PolyhiveApiKey", apiKey);
        Debug.Log("Saved Polyhive API Key");
    }

    
    public static string GetEndpoint()
    {
        // if (EditorPrefs.HasKey("PolyhiveIsDev"))
        // {
        //     bool isDev = EditorPrefs.GetBool("PolyhiveIsDev");
        //     if (isDev)
        //     {
        //         return "http://localhost:8000/api/";
        //     }
        // }
        return "https://prod.polyhive.ai/api/";
    }
}
