using UnityEngine;
using UnityEditor;

namespace Bomb {
    public enum State {
        NONE = 0,
        NORMAL,
        MOVING,
        MOVING_AIR,
        EXPLODING,
        CARRIED
    };

    class StateController {
        /*
         * Esta classe se preocupa com todas as transições automáticas entre os estados
         */

        GameObject bombObject;
        State state;
        float timeCountdown;
        Direction movementDir;

        public StateController(GameObject bombObject, float BOMB_TIMER) {
            this.bombObject = bombObject;
            this.state = State.NORMAL;
            this.timeCountdown = BOMB_TIMER;
            this.movementDir = Direction.NONE;
        }

        public void Update() {
            switch (state) {
                case State.NONE: {
                    Debug.LogError("Object with invalid state. (OBJ=BOMB)");
                } break;
                case State.NORMAL: {
                    UpdateTimeCountdown();
                    CheckTimeCountdown();
                } break;
                case State.MOVING: {
                    UpdateTimeCountdown();
                    CheckTimeCountdown();
                    // TODO: Esse estado precisa saber quando voltar ao estado normal sem depender das outras
                } break;
                case State.MOVING_AIR: {
                    CheckBombHeight();
                } break;
                case State.EXPLODING: {
                    // Bomba para explodir não possui alteração de estado
                } break;
            }
        }

        public State GetState() {
            return state;
        }

        public void SetState(State state) {
            this.state = state;
        }

        void UpdateTimeCountdown() {
            timeCountdown -= Time.deltaTime;
        }

        void CheckTimeCountdown() {
            if (timeCountdown <= 0) {
                state = State.EXPLODING;
            }
        }

        void CheckBombHeight() {
            Debug.Log(bombObject.transform.position.y);
            if (bombObject.transform.position.y <= 0) {
                Debug.Log("Atingiu o chao");
                state = State.NORMAL; // Atingiu o chao
                Floor.SetCoordinatesContent(Global.GetObjectCoordinates(this.bombObject), this.bombObject, Entity.BOMB);
            }
        }
    }
}