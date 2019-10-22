using UnityEngine;
using UnityEditor;
using System;

namespace Player {
    public class StateController {
        State state;
        IllState illState;
        float immortalityTime;
        float stunTime;
        PlayerController playerController;


        public StateController(PlayerController playerController) {
            state = State.NORMAL;
            illState = IllState.NONE;
            immortalityTime = 0;
            stunTime = 0;
            this.playerController = playerController;
        }

        public void Update() {
            switch (state) {
                case State.IMMORTAL: {
                    immortalityTime -= Time.deltaTime;

                    if (immortalityTime <= 0) {
                        state = State.NORMAL;
                    }
                } break;
                case State.STUN: {
                    stunTime -= Time.deltaTime;

                    if (stunTime <= 0) {
                        state = State.NORMAL;
                        playerController.PlayerAnimator.ResetTrigger("stun");
                    }
                } break;
            }
        }

        public void SetWinLose() {
            state = State.WIN_LOSE;
        }

        public void AddImmortalityTime(float seconds) {
            state = State.IMMORTAL;
            immortalityTime += seconds;
        }

        public void AddStunTime(float seconds)
        {
            playerController.PlayerAnimator.SetTrigger("stun");
            state = State.STUN;
            stunTime += seconds;
        }

        public State GetState() {
            return state;
        }

        public IllState GetIllState() {
            return illState;
        }

        public void AddIllState() {
            SetIllState((IllState)UnityEngine.Random.Range(0, GetIllStatesLength()));
        }

        public void SetIllState(IllState dicease) {
            state = State.ILL;
            illState = dicease;
        }

        public void RemoveIllState() {
            state = State.NORMAL;
            illState = IllState.NONE;
        }

        public static int GetIllStatesLength() {
            return Enum.GetNames(typeof(IllState)).Length;
        }
    }
}