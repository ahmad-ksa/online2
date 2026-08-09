using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GameAssetManager : EditorWindow
{
    private Vector2 scrollPosition;
    private List<AssetData> assets = new List<AssetData>();
    private string searchQuery = "";
    
    [MenuItem("Hide and Seek/Asset Manager")]
    public static void ShowWindow()
    {
        GetWindow<GameAssetManager>("Asset Manager");
    }
    
    void OnGUI()
    {
        GUILayout.Label("📦 Asset Manager", EditorStyles.boldLabel);
        
        // Search
        EditorGUILayout.BeginHorizontal();
        searchQuery = EditorGUILayout.TextField("Search", searchQuery);
        if (GUILayout.Button("Search", GUILayout.Width(60)))
        {
            SearchAssets();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Asset Categories
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Maps"))
        {
            LoadAssets("Maps");
        }
        
        if (GUILayout.Button("Characters"))
        {
            LoadAssets("Characters");
        }
        
        if (GUILayout.Button("Props"))
        {
            LoadAssets("Props");
        }
        
        if (GUILayout.Button("All"))
        {
            LoadAssets("");
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Assets List
        GUILayout.Label("Assets", EditorStyles.boldLabel);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        foreach (AssetData asset in assets)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Icon
            if (asset.icon != null)
            {
                GUILayout.Label(asset.icon, GUILayout.Width(40), GUILayout.Height(40));
            }
            
            // Name
            GUILayout.Label(asset.name, GUILayout.Width(150));
            
            // Type
            GUILayout.Label(asset.type, GUILayout.Width(60));
            
            // Use
            if (GUILayout.Button("Use", GUILayout.Width(50)))
            {
                UseAsset(asset);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space();
        
        // Import
        if (GUILayout.Button("Import from Asset Store"))
        {
            Application.OpenURL("https://assetstore.unity.com/");
        }
    }
    
    void SearchAssets()
    {
        assets.Clear();
        
        // Search in project
        string[] guids = AssetDatabase.FindAssets(searchQuery);
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            
            if (obj != null)
            {
                assets.Add(new AssetData
                {
                    name = obj.name,
                    path = path,
                    type = obj.GetType().Name,
                    icon = AssetPreview.GetAssetPreview(obj)
                });
            }
        }
    }
    
    void LoadAssets(string category)
    {
        assets.Clear();
        
        string folder = string.IsNullOrEmpty(category) ? "Assets" : $"Assets/{category}";
        
        if (!AssetDatabase.IsValidFolder(folder))
        {
            Debug.LogWarning("Folder not found: " + folder);
            return;
        }
        
        string[] guids = AssetDatabase.FindAssets("", new[] { folder });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            
            if (obj != null)
            {
                assets.Add(new AssetData
                {
                    name = obj.name,
                    path = path,
                    type = obj.GetType().Name,
                    icon = AssetPreview.GetAssetPreview(obj)
                });
            }
        }
    }
    
    void UseAsset(AssetData asset)
    {
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(asset.path);
        
        if (obj is GameObject)
        {
            // Instantiate in scene
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(obj);
            instance.transform.position = Vector3.zero;
            Selection.activeGameObject = instance;
        }
        else
        {
            // Select in project
            Selection.activeObject = obj;
        }
        
        Debug.Log("Using asset: " + asset.name);
    }
}

[System.Serializable]
public class AssetData
{
    public string name;
    public string path;
    public string type;
    public Texture2D icon;
}