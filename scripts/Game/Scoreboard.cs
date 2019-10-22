using UnityEngine;
using UnityEditor;

public enum PlayerNumb {
    NONE = 0,
    PLAYER_1,
    PLAYER_2
}

class Scoreboard : MonoBehaviour {
    public PlayerNumb player = PlayerNumb.NONE;
    int score;

    void Start() {
        score = ScoreboardController.GetScore(player);
        gameObject.GetComponent<TMPro.TMP_Text>().text = score.ToString();
    }
}
