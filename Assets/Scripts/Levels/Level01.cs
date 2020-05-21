
using MetroVR.Environmental;
using MetroVR.NPC;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroVR.Levels {

    public class Level01 : MonoBehaviour {

        //[SerializeField] GameObject worldInteractableObjects;

        public Transform currentObjective;

        [SerializeField] AmbientSound ambientSound;

        [Header ("First door properties")]
        bool firstDoorUnlocked = false;
        [SerializeField] GameObject doorObject;
        [SerializeField] Transform firstRoomCharger;

        [Header ("Train")]
        bool trainAttackTriggered = false;
        [SerializeField] GameObject[] trainMobs;

        [Header ("End Door")]
        [SerializeField] Transform endDoorTransform;
        [SerializeField] AudioClip doorMoveStart, doorMoveLoop, doorMoveEnd;
        [SerializeField] AudioSource doorAudioSource;

        [Header ("Generator")]
        [SerializeField] Transform generator;
        [SerializeField] AudioSource generatorAudioSource;
        [SerializeField] AudioClip generatorPowerOn, generatorLoop, generatorPowerOff;
        [SerializeField] NPC.Npc generatorNosalis;

        [Header ("Post Generator Attack")]
        [SerializeField] GameObject[] postGenMobs;

        bool exitDoorRotatorOpened = false;

        bool generatorFuelled = false;

        public bool canMove = true;

        public static Level01 Instance;

        void Awake () {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy (gameObject);
            }
        }

        void Start () {
            currentObjective = firstRoomCharger;
        }
        
        void Update () {
            if (Input.GetKeyDown (KeyCode.Alpha1)) {
                FirstDoorPowered ();
            }

            if (Input.GetKeyDown (KeyCode.Alpha2)) {
                TrainAttackTriggered ();
            }
            if (Input.GetKeyDown (KeyCode.Alpha3)) {
                ExitDoorRotatorReachedMax ();
            }
            if (Input.GetKeyDown (KeyCode.Alpha4)) {
                GeneratorFuelled ();
            }
            if (Input.GetKeyDown (KeyCode.Alpha5)) {
                AfterGeneratorPoweredTrigger ();
            }
        }

        public void FirstDoorPowered () {
            Debug.Log ("First door has been powered");
            firstDoorUnlocked = true;
            var rb = doorObject.GetComponent<Rigidbody> ();
            rb.constraints = RigidbodyConstraints.None;
            currentObjective = doorObject.transform;
        }

        public void GoneThroughFirstDoor () {
            currentObjective = endDoorTransform;
        }

        [SerializeField] List<Npc> triggeredMobs;

        public void TrainAttackTriggered () {
            trainAttackTriggered = true;
            triggeredMobs = new List<Npc> ();
            foreach (GameObject go in trainMobs) {
                go.SetActive (true);
                triggeredMobs.Add (go.GetComponent<Npc> ());
            }
            StartCoroutine (CheckHealthStateOfTriggeredMobs (true));
        }

        IEnumerator CheckHealthStateOfTriggeredMobs (bool first) {
            yield return new WaitForSeconds (2f);
            if (first)
                ambientSound.CombatMusicStart ();
            else
                ambientSound.SecondCombatStart ();

            bool keepChecking = true;
            while (keepChecking) {
                bool refresh = false;
                foreach (Npc npc in triggeredMobs) {
                    if (npc.CurrentHP > 0) {
                        refresh = true;
                    }
                }
                keepChecking = refresh;
                yield return new WaitForSeconds (0.5f);
            }
            triggeredMobs.Clear ();
            if (first)
                ambientSound.CombatEnd ();
            else
                ambientSound.SecondCombatEnd ();
        }

        public void ExitDoorRotatorReachedMax () {
            Debug.Log ("Done thing");
            currentObjective = generator;
            if (generatorFuelled && !exitDoorRotatorOpened) {
                //do finalised things
                StartCoroutine (FullDoorAnim ());
            } else {
                //play broken down animation
                if (!exitDoorRotatorOpened)
                    StartCoroutine (BrokenDoorAnim ());
            }
        }

        IEnumerator BrokenDoorAnim () {
            float elapsed = 0f;
            float y = endDoorTransform.position.y;
            doorAudioSource.clip = doorMoveStart;
            doorAudioSource.Play ();
            yield return new WaitForSeconds (1.5f);
            while (elapsed < 4f) {
                y = Mathf.Lerp (endDoorTransform.position.y, 2.1f, elapsed / 4f);
                endDoorTransform.position = new Vector3 (endDoorTransform.position.x, y, endDoorTransform.position.z);
                elapsed += Time.deltaTime;
                yield return null;
            }
            doorAudioSource.PlayOneShot (doorMoveEnd);
        }

        IEnumerator FullDoorAnim () {
            float elapsed = 0f;
            float y = endDoorTransform.position.y;
            doorAudioSource.clip = doorMoveStart;
            doorAudioSource.Play ();
            yield return new WaitForSeconds (1.5f);
            doorAudioSource.clip = doorMoveLoop;
            doorAudioSource.loop = true;
            doorAudioSource.Play ();
            while (elapsed < 14f) {
                y = Mathf.Lerp (endDoorTransform.position.y, 6f, elapsed / 14f);
                endDoorTransform.position = new Vector3 (endDoorTransform.position.x, y, endDoorTransform.position.z);
                elapsed += Time.deltaTime;
                yield return null;
            }
            doorAudioSource.PlayOneShot (doorMoveEnd);
        }

        public void GeneratorFuelled () {
            currentObjective = endDoorTransform;
            generatorFuelled = true;
            generatorAudioSource.PlayOneShot (generatorPowerOn);
            generatorAudioSource.clip = generatorLoop;
            generatorAudioSource.loop = true;
            generatorAudioSource.PlayDelayed ((ulong) generatorPowerOn.length);
            if (exitDoorRotatorOpened) {
                //open door the rest of the way
                StartCoroutine (FullDoorAnim ());
            }
        }

        public void PlayerTooCloseToGeneratorNosalis () {
            if (generatorNosalis.CurrentHP > 0) {
                generatorNosalis.ForceState (NPC.NpcState.FightingPlayer);
            }
        }

        public void AfterGeneratorPoweredTrigger () {
            if (generatorFuelled) {
                triggeredMobs.Clear ();
                triggeredMobs = new List<Npc> ();
                foreach (GameObject go in postGenMobs) {
                    go.SetActive (true);
                    triggeredMobs.Add (go.GetComponent<Npc> ());
                }
                StartCoroutine (CheckHealthStateOfTriggeredMobs (false));
            }
        }

        public void PlayerReachedEndStairs () {
            //do outro thing
            MetroVR.Util.VignetteEffects.Instance.PlayerDeath ();
            StartCoroutine (Delay ());
        }

        IEnumerator Delay () {
            yield return new WaitForSeconds (1f);
            canMove = false;
            yield return new WaitForSeconds (3f);
            ambientSound.Outro ();
            yield return new WaitForSeconds (12f);
            SceneManager.LoadScene (0);
        }

    }

}

