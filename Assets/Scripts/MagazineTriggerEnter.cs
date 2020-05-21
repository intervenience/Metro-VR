
using UnityEngine;

using VRTK;

namespace MetroVR {

    public class MagazineTriggerEnter : MonoBehaviour {

        VRTK_ControllerEvents closestControllerEvents;
        [SerializeField] AutomaticGun gun;

        void Start () {
            if (gun == null) {
                gun = GetComponentInParent<AutomaticGun> ();
            }
        }

        void OnDisable () {
            if (closestControllerEvents != null)
                closestControllerEvents.GripReleased -= ClosestControllerEvents_GripReleased;
        }

        Magazine nearbyMagazine;

        void OnTriggerEnter (Collider collider) {
            if (collider.tag == "Magazine") {
                Debug.Log ("Got magazine");
                var m = collider.GetComponentInParent<Magazine> ();
                nearbyMagazine = m;

                if (m.itemName == "AK Magazine" && gun.itemName == "Kalashnikov") {
                    Debug.Log ("matches ak");
                    closestControllerEvents = m.GetGrabbingObject ().GetComponent<VRTK_ControllerEvents> ();
                    closestControllerEvents.GripReleased += ClosestControllerEvents_GripReleased;
                }
            }
        }

        void ClosestControllerEvents_GripReleased (object sender, ControllerInteractionEventArgs e) {
            Debug.Log ("Detected grip release");
            gun.AttachMagazine (nearbyMagazine.gameObject);

            closestControllerEvents.GripReleased -= ClosestControllerEvents_GripReleased;
            closestControllerEvents = null;
        }

        void OnTriggerExit (Collider collider) {
            if (nearbyMagazine != null) {
                if (collider.gameObject == nearbyMagazine.gameObject) {

                    closestControllerEvents.GripReleased -= ClosestControllerEvents_GripReleased;
                    closestControllerEvents = null;
                }
            }
        }

    }

}
