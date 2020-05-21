using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroVR.Environmental {

    public class LightController : MonoBehaviour {

        [SerializeField] MeshRenderer lightMesh;
        Material m;
        [SerializeField] bool isOn = false;
        [SerializeField] Color emissionColor;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip powerOn, powerOff;
        [SerializeField] Light myLight;

        void Awake () {
            if (m == null)
                m = lightMesh.material;
            audioSource = GetComponent<AudioSource> ();

            SetState (isOn, false);
        }

        void SetState (bool on, bool soundOn) {
            myLight.enabled = on;

            audioSource.clip = on ? powerOn : powerOff;
            m.SetColor ("_Emission", on ? emissionColor : Color.black);
            if (soundOn)
                audioSource.Play ();
        }

        public void Toggle () {
            SetState (!isOn, true);
            isOn = !isOn;
        }

    }

}
