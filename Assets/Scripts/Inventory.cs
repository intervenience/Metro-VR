
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VRTK;

namespace MetroVR {

    public class Inventory : MonoBehaviour {

        public static Inventory Instance;

        public int largeSlotCount = 2;
        public int smallSlotCount = 4;

        public Transform playspace, head;
        public Transform inventoryContainer;
        public Material transparentMaterial;
        public float largeHorizontalSteps = 0.2f;
        public float smallHorizontalSteps = 0.1f;

        public GameObject snapZonePrefab, snapZoneChargerPrefab, snapZoneAkMagPrefab, snapZoneShotgunShellPrefab;

        public Transform[] largeSlots, smallSlots;

        void Awake () {
            if (Instance == null) {
                Instance = this;
            }
        }

        void LateUpdate () {
            if (head != null) {
                if ((head.localRotation.eulerAngles.x > 15f && head.localRotation.eulerAngles.x < 345f) ||
                        (head.localRotation.eulerAngles.z > 15f && head.localRotation.eulerAngles.z < 345f)) {

                } else {
                    inventoryContainer.rotation = Quaternion.Euler (0, head.rotation.eulerAngles.y, 0);
                    for (int i = 0; i < largeSlotCount; i++) {
                        largeSlots[i].localPosition = new Vector3 (head.localPosition.x + (-1 + 2 * i) * largeHorizontalSteps, 0.65f * head.localPosition.y, head.localPosition.z + 0.15f);
                    }
                    for (int i = 0; i < smallSlotCount; i++) {
                        smallSlots[i].localPosition = new Vector3 (head.localPosition.x + ((-4 + 2 * i) * smallHorizontalSteps) / 2, 0.5f * head.localPosition.y, head.localPosition.z + 0.15f);
                    }
                }
            }
        }

        void OnEnable () {
            VRTK_SDKManager.instance.LoadedSetupChanged += SetupChanged;
        }

        void OnDisable () {
            VRTK_SDKManager.instance.LoadedSetupChanged -= SetupChanged;
        }

        void SetupChanged (VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            playspace = sender.loadedSetup.actualBoundaries.transform;
            head = sender.loadedSetup.actualHeadset.transform;
            Debug.Log (head.gameObject);
            inventoryContainer = Instantiate (new GameObject ("Inventory Container"), playspace).transform;

            foreach (Transform largeSlot in largeSlots) {
                largeSlot.localScale = new Vector3 (0.15f, 0.15f, 0.15f);
                largeSlot.parent = inventoryContainer;
            }

            foreach (Transform smallSlot in smallSlots) {
                smallSlot.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
                smallSlot.parent = inventoryContainer;
            }

            /*largeSlots = new Transform[largeSlotCount];

            for (int i = 0; i < largeSlotCount; i++) {
                GameObject largeZone = Instantiate (snapZonePrefab, inventoryContainer);
                largeZone.name = "Large slot " + i;
                largeZone.transform.localScale = new Vector3 (0.15f, 0.15f, 0.15f);
                largeSlots[i] = largeZone.transform;
            }

            smallSlots = new Transform[smallSlotCount];

            smallSlots[0] = Instantiate (snapZoneShotgunShellPrefab, inventoryContainer).transform;
            smallSlots[1] = Instantiate (snapZoneAkMagPrefab, inventoryContainer).transform;
            GameObject go = Instantiate (snapZonePrefab, inventoryContainer);
            go.name = "Small slot 2";
            go.transform.localScale = new Vector3 (.1f, .1f, .1f);
            smallSlots[2] = go.transform;
            smallSlots[smallSlotCount-1] = Instantiate (snapZoneChargerPrefab, inventoryContainer).transform;*/
        }
        
    }

}
