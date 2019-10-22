using UnityEngine;
using UnityEditor;

class MovementController : MonoBehaviour {
    public string player = "";
    
    private CharacterController charCtrl = null;

    void Start() {
        charCtrl = GetComponent<CharacterController>();
    }

    void Update() {
        
    }

    void Movement() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0) {
            Vector3 movement = Vector3.zero;
            movement.x = h;
            movement.z = v;
            charCtrl.Move(movement);
        }
    }
}