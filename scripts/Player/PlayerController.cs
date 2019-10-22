using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public enum Number {
        NONE = 0,
        PLAYER_1,
        PLAYER_2
    };

    public class PlayerController : MonoBehaviour {
        const float BASE_SPEED = 30;
        const float SPEED_RATE = 5;

        public Number number;
        public GameObject defaultBombPrefab = null;
        public GameObject spikedBombPrefab = null;

        public Attributes attributes;
        public StateController stateController;
        public InputController inputController;
        public KickController kickController;

        public Bomb.Type plantBombType;

        public Animator PlayerAnimator;

        private int plantedBombs;

        [SerializeField] private bool isTrapped;

        private void Start() {
            attributes = new Attributes(this);
            stateController = new StateController(this);
            inputController = new InputController(gameObject,
                                                  this,
                                                  gameObject.GetComponent<CharacterController>(),
                                                  stateController,
                                                  number);
            kickController = new KickController(this);
            plantedBombs = 0;
            isTrapped = false;
            plantBombType = Bomb.Type.DEFAULT;
        }

        void FixedUpdate() {
            if (Game.GameController.IsPaused()) {
                return;
            }

            if (attributes.GetHealthPoints() <= 0) {
                Death();
                return;
            }

            stateController.Update();
            if (CheckIsTrapped()) {
//                Debug.Log("Is trapped.");
                isTrapped = true;
                PlayerAnimator.SetTrigger("trap");

            }
            else {
//                Debug.Log("Is not trapped.");
                isTrapped = false;
                PlayerAnimator.ResetTrigger("trap");
            }

            if (gameObject.transform.position.y != 0) {
                var pos = new Vector3(gameObject.transform.position.x,
                                      gameObject.transform.position.y,
                                      gameObject.transform.position.z);
                pos.y = 0;
                gameObject.transform.position = pos;
            }
        }

        bool IsBlock(Entity entity) {
            switch (entity) {
                case Entity.NONE: return false;
                case Entity.BOMB: return true;
                case Entity.HARD_BLOCK: return true;
                case Entity.SOFT_BLOCK: return true;
                case Entity.POWER_UP: return false;
                case Entity.PLAYER: return false;
                default: return true;
            }
        }

        public void SetBombType(Bomb.Type bombType) {
            plantBombType = bombType;
        }

        bool IsWalkable(Vector3 pos) {
            Vector2Int posCoord = Global.getPositionCell(pos);
            if (Floor.GetCoordExists(posCoord)) {
                if (Floor.GetCoordinates(posCoord).Value.contentEntity != Entity.NONE &&
                    Floor.GetCoordinates(posCoord).Value.contentEntity != Entity.POWER_UP)
                {
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                return false;
            }
        }

        bool CheckIsTrapped() {
            var eastPos = gameObject.transform.position + new Vector3(10, 0, 0);
            var westPos = gameObject.transform.position + new Vector3(-10, 0, 0);
            var northPos = gameObject.transform.position + new Vector3(0, 0, 10);
            var southPos = gameObject.transform.position + new Vector3(0, 0, -10);

            return (!IsWalkable(eastPos) && !IsWalkable(westPos) && !IsWalkable(northPos) && !IsWalkable(southPos));
        }

        void Update() {
            switch (stateController.GetState()) {
                case State.NONE: {
                    Debug.LogError("Player Object is not in a proper state (NONE).");
                } break;
                case State.NORMAL: {
                    inputController.Update();
                } break;
                case State.STUN: {
                    // Does not call INPUT UPDATE while Stunned
                } break;
                case State.ILL: {
                    inputController.Update();    
                } break;
                case State.IMMORTAL: {
                    inputController.Update();    
                } break;
                case State.WIN_LOSE: {
                    // Do nothing
                } break;
            }
            
            if(number == Number.PLAYER_1 && Game.GameController.aliveP2 == false && Game.GameController.aliveP1 == true)
            {
                PlayerAnimator.SetTrigger("win");
                stateController.AddImmortalityTime(2.0f);
                stateController.SetWinLose();
            }
            else if (number == Number.PLAYER_2 && Game.GameController.aliveP1 == false && Game.GameController.aliveP2 == true)
            {
                PlayerAnimator.SetTrigger("win");
                stateController.AddImmortalityTime(2.0f);
                stateController.SetWinLose();
            }
        }

        private void Death() {
            switch (number) {
                case Number.PLAYER_1: {
                    Game.GameController.aliveP1 = false;
                } break;
                case Number.PLAYER_2: {
                    Game.GameController.aliveP2 = false;
                } break;
            }

            PlayerAnimator.SetTrigger("death");
            stateController.SetWinLose();
            //Destroy(gameObject);
        }

        public Direction GetDirection() {
            return inputController.GetPlayerDirection();
        }

        public float GetPlayerSpeed() {
            int speedMultiplier = -1;
            
            if (stateController.GetState() == State.ILL) {
                switch (stateController.GetIllState()) {
                    case IllState.LOW_PACE: {
                        speedMultiplier = 1;
                    } break;
                    case IllState.FAST_PACE: {
                        speedMultiplier = 8;
                    } break;
                }
            }

            if (speedMultiplier < 0) {
                speedMultiplier = attributes.GetSpeedLevel();
            }

            return BASE_SPEED + speedMultiplier*SPEED_RATE;
        }

        public int GetPlantedBombs() {
            return plantedBombs;
        }

        public void IncreasePlantedBombs() {
            plantedBombs ++;
        }

        public void DecreasePlantedBombs() {
            plantedBombs --;
        }

        void OnCollisionEnter(Collision collision) {
            kickController.OnCollisionEnter(collision);
        }
    }
}