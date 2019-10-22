using UnityEngine;
using UnityEditor;

namespace Player {
    public class InputController {
        GameObject playerObject;
        PlayerController playerController;
        CharacterController characterController;
        StateController stateController;
        Direction playerDir;

        bool hasLiftedBomb;
        Bomb.BombController liftedBomb = null;
        Number playerNum = Number.NONE;

        public InputController(GameObject playerObject, 
                               PlayerController playerController,
                               CharacterController characterController,
                               StateController stateController,
                               Number playerNum) {
            this.playerObject = playerObject;
            this.playerController = playerController;
            this.characterController = characterController;
            this.stateController = stateController;
            this.hasLiftedBomb = false;
            this.playerNum = playerNum;
            
            StartControllers();
        }

        private string HorizontalButton = "Horizontal";
        private string VerticalButton = "Vertical";
        private string Fire1Button = "Fire1";
        private string Fire2Button = "Fire2";

        private void StartControllers() {
            switch (playerNum) {
                case Number.PLAYER_1: {
                    HorizontalButton += '1';
                    VerticalButton += '1';
                    Fire1Button += '1';
                    Fire2Button += '1';
                } break;
                case Number.PLAYER_2: {
                    HorizontalButton += '2';
                    VerticalButton += '2';
                    Fire1Button += '2';
                    Fire2Button += '2';
                } break;
                default:
                Debug.Log("TROSLEI");
                break;
            }
            Debug.Log(VerticalButton);
            Debug.Log(HorizontalButton);
        }

        public void Update() {
            if (Input.GetButtonDown("Pause")) {
                Game.GameController.PauseButton();
            }

            if (Game.GameController.IsPaused()) {
                return;
            }

            Movement();
            if (! hasLiftedBomb) {
                FireButton();
                DoPunch();
            }
            else {
                if (Input.GetButtonUp(Fire1Button)) {
                    hasLiftedBomb = false;
                    liftedBomb.transform.parent = null;
                    var pos = liftedBomb.transform.position;
                    liftedBomb.transform.position = Global.GetCoordinatesPosition(Global.GetObjectCoordinates(liftedBomb.gameObject), pos.y);
                    liftedBomb.Throw(playerDir, 3);
                }
            }

        }

        public Direction GetPlayerDirection() {
            return playerDir;
        }

        private void Movement() {
            float h = Input.GetAxis(HorizontalButton);
            float v = Input.GetAxis(VerticalButton);

            // Skull -> Change Effect
            if (stateController.GetIllState() == IllState.CHANGE) {
                h *= -1;
                v *= -1;
            } 

            DefinePlayerDirection(h, v);

            RotatePlayer(h, v);

            characterController.Move(
                new Vector3(h, 0, v) 
                * playerController.GetPlayerSpeed()
                * Time.deltaTime);
            
            Vector3 pos = playerObject.transform.position;
            pos.y = 0;
            playerObject.transform.position = pos;
        }

        private void RotatePlayer(float h, float v)
        {
            var rotation = Vector3.Normalize(new Vector3(h, 0.0f, v));
            if (rotation != Vector3.zero)
            {
                playerObject.transform.forward = rotation;
            }
        }

        private void DefinePlayerDirection(float h, float v) {

            if (h < 0) {
                playerDir = Direction.WEST;
            }
            else if (h > 0) {
                playerDir = Direction.EAST;
            }
            else if (v < 0) {
                playerDir = Direction.SOUTH;
            }
            else if (v > 0) {
                playerDir = Direction.NORTH;
            }
        }

