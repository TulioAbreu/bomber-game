using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global {
    public const int ARENA_WIDTH = 13;
    public const int ARENA_HEIGHT = 12;
    public static Vector2Int ARENA_SIZE = new Vector2Int(13, 12);
    public static int CELL_SIZE = 10;


    public static GameObject floor = null;
    public static Vector3? floorSize = null;
    public static Vector2? floorInitialPos = null;

    private static void CheckStaticVars() {
        if (floor == null) {
            floor = GameObject.FindGameObjectWithTag("Floor");
        }
        if (floorSize.HasValue == false) {
            floorSize = floor.GetComponent<MeshRenderer>().bounds.size;
        }
        if (floorInitialPos.HasValue == false) {
            floorInitialPos = new Vector2 (
                floor.transform.position.x - floorSize.Value.x / 2,
                floor.transform.position.z - floorSize.Value.z / 2
            );
        }
    }

    public static Vector2Int GetObjectCoordinates(GameObject gObj) {
        CheckStaticVars();
        var objPosition = new Vector2 (
            gObj.transform.position.x,
            gObj.transform.position.z
        );
        Vector2 relativePos = objPosition - floorInitialPos.Value;
        return new Vector2Int (
            Mathf.FloorToInt(Mathf.Abs(relativePos.x) / CELL_SIZE), 
            Mathf.FloorToInt(Mathf.Abs(relativePos.y) / CELL_SIZE)
        );
    }


    public static Vector3 GetCoordinatesPosition(Vector2Int position, float y) {
        CheckStaticVars();
        return new Vector3 (
            floorInitialPos.Value.x + (CELL_SIZE / 2) * (position.x * 2 + 1),
            y,
            floorInitialPos.Value.y + (CELL_SIZE / 2) * (position.y * 2 + 1)
        );
    }

    
    public static Vector2Int getPositionCell(Vector3 position) {
        CheckStaticVars();
        Vector2 objPos = new Vector2(
            position.x,
            position.z
        );

        Vector2 relPos = objPos - floorInitialPos.Value;
        return new Vector2Int(
            Mathf.FloorToInt(relPos.x / CELL_SIZE), 
            Mathf.FloorToInt(relPos.y / CELL_SIZE)
        );
    }

    
    public static Vector3 getCellsPosition(Vector2Int cell, float y) {
        CheckStaticVars();
        return new Vector3(
            floorInitialPos.Value.x + (CELL_SIZE / 2) * (cell.x * 2 + 1),
            y,
            floorInitialPos.Value.y + (CELL_SIZE / 2) * (cell.y * 2 + 1)
        );
    }

    // IMPORTANT: The center of Cell isnt the real place to spawn things. 
    public static Vector3 getCellPosition(Vector2Int cell) {
        GameObject floor = GameObject.FindGameObjectWithTag("Floor");
        Vector3 floorSize = floor.GetComponent<MeshRenderer>().bounds.size;

        Vector2 initialPos = new Vector2(
            floor.transform.position.x - floorSize.x / 2,
            floor.transform.position.z - floorSize.z /2
        );
        return new Vector3(
            initialPos.x + (2*cell.x - 1) * (CELL_SIZE / 2), 
            5, 
            initialPos.y + (2*cell.y - 1) * (CELL_SIZE / 2)
        );
    }
}
