
using MetroVR.Util;

using UnityEngine;

namespace MetroVR {

    public class Magazine : Item {

        [SerializeField] AmmoType ammoType;
        public AmmoType AmmoType {
            get {
                return ammoType;
            }
        }
        [SerializeField] int maxAmmo = 0;
        public int MaxAmmo {
            get {
                return maxAmmo;
            }
        }
        public int currentAmmo = 0;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] GameObject[] roundsInMag;
        [SerializeField] Collider meshCollider;
        public Rigidbody rb;
        DissolveController dissolveController;

        protected override void Awake () {
            base.Awake ();

            dissolveController = GetComponent<DissolveController> ();
        }

        protected override void OnEnable () {
            base.OnEnable ();

            rb.useGravity = transform.parent == null ? true : false;
        }

        void Start () {
            UpdateRoundsInMag ();
        }

        public void ToggleVisibility () {
            meshRenderer.enabled = !meshRenderer.enabled;
            if (roundsInMag.Length > 0) {
                foreach (GameObject round in roundsInMag) {
                    round.SetActive (!round.activeSelf);
                }
            }
        }

        public void OnDrop () {
            PlayerInventory.Instance.AddAmmo (ammoType, currentAmmo);
            meshCollider.enabled = true;
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody> ();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.mass = 1;
            dissolveController.StartDissolve (1f);
            Destroy (gameObject, 2f);
        }

        public void AddAmmo (int currentAmount, out int amountToRemove) {
            amountToRemove = 0;

            currentAmmo += Mathf.Clamp (currentAmount, 0, maxAmmo);
            amountToRemove = currentAmmo;
        }

        public void RemoveRound () {
            currentAmmo -= 1;
            UpdateRoundsInMag ();
        }

        void UpdateRoundsInMag () {
            if (currentAmmo != MaxAmmo)
                for (int i = 0; i < (roundsInMag.Length - currentAmmo); i++) {
                    roundsInMag[i].SetActive (false);
                }
        }

    }

}
