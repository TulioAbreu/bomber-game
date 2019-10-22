static class ScoreboardController {
    static public int scoreP1;
    static public int scoreP2;

    static ScoreboardController() {
        scoreP1 = 0;
        scoreP2 = 0;
    }

    public static void AddPoint(PlayerNumb player) {
        switch(player) {
            case PlayerNumb.PLAYER_1: 
                scoreP1 ++;
            break;
            case PlayerNumb.PLAYER_2: 
                scoreP2 ++;
            break;
        }
    }

    public static void IncreaseScore(PlayerNumb player, int value=1) {
        switch (player) {
            case PlayerNumb.PLAYER_1: 
                scoreP1 += value;
            break;
            case PlayerNumb.PLAYER_2: 
                scoreP2 += value;
            break;
        }
    }

    public static int GetScore(PlayerNumb player) {
        switch (player) {
            case PlayerNumb.NONE: return 0;
            case PlayerNumb.PLAYER_1: return scoreP1;
            case PlayerNumb.PLAYER_2: return scoreP2;
            default: return 0;
        }
    }
}