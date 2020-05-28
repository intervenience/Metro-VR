
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VRTK;

namespace MetroVR {

    [RequireComponent (typeof (Rigidbody))]
    public class AutomaticGun : Item {

        [Header ("Automatic Weapon")]
        public float rpm = 0;
        float timeBetweenShots {
            get {
                return 60 / rpm;
            }
        }
        Rigidbody rb;
        [SerializeField] Transform nozzle;
        [SerializeField] ParticleSystem firingFx, shellEjectionFx;
        [SerializeField] AudioSource[] audioSources;
        [SerializeField] AudioClip[] firingSounds;
        [SerializeField] AudioClip magInSound, magOutSound;
        [SerializeField] AudioClip noAmmoSound;
        [SerializeField] Transform magazineOffset;
        [SerializeField] GameObject magazineObj;
        [SerializeField] Magazine magazine;
        [SerializeField] float recoilForce = 10f;
        [SerializeField] float roundDamage = 80f;

        void Start () {
            rb = GetComponent<Rigidbody> ();

            if (magazine != null)
                magazine.isGrabbable = false;
        }

        Coroutine firing;

        public override void StartUsing (VRTK_InteractUse currentUsingObject = null) {
            base.StartUsing (currentUsingObject);
            if (magazine != null && magazine.currentAmmo > 0) {
                firing = StartCoroutine (Shooting ());
            } else {
                audioSources[0].clip = noAmmoSound;
                audioSources[0].Play ();
            }
        }

        protected override void Update () {
            base.Update ();

            if (Input.GetKeyDown (KeyCode.Space)) {
                if (magazine != null && magazine.currentAmmo > 0) {
                    firing = StartCoroutine (Shooting ());
                } else {
                    audioSources[0].clip = noAmmoSound;
                    audioSources[0].Play ();
                }
            }

            if (Input.GetKeyUp (KeyCode.Space)) {
                if (firing != null) {
                    StopCoroutine (firing);
                }
            }
        }

        public override void StopUsing (VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true) {
            base.StopUsing (previousUsingObject, resetUsingObjectState);
            if (firing != null) {
                StopCoroutine (firing);
            }
        }

        IEnumerator Shooting () {
            RaycastHit hit;
            Vector3 recoil;
            int i = 0;
            while (magazine.currentAmmo > 0) {
                i++;
                switch (i % 2) {
                    case 0:
                        audioSources[0].clip = firingSounds[Random.Range (0, firingSounds.Length - 1)];
                        audioSources[0].Play ();
                        break;
                    case 1:
                        audioSources[1].clip = firingSounds[Random.Range (0, firingSounds.Length - 1)];
                        audioSources[1].Play ();
                        break;
                }

                firingFx.Play ();
                shellEjectionFx.Play ();
                recoil = new Vector3 (Random.Range (-0.05f, 0.05f) * recoilForce, Random.Range (-0.05f, 0.05f) * recoilForce, Random.Range (0.05f, 0.15f) * recoilForce);
                Debug.DrawRay (nozzle.transform.position, recoil, Color.blue, 5f);
                
                rb.AddForceAtPosition (recoil, nozzle.transform.position, ForceMode.Impulse);

                if (Physics.Raycast (nozzle.position, nozzle.forward, out hit, 100f)) {
                    switch (hit.collider.tag) {
                        case "Hitbox":
                            hit.collider.gameObject.GetComponent<Hitbox> ().Hit (roundDamage);
                            break;
                    }
                }

                magazine.RemoveRound ();
                yield return new WaitForSeconds (timeBetweenShots + Random.Range (0f, 0.05f));
            }
        }

        public void AttachMagazine (GameObject magObj) {
            if (magazineObj == null) {
                magObj.transform.parent = magazineOffset;
                var rb = magObj.GetComponent<Rigidbody> ();
                rb.velocity = Vector3.zero;
                Destroy (rb);
                magObj.transform.localPosition = Vector3.zero;
                magObj.transform.localRotation = Quaternion.Euler (Vector3.zero);
                magazine = magObj.GetComponent<Magazine> ();
                audioSources[0].clip = magInSound;
                audioSources[0].Play ();
            }
        }

        public void ReleaseMagazine () {
            if (magazine != null) {
                magazine.OnDrop ();
                audioSources[0].clip = magOutSound;
                audioSources[0].Play ();

                magazineObj.transform.parent = null;
                magazineObj = null;
                magazine = null;
            }
        }

    }

}
