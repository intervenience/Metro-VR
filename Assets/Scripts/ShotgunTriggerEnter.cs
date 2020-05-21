
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR {

    public class ShotgunTriggerEnter : MonoBehaviour {

        Material m;
        MeshRenderer mesh;
        [SerializeField] ShotgunMagazine magazine;
        [SerializeField] int mySegmentNumber;

        Item closestItem;

        VRTK.VRTK_ControllerEvents closestControllerEvents;

        void Start () {
            mesh = GetComponent<MeshRenderer> ();
            m = mesh.material;
        }

        void OnTriggerEnter (Collider collider) {
            if (collider.tag == "Magazine") {
                closestItem = collider.gameObject.GetComponent<Item> ();

                if (closestItem.itemName == "Shotgun Shell") {
                    magazine.AmmoInRangeOfRevolverSegment (mySegmentNumber, this);
                    closestControllerEvents = closestItem.GetGrabbingObject ().GetComponent<VRTK.VRTK_ControllerEvents> ();
                    closestControllerEvents.GripReleased += ClosestControllerEvents_GripReleased;
                }
            }
        }

        void OnTriggerExit (Collider collider) {
            if (collider.tag == "Magazine") {
                var item = collider.gameObject.GetComponent<Item> ();

                if (item == closestItem) {
                    mesh.enabled = false;
                    closestControllerEvents.GripReleased -= ClosestControllerEvents_GripReleased; 
                    closestControllerEvents = null;
                }
            }
        }

        public void UpdateSlotStatus (bool free) {
            m.color = free ? Color.green : Color.red;
            mesh.enabled = true;
        }

        void ClosestControllerEvents_GripReleased (object sender, VRTK.ControllerInteractionEventArgs e) {
            if (m.color == Color.green) {
                magazine.AmmoAttachedToSegment (mySegmentNumber);
                Destroy (closestItem.gameObject);
            }

            //Have to remove event listener here otherwise you can infinitely spawn in the same slot
            closestControllerEvents.GripReleased -= ClosestControllerEvents_GripReleased;
            closestControllerEvents = null;
            mesh.enabled = false;
        }
    }

}
