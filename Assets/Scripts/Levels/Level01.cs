
using MetroVR.Environmental;
using MetroVR.NPC;
using MetroVR.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroVR.Levels {

    public class Level01 : MonoBehaviour {

        Persistence persistence;
        public Database database;

        int currentStage = 0;

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


        private List<GameObject> allMobs {
            get {
                var t = trainMobs.Concat (postGenMobs).ToList ();
                t.Add (generatorNosalis.gameObject);
                return new List<GameObject> (t);
            }
        }

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

            persistence = GetComponent<Persistence> ();
            VRTK.VRTK_SDKManager.instance.LoadedSetupChanged += Instance_LoadedSetupChanged;
        }

        void OnDisable () {
            VRTK.VRTK_SDKManager.instance.LoadedSetupChanged -= Instance_LoadedSetupChanged;
        }

        void Instance_LoadedSetupChanged (VRTK.VRTK_SDKManager sender, VRTK.VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            SetupGameData ();
            PostProcessControl.Instance.GameIsReady ();
        }

        void Start () {
            //SetupGameData ();
            currentObjective = firstRoomCharger;
        }
        
        void Update () {
            if (Input.GetKeyDown (KeyCode.Alpha1)) {
                SaveGameData ();
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
            currentStage++;
            SaveGameData ();
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
            PostProcessControl.Instance.PlayerDeath ();
            StartCoroutine (EndOfLevelResetDelay ());
        }

        IEnumerator EndOfLevelResetDelay () {
            yield return new WaitForSeconds (1f);
            canMove = false;
            yield return new WaitForSeconds (3f);
            ambientSound.Outro ();
            yield return new WaitForSeconds (12f);
            SceneManager.LoadScene (0);
        }

        public void PlayerDeath () {
            PostProcessControl.Instance.PlayerDeath ();
            StartCoroutine (PlayerDeathResetDelay ());
        }

        IEnumerator PlayerDeathResetDelay () {
            yield return new WaitForSeconds (5f);
            SceneManager.LoadScene (0);
        }

        void SaveGameData () {
            Debug.Log ("Start time " + Time.realtimeSinceStartup);
            SaveData data = new SaveData ();
            data.mobs = new List<MobPositionData> ();
            data.worldObjects = new List<WorldObjectData> ();
            data.itemData = new List<ItemData> ();

            foreach (GameObject mob in allMobs) {
                MobPositionData pos = new MobPositionData ();
                pos.gameObjectName = mob.name;
                var npc = mob.GetComponent<Npc> ();
                pos.hp = npc.CurrentHP;
                pos.id = npc.ID;
                pos.position = mob.transform.position;
                pos.rotation = mob.transform.rotation.eulerAngles;
                pos.isActive = mob.activeSelf;
                data.mobs.Add (pos);
            }

            List<GameObject> worldObjects = new List<GameObject> ();
            worldObjects.AddRange (GameObject.FindGameObjectsWithTag ("ObjectiveItem"));
            worldObjects.AddRange (GameObject.FindGameObjectsWithTag ("WorldObject"));

            foreach (GameObject go in worldObjects) {
                WorldObjectData pos = new WorldObjectData ();
                pos.gameObjectName = go.name;
                pos.position = go.transform.position;
                pos.rotation = go.transform.rotation.eulerAngles;
                data.worldObjects.Add (pos);
            }

            data.playspacePosition = VRTK.VRTK_SDKManager.instance.loadedSetup.actualBoundaries.transform.position;
            data.playspaceRotation = VRTK.VRTK_SDKManager.instance.loadedSetup.actualBoundaries.transform.rotation.eulerAngles;
            data.playerLocalPosition = VRTK.VRTK_SDKManager.instance.loadedSetup.actualHeadset.transform.localPosition;

            List<GameObject> items = new List<GameObject> ();
            items.AddRange (GameObject.FindGameObjectsWithTag ("LargeItem"));
            items.AddRange (GameObject.FindGameObjectsWithTag ("SmallItem"));
            items.AddRange (GameObject.FindGameObjectsWithTag ("Magazine"));

            foreach (GameObject go in items) {
                ItemData itemData = new ItemData ();
                var item = go.GetComponent<Item> ();
                itemData.gameObjectName = go.name;
                itemData.itemId = item.ID;
                if (!item.IsGrabbed ()) {
                    //If the item isn't grabbed and has no parent, ID = -1
                    if (item.transform.parent == null)
                        itemData.holsterId = -1;
                    else
                        itemData.holsterId = 2;
                } else if (item.IsGrabbed ()) {
                    //Left hand ID is 0, right hand ID is 1
                    itemData.holsterId = item.GetGrabbingObject ().GetComponent<HandController> ().LeftOrRight == HandLeftOrRight.Left ? 0 : 1;
                }
                data.itemData.Add (itemData);
            }
            persistence.saveData = data;
            persistence.Save ();
            Debug.Log ("Start time " + Time.realtimeSinceStartup);
        }

        void SetupGameData () {
            SaveData data = persistence.saveData;

            if (data.levelStage > 0 && data.levelStage != 6) {

            }

            //Playspace
            Debug.Log (VRTK.VRTK_SDKManager.instance.loadedSetup.actualBoundaries.name);
            VRTK.VRTK_SDKManager.instance.loadedSetup.actualBoundaries.transform.position = data.playspacePosition;
            VRTK.VRTK_SDKManager.instance.loadedSetup.actualBoundaries.transform.rotation = Quaternion.Euler (data.playspaceRotation);
            VRTK.VRTK_SDKManager.instance.loadedSetup.actualHeadset.transform.localPosition = new Vector3 (data.playerLocalPosition.x, VRTK.VRTK_SDKManager.instance.loadedSetup.actualHeadset.transform.localPosition.y, data.playerLocalPosition.z);

            //Mobs
            List<GameObject> allMobs = new List<GameObject> ();
            allMobs.AddRange (trainMobs);
            allMobs.AddRange (postGenMobs);
            allMobs.Add (generatorNosalis.gameObject);
            foreach (MobPositionData mob in data.mobs) {
                try {
                    //First, try find the object via name (not efficient, I know)
                    //If this fails, we get a null value, so we just check for that and
                    //create a new instance
                    //Apply properties last
                    GameObject myMob = allMobs.Where (i => i.name == mob.gameObjectName).FirstOrDefault ();

                    if (myMob == null) {
                        //Create a new instance of this mob
                        //Have to search by ID, not index in case we are missing items
                        myMob = Instantiate (database.mobs.Where (i => i.id == mob.id).FirstOrDefault ().prefab);
                    }

                    //then apply properties
                    myMob.transform.position = mob.position;
                    myMob.transform.rotation = Quaternion.Euler (mob.rotation);
                    myMob.GetComponent<Npc> ().ForceSetHP (mob.hp);
                    myMob.SetActive (mob.isActive);
                } catch (System.Exception e) {
                    Debug.LogWarning (e);
                }
            }

            //Items
            List<GameObject> allItems = new List<GameObject> ();
            allItems.AddRange (GameObject.FindGameObjectsWithTag ("LargeItem"));
            allItems.AddRange (GameObject.FindGameObjectsWithTag ("SmallItem"));
            allItems.AddRange (GameObject.FindGameObjectsWithTag ("Magazine"));
            foreach (ItemData item in data.itemData) {
                try {
                    GameObject myItem = allItems.Where (i => i.name == item.gameObjectName).FirstOrDefault ();

                    if (myItem == null) {
                        myItem = Instantiate (database.items.Where (i => i.id == item.holsterId).First ().prefab);
                    }

                    switch (item.holsterId) {
                        default:
                            myItem.transform.position = item.position;
                            myItem.transform.rotation = Quaternion.Euler (item.rotation);
                            break;
                        case 0:
                            VRTK.VRTK_SDKManager.instance.loadedSetup.actualLeftController.
                                GetComponent<VRTK.VRTK_InteractTouch> ().ForceTouch (myItem);
                            VRTK.VRTK_SDKManager.instance.loadedSetup.actualLeftController.
                                GetComponent<VRTK.VRTK_InteractGrab> ().AttemptGrab ();
                            break;
                        case 1:
                            VRTK.VRTK_SDKManager.instance.loadedSetup.actualRightController.
                                GetComponent<VRTK.VRTK_InteractTouch> ().ForceTouch (myItem);
                            VRTK.VRTK_SDKManager.instance.loadedSetup.actualRightController.
                                GetComponent<VRTK.VRTK_InteractGrab> ().AttemptGrab ();
                            break;
                        //Holster id of 2 is a child-parent set up (ie how our snap drop zones work)
                        case 2:
                            switch (myItem.tag) {
                                case "Magazine":
                                    switch (myItem.GetComponent <Item> ().itemName) {
                                        //Primary weapon is always the first slot
                                        case "AK 2012 Magazine":
                                        case "AK Magazine":
                                            Inventory.Instance.smallSlots[0].GetComponent<VRTK.VRTK_SnapDropZone> ().ForceSnap (myItem);
                                            break;
                                        //For now I'm cheating, and every other weapon is the second slot
                                        case "Shotgun Shell":
                                            Inventory.Instance.smallSlots[1].GetComponent<VRTK.VRTK_SnapDropZone> ().ForceSnap (myItem);
                                            break;
                                    }
                                    break;
                                case "Charger":
                                    //Charger is the last small snap drop zone in the inventory
                                    Inventory.Instance.smallSlots[3].GetComponent<VRTK.VRTK_SnapDropZone> ().ForceSnap (myItem);
                                    break;
                                case "ObjectiveItem":
                                    //Realistically we only get to this point for this type of item if they're attached to the objective
                                    //So I'm just gonna do that.
                                    generator.gameObject.GetComponent<VRTK.VRTK_SnapDropZone> ().ForceSnap (myItem);
                                    break;
                                default:
                                    //Should just be the large weaponry here, and I'm lazy right now so we just procedurally
                                    //snap objects (so we lose original position but it should work)
                                    var largeSlot = Inventory.Instance.largeSlots[0].GetComponent<VRTK.VRTK_SnapDropZone> ();
                                    //If we don't have a snapped object in slot 0, snap there.
                                    if (largeSlot.GetCurrentSnappedObject () == null) {
                                        largeSlot.ForceSnap (myItem);
                                    } else {
                                        //Otherwise use the second one.
                                        Inventory.Instance.largeSlots[1].GetComponent<VRTK.VRTK_SnapDropZone> ().ForceSnap (myItem);
                                    }
                                    break;
                            }
                            break;
                    }
                } catch (System.Exception e) {
                    Debug.LogWarning (e);
                }
            }

            //World interactable objects
            List<GameObject> allWorldObjects = new List<GameObject> ();
            allWorldObjects.AddRange (GameObject.FindGameObjectsWithTag ("ObjectiveItem"));
            allWorldObjects.AddRange (GameObject.FindGameObjectsWithTag ("WorldObject"));
            foreach (WorldObjectData worldObject in data.worldObjects) {
                GameObject myObj = allWorldObjects.Where (i => i.name == worldObject.gameObjectName).FirstOrDefault ();

                if (myObj != null) {
                    myObj.transform.position = worldObject.position;
                    myObj.transform.rotation = Quaternion.Euler (worldObject.rotation);
                }
            }
        }

    }

}

