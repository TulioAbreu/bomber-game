using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBlock : MonoBehaviour
{
    public GameObject loot = null;
    public Vector3 position;

   // Start is called before the first frame update
    void Start() {
        position = gameObject.transform.position;
        Invoke("SetOnCoordinates", 0.01f);
    }

    void SetOnCoordinates() {
        Vector2Int objCell = Global.GetObjectCoordinates(gameObject);
        Floor.SetCoordinatesContent(objCell, gameObject, Entity.SOFT_BLOCK);
    }

    // Update is called once per frame
    void Update() {
        SetOnCoordinates();
    }

    public void Destruction() {
        Vector2Int cell = Global.getPositionCell(gameObject.transform.position);
        Floor.SetCoordinatesContent(cell, null, Entity.NONE);

        if (loot != null) {
            Invoke("spawnLoot", 0.4f);
            gameObject.SetActive(false);
        }
        else {
             Destroy(gameObject);
        }
    }

    private void spawnLoot() {
        var powerup = Instantiate(loot);
        var newPos = Global.getCellsPosition(Global.getPositionCell(position), 0.1f);
        powerup.transform.position = newPos;
        Destroy(gameObject);
    }
}
