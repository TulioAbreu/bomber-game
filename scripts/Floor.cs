using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {
    struct PwQuantity {
        public int quantity;
        public int quantityLeft;
        public GameObject prefab;

        public PwQuantity(GameObject p, int q) {
            quantity = q;
            quantityLeft= q;
            prefab = p;
        }

        public GameObject GetPrefab() {
            quantityLeft --;
            return prefab;
        }
    };
    
    public const int ignoreMe = 0;
    public int[,] softBlocksMap = new int [13, 12]{
        {ignoreMe, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0},
        {ignoreMe, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0},
        {ignoreMe, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {ignoreMe, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1},
        {ignoreMe, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {ignoreMe, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1},
        {ignoreMe, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {ignoreMe, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1},
        {ignoreMe, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0},
        {ignoreMe, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1},
        {ignoreMe, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
        {ignoreMe, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1},
        {ignoreMe, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1}
    };

    public struct Coordinates {
        public GameObject content;
        public Entity contentEntity;
    };

    private static Coordinates[,] arenaMatrix = new Coordinates[Global.ARENA_WIDTH, Global.ARENA_HEIGHT];
    public GameObject mapEntities;
    public static bool isPaused = false;
    
    public static bool GetCoordExists(Vector2Int coord) {
        if (coord.x < 0 || coord.x > Global.ARENA_WIDTH-1) {
            return false;
        }
        if (coord.y < 0 || coord.y > Global.ARENA_HEIGHT-2) {
            return false;
        }
        return true;
    }

    // Prefabs
    public GameObject hardBlockPrefab = null;
    public GameObject softBlockPrefab = null;
    public GameObject borderBlockPrefab = null;
    public GameObject pwFullfPrefab = null;
    public GameObject pwFireupPrefab = null;
    public GameObject pwBombupPrefab = null;
    public GameObject pwSkatePrefab = null;
    public GameObject pwSkullPrefab = null;
    public GameObject pwVestPrefab = null;
    public GameObject pwKickPrefab = null;
    public GameObject pwBoxPrefab = null;
    public GameObject pwGlovesPrefab = null;
    public GameObject pwSpikePrefab = null;

    void Start() {
        StartArena();
    }

    Player.Number? winner = null;
    bool waitingTime = false;
    void FixedUpdate() {
        Player.Number? result = Game.GameController.Update();

        if (result.HasValue && ! waitingTime) {
            Debug.Log("[Floor] Game have result! Invoking new stage on 2 seconds...");
            this.winner = result.Value;
            waitingTime = true;
            switch (result.Value) {
                case Player.Number.NONE: { // Draw
                    Invoke("NextStage", 4);
                } break;
                case Player.Number.PLAYER_1: { // Player 1 Win
                    Invoke("NextStage", 4);
                } break;
                case Player.Number.PLAYER_2: { // Player 2 Win
                    Invoke("NextStage", 4);
                } break;
            }
        }
        else {
            // Do nothing, sem vencedor
        }
     }

    void NextStage() {
        Debug.Log("[Floor] Creating next stage... " + winner.Value.ToString());
        Game.GameController.NextStage(winner.Value);
        winner = null;
        waitingTime = false;
    }

    public static void SetCoordinatesContent(
        Vector2Int pos, 
        GameObject gObj, 
        Entity gObjEntity
    ) {
        if ((pos.x < 0 || pos.x > Global.ARENA_WIDTH - 1) ||
            (pos.y < 0 || pos.y > Global.ARENA_HEIGHT - 1)
        ) {
            Debug.LogError(@"SetCoordinatesContent: Invalid Position " + 
                pos.ToString());
            return;
        }
        arenaMatrix[pos.x, pos.y] = new Coordinates {
            content = gObj,
            contentEntity = gObjEntity
        };
    }

    public static Coordinates? GetCoordinates(Vector2Int pos) {
        if ((pos.x < 0 || pos.x > Global.ARENA_WIDTH - 1) ||
            (pos.y < 0 || pos.y > Global.ARENA_HEIGHT - 2)
        ) {
            Debug.LogError(@"SetCoordinatesContent: Invalid Position " + 
                pos.ToString());
            return null;
        }
        return arenaMatrix[pos.x, pos.y];
    }

    void StartArena() {
        StartArenaMatrix();
        GenerateHardBlocks();
        GenerateSoftBlocks();
    }

    void GenerateHardBlocks() {
        // Outside blocks
        for (int i = 0; i < 15; ++i) {
            GameObject h_block = Instantiate(borderBlockPrefab);
            h_block.transform.SetParent(mapEntities.transform);
            h_block.transform.position = Global.getCellPosition(new Vector2Int(i, 0));
            GameObject h_block2 = Instantiate(borderBlockPrefab);
            h_block2.transform.SetParent(mapEntities.transform);
            h_block2.transform.position = Global.getCellPosition(new Vector2Int(i, 12));
        }
        for (int i = 1; i < 13; ++i) {
            GameObject h_block = Instantiate(borderBlockPrefab);
            h_block.transform.position = Global.getCellPosition(new Vector2Int(0, i));
            h_block.transform.SetParent(mapEntities.transform);
            GameObject h_block2 = Instantiate(borderBlockPrefab);
            h_block2.transform.SetParent(mapEntities.transform);
            h_block2.transform.position = Global.getCellPosition(new Vector2Int(14, i));
        }
        // Arena blocks
        for (int i = 2; i < Global.ARENA_WIDTH; i += 2) {
            for (int j = 2; j < Global.ARENA_HEIGHT; j += 2) {
                GameObject h_block = Instantiate(hardBlockPrefab);
                h_block.transform.position = Global.getCellPosition(new Vector2Int(i, j));
                h_block.transform.SetParent(mapEntities.transform);
            }
        }
    }

    void GenerateSoftBlocks() {
        List<GameObject> softBlocks = new List<GameObject>();
        for (int i = 0; i < Global.ARENA_HEIGHT; ++i) {
            for (int j = 0; j < Global.ARENA_WIDTH; ++j) {
                if (softBlocksMap[j, i] == 1) {
                    softBlocks.Add(CreateSoftBlock(new Vector2Int(j + 1, i)));
                }
            }
        }
        GeneratePowerUps(softBlocks);
    }

    private bool PwIsOver(List<PwQuantity> pwQuantityList) {
        foreach (var pwQuantity in pwQuantityList) {
            if (pwQuantity.quantityLeft > 0) {
                return false;
            }
        }
        return true;
    }
    
    private List<PwQuantity> CreatePowerupList() {
        List<PwQuantity> powerups = new List<PwQuantity>();
        powerups.Add(new PwQuantity(pwFullfPrefab, 1));
        powerups.Add(new PwQuantity(pwSkullPrefab, 1));
        powerups.Add(new PwQuantity(pwVestPrefab, 1));
        powerups.Add(new PwQuantity(pwSkatePrefab, 8));
        powerups.Add(new PwQuantity(pwFireupPrefab, 8));
        powerups.Add(new PwQuantity(pwBombupPrefab, 8));
        powerups.Add(new PwQuantity(pwKickPrefab, 5));
        powerups.Add(new PwQuantity(pwBoxPrefab, 3));
        powerups.Add(new PwQuantity(pwGlovesPrefab, 3));
        powerups.Add(new PwQuantity(pwSpikePrefab, 1));
        return powerups;
    }

    private void GeneratePowerUps(List<GameObject> softBlocks) {
        List<PwQuantity> pwQuantity = CreatePowerupList();
       
        while (!PwIsOver(pwQuantity)) {
            int softBlockNumber = Random.Range(0, softBlocks.Count);
            var softBlock = softBlocks[softBlockNumber].GetComponent<SoftBlock>();
            if (softBlock.loot == null) {
                int chosenPowerUp;
                do {
                    chosenPowerUp = Random.Range(0, pwQuantity.Count);
                } while(pwQuantity[chosenPowerUp].quantityLeft <= 0);
                
                var chosenPw = pwQuantity[chosenPowerUp];
                chosenPw.quantityLeft --;
                pwQuantity[chosenPowerUp] = chosenPw;
                softBlock.loot = pwQuantity[chosenPowerUp].prefab;
            }
        }
    }

    private GameObject CreateSoftBlock(Vector2Int coords) {
        GameObject soft_block = Instantiate(softBlockPrefab);
        soft_block.transform.position = Global.getCellPosition(coords);
        soft_block.transform.eulerAngles = new Vector3(0, 90, 0);
        soft_block.transform.SetParent(mapEntities.transform);
        return soft_block;
    }

    void StartArenaMatrix() {
        for (int i = 0; i < Global.ARENA_WIDTH; ++i) {
            for (int j = 0; j < Global.ARENA_HEIGHT; ++j) {
                SetCoordinatesContent(
                    new Vector2Int(i, j),
                    null,
                    Entity.NONE
                );
            }
        }
    }
}
