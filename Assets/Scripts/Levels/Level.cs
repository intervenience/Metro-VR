
using MetroVR.Util;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroVR.Levels {
    public class Level : MonoBehaviour {

        Persistence persistence;
        public Database database;

        protected int currentStage;
        public Transform currentObjective;
        public bool canMove = true;
        public static Level Instance;

        protected virtual void Awake () {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy (gameObject);
            }

            persistence = GetComponent<Persistence> ();
            VRTK.VRTK_SDKManager.instance.LoadedSetupChanged += Instance_LoadedSetupChanged;
        }

        protected virtual void OnDisable () {
            VRTK.VRTK_SDKManager.instance.LoadedSetupChanged -= Instance_LoadedSetupChanged;
        }

        protected virtual void Instance_LoadedSetupChanged (VRTK.VRTK_SDKManager sender, VRTK.VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            persistence.Load ();
            SetupGameData ();
            PostProcessControl.Instance.GameIsReady ();
        }

        public void PlayerDeath () {

        }

        protected virtual IEnumerator EndOfLevelResetDelay () {
            yield return null;
        }

        protected virtual IEnumerator PlayerDeathResetDelay () {
            yield return null;
        }

        protected virtual void PostLoadDataChecks () {

        }

        /// <summary>
        /// Save info about the player, weaponry, NPCs, and world objects.
        /// </summary>
        protected virtual void SaveGameData () {

        }

        /// <summary>
        /// Load game data, and parse it back into the level.
        /// </summary>
        protected virtual void SetupGameData () {

        }

    }

}
