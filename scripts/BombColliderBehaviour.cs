using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombColliderBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other) {
        transform.parent.gameObject.layer = 10;

        if (other.gameObject.tag == "Player") {
            var pController = other.gameObject.GetComponent<Player.PlayerController>();
            other.gameObject.layer = 8;
        }
    }
}
