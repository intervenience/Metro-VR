
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VRTK;

namespace MetroVR {

    [RequireComponent (typeof (Rigidbody))]
    public class Shotgun : Item {

        Rigidbody rb;

        [SerializeField] Transform nozzle;
        [SerializeField] GameObject gunFx;
        [SerializeField] AudioSource[] audioSources;
        [SerializeField] AudioClip[] firingSounds;
        [SerializeField] AudioClip[] magInSounds;
        [SerializeField] AudioClip magOutSound;
        [SerializeField] AudioClip noAmmoSound;

        [SerializeField] ParticleSystem roundFx;
        [SerializeField] ParticleSystem shellEjectionFx;
        [SerializeField] ShotgunMagazine magazine;

        [SerializeField] float recoilForce = 10f;
        [SerializeField] float pelletDamage = 30f;
        [SerializeField] int pelletRowCount = 4;
        [SerializeField] float spread = 0.3f;

        void Start () {
            rb = GetComponent<Rigidbody> ();
        }

        public override void StartUsing (VRTK_InteractUse currentUsingObject = null) {
            base.StartUsing (currentUsingObject);

            FireRound ();
        }

        void FireRound () {
            Vector3 recoil;
            Debug.Log (magazine);
            Debug.Log (magazine.CanFire ());
            if (magazine != null) {
                if (magazine.CanFire () == true) {
                    if (!audioSources[0].isPlaying) {
                        audioSources[0].clip = firingSounds[Random.Range (0, firingSounds.Length)];
                        audioSources[0].Play ();
                    } else {
                        audioSources[2].clip = firingSounds[Random.Range (0, firingSounds.Length)];
                        audioSources[2].Play ();
                    }

                    roundFx.Play ();
                    shellEjectionFx.Play ();
                    magazine.Shoot ();
                    audioSources[1].clip = magOutSound;
                    audioSources[1].PlayDelayed (0.1f);

                    recoil = new Vector3 (Random.Range (-0.05f, 0.05f) * recoilForce, Random.Range (-0.05f, 0.05f) * recoilForce, Random.Range (0.05f, 0.15f) * recoilForce);
                    rb.AddForceAtPosition (recoil, nozzle.transform.position, ForceMode.Impulse);

                    RaycastHit hit;
                    if (Physics.Raycast (nozzle.position, nozzle.forward, out hit, 15f)) {
                        switch (hit.collider.tag) {
                            case "Hitbox":
                                hit.collider.gameObject.GetComponent<Hitbox> ().Hit (pelletDamage);
                                break;
                        }
                    }
                    for (int i = 0; i < pelletRowCount; i++) {
                        for (int j = 0; j < pelletRowCount; j++) {
                            if (Physics.Raycast (nozzle.position, new Vector3 (nozzle.forward.x - spread + (spread * i), nozzle.forward.y - spread + (spread * j), nozzle.forward.z), out hit, 15f)) {
                                switch (hit.collider.tag) {
                                    case "Hitbox":
                                        hit.collider.gameObject.GetComponent<Hitbox> ().Hit (pelletDamage);
                                        break;
                                }
                            }
                        }
                    }
                } else {
                    audioSources[1].clip = noAmmoSound;
                    audioSources[1].Play ();
                }
                magazine.TurnRevolver ();
            }
        }

        public override void StopUsing (VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true) {
            base.StopUsing (previousUsingObject, resetUsingObjectState);

        }

        public void TurnRevolver () {
            magazine.TurnRevolver ();
        }

        public void PlayReloadSound () {
            audioSources[2].clip = magInSounds[Random.Range (0, magInSounds.Length)];
            audioSources[2].Play ();
        }

    }

}

