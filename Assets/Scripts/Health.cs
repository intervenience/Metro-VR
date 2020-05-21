
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR {

    public class Health : MonoBehaviour {

        public int currentHp;
        [SerializeField] int maxHp;

        void Start () {
            currentHp = maxHp;
        }

        void Hit () {
        }

    }

}
