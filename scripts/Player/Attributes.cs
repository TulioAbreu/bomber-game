using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class Attributes {
        const int MAX_ATTRIBUTE_LEVEL = 8;
        PlayerController playerController;

        int healthPoints;
        int speedLevel;
        int extraRadius;
        int maxBombSlots;
        bool canKickBombs;
        bool canThrowBombs;
        bool canPunchBombs;

        public Attributes(PlayerController playerController) {
            this.playerController = playerController;
            healthPoints = 1;
            speedLevel = 0;
            extraRadius = 0;
            maxBombSlots = 1;
            canKickBombs = false;
            canThrowBombs = false;
            canPunchBombs = false;
        }

        public int GetHealthPoints() {
            return healthPoints;
        }

        public void AddHealthPoint() {
            healthPoints += 1;
        }

        public void RemoveHealthPoint() {
            if (playerController.stateController.GetState() != State.IMMORTAL) {
                healthPoints -= 1;
            }
        }

        public int GetSpeedLevel() { 
            return speedLevel; 
        }
        
        public void AddSpeedLevel(int value=1) { 
            if (speedLevel >= MAX_ATTRIBUTE_LEVEL) {
                return;
            }

            speedLevel += value;

            if (speedLevel > MAX_ATTRIBUTE_LEVEL) {
                speedLevel = MAX_ATTRIBUTE_LEVEL;
            } 
        }

        public int GetExtraRadius() {
            return extraRadius;
        }
        
        public void AddExtraRadius(int value=1) {
            if (extraRadius >= MAX_ATTRIBUTE_LEVEL) {
                return;
            }

            extraRadius += value;

            if (extraRadius > MAX_ATTRIBUTE_LEVEL) {
                extraRadius = MAX_ATTRIBUTE_LEVEL;
            }
        }

        public int GetMaxBombSlots() { 
            return maxBombSlots; 
        }

        public void AddMaxBombSlots(int value=1) {
            if (maxBombSlots >= MAX_ATTRIBUTE_LEVEL) {
                return;
            }

            maxBombSlots += value;

            if (maxBombSlots > MAX_ATTRIBUTE_LEVEL) {
                maxBombSlots = MAX_ATTRIBUTE_LEVEL;
            }
        }

        public bool CanKickBombs() {
            return canKickBombs;
        }

        public void SetCanKickBombs(bool value=true) {
            canKickBombs = value;
        }

        public bool CanThrowBombs() {
            return canThrowBombs;
        }

        public void SetCanThrowBombs(bool value=true) {
            canThrowBombs = value;
        }

        public bool CanPunchBombs() {
            return canPunchBombs;
        }

        public void SetCanPunchBombs(bool value=true) {
            canPunchBombs = value;
        }

    };
}