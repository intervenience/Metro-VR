using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MetroVR.Environmental {
    
    public class AmbientSoundState : ScriptableObject {
        [SerializeField] AudioClip clip;
        [SerializeField] PlayerSituation situation;
        [SerializeField] int priority;
    }
}
