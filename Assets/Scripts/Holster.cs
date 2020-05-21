
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using VRTK.GrabAttachMechanics;

namespace MetroVR {

    public class Holster : MonoBehaviour {

        public string HolsterSlotName;
        public GameObject heldObject;
        public bool lockHeldObject = false;
        public ItemTypes allowedTypes;

        public float triggerRadius = 0.1f;

        protected GameObject currentClosestHand;

        [SerializeField] protected SphereCollider sphereCollider;

        protected virtual void Start () {
            sphereCollider = GetComponent<SphereCollider> () == null ? gameObject.AddComponent<SphereCollider> () : GetComponent<SphereCollider> ();

            sphereCollider.radius = triggerRadius;
            sphereCollider.isTrigger = true;
            if (heldObject != null)
                heldObject.SetActive (false);
        }

        protected virtual void OnTriggerEnter (Collider collider) {
            if (collider.tag == "Hand") {
                PlayerInventory.Instance.HandEnteredInventoryHolster (collider.gameObject, this);
                currentClosestHand = collider.gameObject;
            }
        }

        protected virtual void OnTriggerExit (Collider collider) {
            if (currentClosestHand == collider.gameObject) {
                PlayerInventory.Instance.HandExitedInventoryHolster (collider.gameObject, this);
            }
        }

    }

}
