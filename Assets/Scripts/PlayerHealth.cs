
using MetroVR.Util;

using System.Collections;

using UnityEngine;

namespace MetroVR {

    public class PlayerHealth : MonoBehaviour, IDamage {
        public int MaxHP = 100;
        [SerializeField] float currentHp = 100;

        bool maskEquipped = false;
        bool outsideOrInRadioactiveZone = false;

        Coroutine regenHpRoutine;
        [SerializeField] AudioSource voiceAudioSource;
        [SerializeField] AudioClip[] playerHitClips, playerDeathClips, playerCoughingClips;

        void Start () {
            currentHp = MaxHP;
        }

        public void TakeDamage (float amount) {
            currentHp -= amount;
            if (currentHp < 0) {
                //do stuff for player death
                voiceAudioSource.PlayOneShot (playerDeathClips[0]);
                PostProcessControl.Instance.PlayerDeath ();
            } else {
                voiceAudioSource.PlayOneShot (playerHitClips[Random.Range (0, playerHitClips.Length)]);
                if (regenHpRoutine != null) {
                    StopCoroutine (regenHpRoutine);
                }
                PostProcessControl.Instance.HealthUpdated (currentHp);
                regenHpRoutine = StartCoroutine (RegenHp ());
            }
        }

        Coroutine oxygenRoutine;

        IEnumerator CountdownOxygen () {
            int time = 300;
            while (maskEquipped) {
                if (outsideOrInRadioactiveZone) {
                    time -= 1;
                }
                yield return new WaitForSeconds (1);
            }
        }

        public void EnteredHeavyRadioactiveZone () {
            outsideOrInRadioactiveZone = true;
            if (!maskEquipped) {
                if (oxygenRoutine != null) {
                    StopCoroutine (oxygenRoutine);
                }
                oxygenRoutine = StartCoroutine (RadioactiveZoneBreathing ());
            }
        }

        IEnumerator RadioactiveZoneBreathing () {
            yield return new WaitForSeconds (3f);
            for (int i = 0; i < playerCoughingClips.Length; i++) {
                voiceAudioSource.clip = playerCoughingClips[i];
                voiceAudioSource.Play ();
                yield return new WaitForSeconds (voiceAudioSource.clip.length * 1.75f);
            }
            //If we reach this point the player will have suffocated
            PostProcessControl.Instance.PlayerDeath ();
        }

        public void ExitedHeavyRadioactiveZone () {
            StopCoroutine (oxygenRoutine);
        }

        IEnumerator RegenHp () {
            yield return new WaitForSeconds (4f);

            while (currentHp < 100) {
                currentHp += 5f;
                PostProcessControl.Instance.HealthUpdated (currentHp);
                yield return new WaitForSeconds (1f);
            }
            currentHp = Mathf.Clamp (currentHp, 0, 100f);
        }

    }

}
