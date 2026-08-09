using UnityEngine;
using UnityEngine.UI;

public class GameAutoSetup : MonoBehaviour
{
    [Header("Auto Setup Settings")]
    public bool setupOnStart = true;
    public int mapWidth = 20;
    public int mapHeight = 20;
    public float cellSize = 2f;
    
    [Header("Materials")]
    public Material floorMaterial;
    public Material wallMaterial;
    public Material classroomMaterial;
    
    [Header("UI Settings")]
    public Color joystickColor = Color.white;
    public Color buttonColor = Color.yellow;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupEverything();
        }
    }
    
    [ContextMenu("Setup Everything")]
    public void SetupEverything()
    {
        ClearEverything();
        
        // Create map
        CreateMap();
        
        // Create player
        CreatePlayer();
        
        // Create camera
        CreateCamera();
        
        // Create UI
        CreateUI();
        
        Debug.Log("✅ Game setup complete!");
    }
    
    [ContextMenu("Clear Everything")]
    public void ClearEverything()
    {
        // Destroy all children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        
        Debug.Log("🗑️ Everything cleared!");
    }
    
    void CreateMap()
    {
        GameObject mapParent = new GameObject("SchoolMap");
        mapParent.transform.SetParent(transform);
        
        // Create floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.SetParent(mapParent.transform);
        floor.transform.position = new Vector3(mapWidth * cellSize / 2, 0, mapHeight * cellSize / 2);
        floor.transform.localScale = new Vector3(mapWidth * cellSize / 10, 1, mapHeight * cellSize / 10);
        
        if (floorMaterial != null)
        {
            floor.GetComponent<Renderer>().material = floorMaterial;
        }
        
        // Create walls
        CreateWall(mapParent, new Vector3(0, 1.5f, 0), new Vector3(mapWidth * cellSize, 3, 0.2f), "Wall_Bottom");
        CreateWall(mapParent, new Vector3(0, 1.5f, mapHeight * cellSize), new Vector3(mapWidth * cellSize, 3, 0.2f), "Wall_Top");
        CreateWall(mapParent, new Vector3(0, 1.5f, 0), new Vector3(0.2f, 3, mapHeight * cellSize), "Wall_Left");
        CreateWall(mapParent, new Vector3(mapWidth * cellSize, 1.5f, 0), new Vector3(0.2f, 3, mapHeight * cellSize), "Wall_Right");
        
        // Create inner walls (classrooms)
        CreateWall(mapParent, new Vector3(mapWidth * cellSize * 0.3f, 1.5f, 0), new Vector3(0.2f, 3, mapHeight * cellSize * 0.4f), "Wall_Inner1");
        CreateWall(mapParent, new Vector3(mapWidth * cellSize * 0.6f, 1.5f, 0), new Vector3(0.2f, 3, mapHeight * cellSize * 0.4f), "Wall_Inner2");
        CreateWall(mapParent, new Vector3(0, 1.5f, mapHeight * cellSize * 0.4f), new Vector3(mapWidth * cellSize * 0.3f, 3, 0.2f), "Wall_Inner3");
        CreateWall(mapParent, new Vector3(mapWidth * cellSize * 0.3f, 1.5f, mapHeight * cellSize * 0.6f), new Vector3(mapWidth * cellSize * 0.3f, 3, 0.2f), "Wall_Inner4");
        
        // Create classrooms
        CreateClassroom(mapParent, new Vector3(mapWidth * cellSize * 0.15f, 0, mapHeight * cellSize * 0.2f), "Math Class");
        CreateClassroom(mapParent, new Vector3(mapWidth * cellSize * 0.45f, 0, mapHeight * cellSize * 0.2f), "Science Class");
        CreateClassroom(mapParent, new Vector3(mapWidth * cellSize * 0.15f, 0, mapHeight * cellSize * 0.7f), "English Class");
        CreateClassroom(mapParent, new Vector3(mapWidth * cellSize * 0.45f, 0, mapHeight * cellSize * 0.7f), "History Class");
        
        // Create lockers
        for (int i = 0; i < 5; i++)
        {
            Vector3 lockerPos = new Vector3(mapWidth * cellSize * 0.5f, 0.75f, i * 4f + 2f);
            CreateLocker(mapParent, lockerPos, "Locker_" + i);
        }
    }
    
    void CreateWall(GameObject parent, Vector3 position, Vector3 scale, string name)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent.transform);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        
        if (wallMaterial != null)
        {
            wall.GetComponent<Renderer>().material = wallMaterial;
        }
    }
    
    void CreateClassroom(GameObject parent, Vector3 position, string name)
    {
        GameObject classroom = new GameObject(name);
        classroom.transform.SetParent(parent.transform);
        classroom.transform.position = position;
        
        // Add desks
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector3 deskPos = position + new Vector3(i * 2f - 3f, 0.5f, j * 2f - 2f);
                CreateDesk(classroom, deskPos, "Desk_" + i + "_" + j);
            }
        }
        
        // Add teacher desk
        CreateDesk(classroom, position + new Vector3(0, 0.5f, -4f), "TeacherDesk");
    }
    
    void CreateDesk(GameObject parent, Vector3 position, string name)
    {
        GameObject desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        desk.name = name;
        desk.transform.SetParent(parent.transform);
        desk.transform.position = position;
        desk.transform.localScale = new Vector3(1.2f, 0.5f, 0.6f);
        
        if (classroomMaterial != null)
        {
            desk.GetComponent<Renderer>().material = classroomMaterial;
        }
    }
    
    void CreateLocker(GameObject parent, Vector3 position, string name)
    {
        GameObject locker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        locker.name = name;
        locker.transform.SetParent(parent.transform);
        locker.transform.position = position;
        locker.transform.localScale = new Vector3(0.5f, 1.5f, 0.3f);
        
        if (classroomMaterial != null)
        {
            locker.GetComponent<Renderer>().material = classroomMaterial;
        }
    }
    
    void CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.SetParent(transform);
        player.transform.position = new Vector3(mapWidth * cellSize / 2, 1, mapHeight * cellSize / 2);
        
        // Add CharacterController
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.5f;
        
        // Add PlayerController
        PlayerController playerController = player.AddComponent<PlayerController>();
        
        // Add tag
        player.tag = "Player";
        
        Debug.Log("✅ Player created!");
    }
    
    void CreateCamera()
    {
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.transform.SetParent(transform);
        cameraObj.transform.position = new Vector3(mapWidth * cellSize / 2, 10, -5);
        cameraObj.transform.rotation = Quaternion.Euler(30, 0, 0);
        
        Camera camera = cameraObj.AddComponent<Camera>();
        camera.tag = "MainCamera";
        
        // Add AudioListener
        cameraObj.AddComponent<AudioListener>();
        
        Debug.Log("✅ Camera created!");
    }
    
    void CreateUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(transform);
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem if not exists
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create MobileUI
        GameObject mobileUIObj = new GameObject("MobileUI");
        mobileUIObj.transform.SetParent(canvasObj.transform);
        MobileUI mobileUI = mobileUIObj.AddComponent<MobileUI>();
        
        // Create GameUI
        GameObject gameUIObj = new GameObject("GameUI");
        gameUIObj.transform.SetParent(canvasObj.transform);
        GameUI gameUI = gameUIObj.AddComponent<GameUI>();
        
        // Create Joystick
        GameObject joystickObj = new GameObject("Joystick");
        joystickObj.transform.SetParent(canvasObj.transform);
        
        RectTransform joystickRect = joystickObj.AddComponent<RectTransform>();
        joystickRect.anchoredPosition = new Vector2(200, 200);
        joystickRect.sizeDelta = new Vector2(150, 150);
        
        Image joystickImage = joystickObj.AddComponent<Image>();
        joystickImage.color = joystickColor;
        
        SimpleJoystick joystick = joystickObj.AddComponent<SimpleJoystick>();
        
        // Create Joystick Handle
        GameObject handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(joystickObj.transform);
        
        RectTransform handleRect = handleObj.AddComponent<RectTransform>();
        handleRect.anchoredPosition = Vector2.zero;
        handleRect.sizeDelta = new Vector2(50, 50);
        
        Image handleImage = handleObj.AddComponent<Image>();
        handleImage.color = Color.red;
        
        joystick.handle = handleRect;
        joystick.background = joystickRect;
        
        // Create Buttons
        CreateButton(canvasObj, "HideButton", new Vector2(-200, 200), "Hide", buttonColor);
        CreateButton(canvasObj, "CatchButton", new Vector2(-200, 300), "Catch", buttonColor);
        CreateButton(canvasObj, "JumpButton", new Vector2(-200, 400), "Jump", buttonColor);
        
        // Link UI to Player
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.joystick = joystick;
        }
        
        Debug.Log("✅ UI created!");
    }
    
    void CreateButton(GameObject parent, string name, Vector2 position, string text, Color color)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform);
        
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(120, 50);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = color;
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = image;
        
        // Create Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(100, 30);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 18;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.black;
    }
}