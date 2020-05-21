using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

namespace MetroVR {

    public class JumpControl : VRTK_BaseObjectControlAction {

        [Header ("Jump stats")]
        public float jumpPower = 5f;

        Rigidbody rb;
        GameObject controlledGameObject;

        protected override void Awake () {
            base.Awake ();
            objectControlScript.StartingEvent += ObjectControlScript_StartingEvent;
        }

        protected override void OnDisable () {
            base.OnDisable ();
            objectControlScript.StartingEvent -= ObjectControlScript_StartingEvent;
        }

        void ObjectControlScript_StartingEvent (object sender, StartEventArgs e) {
            rb = e.controlledGameObject.GetComponent<Rigidbody> ();
            controlledGameObject = e.controlledGameObject;
        }

        bool IsGrounded () {
            Debug.Log ("Is grounded?");
            Debug.DrawLine (controlledGameObject.transform.position, Vector3.down * .2f, Color.red, 1f);
            return Physics.Raycast (controlledGameObject.transform.position, Vector3.down, 0.2f);
        }

        protected override void Process (GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive) {
            if (axis > 0.8f) {
                if (IsGrounded ()) {
                    rb.AddForce (0, jumpPower, 0, ForceMode.Impulse);
                }
            }
        }

    }

}
