
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR.Environmental {

    public class RadiationZone : MonoBehaviour {

        [Range (1, 7)]
        public int strength = 0;
        public int Strength {
            get {
                return strength;
            }
        }

    }

}
