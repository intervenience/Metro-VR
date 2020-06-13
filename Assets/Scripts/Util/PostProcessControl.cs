
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MetroVR.Util {

    public class PostProcessControl : MonoBehaviour {

        [SerializeField] PostProcessVolume volume;

        public static PostProcessControl Instance;

        Vignette vignette;
        Grain grain;
        Bloom bloom;

        void Awake () {
            if (Instance == null)
                Instance = this;
            else
                Destroy (gameObject);

            volume = GetComponent<PostProcessVolume> ();
            grain = volume.profile.GetSetting<Grain> ();
            vignette = volume.profile.GetSetting<Vignette> ();
            bloom = volume.profile.GetSetting<Bloom> ();

            vignette.color.value = Color.black;
            vignette.intensity.value = 10f;
            vignette.center.value.x = 5;
        }

        float hp;
        Coroutine redFlashRoutine;

        public void GameIsReady () {
            StartCoroutine (BlackScreenToVignette ());
        }

        IEnumerator BlackScreenToVignette () {
            yield return new WaitForSeconds (0.25f);
            float elapsed = 0f;
            vignette.center.value.x = 0.5f;
            while (elapsed < .5f) {
                vignette.intensity.value = Mathf.Lerp (10f, 0.2f, elapsed / 0.5f);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

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

        bool inRadiationZone = false;
        bool defendedFromRadiation = false;
        Coroutine radioactiveGrainRoutine;

        /// <summary>
        /// When the user has a gas mask on and still has oxygen, input true.
        /// In any other case this will be false.
        /// </summary>
        /// <param name="hasOxygen"></param>
        public void PlayerOxygenState (bool hasOxygen) {
            defendedFromRadiation = hasOxygen;
        }

        /// <summary>
        /// When the player has entered a heavy radiation zone.
        /// </summary>
        public void EnteredHeavyRadioactiveZone () {
            inRadiationZone = true;
            if (radioactiveGrainRoutine != null)
                StopCoroutine (radioactiveGrainRoutine);
            radioactiveGrainRoutine = StartCoroutine (RadioactiveGrainEffect ());
        }

        public void ExitedHeavyRadioactiveZone () {
            inRadiationZone = false;
        }

        /// <summary>
        /// Progressively increases the intensity and value of the luminosity contribution
        /// (I don't know what this is but it changes the effect on screen, so I'm using it)
        /// </summary>
        IEnumerator RadioactiveGrainEffect () {
            while (inRadiationZone) {
                if (!defendedFromRadiation) {
                    grain.intensity.value += 1f * Time.deltaTime;
                    grain.intensity.value = Mathf.Clamp (grain.intensity.value, 0, 1);
                    grain.lumContrib.value += .25f * Time.deltaTime; 
                } else {
                    grain.intensity.value -= 4f * Time.deltaTime;
                    grain.lumContrib.value -= 1f * Time.deltaTime;
                }
                yield return new WaitForFixedUpdate ();
            }
            while (grain.intensity.value > 0) {
                grain.intensity.value -= 10f * Time.deltaTime;
                grain.intensity.value = Mathf.Clamp (grain.intensity.value, 0, 1);
                grain.lumContrib.value -= 4f * Time.deltaTime;
                yield return new WaitForFixedUpdate ();
            }
        }

    }

}
