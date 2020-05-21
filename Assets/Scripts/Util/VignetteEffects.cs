
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MetroVR.Util {

    public class VignetteEffects : MonoBehaviour {

        [SerializeField] PostProcessVolume volume;

        public static VignetteEffects Instance;
        [SerializeField] Vignette vignette;

        void Awake () {
            if (Instance == null)
                Instance = this;
            else
                Destroy (gameObject);
        }

        void Start () {
            volume = GetComponent<PostProcessVolume> ();
            vignette = volume.profile.GetSetting<Vignette> ();
        }

        float hp;
        Coroutine redFlashRoutine;

        public void HealthUpdated (float currentHealth) {
            if (currentHealth < hp) {
                redFlashRoutine = StartCoroutine (FlashRedBorder ());
            }
            hp = currentHealth;
        }

        IEnumerator FlashRedBorder () {
            vignette.color.Interp (vignette.color.value, Color.red, .25f);
            vignette.intensity.Interp (vignette.intensity.value, .75f, 1f);
            if (hp < 40) {
                while (hp < 40) {
                    yield return new WaitForSeconds (1f);
                    vignette.intensity.Interp (vignette.intensity.value, .4f, 1f);
                    yield return new WaitForSeconds (1f);
                    vignette.intensity.Interp (vignette.intensity.value, .6f, 1f);
                }
            }
            yield return new WaitForSeconds (1f);
            vignette.color.Interp (vignette.color.value, Color.black, .25f);
            vignette.intensity.Interp (vignette.intensity.value, .2f, 1f);
        }

        public void PlayerDeath () {
            if (redFlashRoutine != null) {
                StopCoroutine (redFlashRoutine);
            }
            vignette.color.value = Color.black;
            vignette.intensity.Interp (vignette.intensity.value, 10f, 3f);
            vignette.center.value.x = 5;
        }

    }

}
