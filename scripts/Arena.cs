using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {
    // Structs
    struct Coordinates {
        public GameObject content;
        public Entity contentEntity;
    };
    // Consts
    public const int ARENA_WIDTH = 13;
    public const int ARENA_HEIGHT = 12;
    public const int CELL_SIZE = 10;
    // Attributes
    private static Coordinates[,] coordinates = new Coordinates[ARENA_WIDTH, ARENA_HEIGHT];
    private static GameObject floor; 
    public static Vector3 floorSize;
    public static Vector2 initialPos;
    // Start is called before the first frame update
    void Start() {
        floor = GameObject.FindGameObjectWithTag("Floor");
        Debug.Assert(floor != null);
        
        floorSize = floor.GetComponent<MeshRenderer>().bounds.size;
        Debug.Assert(floorSize.x > 0 && floorSize.y > 0 && floorSize.z > 0);

        initialPos = new Vector2 (
            floor.transform.position.x - floorSize.x / 2,
            floor.transform.position.z - floorSize.z / 2
        );

        StartCoordinates();
    }
    // Update is called once per frame
    void Update() {
        
    }
    // Start Arena Coordinates
    void StartCoordinates() {
        for (int i = 0; i < ARENA_WIDTH; ++i) {
            for (int j = 0; j < ARENA_HEIGHT; ++i) {
                coordinates[i, j] = new Coordinates {
                    content = null,
                    contentEntity = Entity.NONE
                };
            }
        }
    }

    public static Entity GetCoordinatesEntity(Vector2Int index) {
        if (index.x < 0 || index.x >= ARENA_WIDTH ||
            index.y < 0 || index.y >= ARENA_HEIGHT) 
        {
            return Entity.HARD_BLOCK;
        }
        return coordinates[index.x, index.y].contentEntity;
    }

    public static GameObject GetCoordinatesContent(Vector2Int indexes) {
        Debug.Assert(indexes.x >= 0 && indexes.x < ARENA_WIDTH);
        Debug.Assert(indexes.y >= 0 && indexes.y < ARENA_HEIGHT);
        return coordinates[indexes.x, indexes.y].content;
    }

    public static void SetCoordinatesContent(Vector2Int indexes, GameObject gObj, Entity gObjEntity) {
        Debug.Assert(indexes.x >= 0 && indexes.x < ARENA_WIDTH);
        Debug.Assert(indexes.y >= 0 && indexes.y < ARENA_HEIGHT);
        coordinates[indexes.x, indexes.y] = new Coordinates {
            content = gObj,
            contentEntity = gObjEntity
        };
    }

    public static Vector3 GetCoordinatesPosition(Vector2Int index, int y) {
        return new Vector3 (
            initialPos.x + (CELL_SIZE / 2) * (index.x * 2 + 1),
            y,
            initialPos.y + (CELL_SIZE / 2) * (index.y * 2 + 1)
        );
    }

    public static Vector2Int GetObjectCoordinates(GameObject gObj) {
        var objPos = new Vector2 (
            gObj.transform.position.x,
            gObj.transform.position.z
        );
        Vector2 posOffset = objPos - initialPos;
        return new Vector2Int (
            Mathf.FloorToInt(Mathf.Abs(posOffset.x) / CELL_SIZE),
            Mathf.FloorToInt(Mathf.Abs(posOffset.y) / CELL_SIZE)
        );
    }
}
