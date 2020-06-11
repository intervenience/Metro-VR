
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using VRTK;

namespace MetroVR {

    public enum AmmoType {
        FiveFiveSix,
        SevenSixTwo,
        ShotgunShell
    }

    public enum Handedness {
        Left,
        Right
    }

    public class PlayerInventory : MonoBehaviour {

        public static PlayerInventory Instance;

        public Transform head, torso, legs;

        public Transform mainGunSlotLeft, mainGunSlotRight, hipSlotLeft, hipSlotRight, ammoSlot, chargerSlot, maskSlot;

        public GameObject ammoObj;

        [SerializeField] GameObject akMagPrefab, shotgunShellPrefab;
        [SerializeField] GameObject filterPrefab;

        public int fivefivesixAmmo = 120;
        public int sevensixtwoAmmo = 270;
        public int shotgunShells = 120;
        public int maxFivefivesix = 120;
        public int maxSevenSixTwo = 270;
        public int maxShotgun = 120;

        [SerializeField] Transform canvasTransform;
        [SerializeField] Text leftHandInfo, rightHandInfo;

        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip handInBagClip, handExitBagClip, ammoRestockClip;

        void Awake () {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy (gameObject);
            }

            leftHandInfo.text = "";
            rightHandInfo.text = "";
        }

        GameObject leftController, rightController;
        VRTK_ControllerEvents leftControllerEvents, rightControllerEvents;

        void OnEnable () {
            VRTK_SDKManager.instance.LoadedSetupChanged += VRTK_LoadedSetupChanged;
            HandController.OnPickedUpRangedWeapon += HandController_OnPickedUpRangedWeapon;
            PlayerMovement.OnPlayerCollidersSetUp += PlayerMovement_OnPlayerCollidersSetUp;

            leftController = GameObject.FindGameObjectWithTag ("Left");
            leftControllerEvents = leftController.GetComponent<VRTK_ControllerEvents> ();
            leftControllerEvents.GripPressed += LeftController_GripPressed;
            leftControllerEvents.GripReleased += LeftController_GripReleased;

            rightController = GameObject.FindGameObjectWithTag ("Right");
            rightControllerEvents = rightController.GetComponent<VRTK_ControllerEvents> ();
            rightControllerEvents.GripPressed += RightController_GripPressed;
            rightControllerEvents.GripReleased += RightController_GripReleased;
        }

        void OnDisable () {
            VRTK_SDKManager.instance.LoadedSetupChanged -= VRTK_LoadedSetupChanged;
            HandController.OnPickedUpRangedWeapon -= HandController_OnPickedUpRangedWeapon;
            PlayerMovement.OnPlayerCollidersSetUp -= PlayerMovement_OnPlayerCollidersSetUp;

            rightControllerEvents.GripPressed -= RightController_GripPressed;
            rightControllerEvents.GripReleased -= RightController_GripReleased;

            leftControllerEvents.GripPressed -= LeftController_GripPressed;
            leftControllerEvents.GripReleased -= LeftController_GripReleased;
        }

        Item heldWeapon;

        void HandController_OnPickedUpRangedWeapon (Item item, HandLeftOrRight leftOrRight) {
            //We destroy the old ammo piece here though
            //Debug.Log ("Picked up a ranged weapon");
            /*if (item.GetComponent <Rigidbody> () != null) {
                item.GetComponent<Rigidbody> ().useGravity = true;
            }
            if (ammoObj != null)
                Destroy (ammoObj);
            heldWeapon = item;
            //Debug.Log (heldWeapon);
            SpawnCorrectAmmo ();*/
        }

        void SpawnCorrectAmmo () {
            //Debug.Log (heldWeapon.itemName);
            //We don't destroy the old ammo piece here in case it is currently being held by the player
            switch (heldWeapon.itemName) {
                case "Kalashnikov":
                    if (sevensixtwoAmmo > 0) {
                        ammoObj = Instantiate (akMagPrefab, ammoSlot);
                        ammoObj.transform.localPosition = Vector3.zero;
                        var mag = ammoObj.GetComponent<Magazine> ();
                        var rounds = sevensixtwoAmmo < mag.MaxAmmo ? sevensixtwoAmmo : mag.MaxAmmo;
                        mag.currentAmmo = rounds;
                        sevensixtwoAmmo -= rounds;
                    }
                    break;
                case "Shotgun":
                    if (shotgunShells > 0) {
                        ammoObj = Instantiate (shotgunShellPrefab, ammoSlot);
                        ammoObj.transform.localPosition = Vector3.zero;
                        shotgunShells -= 1;
                    }
                    break;
            }
            ammoObj.SetActive (false);
        }

