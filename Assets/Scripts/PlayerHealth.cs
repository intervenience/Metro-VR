
using MetroVR.Util;

using System.Collections;

using UnityEngine;

namespace MetroVR {

    public class PlayerHealth : MonoBehaviour, IDamage {
        public int MaxHP = 100;
        [SerializeField] float currentHp = 100;

        Coroutine regenHpRoutine;
        [SerializeField] AudioSource voiceAudioSource;
        [SerializeField] AudioClip[] playerHitClips, playerDeathClip;

        void Start () {
            currentHp = MaxHP;
        }

        public void TakeDamage (float amount) {
            currentHp -= amount;
            if (currentHp < 0) {
                //do stuff for player death
                voiceAudioSource.PlayOneShot (playerDeathClip[0]);
                VignetteEffects.Instance.PlayerDeath ();
            } else {
                voiceAudioSource.PlayOneShot (playerHitClips[Random.Range (0, playerHitClips.Length)]);
                if (regenHpRoutine != null) {
                    StopCoroutine (regenHpRoutine);
                }
                VignetteEffects.Instance.HealthUpdated (currentHp);
                regenHpRoutine = StartCoroutine (RegenHp ());
            }
        }

        IEnumerator RegenHp () {
            yield return new WaitForSeconds (4f);

            while (currentHp < 100) {
                currentHp += 5f;
                VignetteEffects.Instance.HealthUpdated (currentHp);
                yield return new WaitForSeconds (1f);
            }
            currentHp = Mathf.Clamp (currentHp, 0, 100f);
        }

    }

}