        private void FireButton() {
            if (stateController.GetIllState() == IllState.CONSTIPATION) {
                // Skull -> Constipation Effect
                return;
            }

            if (Input.GetButtonDown(Fire1Button) ||
                stateController.GetIllState() == IllState.DIARRHEA) 
            {
                GameObject bombToCreate = null;
                switch (playerController.plantBombType) {
                    case Bomb.Type.DEFAULT: {
                        bombToCreate = playerController.defaultBombPrefab;
                    } break;
                    case Bomb.Type.PIERCE: {
                        bombToCreate = playerController.spikedBombPrefab;
                    } break;
                }
                
                if (bombToCreate != null) {
                    if (CreateBomb(bombToCreate)) {
                        if (playerController.attributes.CanThrowBombs() &&
                            playerController.stateController.GetIllState() != IllState.DIARRHEA
                        ) {
                            LiftUpBomb(Floor.GetCoordinates(Global.GetObjectCoordinates(playerObject)).GetValueOrDefault().content);
                        }
                    }
                }
            }
        }

        // Gloves
        private void LiftUpBomb(GameObject bomb) {
            var bombCtrl = bomb.GetComponent<Bomb.BombController>();

            // Set coordinate to Empty
            Floor.SetCoordinatesContent(Global.GetObjectCoordinates(bomb), null, Entity.NONE);

            // Carry bomb
            bombCtrl.SetBombStatus(Bomb.State.CARRIED); 
            bombCtrl.gameObject.transform.Translate(new Vector3(0, 10, 0));
            bombCtrl.transform.parent = playerController.transform;
            this.hasLiftedBomb = true;
            this.liftedBomb = bombCtrl;
        }

        // Returns true if player tried to create a bomb above another
        private bool CreateBomb(GameObject bombPrefab) {
            var p1 = GameObject.Find("Bomberman White");
            var p2 = GameObject.Find("Bomberman Black");

            Vector2Int p1Coords = Global.GetObjectCoordinates(p1);
            Vector2Int p2Coords = Global.GetObjectCoordinates(p2);

            if (p1Coords.x == p2Coords.x && p1Coords.y == p2Coords.y) {
                p1.layer = 10;
                p2.layer = 11;
            }
            else if (playerController.number == Number.PLAYER_1) {
                p1.layer = 10;
            }
            else if (playerController.number == Number.PLAYER_2) {
                p2.layer = 11;
            }


            Vector2Int bombCell = Global.GetObjectCoordinates(playerObject);
            // Player cannot create a bomb above another Bomb, a Hard Block or Soft Block
            switch (Floor.GetCoordinates(bombCell).GetValueOrDefault().contentEntity) {
                case Entity.BOMB: return true;
                case Entity.HARD_BLOCK: return false;
                case Entity.SOFT_BLOCK: return false;
            }

            if (playerController.GetPlantedBombs() >= playerController.attributes.GetMaxBombSlots()) {
                // Player is trying to create more bombs than it levels limit
                return false;
            }


            // TODO: Change this default bomb to the bomb the player is currently using
            GameObject bomb = GameObject.Instantiate(bombPrefab);
            // TODO: Change this Bomb Class to Bomb Controller, after refactory
            var bombController = bomb.GetComponent<Bomb.BombController>();
            bombController.bombOwner = playerObject;
            playerController.IncreasePlantedBombs();
            // TODO: Check why do I need Instance ID instead the object reference?
            bombController.SetBombRadius(
                // Skull -> Low Power Effect
                (stateController.GetIllState() == IllState.LOW_POWER)? 0 : playerController.attributes.GetExtraRadius()
            );

            // Move bomb to correct place
            Vector3 newPos = Global.getCellsPosition(bombCell, 0);
            Floor.SetCoordinatesContent(bombCell, bomb, Entity.BOMB);
            bomb.transform.position = newPos;

            return false;
        }

        private void DoPunch() {
            if (! playerController.attributes.CanPunchBombs()) {
                // If the player cannot punch bombs, just ignore
                return;
            }

            playerController.PlayerAnimator.ResetTrigger("punch");
            if (Input.GetButtonDown(Fire2Button)) {
                PunchController.Punch(playerObject, playerDir);
                playerController.PlayerAnimator.SetTrigger("punch");
            }
        }
    }
}
