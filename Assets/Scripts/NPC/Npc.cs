
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AI;
using VRTK;

namespace MetroVR.NPC {

    public enum NpcState {
        Idle,
        SearchingForPlayer,
        MoveToPlayer,
        FightingPlayer,
        FightingNpcs
    }

    //Refer to https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition/blob/master/Chapter06/Assets/Scripts/Samples/CardGame/BehaviorTrees/EnemyBehaviorTree.cs

    public class Npc : MonoBehaviour, IDamage {

        [SerializeField] protected string npcName;
        [SerializeField] protected bool damageable = true;
        [SerializeField] protected bool hostileToPlayer = false;
        [SerializeField] protected int maxHp;
        [SerializeField] protected float currentHp;
        public float CurrentHP {
            get {
                return currentHp;
            }
        }

        [SerializeField] protected AudioClip[] footsteps, hitSounds;
        [SerializeField] protected AudioClip deathAudioClip;
        [SerializeField] protected AudioSource audioSource, footstepAudioSource;

        [Tooltip ("Maximum number of animations that can be used")]
        [SerializeField] int maxIdles;
        [Tooltip ("Generated in Start function")]
        [SerializeField] int currentIdle;

        [SerializeField] protected Transform headBone;
        protected NpcState state;

        protected Animator animator;
        protected NavMeshAgent nav;

        protected int tickDelay = 1000; //Milliseconds
        protected int secondaryTickMultiplier = 5;

        protected Transform playerHead, playerBoundary, playerHandLeft, playerHandRight;
        protected Vector3 playerEstimatedBodyPosition;

        public int ID {
            get {
                return id;
            }
        }
        [SerializeField] private int id;

        protected virtual void Start () {
            animator = GetComponent<Animator> ();
            SetPlayerTransforms ();

            nav = GetComponent<NavMeshAgent> ();

            nav.Warp (transform.position);
            currentIdle = Random.Range (0, maxIdles);

            if (playerHead != null) {
                if (hostileToPlayer) {
                    state = Vector3.Distance (transform.position, playerHead.position) > 5f ? NpcState.MoveToPlayer : NpcState.FightingPlayer;
                } else {
                    state = NpcState.Idle;
                }
            }
        }

        public virtual void ForceSetHP (float hp) {
            this.currentHp = hp;
        }

        protected virtual void FixedUpdate () {
            if (currentHp > 0) {
                if (playerHead != null) {
                    if (hostileToPlayer) {
                        state = Vector3.Distance (transform.position, playerHead.position) > 1.5f ? NpcState.MoveToPlayer : NpcState.FightingPlayer;
                    } else {
                        state = NpcState.Idle;
                    }

                    switch (state) {
                        case NpcState.FightingPlayer:
                            FightingPlayer ();
                            break;
                        case NpcState.MoveToPlayer:
                            MoveToPlayer ();
                            break;
                    }
                }

                SetAnimatorMovementSpeed ();
            }
        }

        protected virtual void SetAnimatorMovementSpeed () {
            if (nav.velocity.magnitude > 0.05f) {
                animator.SetFloat ("Movespeed", nav.velocity.magnitude);
            } else {
                animator.SetInteger ("Idle", currentIdle);
            }
        }

        protected virtual void PlayFootstep () {
            footstepAudioSource.clip = footsteps[Random.Range (0, footsteps.Length)];
            footstepAudioSource.Play ();
        }

        /// <summary>
        /// Override this in sub classes based on requirements, ie monsters, gunman npcs
        /// </summary>
        protected virtual void FightingPlayer () { }

        protected virtual void MoveToPlayer () {
            if (playerHead != null)
                nav.SetDestination (new Vector3 (playerHead.position.x, playerBoundary.position.y, playerHead.position.z));
        }

        //Task t;

        protected virtual void OnEnable () {
            VRTK_SDKManager.instance.LoadedSetupChanged += VRTK_SetupLoaded;
            //t = Task.Run (() => UpdateTick ());
        }

