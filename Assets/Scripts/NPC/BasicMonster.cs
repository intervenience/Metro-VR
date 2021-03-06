﻿
using System.Collections;
using UnityEngine;

namespace MetroVR.NPC {

    public class BasicMonster : Npc {

        [SerializeField] AudioClip[] snarls;

        protected override void Start () {
            base.Start ();
        }

        protected override void OnEnable () {
            base.OnEnable ();
        }

        protected override void OnDisable () {
            base.OnDisable ();
        }

        protected override void FightingPlayer () {
            if (attackingInProgress) {
                Loiter ();
            } else {
                PickAttackStyle ();
            }
        }

        private void PickAttackStyle () {
            if (!attackingInProgress) {
                var f = Random.Range (0f, 1f);
                if (f < .8f) {
                    LungeAtPlayer ();
                } else {
                    SwipeAtPlayer ();
                }
            }
        }

        private Coroutine preattackRoutine, attackRoutine;

        protected virtual void SwipeAtPlayer () {
            if (attackRoutine == null || !attackingInProgress)
                attackRoutine = StartCoroutine (SwipePlayerRoutine ());
        }

        IEnumerator SwipePlayerRoutine () {
            attackingInProgress = true;
            yield return new WaitForSeconds (PlaySnarlAnimation ());
            //RaycastHit hit;
            animator.SetInteger ("Attack", Random.Range (0, 4));
            yield return new WaitForSeconds (.533f / 2f);
            Vector3 startRayPosition = transform.position;
            startRayPosition.y += 0.5f;
            Vector3 targetPosition = new Vector3 (playerHead.position.x, playerBoundary.position.y / 2, playerHead.position.x);
            Debug.DrawRay (startRayPosition, (targetPosition - transform.position).normalized, Color.red, 3f);
            
            var hits = Physics.RaycastAll (startRayPosition, (targetPosition - transform.position).normalized, 2f);
            foreach (RaycastHit raycastHit in hits) {
                if (raycastHit.collider.tag == "Player") {
                    raycastHit.collider.GetComponentInParent<IDamage> ().TakeDamage (15f);
                }
            }

            /*if (Physics.Raycast (startRayPosition, (playerHead.position - transform.position).normalized, out hit, 2f)) {
                Debug.Log ("Mob hit: " + hit.collider.gameObject);
                if (hit.collider.tag == "Player") {
                    hit.collider.GetComponent<IDamage> ().TakeDamage (15f);
                }
            }*/
            yield return new WaitForSeconds (.533f / 2f);
            animator.SetInteger ("Attack", -1);

            preattackRoutine = StartCoroutine (WaitForRandomTimeToBeginAttacking ());
        }

        protected virtual void LungeAtPlayer () {
            if (attackRoutine == null || !attackingInProgress)
                attackRoutine = StartCoroutine (LungeAtPlayerRoutine ());
        }

        bool attackingInProgress = false;

        IEnumerator LungeAtPlayerRoutine () {
            attackingInProgress = true;
            nav.isStopped = true;
            yield return new WaitForSeconds (PlaySnarlAnimation ());
            Vector3 startRayPosition = transform.position;
            startRayPosition.y += 0.5f;
            Vector3 targetPosition = new Vector3 (playerHead.position.x, playerHead.position.y / 2f, playerHead.position.z);

            Vector3 displacement = 2 * (targetPosition - startRayPosition).normalized;
            displacement.y /= 2f;
            nav.destination = targetPosition + displacement;
            nav.isStopped = false;
            animator.SetTrigger ("JumpAttack");
            yield return new WaitForSeconds (.3f);
            //RaycastHit hit;
            Debug.DrawLine (transform.position, targetPosition + displacement, Color.cyan, 3f);

            var hits = Physics.RaycastAll (startRayPosition, (targetPosition - transform.position).normalized * 1.5f, 2f);
            foreach (RaycastHit raycastHit in hits) {
                if (raycastHit.collider.tag == "Player") {
                    raycastHit.collider.GetComponentInParent<IDamage> ().TakeDamage (25);
                }
            }

            /*if (Physics.Linecast (transform.position, targetPosition + displacement, out hit)) {
                if (hit.collider.tag == "Player") {
                    hit.collider.GetComponentInParent<IDamage> ().TakeDamage (25f);
                }
            }*/
            animator.ResetTrigger ("JumpAttack");
            yield return new WaitForSeconds (.2f);

            preattackRoutine = StartCoroutine (WaitForRandomTimeToBeginAttacking ());
        }

        /// <summary>
        /// Plays a random snarl through the animator and audiosource.
        /// </summary>
        /// <returns>Audio source clip length</returns>
        float PlaySnarlAnimation () {
            animator.SetInteger ("Snarl", Random.Range (0, 2));
            audioSource.clip = snarls[Random.Range (0, snarls.Length)];
            audioSource.Play ();
            return audioSource.clip.length;
        }

        IEnumerator WaitForRandomTimeToBeginAttacking () {
            Debug.Log (gameObject.name + " beginning to wait for time to attack");
                Loiter ();
            yield return new WaitForSeconds (Random.Range (.4f, 2f));
            attackingInProgress = false;
            PickAttackStyle ();
        }

        public override void TakeDamage (float amount, bool soundOff) {
            base.TakeDamage (amount, soundOff);
        }

        protected virtual void Loiter () {
            nav.destination = Vector3.Distance (transform.position, new Vector3 (playerHead.position.x, playerBoundary.position.y, playerHead.position.z)) > 1.5f ? (new Vector3 (playerHead.position.x, playerBoundary.position.y, playerHead.position.z)) : transform.position;
        }

    }

}
