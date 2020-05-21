
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using VRTK.Controllables;
using VRTK.Controllables.PhysicsBased;

namespace MetroVR.Interactables {
       
    public class FourSwitchControlBox : MonoBehaviour {

        [SerializeField] UnityEvent onFullyChargedEvents;
        bool leftTriggered = false;
        bool clTriggered = false;
        bool crTriggered = false;
        bool rightTriggered = false;
        [SerializeField] UnityEvent leftChargeEvents, centerLeftChargeEvents, centerRightChargeEvents, rightChargeEvents;

        [SerializeField] bool isPowered = false;

        [HideInInspector] public bool leftOn, centerLeftOn, centerRightOn, rightOn;

        GameObject greenLedPrefab, redLedPrefab;
        Transform leftLedT, innerLeftLedT, innerRightLedT, rightLedT;

        [SerializeField] Material unpoweredMaterial;
        [SerializeField] Material redMaterial, greenMaterial;
        [SerializeField] MeshRenderer leftLed, centerLeftLed, centerRightLed, rightLed;
        [SerializeField] VRTK_PhysicsRotator leftRotator, centerLeftRotator, centerRightRotator, rightRotator;
        [SerializeField] float charge = 0f;

        GameObject connectedChargerObject = null;
        [SerializeField] BatteryCharger connectedCharger = null;

        void Start () {
            //Have to separate these otherwise you get some weird interaction between the objects.
            leftRotator.MaxLimitReached += LeftRotator_LimitReached;
            leftRotator.MinLimitReached += LeftRotator_MinLimitReached;
            centerLeftRotator.MaxLimitReached += CenterLeftRotator_LimitReached;
            centerLeftRotator.MinLimitReached += CenterLeftRotator_MinLimitReached;
            centerRightRotator.MaxLimitReached += CenterRightRotator_LimitReached;
            centerRightRotator.MinLimitReached += CenterRightRotator_MinLimitReached;
            rightRotator.MaxLimitReached += RightRotator_LimitReached;
            rightRotator.MinLimitReached += RightRotator_MinLimitReached;

            SetRotarAndLedStatus ();
        }

        void OnDisable () {
            leftRotator.MaxLimitReached -= LeftRotator_LimitReached;
            leftRotator.MinLimitReached -= LeftRotator_MinLimitReached;
            centerLeftRotator.MaxLimitReached -= CenterLeftRotator_LimitReached;
            centerLeftRotator.MinLimitReached -= CenterLeftRotator_MinLimitReached;
            centerRightRotator.MaxLimitReached -= CenterRightRotator_LimitReached;
            centerRightRotator.MinLimitReached -= CenterRightRotator_MinLimitReached;
            rightRotator.MaxLimitReached -= RightRotator_LimitReached;
            rightRotator.MinLimitReached -= RightRotator_MinLimitReached;
        }

        void SetRotarAndLedStatus () {
            if (isPowered) {
                if (leftOn) {
                    leftLed.material = greenMaterial;
                    leftRotator.SetValue (leftRotator.angleLimits.maximum);
                    leftRotator.transform.localRotation = Quaternion.Euler (0, leftRotator.transform.localRotation.y, leftRotator.transform.localRotation.z);
                    //play sound
                    if (!leftTriggered)
                        leftChargeEvents.Invoke ();
                } else {
                    leftLed.material = redMaterial;
                }

                if (centerLeftOn) {
                    centerLeftLed.material = greenMaterial;
                    centerLeftRotator.SetValue (centerLeftRotator.angleLimits.maximum);
                    centerLeftRotator.transform.localRotation = Quaternion.Euler (0, centerLeftRotator.transform.localRotation.y, centerLeftRotator.transform.localRotation.z);
                    //play sound
                    if (!clTriggered)
                        centerLeftChargeEvents.Invoke ();
                } else {
                    centerLeftLed.material = redMaterial;
                }

                if (centerRightOn) {
                    centerRightLed.material = greenMaterial;
                    centerRightRotator.SetValue (centerRightRotator.angleLimits.maximum);
                    centerRightRotator.transform.localRotation = Quaternion.Euler (0, centerRightRotator.transform.localRotation.y, centerRightRotator.transform.localRotation.z);
                    //play sound
                    if (!crTriggered)
                        centerRightChargeEvents.Invoke ();
                } else {
                    centerRightLed.material = redMaterial;
                }

                if (rightOn) {
                    rightLed.material = greenMaterial;
                    rightRotator.SetValue (rightRotator.angleLimits.maximum);
                    rightRotator.transform.localRotation = Quaternion.Euler (0, rightRotator.transform.localRotation.y, rightRotator.transform.localRotation.z);
                    //play sound
                    if (!rightTriggered)
                        rightChargeEvents.Invoke ();
                } else {
                    rightLed.material = redMaterial;
                }

                if (leftOn && centerLeftOn && centerRightOn && rightOn) {
                    Debug.Log ("all on and powered");

                    onFullyChargedEvents.Invoke ();
                    //play sound
                }
            } else {
                if (leftOn)
                    leftRotator.transform.localRotation = Quaternion.Euler (0, leftRotator.transform.localRotation.y, leftRotator.transform.localRotation.z);
                if (centerLeftOn)
                    centerLeftRotator.transform.localRotation = Quaternion.Euler (0, centerLeftRotator.transform.localRotation.y, centerLeftRotator.transform.localRotation.z);
                if (centerRightOn)
                    centerRightRotator.transform.localRotation = Quaternion.Euler (0, centerRightRotator.transform.localRotation.y, centerRightRotator.transform.localRotation.z);
                if (rightOn)
                    rightRotator.transform.localRotation = Quaternion.Euler (0, rightRotator.transform.localRotation.y, rightRotator.transform.localRotation.z);

                leftLed.material = unpoweredMaterial;
                centerLeftLed.material = unpoweredMaterial;
                centerRightLed.material = unpoweredMaterial;
                rightLed.material = unpoweredMaterial;
            }
        }

