using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game {
    public enum State {
        NONE = 0,
        MAIN_MENU,
        PLAYING,
        PLAYING_PAUSED
    };

    public static class GameController {
        static State state;

        static public int maxRounds;

        static GameObject pauseTile;

        static public bool aliveP1;
        static public bool aliveP2;

        static GameController() {
            StartGameStage();
            state = State.MAIN_MENU;
        }

        static public void StartGameStage() {
            ScoreboardController.scoreP1 = 0;
            ScoreboardController.scoreP2 = 0;
            // todo: change initial game state!
            state = State.PLAYING;
            aliveP1 = true;
            aliveP2 = true;
        }

        static public Player.Number? Update() {
            Player.Number? winner = null;
            if (!aliveP1 && !aliveP2) {
                Debug.Log("Draw!");
                winner = Player.Number.NONE;
            }
            else if (!aliveP2) { // Player 1 vence
                Debug.Log("Player 1 WIN");
                winner = Player.Number.PLAYER_1;
            }
            else if (!aliveP1) { // Player 2 vence
                Debug.Log("Player 2 WIN");
                winner = Player.Number.PLAYER_2;
            }

            return winner;
        }

        static public void RestartScene() {
            aliveP1 = true;
            aliveP2 = true;
            Debug.Log(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        static public void NextStage(Player.Number winner) {
            if (winner == Player.Number.NONE) {
                RestartScene();
            }
            else if (winner == Player.Number.PLAYER_1) {
                ScoreboardController.scoreP1 ++;
                if (ScoreboardController.scoreP1 >= maxRounds) {
                    // Rounds OVER
                    SceneManager.UnloadSceneAsync("GameStage");
                    SceneManager.LoadScene("MainMenu");
                }
                else {
                    RestartScene();
                }
            }
            else if (winner == Player.Number.PLAYER_2) {
                ScoreboardController.scoreP2++;
                if (ScoreboardController.scoreP2 >= maxRounds) {
                    // Rounds OVER
                    SceneManager.UnloadSceneAsync("GameStage");
                    SceneManager.LoadScene("MainMenu");
                }
                else {
                    RestartScene();
                }
            }
        }

        static public void PauseButton() {
            switch (state) {
                case State.PLAYING: {
                    Time.timeScale = 0;
                    Debug.Log("[Game::GameController] Paused.");
                    state = State.PLAYING_PAUSED;
                    pauseTile = GameObject.Find("PauseImage");
                    pauseTile.GetComponent<Image>().enabled = true;
                } break;
                case State.PLAYING_PAUSED: {
                    Time.timeScale = 1;
                    Debug.Log("[Game::GameController] Unpaused.");  
                    state = State.PLAYING;
                    pauseTile = GameObject.Find("PauseImage");
                    pauseTile.GetComponent<Image>().enabled = false;
                } break;
            }
        }

        static public bool IsPaused() {
            return (state == State.PLAYING_PAUSED);
        }
    }
}

