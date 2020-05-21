
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR {

    public class ShotgunMagazine : MonoBehaviour {

        [SerializeField] int[] ammoSlots = new int[] { 1, 1, 1, 1, 1, 1 };
        [SerializeField] GameObject[] ammoObjects;

        [SerializeField] Shotgun shotgun;

        public bool CanFire () {
            return ammoSlots[0] == 1 ? true : false;
        }

        public void TurnRevolver () {
            if (!turning)
                StartCoroutine (TurnRevolverRoutine ());
        }

        public T[] ShiftLeft<T> (T[] array) {
            T [] updated = new T [array.Length];

            for (int i = 0; i < array.Length - 1; i++) {
                updated[i] = array[i + 1];
            }

            updated[updated.Length - 1] = array[0];

            return updated;
        }

        public T[] ShiftRight<T> (T[] array) {
            T[] updated = new T[array.Length];

            for (int i = 1; i < array.Length ; i++) {
                updated[i] = array[i - 1];
            }
            updated[0] = array[array.Length - 1];

            return updated;
        }

        public void Shoot () {
            ammoSlots[0] = 0;
            ammoObjects[0].SetActive (false);
        }

        bool turning = false;

        IEnumerator TurnRevolverRoutine () {
            Vector3 targetRotation = transform.localRotation.eulerAngles + new Vector3 (0, 60, 0);

            float elapsed = 0;
            turning = true;
            while (elapsed < 0.1f) {
                transform.localRotation = Quaternion.Lerp (transform.localRotation, Quaternion.Euler (targetRotation), elapsed / 0.1f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            turning = false;
            ammoSlots = ShiftLeft (ammoSlots);
            ammoObjects = ShiftRight (ammoObjects);
            //ammoObjects = ShiftLeft (ammoObjects);
        }

        public void AmmoInRangeOfRevolverSegment (int segment, ShotgunTriggerEnter shotgunTriggerEnter) {
            shotgunTriggerEnter.UpdateSlotStatus (
                ammoSlots[segment] == 0 ? true : false);
        }

        public void AmmoAttachedToSegment (int segment) {
            ammoSlots[segment] = 1;
            ammoObjects[6 - segment].SetActive (true);
            shotgun.PlayReloadSound ();
        }

    }

}
