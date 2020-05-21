
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR.Environmental {
    public class GeigerCounter : MonoBehaviour {

        [SerializeField] PlayerHealth playerHealth;

        [SerializeField] AudioSource mainAudioSource;
        [SerializeField] AudioSource highLevelAudioSource;

        [SerializeField] AudioClip[] mainAudioClips;
        [SerializeField] AudioClip intenseGeigerAudioClip;

        [SerializeField] int activeZoneStrength = 0;
        [SerializeField] bool inHeavyRadiationZone = false;

        void Start () {
            if (inHeavyRadiationZone)
                StartCoroutine (LongGeigerAudio ());

            if (activeZoneStrength > 0)
                StartCoroutine (GeigerAudio ());
        }

        void OnTriggerEnter (Collider collider) {
            if (collider.tag == "RadiationZone") {
                EnteringRadiationZone (collider.gameObject.GetComponent<RadiationZone> ());
            }
        }

        public void EnteringRadiationZone (RadiationZone zone) {
            var str = zone.Strength;
            if (str > activeZoneStrength) {
                if (str != 7) {
                    var lastStr = activeZoneStrength;
                    activeZoneStrength = str;
                    if (lastStr == 0) {
                        StartCoroutine (GeigerAudio ());
                    }
                } else if (str == 7) {
                    inHeavyRadiationZone = true;
                    StartCoroutine (LongGeigerAudio ());
                }
            }
        }

        void OnTriggerExit (Collider collider) {
            if (collider.tag == "RadiationZone") {
                ExitingRadiationZone (collider.gameObject.GetComponent<RadiationZone> ());
            }
        }

        public void ExitingRadiationZone (RadiationZone zone) {
            var str = zone.Strength;
            if (str == activeZoneStrength) {
                activeZoneStrength--;
            } else if (str == 7) {
                inHeavyRadiationZone = false;
            }
        }

        IEnumerator GeigerAudio () {
            int lastClip = -1;
            int temp;
            while (activeZoneStrength > 0) {
                do {
                    temp = Random.Range (0, mainAudioClips.Length);
                } while (temp == lastClip);
                mainAudioSource.clip = mainAudioClips[temp];
                mainAudioSource.Play ();
                yield return new WaitForSeconds (mainAudioSource.clip.length + (1.5f / activeZoneStrength) + Random.Range (-.125f, .25f));
                lastClip = temp;
                Debug.Log (activeZoneStrength);
            }
        }

        IEnumerator LongGeigerAudio () {
            while (inHeavyRadiationZone) {
                highLevelAudioSource.Play ();
                yield return new WaitForSeconds (highLevelAudioSource.clip.length);
            }
        }

    }

}
