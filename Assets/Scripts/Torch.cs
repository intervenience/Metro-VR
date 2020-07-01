
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using VRTK;

namespace MetroVR {

    public class Torch : MonoBehaviour {

        public delegate void BatteryUpdate ();
        public static event BatteryUpdate OnBatteryUpdate;

        VRTK_ControllerEvents leftController;
        [SerializeField] Light myLight;
        [SerializeField] GameObject lightObj;

        //[Range (4, 10)]
        //float lightRange = 10f;
        //[Range (1.25f, 1.85f)]
        //float lightIntensity = 1.85f;

        [SerializeField]
        [Range (0f, 1f)]
        float battery = .2f;
        public float Battery {
            get {
                return battery;
            }
        }

        Thread batteryUsageThread;
        System.Timers.Timer timer;

        public static Torch Instance;

        void Awake () {
            VRTK_SDKManager.instance.LoadedSetupChanged += VRTK_SetupChanged;
            if (Instance == null)
                Instance = this;
            else
                Destroy (gameObject);
        }

        void VRTK_SetupChanged (VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            leftController = GameObject.FindGameObjectWithTag ("Left").GetComponent<VRTK_ControllerEvents> ();
            leftController.ButtonOnePressed += ToggleFlashlight;

            lightObj.transform.parent = sender.loadedSetup.actualHeadset.transform;
            lightObj.transform.localPosition = Vector3.zero;
            lightObj.SetActive (false);

            batteryUsageThread = new Thread (new ThreadStart (BatteryDiminishTimer));
            batteryUsageThread.Priority = System.Threading.ThreadPriority.Lowest;
            batteryUsageThread.Start ();
        }

        void OnDisable () {
            VRTK_SDKManager.instance.LoadedSetupChanged -= VRTK_SetupChanged;
            leftController.ButtonOnePressed -= ToggleFlashlight;
            timer.Stop ();
            batteryUsageThread.Join ();
        }

        public void ChargeBattery () {
            Debug.Log ("Charging battery");
            ChangeBattery (0.15f);
        }

        void BatteryDiminishTimer () {
            timer = new System.Timers.Timer ();
            timer.Interval = 30000; //30 seconds
            timer.AutoReset = true;
            timer.Enabled = true;

            timer.Start ();

            timer.Elapsed += delegate {
                if (lightObj.activeSelf)
                    ChangeBattery (-0.05f); //5%
            };
        }

        void ToggleFlashlight (object sender, ControllerInteractionEventArgs e) {
            if (Battery > 0f) {
                lightObj.SetActive (!lightObj.activeSelf);
                if (lightObj.activeSelf)
                    ChangeBattery ( -0.03f);
            } else if (Battery <= 0f) {
                lightObj.SetActive (false);
            }
        }

        void ChangeBattery (float amount) {
            battery += amount;
            battery = Mathf.Clamp (battery, 0, 1);

            if (OnBatteryUpdate != null) {
                OnBatteryUpdate.Invoke ();
            }
            myLight.range = 30f * battery;
            myLight.intensity = 1.25f - (1.25f - 0.85f) * battery;
        }
    }

}
