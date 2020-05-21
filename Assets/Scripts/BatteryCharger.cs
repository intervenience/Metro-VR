
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VRTK;

namespace MetroVR {

    public class BatteryCharger : Item {

        [Header ("Battery Charger")]
        [SerializeField]
        Holster myHolster;
        Animator animator;
        public float amountToCharge = 0.15f;
        public float timeBetweenUses = 0.5f;

        [SerializeField] Transform chargeNeedle;
        [SerializeField] float minRotation, maxRotation;

        Coroutine charging;

        bool canCharge = true;

        GameObject connectedInteractable = null;

        void Start () {
            animator = GetComponent<Animator> ();
        }

        protected override void OnEnable () {
            base.OnEnable ();
            SetBatteryNeedlePosition ();

            Torch.OnBatteryUpdate += Torch_OnBatteryUpdate;
        }

        protected override void OnDisable () {
            base.OnDisable ();

            Torch.OnBatteryUpdate -= Torch_OnBatteryUpdate;
        }

        public override void StartUsing (VRTK_InteractUse currentUsingObject = null) {
            base.StartUsing (currentUsingObject);

            Debug.Log ("Charger in use");
            animator.SetBool ("Charge", true);
            if (canCharge) {
                charging = StartCoroutine (ChargeSequence ());
            }
        }

        public void SetConnectedInteractable (GameObject go) {
            connectedInteractable = go;
        }

        public void OutOfInteractableRange (GameObject go) {
            if (connectedInteractable == go) {
                connectedInteractable = null;
            }
        }

        IEnumerator ChargeSequence () {
            if (connectedInteractable != null) {
                //message other device
                connectedInteractable.SendMessage ("AttemptCharge");
            } else {
                //charge my battery
                Torch.Instance.ChargeBattery ();
            }
            canCharge = false;
            yield return new WaitForSeconds (timeBetweenUses);
            canCharge = true;
        }

        public override void StopUsing (VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true) {
            base.StopUsing (previousUsingObject, resetUsingObjectState);
            animator.SetBool ("Charge", false);
        }

        public override void OnInteractableObjectUngrabbed (InteractableObjectEventArgs e) {
            base.OnInteractableObjectUngrabbed (e);

            if (connectedInteractable != null) {
                connectedInteractable.SendMessage ("ChargerDisabled");
            }

            transform.parent = myHolster.transform;
            transform.localPosition = Vector3.zero;
            gameObject.SetActive (false);
        }

        void Torch_OnBatteryUpdate () {
            SetBatteryNeedlePosition ();
        }

        void SetBatteryNeedlePosition () {
            float delta = 0;
            if (minRotation > maxRotation) { 
                delta = maxRotation - minRotation;
            } else {
                delta = minRotation - maxRotation;
            }
            StartCoroutine (MoveNeedleOverTime (minRotation + (Torch.Instance.Battery * delta)));
        }

        IEnumerator MoveNeedleOverTime (float targetRotation) {
            float elapsedTime = 0f;
            while (elapsedTime < .5f) {
                chargeNeedle.localRotation = Quaternion.Lerp (chargeNeedle.localRotation, Quaternion.Euler (chargeNeedle.localRotation.x, chargeNeedle.localRotation.y, targetRotation), elapsedTime / .5f);
                yield return null;
                elapsedTime += Time.deltaTime;
            }
        }

    }

}
