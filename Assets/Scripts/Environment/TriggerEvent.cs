
using UnityEngine;
using UnityEngine.Events;

namespace MetroVR {

    public class TriggerEvent : MonoBehaviour {

        [Tooltip ("Player by default")]
        [SerializeField] string tagToLookFor = "Player";
        [SerializeField] bool invokeOnce = true;
        bool invoked = false;
        [SerializeField] UnityEvent onTriggerEnterEvents;
        [SerializeField] UnityEvent onTriggerExitEvents;
    
        void OnTriggerEnter (Collider collider) {
            if (collider.tag == tagToLookFor) {
                Debug.Log ("Found player");
                if (!invoked) {
                    onTriggerEnterEvents.Invoke ();
                    invoked = invokeOnce ? true : false;
                }
            }
        }

        void OnTriggerExit (Collider collider) {
            if (collider.tag == tagToLookFor) {
                if (!invoked) {
                    onTriggerExitEvents.Invoke ();
                    invoked = invokeOnce ? true : false;
                }
            }
        }

    }

}
