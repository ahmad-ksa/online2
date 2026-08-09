using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditorWindow : EditorWindow
{
    private string mapName = "New Map";
    private Vector2 scrollPosition;
    private List<MapElement> mapElements = new List<MapElement>();
    private MapElement selectedElement;
    
    private GameObject selectedPrefab;
    private Vector3 spawnPosition = Vector3.zero;
    private Vector3 spawnRotation = Vector3.zero;
    private Vector3 spawnScale = Vector3.one;
    
    [MenuItem("Hide and Seek/Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<MapEditorWindow>("Map Editor");
    }
    
    void OnGUI()
    {
        GUILayout.Label("🏫 School Map Editor", EditorStyles.boldLabel);
        
        // Map Name
        mapName = EditorGUILayout.TextField("Map Name", mapName);
        
        EditorGUILayout.Space();
        
        // Add New Element
        GUILayout.Label("Add New Element", EditorStyles.boldLabel);
        
        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);
        spawnPosition = EditorGUILayout.Vector3Field("Position", spawnPosition);
        spawnRotation = EditorGUILayout.Vector3Field("Rotation", spawnRotation);
        spawnScale = EditorGUILayout.Vector3Field("Scale", spawnScale);
        
        if (GUILayout.Button("Add Element") && selectedPrefab != null)
        {
            AddElement();
        }
        
        EditorGUILayout.Space();
        
        // Elements List
        GUILayout.Label("Map Elements", EditorStyles.boldLabel);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        for (int i = 0; i < mapElements.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Select button
            if (GUILayout.Button(mapElements[i].name, GUILayout.Width(150)))
            {
                selectedElement = mapElements[i];
            }
            
            // Delete button
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                mapElements.RemoveAt(i);
                i--;
                continue;
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space();
        
        // Selected Element Properties
        if (selectedElement != null)
        {
            GUILayout.Label("Selected Element", EditorStyles.boldLabel);
            
            selectedElement.position = EditorGUILayout.Vector3Field("Position", selectedElement.position);
            selectedElement.rotation = EditorGUILayout.Vector3Field("Rotation", selectedElement.rotation);
            selectedElement.scale = EditorGUILayout.Vector3Field("Scale", selectedElement.scale);
        }
        
        EditorGUILayout.Space();
        
        // Save/Load
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Save Map"))
        {
            SaveMap();
        }
        
        if (GUILayout.Button("Load Map"))
        {
            LoadMap();
        }
        
        if (GUILayout.Button("Generate Map"))
        {
            GenerateMap();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    void AddElement()
    {
        MapElement element = new MapElement
        {
            name = selectedPrefab.name,
            prefab = selectedPrefab,
            position = spawnPosition,
            rotation = spawnRotation,
            scale = spawnScale
        };
        
        mapElements.Add(element);
    }
    
    void SaveMap()
    {
        string path = EditorUtility.SaveFilePanel("Save Map", "Assets/Maps", mapName + ".json", "json");
        
        if (!string.IsNullOrEmpty(path))
        {
            MapData data = new MapData
            {
                name = mapName,
                elements = mapElements
            };
            
            string json = JsonUtility.ToJson(data, true);
            System.IO.File.WriteAllText(path, json);
            
            Debug.Log("Map saved to: " + path);
        }
    }
    
    void LoadMap()
    {
        string path = EditorUtility.OpenFilePanel("Load Map", "Assets/Maps", "json");
        
        if (!string.IsNullOrEmpty(path))
        {
            string json = System.IO.File.ReadAllText(path);
            MapData data = JsonUtility.FromJson<MapData>(json);
            
            mapName = data.name;
            mapElements = data.elements;
            
            Debug.Log("Map loaded from: " + path);
        }
    }
    
    void GenerateMap()
    {
        // Clear existing
        foreach (MapElement element in mapElements)
        {
            if (element.gameObject != null)
            {
                DestroyImmediate(element.gameObject);
            }
        }
        
        // Spawn new
        foreach (MapElement element in mapElements)
        {
            if (element.prefab != null)
            {
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(element.prefab);
                obj.transform.position = element.position;
                obj.transform.rotation = Quaternion.Euler(element.rotation);
                obj.transform.localScale = element.scale;
                element.gameObject = obj;
            }
        }
        
        Debug.Log("Map generated!");
    }
}

[System.Serializable]
public class MapElement
{
    public string name;
    public GameObject prefab;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    [System.NonSerialized]
    public GameObject gameObject;
}

[System.Serializable]
public class MapData
{
    public string name;
    public List<MapElement> elements;
}