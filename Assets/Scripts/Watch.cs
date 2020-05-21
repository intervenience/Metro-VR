using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroVR {

    public class Watch : MonoBehaviour {

        public SpriteRenderer hourOne, hourTwo, minuteOne, minuteTwo;
        public Sprite[] numbered;

        System.Timers.Timer timer;

        void Start () {
            timer = new System.Timers.Timer ();
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = GetInterval ();
            timer.Start ();

            UpdateTimes ();
        }

        void OnDisable () {
            timer.Elapsed -= Timer_Elapsed;
        }

        void UpdateTimes () {
            System.DateTime now = System.DateTime.Now;
            if (now.Hour.ToString ().Length == 2) {
                hourOne.sprite = numbered[now.Hour / 10];
                hourTwo.sprite = numbered[now.Hour % 10];
            } else {
                hourOne.sprite = numbered[0];
                hourTwo.sprite = numbered[now.Hour];
            }

            if (now.Minute.ToString ().Length == 2) {
                minuteOne.sprite = numbered[now.Minute / 10];
                minuteTwo.sprite = numbered[now.Minute % 10];
            } else {
                minuteOne.sprite = numbered[0];
                minuteTwo.sprite = numbered[now.Minute];
            }
        }

        double GetInterval () {
            System.DateTime now = System.DateTime.Now;
            return ((60 - now.Second) * 1000 - now.Millisecond);
        }

        void Timer_Elapsed (object sender, System.Timers.ElapsedEventArgs e) {
            UpdateTimes ();
            timer.Interval = GetInterval ();
            timer.Start ();
        }
    }

}
