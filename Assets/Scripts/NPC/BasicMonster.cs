
using System.Collections;
using UnityEngine;

namespace MetroVR.NPC {

    public class BasicMonster : Npc {

        delegate void MonsterAttack (bool started, BasicMonster monster);
        static event MonsterAttack OnMonsterAttack;

        protected bool playerIsAlreadyUnderAttack = false;

        [SerializeField] AudioClip[] snarls;

        protected override void Start () {
            base.Start ();
        }

        protected override void OnEnable () {
            base.OnEnable ();

            OnMonsterAttack += BasicMonster_OnMonsterAttack;
        }

        protected override void OnDisable () {
            base.OnDisable ();

            OnMonsterAttack -= BasicMonster_OnMonsterAttack;
        }

        protected override void FightingPlayer () {
            if (playerIsAlreadyUnderAttack)
                Loiter ();
            else
                SwoopPlayer ();
        }

        private Coroutine preattackRoutine, swoopingRoutine;

        protected virtual void BasicMonster_OnMonsterAttack (bool started, BasicMonster monster) {
            //Check if this is the start of a new monster attack,
            //and if it wasn't called by this instance
            //then the player is already under attack and this instance
            //should not begin attacking
            if (started && monster != this) {
                playerIsAlreadyUnderAttack = true;
            } else if (!started) {
                if (preattackRoutine != null)
                    StopCoroutine (preattackRoutine);
                preattackRoutine = StartCoroutine (WaitForRandomTimeToBeginAttacking ());
            }
        }

        IEnumerator WaitForRandomTimeToBeginAttacking () {
            yield return new WaitForSeconds (Random.Range (1f, 5f));
            if (playerIsAlreadyUnderAttack) {
                yield return null;
            } else {
                SwoopPlayer ();
            }
        }

        protected virtual void SwoopPlayer () {
            if (!playerIsAlreadyUnderAttack) {
                if (OnMonsterAttack != null) {
                    OnMonsterAttack.Invoke (true, this);
                }
                swoopingRoutine = StartCoroutine (SwoopPlayerRoutine ());
            }
        }

        IEnumerator SwoopPlayerRoutine () {
            animator.SetInteger ("Snarl", Random.Range (0,2));
            audioSource.PlayOneShot (snarls[Random.Range (0, snarls.Length)]);
            Vector3 startRayPosition = transform.position;
            Vector3 targetPosition = new Vector3 (playerHead.position.x, playerHead.position.y - 0.3f, playerHead.position.z);
            nav.isStopped = true;
            if (Vector3.Distance (startRayPosition, targetPosition) < 7f) {
                Vector3 endPosition = targetPosition + 0.25f * (startRayPosition - targetPosition);
                yield return new WaitForSeconds (0.5f);
                animator.SetTrigger ("JumpAttack");
                RaycastHit hit;
                if (Physics.Linecast (startRayPosition, targetPosition, out hit)) {
                    if (hit.collider.gameObject.tag == "Player") {
                        hit.collider.gameObject.GetComponent<Hitbox> ().Hit (20f);
                    }
                }
                nav.destination = endPosition;
            }
            if (OnMonsterAttack != null) {
                OnMonsterAttack.Invoke (false, this);
            }
            yield return new WaitForSeconds (0.3f);
            animator.ResetTrigger ("JumpAttack");
        }

        public override void TakeDamage (float amount) {
            base.TakeDamage (amount);
        }

        protected virtual void Loiter () {
            nav.destination = Vector3.Distance (transform.position, playerHead.position) > 2f ? playerHead.position : transform.position;
        }

    }

}
