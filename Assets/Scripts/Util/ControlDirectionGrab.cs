
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VRTK;
using VRTK.SecondaryControllerGrabActions;

namespace MetroVR.Util {

    public class ControlDirectionGrab : VRTK_BaseGrabAction {

        public float ungrabDistance = 1f;
        public float releaseSnapSpeed = 0.1f;

        protected Vector3 initialPosition;
        protected Quaternion initialRotation;
        protected Quaternion initialCameraRigRotation;
        protected Quaternion releaseRotation;
        Quaternion offset;
        protected Coroutine snappingOnRelease;

        public ConfigurableJoint joint;
        public Transform cameraRig;
        //SoftJointLimit limit;

        void OnEnable () {
            VRTK_SDKManager.instance.LoadedSetupChanged += Instance_LoadedSetupChanged;
        }

        void OnDisable () {
            VRTK_SDKManager.instance.LoadedSetupChanged -= Instance_LoadedSetupChanged;
        }

        void Instance_LoadedSetupChanged (VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            cameraRig = VRTK_SDKManager.instance.loadedSetup.actualBoundaries.transform;
        }

        public override void Initialise (VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint) {
            base.Initialise (currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
            initialPosition = currentGrabbdObject.transform.localPosition;
            initialRotation = currentGrabbdObject.transform.localRotation;

            initialCameraRigRotation = cameraRig.localRotation;

            offset = Quaternion.Inverse (initialCameraRigRotation);

            joint = currentGrabbdObject.GetComponent<ConfigurableJoint> ();
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularXMotion = ConfigurableJointMotion.Free;
            //joint.angularYMotion = ConfigurableJointMotion.Free;
            //limit = joint.angularYLimit;
            //limit.limit = 105f;

            StopRealignOnRelease ();
        }

        protected virtual void StopRealignOnRelease () {
            if (snappingOnRelease != null) {
                StopCoroutine (snappingOnRelease);
            }
            snappingOnRelease = null;
        }

        public override void ResetAction () {
            releaseRotation = transform.localRotation;
            if (!grabbedObject.grabAttachMechanicScript.precisionGrab) {
                if (releaseSnapSpeed < float.MaxValue && releaseSnapSpeed > 0) {
                    snappingOnRelease = StartCoroutine (RealignOnRelease ());
                } else if (releaseSnapSpeed == 0) {
                    transform.localRotation = initialRotation;
                }
            }

            //transform.localRotation = initialRotation;
            transform.localPosition = initialPosition;
            joint.targetRotation = initialRotation;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularXMotion = ConfigurableJointMotion.Limited;

            base.ResetAction ();
            joint = null;
        }

        protected virtual IEnumerator RealignOnRelease () {
            float elapsedTime = 0f;

            while (elapsedTime < releaseSnapSpeed) {
                transform.localRotation = Quaternion.Lerp (releaseRotation, initialRotation, (elapsedTime / releaseSnapSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.localRotation = initialRotation;
            transform.localPosition = initialPosition;
            joint.targetRotation = initialRotation;
        }

        public override void OnDropAction () {
            base.OnDropAction ();
            StopRealignOnRelease ();
        }

        public override void ProcessUpdate () {
            base.ProcessUpdate ();
            CheckForceStopDistance (ungrabDistance);
        }

        public override void ProcessFixedUpdate () {
            base.ProcessFixedUpdate ();

            //transform.rotation = Quaternion.LookRotation (secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position, primaryGrabbingObject.transform.TransformDirection (Vector3.forward));

            if (initialised) {
                Vector3 forward = (secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position).normalized;

                // calculate rightLocked rotation
                Quaternion rightLocked = Quaternion.LookRotation (forward, Vector3.Cross (-primaryGrabbingObject.transform.right, forward).normalized);
                Debug.Log ("FORWARD = " + rightLocked.eulerAngles.x + " " + rightLocked.eulerAngles.y + " " + rightLocked.eulerAngles.z);

                // delta from current rotation to the rightLocked rotation
                Quaternion rightLockedDelta = Quaternion.Inverse (grabbedObject.transform.rotation) * rightLocked;

                float rightLockedAngle;
                Vector3 rightLockedAxis;

                // forward direction and roll
                rightLockedDelta.ToAngleAxis (out rightLockedAngle, out rightLockedAxis);

                if (rightLockedAngle > 180f) {
                    // remap ranges from 0-360 to -180 to 180
                    rightLockedAngle -= 360f;
                }

                // make any negative values into positive values;
                rightLockedAngle = Mathf.Abs (rightLockedAngle);

                // calculate upLocked rotation
                Quaternion upLocked = Quaternion.LookRotation (forward, primaryGrabbingObject.transform.forward);

                // delta from current rotation to the upLocked rotation
                Quaternion upLockedDelta = Quaternion.Inverse (grabbedObject.transform.rotation) * upLocked;

                float upLockedAngle;
                Vector3 upLockedAxis;

                // forward direction and roll
                upLockedDelta.ToAngleAxis (out upLockedAngle, out upLockedAxis);

                // remap ranges from 0-360 to -180 to 180
                if (upLockedAngle > 180f) {
                    upLockedAngle -= 360f;
                }

                // make any negative values into positive values;
                upLockedAngle = Mathf.Abs (upLockedAngle);

                upLocked *= Quaternion.Inverse (cameraRig.localRotation * Quaternion.Inverse (initialCameraRigRotation));
                rightLocked *= Quaternion.Inverse (cameraRig.localRotation * Quaternion.Inverse (initialCameraRigRotation));

                joint.targetRotation = (upLockedAngle < rightLockedAngle ? Quaternion.Inverse (upLocked) : Quaternion.Inverse (rightLocked));
                Debug.Log ("TARGET ROTATION = " + joint.targetRotation.eulerAngles.x + " " + joint.targetRotation.eulerAngles.y + " " + joint.targetRotation.eulerAngles.z);
                Debug.Log ("ACTUAL ROTATION = " + grabbedObject.transform.localRotation.eulerAngles.x + " " + grabbedObject.transform.localRotation.eulerAngles.y + " " + grabbedObject.transform.localRotation.eulerAngles.z);
                
            }

        }

    }

}
