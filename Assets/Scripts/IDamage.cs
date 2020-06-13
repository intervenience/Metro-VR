
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR {

    public interface IDamage {

        /// <param name="amount"></param>
        /// <param name="soundOff">If this is true, audio won't be used when the player is hit</param>
        void TakeDamage (float amount, bool soundOff = false);

    }

}