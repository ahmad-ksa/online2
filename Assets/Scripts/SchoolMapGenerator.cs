using UnityEngine;

public class SchoolMapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 20;
    public int mapHeight = 20;
    public float cellSize = 2f;
    
    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject classroomPrefab;
    public GameObject hallwayPrefab;
    public GameObject doorPrefab;
    public GameObject lockerPrefab;
    
    private int[,] mapLayout;
    
    void Start()
    {
        GenerateSchoolMap();
    }
    
    void GenerateSchoolMap()
    {
        // Create floor
        CreateFloor();
        
        // Create walls
        CreateWalls();
        
        // Create classrooms
        CreateClassrooms();
        
        // Create hallways
        CreateHallways();
        
        // Create lockers
        CreateLockers();
    }
    
    void CreateFloor()
    {
        GameObject floor = Instantiate(floorPrefab, transform);
        floor.transform.position = new Vector3(mapWidth * cellSize / 2, 0, mapHeight * cellSize / 2);
        floor.transform.localScale = new Vector3(mapWidth * cellSize / 10, 1, mapHeight * cellSize / 10);
    }
    
    void CreateWalls()
    {
        // Outer walls
        CreateWall(new Vector3(0, 1.5f, 0), new Vector3(mapWidth * cellSize, 3, 0.2f)); // Bottom
        CreateWall(new Vector3(0, 1.5f, mapHeight * cellSize), new Vector3(mapWidth * cellSize, 3, 0.2f)); // Top
        CreateWall(new Vector3(0, 1.5f, 0), new Vector3(0.2f, 3, mapHeight * cellSize)); // Left
        CreateWall(new Vector3(mapWidth * cellSize, 1.5f, 0), new Vector3(0.2f, 3, mapHeight * cellSize)); // Right
        
        // Inner walls (classrooms)
        CreateWall(new Vector3(mapWidth * cellSize * 0.3f, 1.5f, 0), new Vector3(0.2f, 3, mapHeight * cellSize * 0.4f)); // Vertical 1
        CreateWall(new Vector3(mapWidth * cellSize * 0.6f, 1.5f, 0), new Vector3(0.2f, 3, mapHeight * cellSize * 0.4f)); // Vertical 2
        CreateWall(new Vector3(0, 1.5f, mapHeight * cellSize * 0.4f), new Vector3(mapWidth * cellSize * 0.3f, 3, 0.2f)); // Horizontal 1
        CreateWall(new Vector3(mapWidth * cellSize * 0.3f, 1.5f, mapHeight * cellSize * 0.6f), new Vector3(mapWidth * cellSize * 0.3f, 3, 0.2f)); // Horizontal 2
    }
    
    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, transform);
        wall.transform.localScale = scale;
    }
    
    void CreateClassrooms()
    {
        // Classroom 1 (Top Left)
        CreateClassroom(new Vector3(mapWidth * cellSize * 0.15f, 0, mapHeight * cellSize * 0.2f), "Math Class");
        
        // Classroom 2 (Top Right)
        CreateClassroom(new Vector3(mapWidth * cellSize * 0.45f, 0, mapHeight * cellSize * 0.2f), "Science Class");
        
        // Classroom 3 (Bottom Left)
        CreateClassroom(new Vector3(mapWidth * cellSize * 0.15f, 0, mapHeight * cellSize * 0.7f), "English Class");
        
        // Classroom 4 (Bottom Right)
        CreateClassroom(new Vector3(mapWidth * cellSize * 0.45f, 0, mapHeight * cellSize * 0.7f), "History Class");
    }
    
    void CreateClassroom(Vector3 position, string name)
    {
        GameObject classroom = Instantiate(classroomPrefab, position, Quaternion.identity, transform);
        classroom.name = name;
        
        // Add desks
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector3 deskPos = position + new Vector3(i * 2f - 3f, 0.5f, j * 2f - 2f);
                CreateDesk(deskPos);
            }
        }
        
        // Add teacher desk
        CreateDesk(position + new Vector3(0, 0.5f, -4f));
    }
    
    void CreateDesk(Vector3 position)
    {
        GameObject desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        desk.transform.position = position;
        desk.transform.localScale = new Vector3(1.2f, 0.5f, 0.6f);
        desk.transform.SetParent(transform);
    }
    
    void CreateHallways()
    {
        // Hallway floor
        GameObject hallway = Instantiate(hallwayPrefab, transform);
        hallway.transform.position = new Vector3(mapWidth * cellSize / 2, 0.01f, mapHeight * cellSize / 2);
        hallway.transform.localScale = new Vector3(mapWidth * cellSize * 0.1f, 0.02f, mapHeight * cellSize);
    }
    
    void CreateLockers()
    {
        // Lockers along hallway
        for (int i = 0; i < 5; i++)
        {
            Vector3 lockerPos = new Vector3(mapWidth * cellSize * 0.5f, 0.75f, i * 4f + 2f);
            CreateLocker(lockerPos);
        }
    }
    
    void CreateLocker(Vector3 position)
    {
        GameObject locker = Instantiate(lockerPrefab, position, Quaternion.identity, transform);
        locker.transform.localScale = new Vector3(0.5f, 1.5f, 0.3f);
    }
}