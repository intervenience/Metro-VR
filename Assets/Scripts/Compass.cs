
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VRTK;

namespace MetroVR {

    public class Compass : MonoBehaviour {

        [SerializeField] Transform compassNeedle;
        [SerializeField] Vector3 currentTargetPosition;
        [SerializeField] Levels.Level01 levelGuide;

        Transform playerHeadPosition;
    
        void Start () {
            VRTK_SDKManager.instance.LoadedSetupChanged += Instance_LoadedSetupChanged;
        }

        void Instance_LoadedSetupChanged (VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            playerHeadPosition = sender.loadedSetup.actualHeadset.transform;
        }

        void FixedUpdate () {
            if (levelGuide.currentObjective != null) {
                currentTargetPosition.x = levelGuide.currentObjective.position.x;
                currentTargetPosition.y = compassNeedle.position.y;
                currentTargetPosition.z = levelGuide.currentObjective.position.z;
                compassNeedle.LookAt (currentTargetPosition);
            }
        }

    }

}
