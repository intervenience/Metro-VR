using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR.Util {

    public class Persistence : MonoBehaviour {

        public static Persistence Instance;

        void Awake () {
            if (Instance != null)
                Destroy (gameObject);
            else
                Instance = this;
        }

        public void SaveNow () {
            /* Things to save:
            Level/Scene
            Stage progress (according to current L#.cs)
            Status of scene objects
            Player xyz pos and rotation
            Player hp
            Player inventory data
            */

        }

        public void LoadLatest () {

        }

    }

}
