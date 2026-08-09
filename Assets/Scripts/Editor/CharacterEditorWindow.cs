using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CharacterEditorWindow : EditorWindow
{
    private string characterName = "New Character";
    private GameObject characterPrefab;
    private Texture2D characterIcon;
    private float moveSpeed = 5f;
    private float jumpForce = 5f;
    private bool isHider = true;
    private Color characterColor = Color.white;
    
    private Vector2 scrollPosition;
    private List<CharacterData> characters = new List<CharacterData>();
    
    [MenuItem("Hide and Seek/Character Editor")]
    public static void ShowWindow()
    {
        GetWindow<CharacterEditorWindow>("Character Editor");
    }
    
    void OnGUI()
    {
        GUILayout.Label("👤 Character Editor", EditorStyles.boldLabel);
        
        // Character Properties
        characterName = EditorGUILayout.TextField("Name", characterName);
        characterPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", characterPrefab, typeof(GameObject), false);
        characterIcon = (Texture2D)EditorGUILayout.ObjectField("Icon", characterIcon, typeof(Texture2D), false);
        
        moveSpeed = EditorGUILayout.FloatField("Move Speed", moveSpeed);
        jumpForce = EditorGUILayout.FloatField("Jump Force", jumpForce);
        isHider = EditorGUILayout.Toggle("Is Hider", isHider);
        characterColor = EditorGUILayout.ColorField("Color", characterColor);
        
        EditorGUILayout.Space();
        
        // Add/Update
        if (GUILayout.Button("Add Character"))
        {
            AddCharacter();
        }
        
        EditorGUILayout.Space();
        
        // Characters List
        GUILayout.Label("Characters", EditorStyles.boldLabel);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        for (int i = 0; i < characters.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Icon
            if (characters[i].icon != null)
            {
                GUILayout.Label(characters[i].icon, GUILayout.Width(40), GUILayout.Height(40));
            }
            
            // Name
            GUILayout.Label(characters[i].name, GUILayout.Width(100));
            
            // Role
            GUILayout.Label(characters[i].isHider ? "Hider" : "Seeker", GUILayout.Width(50));
            
            // Edit
            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                EditCharacter(i);
            }
            
            // Delete
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                characters.RemoveAt(i);
                i--;
                continue;
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space();
        
        // Save/Load
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Save Characters"))
        {
            SaveCharacters();
        }
        
        if (GUILayout.Button("Load Characters"))
        {
            LoadCharacters();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    void AddCharacter()
    {
        CharacterData character = new CharacterData
        {
            name = characterName,
            prefab = characterPrefab,
            icon = characterIcon,
            moveSpeed = moveSpeed,
            jumpForce = jumpForce,
            isHider = isHider,
            color = characterColor
        };
        
        characters.Add(character);
    }
    
    void EditCharacter(int index)
    {
        CharacterData character = characters[index];
        
        characterName = character.name;
        characterPrefab = character.prefab;
        characterIcon = character.icon;
        moveSpeed = character.moveSpeed;
        jumpForce = character.jumpForce;
        isHider = character.isHider;
        characterColor = character.color;
    }
    
    void SaveCharacters()
    {
        string path = EditorUtility.SaveFilePanel("Save Characters", "Assets/Characters", "characters.json", "json");
        
        if (!string.IsNullOrEmpty(path))
        {
            CharacterList data = new CharacterList { characters = characters };
            string json = JsonUtility.ToJson(data, true);
            System.IO.File.WriteAllText(path, json);
            
            Debug.Log("Characters saved to: " + path);
        }
    }
    
    void LoadCharacters()
    {
        string path = EditorUtility.OpenFilePanel("Load Characters", "Assets/Characters", "json");
        
        if (!string.IsNullOrEmpty(path))
        {
            string json = System.IO.File.ReadAllText(path);
            CharacterList data = JsonUtility.FromJson<CharacterList>(json);
            
            characters = data.characters;
            
            Debug.Log("Characters loaded from: " + path);
        }
    }
}

[System.Serializable]
public class CharacterData
{
    public string name;
    public GameObject prefab;
    public Texture2D icon;
    public float moveSpeed;
    public float jumpForce;
    public bool isHider;
    public Color color;
}

[System.Serializable]
public class CharacterList
{
    public List<CharacterData> characters;
}