        public void HandExitedInventoryHolster (GameObject hand, Holster holster) {
            if (holster.transform.childCount > 0) {
                holster.transform.GetChild (0).gameObject.SetActive (false);
            }

            switch (hand.GetComponent <HandController> ().LeftOrRight) {
                case HandLeftOrRight.Left:
                    leftHandInfo.text = "";
                    break;
                case HandLeftOrRight.Right:
                    rightHandInfo.text = "";
                    break;
            }
        }

        public void HandEnteredInventoryHolster (GameObject hand, Holster holster) {
            if (hand.tag == "Hand") {
                HandController handController = hand.GetComponent<HandController> ();
                //Item i = handController.HeldItem;
                var itemObj = handController.InteractGrab.GrabbedObject;
                Item i;
                try {
                    i = handController.InteractGrab.GrabbedObject.GetComponent<Item> ();
                } catch (System.Exception e) {
                    i = null;
                }

                //If the user is not holding an item
                if (i == null) {
                    if (holster.HolsterSlotName == "Ammo Pouch") {
                        if (ammoObj != null) {
                            switch (handController.LeftOrRight) {
                                case HandLeftOrRight.Left:
                                    leftItemToMove = ammoObj;
                                    leftHandInfo.text = ammoObj.GetComponent<Item> ().itemName;
                                    break;
                                case HandLeftOrRight.Right:
                                    rightItemToMove = ammoObj;
                                    rightHandInfo.text = ammoObj.GetComponent<Item> ().itemName;
                                    break;
                            }
                        }
                    } else if (holster.HolsterSlotName == "Charger Holster") {
                        if (handController.LeftOrRight == HandLeftOrRight.Right) {
                            rightHandInfo.text = "Charger";
                            //chargerSlot.transform.GetChild (0).gameObject.SetActive (true);
                        }
                    } else {
                        //We expect there to be only 1 child gameobject so we only grab info from the first
                        if (holster.transform.childCount > 0) {
                            holster.transform.GetChild (0).gameObject.SetActive (true);
                            switch (handController.LeftOrRight) {
                                case HandLeftOrRight.Left:
                                    leftItemToMove = holster.transform.GetChild (0).gameObject;
                                    leftHandInfo.text = holster.transform.GetChild (0).gameObject.GetComponent<Item> ().itemName;
                                    break;
                                case HandLeftOrRight.Right:
                                    rightItemToMove = holster.transform.GetChild (0).gameObject;
                                    rightHandInfo.text = holster.transform.GetChild (0).gameObject.GetComponent<Item> ().itemName;
                                    break;
                            }
                        }
                    }
                } else {
                    //if they're holding an item
                    //check if it is an allowed type for the inventory slot
                    if (i.itemType == ItemTypes.Misc && holster.HolsterSlotName == "Ammo Pouch") {
                        /*var ammoCheck = i.GetComponent<Magazine> ();
                        if (ammoCheck != null) {
                            var remainder = AddAmmo (ammoCheck.AmmoType, ammoCheck.currentAmmo);
                            if (remainder == 0) {
                                handController.InteractGrab.ForceRelease ();
                                Destroy (i.gameObject);
                            } else {
                                ammoCheck.currentAmmo = remainder;
                            }
                            audioSource.PlayOneShot (ammoRestockClip);
                        }*/
                    } else if (i.itemType == holster.allowedTypes) {
                        switch (handController.LeftOrRight) {
                            case HandLeftOrRight.Left:
                                leftItemToMove = i.gameObject;
                                leftHolsterToMoveItemTo = holster;
                                leftHandInfo.text = holster.HolsterSlotName;
                                break;
                            case HandLeftOrRight.Right:
                                rightItemToMove = i.gameObject;
                                rightHolsterToMoveItemTo = holster;
                                rightHandInfo.text = holster.HolsterSlotName;
                                break;
                        }
                    } else {
                        switch (handController.LeftOrRight) {
                            case HandLeftOrRight.Left:
                                leftHandInfo.text = "Invalid";
                                break;
                            case HandLeftOrRight.Right:
                                rightHandInfo.text = "Invalid";
                                break;
                        }
                    }
                }
            }
        }

