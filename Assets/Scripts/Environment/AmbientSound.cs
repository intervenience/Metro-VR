
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System;

namespace MetroVR.Environmental {

    public enum PlayerSituation {
        IdleExploring,
        Stealth,
        StealthEnemyAware,
        StealthLost,
        StealthRegained,
        HostileMobsAlerted,
        OpenCombat
    }

    public class AmbientSound : MonoBehaviour {

        [SerializeField] AmbientSoundState soundStates;
        [SerializeField] AudioSource backgroundAudioSource;
        [SerializeField] AudioSource[] additionalAudioSources;

        [SerializeField] AudioClip tunnelAmbience, battleStartClip, battleEndClip, stationAmbienceClip, secondBattleStartClip, secondBattleEndClip, outroClip;

        public void PlayTunnelAmbience () {
            backgroundAudioSource.clip = tunnelAmbience;
            backgroundAudioSource.Play ();
        }

        public void PlayStationAmbience () {
            backgroundAudioSource.clip = stationAmbienceClip;
            backgroundAudioSource.Play ();
        }

        public void CombatMusicStart () {
            backgroundAudioSource.clip = battleStartClip;
            backgroundAudioSource.Play ();
        }

        public void CombatEnd () {
            backgroundAudioSource.clip = battleEndClip;
            backgroundAudioSource.Play ();
        }

        public void SecondCombatStart () {
            backgroundAudioSource.clip = secondBattleStartClip;
            backgroundAudioSource.Play ();
        }

        public void SecondCombatEnd () {
            backgroundAudioSource.clip = secondBattleEndClip;
            backgroundAudioSource.Play ();
        }

        public void Outro () {
            backgroundAudioSource.clip = outroClip;
            backgroundAudioSource.Play ();
        }

        void OnTinnitusEffect () {
            backgroundAudioSource.volume = 0.1f;
            foreach (AudioSource src in additionalAudioSources) {
                src.volume = 0.05f;
            }
        }

        void OnTinnitusBeginsEnding () {
            StartCoroutine (LerpVolumesToNormal ());
        }

        IEnumerator LerpVolumesToNormal () {
            float elapsedTime = 0;
            while (elapsedTime < 2f) {
                //float percentage = elapsedTime / 2f;

                //backgroundAudioSource.volume = 1 - backgroundAudioSource.volume * (elapsedTime / 2f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

    }

}
