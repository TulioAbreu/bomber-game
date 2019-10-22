using UnityEngine;
using UnityEditor;

namespace Explosion {
    public class ExplosionController : MonoBehaviour {
        public float vanishTime = 0.3f;
        private float timeLeft = 0;
        private ParticleSystem explosionAnimation;
        private void Start() {
            gameObject.tag = "Explosion";
            timeLeft = vanishTime;
            explosionAnimation = GetComponent<ParticleSystem>();
        }

        private void Update() {
            if (Game.GameController.IsPaused()) {
//                var main = explosionAnimation.main;
//                Debug.Log(main.simulationSpeed);
//                main.simulationSpeed = 0;
                return;
            }
            else {
//                var main = explosionAnimation.main;
//                main.simulationSpeed = 1;
            }

            UpdateVanishTime();
        }

        // Update vasish time
        void UpdateVanishTime() {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            switch (other.gameObject.tag) {
                // Explosão colide com Bomba
                case "Bomb": {
                    var bombController = other.gameObject.GetComponent<Bomb.BombController>();
                    Debug.Assert(bombController != null);
                    if (bombController.GetState() != Bomb.State.EXPLODING) {
                        bombController.SetBombTriggered();
                    }
                } break;

                // Explosão colide com Player
                case "Player": {
                    var playerController = other.gameObject.GetComponent<Player.PlayerController>();
                    Debug.Assert(playerController != null);
                    playerController.attributes.RemoveHealthPoint();
                } break;

                // Explosão colide com PowerUp
                case "PowerUp": {
                    Destroy(other.gameObject);
                } break;

                // Explosão colide comm Soft Block
                case "Soft Block": {
                    other.gameObject.GetComponent<SoftBlock>().Destruction();
                } break;
            }
        }
    }
}