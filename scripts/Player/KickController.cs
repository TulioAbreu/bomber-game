using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class KickController {
        PlayerController playerController;

        public KickController(PlayerController playerController) {
            this.playerController = playerController;
        }

        public void OnCollisionEnter(Collision Collision) {
            if (Collision.rigidbody.gameObject.tag == "Bomb") {
                if (playerController.attributes.CanKickBombs()) {
                    var bomb = Collision.rigidbody.gameObject.GetComponent<Bomb.BombController>();
                    Direction dir = GetKickDirection(bomb);
                    if (dir != Direction.NONE) {
                        bomb.SetMoving(dir);
                    } 
                }
            }
        }

        private Direction GetKickDirection(Bomb.BombController bomb) {
            if (playerController.GetDirection() == Direction.NORTH &&
                playerController.transform.position.z <  bomb.transform.position.z - 5) 
            {
                return Direction.NORTH;
            }
            if (playerController.GetDirection() == Direction.SOUTH &&
                playerController.transform.position.z >  bomb.transform.position.z + 5) 
            {
                return Direction.SOUTH;
            }
            if (playerController.GetDirection() == Direction.WEST &&
                playerController.transform.position.x >  bomb.transform.position.x + 5) 
            {
                return Direction.WEST;
            }
            if (playerController.GetDirection() == Direction.EAST &&
                playerController.transform.position.x <  bomb.transform.position.x - 5) 
            {
                return Direction.EAST;
            }
            return Direction.NONE;
        }
    }
}