        void RightRotator_MinLimitReached (object sender, ControllableEventArgs e) {
            Debug.Log ("RightRotator_MinLimitReached");
            rightOn = true;
            rightRotator.isLocked = true;
            SetRotarAndLedStatus ();
            StartCoroutine (RemoveInteractions (rightRotator.gameObject));
        }

        void CenterRightRotator_MinLimitReached (object sender, ControllableEventArgs e) {
            Debug.Log ("CenterRightRotator_MinLimitReached");
            centerRightOn = true;
            centerRightRotator.isLocked = true;
            SetRotarAndLedStatus ();
            StartCoroutine (RemoveInteractions (centerRightRotator.gameObject));
        }

        void CenterLeftRotator_MinLimitReached (object sender, ControllableEventArgs e) {
            Debug.Log ("CenterLeftRotator_MinLimitReached");
            centerLeftOn = true;
            centerLeftRotator.isLocked = true;
            SetRotarAndLedStatus ();
            StartCoroutine (RemoveInteractions (centerLeftRotator.gameObject));
        }

        void LeftRotator_MinLimitReached (object sender, ControllableEventArgs e) {
            Debug.Log ("LeftRotator_MinLimitReached");
            leftOn = true;
            leftRotator.isLocked = true;
            SetRotarAndLedStatus ();
            StartCoroutine (RemoveInteractions (leftRotator.gameObject));
        }

        void RightRotator_LimitReached (object sender, ControllableEventArgs e) {
            Debug.Log ("RightRotator_LimitReached");
            rightOn = true;
            rightRotator.isLocked = true;
            SetRotarAndLedStatus ();
            StartCoroutine (RemoveInteractions (rightRotator.gameObject));
        }

        void CenterRightRotator_LimitReached (object sender, ControllableEventArgs e) {
            Debug.Log ("CenterRightRotator_LimitReached");
            centerRightOn = true;
            centerRightRotator.isLocked = true;
            SetRotarAndLedStatus ();
            StartCoroutine (RemoveInteractions (centerRightRotator.gameObject));
        }

        void CenterLeftRotator_LimitReached (object sender, ControllableEventArgs e) {
            Debug.Log ("CenterLeftRotator_LimitReached");
            centerLeftOn = true;
            centerLeftRotator.isLocked = true;
            SetRotarAndLedStatus ();
            StartCoroutine (RemoveInteractions (centerLeftRotator.gameObject));
        }

        void LeftRotator_LimitReached (object sender, VRTK.Controllables.ControllableEventArgs e) {
            Debug.Log ("LeftRotator_LimitReached");
            leftOn = true;
            leftRotator.isLocked = true;
            SetRotarAndLedStatus ();
            StartCoroutine (RemoveInteractions (leftRotator.gameObject));
        }

        IEnumerator RemoveInteractions (GameObject rotar) {
            yield return new WaitForSeconds (1f);
            rotar.GetComponent<Rigidbody> ().freezeRotation = true;
            Debug.Log (Math.Round (rotar.transform.rotation.x / 90.0) *90);
            Destroy (rotar.GetComponent<VRTK.VRTK_InteractableObject> ());
            Destroy (rotar.GetComponent<VRTK.GrabAttachMechanics.VRTK_TrackObjectGrabAttach> ());
        }

        void OnTriggerEnter (Collider collider) {
            if (collider.tag == "Charger") {
                if (!isPowered) {
                    connectedChargerObject = collider.transform.parent.gameObject;
                    connectedCharger = connectedChargerObject.GetComponent<BatteryCharger> ();
                    connectedCharger.SetConnectedInteractable (gameObject);
                }
            }
        }

        void OnTriggerExit (Collider collider) {
            if (collider.transform.parent.gameObject == connectedChargerObject) {
                connectedCharger.OutOfInteractableRange (gameObject);
                connectedChargerObject = null;
            }
        }

        void AttemptCharge () {
            charge += .25f;
            charge = Mathf.Clamp (charge, 0, 1);
            if (connectedCharger != null) {
                //inform charger of current charge
            }
            if (charge >= 1) {
                isPowered = true;
                SetRotarAndLedStatus ();
                connectedCharger.OutOfInteractableRange (gameObject);
            }
        }

        void ChargerDisabled () {
            connectedCharger = null;
        }

        public void Powered (bool on) {
            isPowered = on;
        }

    }

}