        GameObject rightItemToMove, leftItemToMove;
        Holster rightHolsterToMoveItemTo, leftHolsterToMoveItemTo;

        void CheckAmmoAfterDelay () {
            StartCoroutine (CheckAmmoRoutine ());
        }

        IEnumerator CheckAmmoRoutine () {
            yield return new WaitForSeconds (0.05f);
            if (ammoObj != null) {
                if (ammoObj.transform.parent != ammoSlot) {
                    SpawnCorrectAmmo ();
                }
            }
        }

        void RightController_GripPressed (object sender, ControllerInteractionEventArgs e) {
            CheckAmmoAfterDelay ();
            if (rightItemToMove != null) {
                Debug.Log ("Grip pressed, and we found the object " + rightItemToMove);
                rightItemToMove.GetComponent<Rigidbody> ().isKinematic = false;
                var touch = rightController.GetComponent<VRTK_InteractTouch> ();
                touch.ForceTouch (rightItemToMove);
                Debug.Log (touch.TouchedObject);
                var grab = rightController.GetComponent<VRTK_InteractGrab> ();
                grab.AttemptGrab ();
                Debug.Log (grab.GrabbedObject);
                /*if (rightItemToMove.GetComponent <VRTK.GrabAttachMechanics.VRTK_BaseJointGrabAttach> ()) {
                    rightItemToMove.transform.parent = null;
                }*/
                rightItemToMove = null;
                Debug.Log ("End of grip press callback");
            }
        }

        void LeftController_GripPressed (object sender, ControllerInteractionEventArgs e) {
            CheckAmmoAfterDelay ();
            if (leftItemToMove != null) {
                Debug.Log ("Grip pressed, and we found the object " + leftItemToMove);
                leftItemToMove.GetComponent<Rigidbody> ().isKinematic = false;
                var touch = leftController.GetComponent<VRTK_InteractTouch> ();
                touch.ForceTouch (leftItemToMove);
                Debug.Log (touch.TouchedObject);
                var grab = leftController.GetComponent<VRTK_InteractGrab> ();
                grab.AttemptGrab ();
                Debug.Log (grab.GrabbedObject);
                if (leftItemToMove.GetComponent<VRTK.GrabAttachMechanics.VRTK_BaseJointGrabAttach> ()) {
                    leftItemToMove.transform.parent = null;
                }
                leftItemToMove = null;
            }
        }

        void RightController_GripReleased (object sender, ControllerInteractionEventArgs e) {
            Debug.Log ("Right controller grip released");
            if (rightItemToMove != null && rightHolsterToMoveItemTo != null) {
                Debug.Log ("Right controller is holding an object, and is in range of a holster");
                //rightItemToMoveToHolster.transform.parent = rightHolsterToMoveItemTo.transform;
                //rightItemToMoveToHolster.SetActive (false);

                MoveObjectToHolster (rightItemToMove, rightHolsterToMoveItemTo.gameObject);
                rightItemToMove = null;
            }
        }

        void LeftController_GripReleased (object sender, ControllerInteractionEventArgs e) {
            Debug.Log ("Left controller grip released");
            if (leftItemToMove != null && leftHolsterToMoveItemTo != null) {
                Debug.Log ("Left controller is holding an object, and is in range of a holster");
                //leftItemToMoveToHolster.transform.parent = leftHolsterToMoveItemTo.transform;
                //leftItemToMoveToHolster.SetActive (false);

                MoveObjectToHolster (leftItemToMove, leftHolsterToMoveItemTo.gameObject);
                leftItemToMove = null;
            }
        }

