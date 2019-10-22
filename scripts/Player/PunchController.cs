using UnityEngine;
using UnityEditor;

namespace Player {
    public class PunchController : ScriptableObject {
        public static void Punch(GameObject gObj, Direction direction) {
            Vector2Int? frontCoord = GetFrontCoordinate(Global.GetObjectCoordinates(gObj), direction);            
            if (frontCoord == null) {
                return;
            }
            else {
                Floor.Coordinates coordinates = Floor.GetCoordinates(frontCoord.Value).Value;
                Debug.Log(coordinates.contentEntity);
                if (coordinates.contentEntity == Entity.BOMB) {
                    Bomb.BombController bomb = coordinates.content.GetComponent<Bomb.BombController>();
                    if (bomb.gameObject.layer == 10 ||
                        bomb.gameObject.layer == 11) {
                        bomb.Throw(direction, 3);
                    }
                    else {
                        Debug.Log("KICKED NEW BOMB");
                    }
                }
            }
        }

        private static Vector2Int? GetFrontCoordinate(Vector2Int coord, Direction direction) {
            Vector2Int? nextCoord = null;
            switch (direction) {
                case Direction.NORTH: nextCoord = new Vector2Int(coord.x, coord.y+1); break;
                case Direction.EAST:  nextCoord = new Vector2Int(coord.x+1, coord.y); break;
                case Direction.SOUTH: nextCoord = new Vector2Int(coord.x, coord.y-1); break;
                case Direction.WEST:  nextCoord = new Vector2Int(coord.x-1, coord.y); break;
            }
            if (nextCoord.HasValue) {
                if (Floor.GetCoordExists(nextCoord.Value)) {
                    return nextCoord.Value;
                }
                else {
                    return null;
                }
            }
            else {
                return null;
            }
        }
    }

}