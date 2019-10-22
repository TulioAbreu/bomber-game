using UnityEngine;
using UnityEditor;

namespace Powerup {
    public class PowerupController : MonoBehaviour {
        const int PW_VEST_TIME = 3;

        public Type type = Type.NONE;

        private void Start() {
            gameObject.tag = "PowerUp";
        }

        private void OnTriggerEnter(Collider other) {
            switch (other.gameObject.tag) {
                case "Bomb": {
                    Destroy(gameObject);
                } break;

                case "Player": {
                    var playerController = other.gameObject.GetComponent<Player.PlayerController>();
                    
                    switch (type) {
                        case Type.NONE: {
                            Debug.LogError("Player collided with PowerUp without defined type.");
                        } break;
                        case Type.BOMB_UP: {
                            HealIllness(playerController);
                            playerController.attributes.AddMaxBombSlots();
                        } break;
                        case Type.FIRE_UP: {
                            HealIllness(playerController);
                            playerController.attributes.AddExtraRadius();
                        } break;
                        case Type.FULL_FIRE: {
                            HealIllness(playerController);
                            playerController.attributes.AddExtraRadius(8);
                        } break;
                        case Type.SKATE: {
                            HealIllness(playerController);
                            playerController.attributes.AddSpeedLevel();
                        } break;
                        case Type.HEART: {
                            HealIllness(playerController);
                            playerController.attributes.AddHealthPoint();
                        } break;
                        case Type.VEST: {
                            HealIllness(playerController);
                            playerController.stateController.AddImmortalityTime(PW_VEST_TIME);
                        } break;
                        case Type.SKULL: {
                            playerController.stateController.AddIllState();
                        } break;
                        case Type.KICK: {
                            HealIllness(playerController);
                            playerController.attributes.SetCanKickBombs();
                        } break;
                        case Type.BOXING_GLOVES: {
                            HealIllness(playerController);
                            playerController.attributes.SetCanPunchBombs();
                        } break;
                        case Type.POWER_GLOVES: {
                            HealIllness(playerController);
                            playerController.attributes.SetCanThrowBombs();
                        } break;
                        case Type.PIERCE_BOMB: {
                            HealIllness(playerController);
                            playerController.SetBombType(Bomb.Type.PIERCE);
                        } break;
                    }
                    Destroy(gameObject);
                } break;
            }
        }
        
        private void HealIllness(Player.PlayerController playerController) {
            if (playerController.stateController.GetState() == Player.State.ILL) {
                playerController.stateController.RemoveIllState();
            }
        }
    }
}


