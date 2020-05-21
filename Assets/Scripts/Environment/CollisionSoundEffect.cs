
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MetroVR.Environmental {

    public class CollisionSoundEffect : MonoBehaviour {

        [SerializeField] float minVelocity = 0.6f;
        [SerializeField] AudioClip collisionSound;
        Coroutine removeAudioSource;

        void OnCollisionEnter (Collision collision) {
            if (collision.relativeVelocity.sqrMagnitude > minVelocity) {
                var src = GetComponent <AudioSource> () != null ? GetComponent<AudioSource> () : gameObject.AddComponent<AudioSource> ();
                src.loop = false;
                src.playOnAwake = false;
                src.clip = collisionSound;
                src.volume = 0.5f * collision.relativeVelocity.normalized.sqrMagnitude;
                src.Play ();

                if (removeAudioSource != null) {
                    StopCoroutine (removeAudioSource);
                    removeAudioSource = null;
                    StartCoroutine (RemoveAudioSourceInTime ());
                }
            }
        }

        IEnumerator RemoveAudioSourceInTime () {
            yield return new WaitForSeconds (5f);
            Destroy (GetComponent<AudioSource> ());
        }

    }

}
