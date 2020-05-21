
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR {

    public class Hitbox : MonoBehaviour {

        [SerializeField] IDamage mydamage;
        [SerializeField] float modifier = 1f;

        void Start () {
            if (mydamage == null) {
                mydamage = GetComponentInParent<IDamage> ();
            }
        }

        public void Hit (float damage) {
            mydamage.TakeDamage (damage * modifier);
        }

    }

}
