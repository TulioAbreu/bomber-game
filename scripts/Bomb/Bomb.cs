using UnityEngine;
using UnityEditor;

namespace Bomb {
    public abstract class BombController : MonoBehaviour {
        // Constants
        public const int BOMB_MIN_RADIUS = 2;

        // Prefabs
        public GameObject bombOwner = null;
        public GameObject explosionPrefab = null;
        public AudioClip explosionSound = null;
        private Direction movingDirection = Direction.NONE;
        private Vector2Int lastCoord;

        public bool isTriggered;
        
        public float BOMB_TIMER = 3.0f;
        private float timeCountdown = 3.0f;
        private int radius = 0;

        /*
            Refactory attributes
        */
        private StateController stateController;
        private MovementController movementController;

        public GameObject soundEffect = null;
        private void Start() {
            stateController = new StateController(this.gameObject, BOMB_TIMER);
            movementController = new MovementController();
            lastCoord = Global.GetObjectCoordinates(gameObject);
        }

        private void FixedUpdate() {
            if (Game.GameController.IsPaused()) {
                return;
            }

            switch (stateController.GetState()) {
                case State.NONE: {
                } break;
                case State.NORMAL: {
                } break;
                case State.MOVING: {
                    Move();
                } break;
                case State.MOVING_AIR: {
                    Vector3 movement = movementController.GetMovementVector(transform.position, stateController.GetState());
                    gameObject.transform.position += movement;
                    CheckTransporterBounds();

                    if (gameObject.transform.position.y <= 0) {
                        gameObject.GetComponent<Collider>().isTrigger = false;
                    }
                } break;
                case State.EXPLODING: {
                    Destroy(gameObject);
                    Explode();
                } break;
                case State.CARRIED: {
                } break;
            }

            stateController.Update();
        }

        private void GenerateExplosionSFX() {
            if (! isTriggered) {
                if (soundEffect != null) {
                    var gObj = GameObject.Instantiate(soundEffect);
                    AudioSource cloneClipLength = gObj.GetComponent<AudioSource>();
                    Destroy(gObj, cloneClipLength.clip.length);
                    return;
                }
            }
        }

        public void Explode() {
            GenerateExplosionSFX();

            Vector2Int epicenterCell = Global.getPositionCell(gameObject.transform.position);
            Destroy(this);

            if (IsInsideArenaBounds(epicenterCell)) {
                Floor.SetCoordinatesContent(epicenterCell, null, Entity.NONE);
                GenerateExplosionArea(epicenterCell);
            }

            if (bombOwner != null) {
                bombOwner.GetComponent<Player.PlayerController>().DecreasePlantedBombs();
            }

        }

        private bool IsInsideArenaBounds(Vector2Int pos) {
            if (pos.x >= 0 && pos.x <= 12)
                if (pos.y >= 0 && pos.y <= 10)
                {
                    return true;
                }
            return false;
        }

        // Bomb specific collision logic
        protected abstract bool ExplosionLogicAt(Vector2Int coord);

        // Generate bomb area, using specific bomb type collision
        private void GenerateExplosionArea(Vector2Int epicenterCell) {
            bool collided = false;
            // Horizontal - left -> right
            for (
                int i = (int)epicenterCell.x;
                i <= epicenterCell.x + BOMB_MIN_RADIUS + radius && !collided;
                ++i
            )
            {
                if (IsInsideArenaBounds(new Vector2Int(epicenterCell.x, epicenterCell.y)))
                {
                    collided = ExplosionLogicAt(new Vector2Int(i, epicenterCell.y));
                }
            }

            collided = false;
            // Horizontal - right -> left
            for (
                int i = (int)epicenterCell.x - 1;
                i >= epicenterCell.x - BOMB_MIN_RADIUS - radius && !collided;
                --i
            )
            {
                if (IsInsideArenaBounds(new Vector2Int(epicenterCell.x, epicenterCell.y)))
                {
                    collided = ExplosionLogicAt(new Vector2Int(i, epicenterCell.y));
                }
            }

            collided = false;
            // Vertical - up -> down
            for (
                int i = (int)epicenterCell.y;
                i <= epicenterCell.y + BOMB_MIN_RADIUS + radius && !collided;
                ++i
            )
            {
                if (IsInsideArenaBounds(new Vector2Int(epicenterCell.x, epicenterCell.y))) {
                    collided = ExplosionLogicAt(new Vector2Int(epicenterCell.x, i));
                }
            }
            collided = false;
            // Vertical - down -> up
            for (
                int i = (int)epicenterCell.y - 1;
                i >= epicenterCell.y - BOMB_MIN_RADIUS - radius && !collided;
                --i
            )
            {
                if (IsInsideArenaBounds(new Vector2Int(epicenterCell.x, epicenterCell.y)))
                {
                    collided = ExplosionLogicAt(new Vector2Int(epicenterCell.x, i));
                }
            }
        }