        protected virtual void OnDisable () {
            VRTK_SDKManager.instance.LoadedSetupChanged -= VRTK_SetupLoaded;
        }

        /// <summary>
        /// Updates every one second.
        /// </summary>
        protected async virtual void UpdateTick () {
            while (currentHp > 0) {
                UpdateTickContent ();
                await Task.Delay (tickDelay);
            }
        }

        protected virtual void UpdateTickContent () {
            if (nav.pathStatus == NavMeshPathStatus.PathPartial) {
                //Flee
            }
        }

        /// <summary>
        /// Updates every tick delay * secondary tick multiplier;
        /// </summary>
        protected async virtual void UpdateTickSecondary () {
            while (currentHp > 0) {
                UpdateTickSecondaryContent ();
                await Task.Delay (tickDelay * secondaryTickMultiplier);
            }
        }

        protected virtual void UpdateTickSecondaryContent () {

        }
        protected virtual void PlayAnimationWithName (string animationName) {
            animator.Play (animationName);
        }

        protected virtual void NavWarp () {
            nav.Warp (transform.position);
        }

        protected virtual void PathPartial () { }

        protected void VRTK_SetupLoaded (VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            SetPlayerTransforms ();
        }

        protected virtual void SetPlayerTransforms () {
            if (VRTK_SDKManager.instance.loadedSetup != null) {
                playerHead = VRTK_SDKManager.instance.loadedSetup.actualHeadset.transform;
                playerBoundary = VRTK_SDKManager.instance.loadedSetup.actualBoundaries.transform;
                playerHandLeft = VRTK_SDKManager.instance.loadedSetup.actualLeftController.transform;
                playerHandRight = VRTK_SDKManager.instance.loadedSetup.actualRightController.transform;
            }
        }

        protected virtual void AttemptToLocatePlayer () {
            if (!hostileToPlayer) {
                return;
            } else {
                
            }
        }

        protected virtual void OnAnimatorIK (int layerIndex) {
            if (animator) {
                if (state == NpcState.FightingPlayer && playerHead != null) {
                    animator.SetLookAtPosition (playerHead.position);
                    animator.SetLookAtWeight (0.7f, 0.3f);
                }
            }
        }

        public virtual void TakeDamage (float amount) {
            if (damageable) {
                hostileToPlayer = true;
                state = NpcState.MoveToPlayer;
                currentHp -= amount;
                if (currentHp < 0) {
                    OnDeath ();
                }
            }
        }

        protected virtual void OnDeath () {
            animator.SetTrigger ("Dead");

            StartCoroutine (DisableAnimatorAfterDelay ());
            audioSource.clip = deathAudioClip;
            audioSource.Play ();
            nav.enabled = false;
        }

        IEnumerator DisableAnimatorAfterDelay () {
            yield return new WaitForSeconds (0.4f);
            animator.enabled = false;
            var rbs = GetComponentsInChildren<Rigidbody> ();
            foreach (Rigidbody rb in rbs) {
                rb.isKinematic = false;
            }
        }

        public virtual void ForceState (NpcState state) {
            this.state = state;
        }

        /*
        //move to separate script? https://www.youtube.com/watch?v=NYysvuyivc4
        public RenderTexture sourceTexture;
        float LightLevel;
        public int Light;

        void Update () {
            RenderTexture tmp = RenderTexture.GetTemporary (
                        sourceTexture.width,
                        sourceTexture.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit (sourceTexture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D myTexture2D = new Texture2D (sourceTexture.width, sourceTexture.height);

            myTexture2D.ReadPixels (new Rect (0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply ();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary (tmp);

            Color32[] colors = myTexture2D.GetPixels32 ();
            LightLevel = 0;
            for (int i = 0; i < colors.Length; i++) {
                LightLevel += (0.2126f * colors[i].r) + (0.7152f * colors[i].g) + (0.0722f * colors[i].b);
            }
            LightLevel -= 259330;
            LightLevel = LightLevel / colors.Length;
            Light = Mathf.RoundToInt (LightLevel);
        }*/

    }

}