        void MoveObjectToHolster (GameObject obj, GameObject holster) {
            Debug.Log ("Moving object " + obj + " to holster " + holster);
            Debug.Log ("child count = " + holster.transform.childCount);
            if (holster.transform.childCount == 0) {
                obj.transform.SetParent (holster.transform);
                Debug.Log ("Items parent is " + obj.transform.parent);
                obj.GetComponent<VRTK_InteractableObject> ().ForceStopInteracting ();
                obj.transform.localPosition = Vector3.zero;
                var rb = obj.GetComponent<Rigidbody> ();
                rb.isKinematic = true;
                StartCoroutine (SetVelocitiesToZeroAfterDelay (rb, holster.transform));
            }
        }

        IEnumerator SetVelocitiesToZeroAfterDelay (Rigidbody rb, Transform parent) {
            yield return new WaitForSeconds (0.02f);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.transform.SetParent (parent);
        }

        public int AddAmmo (AmmoType ammoType, int amount) {
            int leftover = 0;
            switch (ammoType) {
                case AmmoType.FiveFiveSix:
                    fivefivesixAmmo += amount;
                    if (maxFivefivesix < fivefivesixAmmo) {
                        leftover = fivefivesixAmmo - maxFivefivesix;
                        fivefivesixAmmo = maxFivefivesix;
                    }
                    break;
                case AmmoType.SevenSixTwo:
                    sevensixtwoAmmo += amount;
                    if (maxSevenSixTwo < sevensixtwoAmmo) {
                        leftover = sevensixtwoAmmo - maxSevenSixTwo;
                        sevensixtwoAmmo = maxSevenSixTwo;
                    }
                    break;
                case AmmoType.ShotgunShell:
                    shotgunShells += amount;
                    if (maxShotgun < shotgunShells) {
                        leftover = shotgunShells - maxShotgun;
                        shotgunShells = maxShotgun;
                    }
                    break;
            }
            leftover = Mathf.Clamp (leftover, 0, 100);
            return leftover;
        }

        void PlayerMovement_OnPlayerCollidersSetUp (Transform torso, Transform legs) {
            this.torso = torso;
            this.legs = legs;
            mainGunSlotRight.parent = this.torso;
            mainGunSlotLeft.parent = this.torso;
            chargerSlot.parent = this.legs;
            ammoSlot.parent = this.legs;

            //X is the horizontal location,
            //Y is the vertical location
            //Z is the forward location
            //mainGunSlotRight.position = this.torso.position + new Vector3 (0.2f, 0.05f, -.08f);
            //mainGunSlotLeft.position = this.torso.position + new Vector3 (-0.2f, 0.05f, -.08f);
            //chargerSlot.position = this.torso.position + new Vector3 (0.085f, 0.74f * this.torso.localPosition.y, 0.25f);
            //ammoSlot.position = this.torso.position + new Vector3 (-0.085f, 0.74f * this.torso.localPosition.y, 0.25f);
            mainGunSlotRight.localPosition = Vector3.zero;
            mainGunSlotLeft.localPosition = Vector3.zero;
            chargerSlot.localPosition = Vector3.zero;
            ammoSlot.localPosition = Vector3.zero;
        }

        void VRTK_LoadedSetupChanged (VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            head = sender.loadedSetup.actualHeadset.transform;

            canvasTransform.parent = VRTK_SDKManager.instance.loadedSetup.actualHeadset.transform;
            canvasTransform.localPosition = new Vector3 (0, 0, 0.65f);
        }

        void FixedUpdate () {
            if (torso != null) {
                //mainGunSlotRight.localPosition = new Vector3 (0.211f, 0.143f, 1f);
                //mainGunSlotLeft.localPosition = new Vector3 (-0.211f, 0.143f, 1f);
                mainGunSlotRight.localPosition = new Vector3 (0.211f, .2f, head.localPosition.y * 1.05f - torso.localPosition.y);
                mainGunSlotLeft.localPosition = new Vector3 (-0.211f, .2f, head.localPosition.y * 1.05f - torso.localPosition.y);
                chargerSlot.localPosition = new Vector3 (0.2f, 0.8f, 0.1f);
                ammoSlot.localPosition = new Vector3 (-0.2f, 0.8f, 0.1f);
            }
        }

    }

}