        // Create explosion at this position
        protected void CreateExplosionAtPos(Vector2Int cellPos) {
            var explosion = Instantiate(explosionPrefab);
            explosion.transform.position = Global.getCellsPosition(cellPos, 0);
        }

        public void SetBombRadius(int value) {
            radius = value;
        }

        // Set bomb status
        public void SetBombStatus(State newState) {
            stateController.SetState(newState);
        }

        // Set bomb owner
        public void SetBombOwner(int ownerInstID)
        {
            bombOwner = GameObject.Find(ownerInstID.ToString());
        }

        // Set if bomb was triggered by another
        public void SetBombTriggered() {
            isTriggered = true;
            SetBombStatus(Bomb.State.EXPLODING);
        }
        
        public State GetState() {
            return stateController.GetState();
        }

        public void SetMoving(Direction dir) {
            stateController.SetState(State.MOVING);
            movingDirection = dir;
        }

        public static bool IsInCoordCenter(Vector3 objPos)
        {
            // TODO: Add error cuz these are floats
            float error = 3f;
            Vector2Int objectCell = Global.getPositionCell(objPos);
            Vector3 coordPosition = Global.GetCoordinatesPosition(objectCell, 0);
            if (coordPosition.x - error < objPos.x &&
                objPos.x < coordPosition.x + error)
            {
                return true;
            }
            if (coordPosition.z - error < objPos.z &&
                objPos.z < coordPosition.z + error)
            {
                return true;
            }
            return false;
        }

        public static bool IsInCoordCenter(GameObject gObj) {
            // TODO: Add error cuz these are floats
            float error = 2f;
            Vector2Int objectCell = Global.GetObjectCoordinates(gObj);
            Vector3 coordPosition = Global.GetCoordinatesPosition(objectCell, 0);
            if (coordPosition.x - error < gObj.transform.position.x &&
                gObj.transform.position.x < coordPosition.x + error)
            {
                return true;
            }
            if (coordPosition.z - error < gObj.transform.position.z &&
                gObj.transform.position.z < coordPosition.z + error)
            {
                return true;
            }
            return false;
        }

        private void Move() {
            Vector3 nextCoord = gameObject.transform.position + GetMovementOffset(movingDirection, 6);
            Vector2Int nextCoord2D = Global.getPositionCell(nextCoord);
            Vector2Int nowCoord = Global.getPositionCell(gameObject.transform.position);
            if (Floor.GetCoordExists(nextCoord2D)) {
                if (Floor.GetCoordinates(nextCoord2D).Value.contentEntity == Entity.NONE ||
                    (nowCoord.x == nextCoord2D.x && nowCoord.y == nextCoord2D.y)) 
                {
                    var p1 = GameObject.Find("Bomberman White");
                    var p2 = GameObject.Find("Bomberman Black");

                    var p1Coords = Global.GetObjectCoordinates(p1);
                    var p2Coords = Global.GetObjectCoordinates(p2);

                    if ((nextCoord2D.x == p1Coords.x && nextCoord2D.y == p1Coords.y) &&
                        (nextCoord2D.x == p2Coords.x && nextCoord2D.y == p2Coords.y))
                    {
                        gameObject.transform.position += GetMovementOffset(movingDirection);
                        UpdateCoordMatrix();
                    }
                }
            }
            else {
                Debug.Log("Coord does not exist");
                StopMoving();
            }
        }

        private void UpdateCoordMatrix() {
            Floor.SetCoordinatesContent(lastCoord, null, Entity.NONE);
            lastCoord = Global.GetObjectCoordinates(gameObject);
            Floor.SetCoordinatesContent(lastCoord, gameObject, Entity.BOMB);
        }

        public void StopMoving() {
            stateController.SetState(State.NORMAL);
            this.movingDirection = Direction.NONE;
        }

