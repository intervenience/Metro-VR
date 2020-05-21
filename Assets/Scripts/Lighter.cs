
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR {

    public class Lighter : MonoBehaviour/*, IHeldActivation*/ {
    
        [SerializeField] GameObject lid;

        [SerializeField] GameObject particleSystem;
        [Range (0, 1)]
        [SerializeField] float lightIntensity = 1f;

        void Start () {
            particleSystem.SetActive (false);
        }

        public void PrimaryActivate () {
            particleSystem.SetActive (false);
        }

        public void PrimaryDeactivate () {

        }

        public void SecondaryActivate () {
            //lock or unlock lid

        }

        public void SecondaryDeactivate () {

        }

    }
   
}
