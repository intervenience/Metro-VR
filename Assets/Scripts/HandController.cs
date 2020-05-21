
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VRTK;
using VRTK.GrabAttachMechanics;

namespace MetroVR {

    public enum HandLeftOrRight {
        Left,
        Right
    }

    public class HandController : MonoBehaviour {

        public delegate void PickedUpRangedWeapon (Item item, HandLeftOrRight leftOrRight);
        public static event PickedUpRangedWeapon OnPickedUpRangedWeapon;

        [SerializeField] HandLeftOrRight activeHand;
        public HandLeftOrRight LeftOrRight {
            get {
                return activeHand;
            }
        }
        [SerializeField] GameObject targetHand;
        [SerializeField] Animator animator;

        public Item HeldItem {
            get {
                return interactGrab.GrabbedObject == null ? interactGrab.GrabbedObject.GetComponent <Item> () : null;
            }
        }

        VRTK_InteractGrab interactGrab;
        public VRTK_InteractGrab InteractGrab {
            get {
                return interactGrab;
            }
        }

        VRTK_ControllerEvents events;
        public VRTK_ControllerEvents Events {
            get {
                return events;
            }
        }

        void Awake () {
            animator = GetComponent<Animator> ();
            interactGrab = targetHand.GetComponent<VRTK_InteractGrab> ();
            events = targetHand.GetComponent <VRTK_ControllerEvents> ();
        }

        void OnEnable () {
            if (activeHand == HandLeftOrRight.Left) {
                targetHand = GameObject.FindGameObjectWithTag ("Left");
            } else {
                targetHand = GameObject.FindGameObjectWithTag ("Right");
            }
            interactGrab.ControllerGrabInteractableObject += InteractGrab_ControllerGrabInteractableObject;

            events.ButtonOnePressed += Events_ButtonOnePressed;
            events.GripPressed += Events_GripPressed;
            events.GripReleased += Events_GripReleased;
        }

        void InteractGrab_ControllerGrabInteractableObject (object sender, ObjectInteractEventArgs e) {
            var item = e.target.GetComponent<Item> ();
            if (item != null) {
                if (item.itemType == ItemTypes.LargeWeapon || item.itemType == ItemTypes.SmallWeapon) {
                    if (OnPickedUpRangedWeapon != null) {
                        OnPickedUpRangedWeapon.Invoke (item, activeHand);
                    }
                }
            }
        }

        void OnDisable () {
            interactGrab.ControllerGrabInteractableObject -= InteractGrab_ControllerGrabInteractableObject;
            events.ButtonOnePressed -= Events_ButtonOnePressed;
            events.GripPressed -= Events_GripPressed;
            events.GripReleased -= Events_GripReleased;
        }

        void Events_ButtonOnePressed (object sender, ControllerInteractionEventArgs e) {
            if (interactGrab.GrabbedObject != null) {
                var item = interactGrab.GrabbedObject.GetComponent<Item> ();
                if (item == null) {
                    return;
                }
                switch (item.itemName) {
                    case "Kalashnikov":
                    case "Kalash 2012":
                    case "Bastard gun":
                        item.gameObject.GetComponent<AutomaticGun> ().ReleaseMagazine ();
                        break;
                    case "Shotgun":
                        item.gameObject.GetComponent<Shotgun> ().TurnRevolver ();
                        break;
                }
            }
        }
        
        void Events_GripPressed (object sender, ControllerInteractionEventArgs e) {
            animator.SetBool ("Grip", true);
        }

        void Events_GripReleased (object sender, ControllerInteractionEventArgs e) {
            animator.SetBool ("Grip", false);
        }

    }

}
