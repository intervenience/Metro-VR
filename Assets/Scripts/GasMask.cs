using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroVR {

    public class GasMask : Item {

        Transform filterSnap;

        void OnTriggerEnter (Collider collider) {
            if (collider.tag == "") {
                if (filterSnap.childCount == 0) {

                } else if (filterSnap.childCount == 1) {

                }
            }
        }

    }

}
