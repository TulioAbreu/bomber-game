using UnityEngine;
using UnityEditor;


namespace Bomb {
    public class MovementController {
        const float FLOAT_Y = 15;
        const float SPEED = 2.5f;
        public enum InternalState {
            NONE = 0,
            JUMP_RISE,
            JUMP_FLOAT,
            JUMP_DESCEND
        };

        public InternalState internalState;
        public Direction movementDirection;
        float starterDistToMove;
        float distToMove;
        
        public MovementController() {
            internalState = InternalState.NONE;
            movementDirection = Direction.NONE;
            distToMove = 0.0f;
            starterDistToMove = 0.0f;
        }

        public Vector3 GetMovementVector(Vector3 objPos, State state) {
            switch(state) {
                case State.NONE:break;
                case State.NORMAL:break;
                case State.MOVING:break;
                case State.MOVING_AIR: {
                    return OnAirGetMovement(objPos);
                }
                case State.EXPLODING:break;
                case State.CARRIED:break;
            }

            return new Vector3(0, 0, 0);
        }

        // Make the bomb jump to a direction for X cells
        public void Jump(int cells, Direction jumpDir) {
            SetCellsToMove(cells);
            internalState = InternalState.JUMP_RISE;
            movementDirection = jumpDir;
        }

        private void SetCellsToMove(float cells) {
            // Set cells to move and transforms CELL to distance unity
            distToMove = cells * 10;
            starterDistToMove = cells * 10;
        }

        // Hard coded MOVING-AIR behaviour
        private Vector3 OnAirGetMovement(Vector3 objPos) {
            Vector3 movement = new Vector3(0, 0, 0);

            switch (internalState) {
                case InternalState.JUMP_RISE: {
                    // Lift up the bomb
                    if (objPos.y + SPEED < FLOAT_Y) {
                        movement.y += SPEED;
                    }
                    // If bomb reaches FLOAT-Y height, change the state to JUMP-FLOAT
                    else {
                        internalState = InternalState.JUMP_FLOAT;
                    }
                } break;
                
                case InternalState.JUMP_FLOAT: {
                    // Control bomb Height while JUMP-FLOAT
                    // Lift up the bomb until half of movement distance
                    if (distToMove > starterDistToMove/2) {
                        movement.y += SPEED;
                    }
                    // Moment where bomb does not moves
                    else if (distToMove == starterDistToMove/2) {
                    }
                    // Lower the bomb height
                    else if (distToMove < starterDistToMove/2) {
                        movement.y -= SPEED;
                    }

                    // Control bomb X/Z movement while JUMP-FLOAT
                    switch (movementDirection) {
                        case Direction.NONE: {
                            // Do nothing
                        } break;
                        case Direction.NORTH: {
                            movement.z += SPEED;
                        } break;
                        case Direction.EAST: {
                            movement.x += SPEED;
                        } break;
                        case Direction.SOUTH: {
                            movement.z -= SPEED;
                        } break;
                        case Direction.WEST: {
                            movement.x -= SPEED;
                        } break;
                    }

                    // Check if reached the end of movement
                    distToMove -= SPEED;
                    if (distToMove <= 0) {
                        var middle = Global.GetCoordinatesPosition(Global.getPositionCell(objPos), 0);
                        movement = middle - objPos;
                        movement.y = 0;
                        internalState = InternalState.JUMP_DESCEND;
                    }
                } break;

                case InternalState.JUMP_DESCEND: {
                    // Lower the bomb height until it reaches the ground (Y=0)
                    if (objPos.y - SPEED >= 0) {
                        movement.y -= SPEED;
                    }
                    else {
                        internalState = InternalState.NONE;
                        movement.y = -objPos.y;
                    }
                } break;
            }
            return movement;
        }
    }
}