
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR.Util {

    public class DissolveController : MonoBehaviour {

        Material m;

        void Awake () {
            Material[] materials = GetComponent<MeshRenderer> ().materials;
            foreach (Material material in materials) {
                if (material.name.StartsWith ("Dissolve")) {
                    m = material;
                    break;
                }
            }
        }

        public void StartDissolve (float timeTilDeath) {
            StartCoroutine (DissolveSequence (timeTilDeath));
        }

        IEnumerator DissolveSequence (float timeTilDeath) {
            var amt = m.GetVector ("DissolveAmount");
            Vector4 vector4 = new Vector4 (-1, -1, -1);
            float elapsed = 0;
            while (elapsed < timeTilDeath) {
                amt = Vector4.Lerp (amt, vector4, elapsed / timeTilDeath);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

}