        private Vector3 GetMovementOffset(Direction movingDirection) {
            const float BOMB_SPEED = 1.0f;
            switch (movingDirection) {
                case Direction.NORTH: return new Vector3(0, 0, BOMB_SPEED);
                case Direction.EAST: return new Vector3(BOMB_SPEED, 0, 0);
                case Direction.SOUTH: return new Vector3(0, 0, -BOMB_SPEED);
                case Direction.WEST: return new Vector3(-BOMB_SPEED, 0, 0);
                default: return new Vector3(0, 0, 0);
            }
        }
        private Vector3 GetMovementOffset(Direction movingDirection, int BOMB_SPEED) {
            switch (movingDirection) {
                case Direction.NORTH: return new Vector3(0, 0, BOMB_SPEED);
                case Direction.EAST: return new Vector3(BOMB_SPEED, 0, 0);
                case Direction.SOUTH: return new Vector3(0, 0, -BOMB_SPEED);
                case Direction.WEST: return new Vector3(-BOMB_SPEED, 0, 0);
                default: return new Vector3(0, 0, 0);
            }
        }

        private Vector2Int? GetNextCoord(Vector2Int coord, Direction dir) {
            switch(dir) {
                case Direction.NORTH: {
                    var nextCoord = new Vector2Int(coord.x, coord.y+1);
                    if (Floor.GetCoordExists(nextCoord)) return nextCoord;
                    else return null;
                }
                case Direction.EAST: {
                    var nextCoord = new Vector2Int(coord.x+1, coord.y);
                    if (Floor.GetCoordExists(nextCoord)) return nextCoord;
                    else return null;
                }
                case Direction.SOUTH: {
                    var nextCoord = new Vector2Int(coord.x, coord.y-1);
                    if (Floor.GetCoordExists(nextCoord)) return nextCoord;
                    else return null;
                }
                case Direction.WEST: {
                    var nextCoord = new Vector2Int(coord.x-1, coord.y);
                    if (Floor.GetCoordExists(nextCoord)) return nextCoord;
                    else return null;
                }
                default: return coord;
            }
        }

        public bool CheckNextCoord(Direction dir) {
            Vector2Int nowCoord = Global.GetObjectCoordinates(gameObject);
            Vector2Int? nextCoord = GetNextCoord(nowCoord, dir);

            if (nextCoord.HasValue) {
                if (Floor.GetCoordinates(nextCoord.Value).Value.contentEntity == Entity.NONE) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public void Throw(Direction dir, int cells) {
            stateController.SetState(State.MOVING_AIR);
            movementController.Jump(cells, dir);
            gameObject.GetComponent<Collider>().isTrigger = true;
            Floor.SetCoordinatesContent(Global.GetObjectCoordinates(gameObject), null, Entity.NONE);
        }

        // Teleport the bomb to arena opposite side
        private void CheckTransporterBounds() {
            Vector3 pos = this.gameObject.transform.position;
            if (pos.z > (Arena.initialPos.y + 70)) { // north
                pos.z = Arena.initialPos.y - 59;
            }
            if (pos.z < (Arena.initialPos.y - 60)) { // south
                pos.z = Arena.initialPos.y + 64.5f;
            }
            if (pos.x > (Arena.initialPos.x + 74.5f)) { // east
                pos.x = Arena.initialPos.x - 65f;
            }
            if (pos.x < (Arena.initialPos.x - 65)) { // west
                pos.x = Arena.initialPos.x + 74.5f;
            }
            gameObject.transform.position = pos;
        }

        private void OnCollisionEnter(Collision collision) {
            // Stop Moving when the bomb is moving and collides with player
            if (stateController.GetState() == State.MOVING 
                && collision.gameObject.tag == "Player") 
            {
                StopMoving();
            }
        }

        void OnTriggerEnter(Collider other) {
            if (stateController.GetState() == State.MOVING_AIR) {
                switch (other.gameObject.tag) {
                    case "Player": {
                        if (movementController.internalState == MovementController.InternalState.JUMP_DESCEND) {
                            // Stun the player
                            other.gameObject.GetComponent<Player.PlayerController>().stateController.AddStunTime(1.0f);
                            Throw(movementController.movementDirection, 1);
                        }
                    } break;
                    case "Hard Block": {
                        Throw(movementController.movementDirection, 1);
                    } break;
                    case "Soft Block": {
                        Throw(movementController.movementDirection, 1);
                    } break;
                    case "Bomb":
                        Throw(movementController.movementDirection, 1);
                    break;
                }
            }
        }

    }
}