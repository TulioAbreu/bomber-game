using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomb {
    public class DefaultBomb : BombController {
        // Check explosion collision with each type of Entity
        protected override bool ExplosionLogicAt(Vector2Int coord) {
            if (coord.x < 0 || coord.x > 12 ||
                coord.y < 0 || coord.y > 10)
            {
                return true;
            }
            Entity posEntity = Floor.GetCoordinates(coord).GetValueOrDefault().contentEntity;

            switch (posEntity) {
            case Entity.NONE: {
                base.CreateExplosionAtPos(new Vector2Int(coord.x, coord.y));
            } break;
            case Entity.PLAYER: {
                base.CreateExplosionAtPos(new Vector2Int(coord.x, coord.y));
            } break;
            case Entity.HARD_BLOCK: {
                return true;
            };
            case Entity.SOFT_BLOCK: {
                base.CreateExplosionAtPos(new Vector2Int(coord.x, coord.y));
                return true;
            };
            case Entity.BOMB: {
                base.CreateExplosionAtPos(new Vector2Int(coord.x, coord.y));
                return true;
            };
            case Entity.POWER_UP: {
                return true;
            }
            }
            return false;
        }
    }
}