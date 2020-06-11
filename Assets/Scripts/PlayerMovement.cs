
using UnityEngine;

using VRTK;

namespace MetroVR {
    public class PlayerMovement : MonoBehaviour {

        public delegate void PlayerCollidersSetUp (Transform torso, Transform legs);
        public static event PlayerCollidersSetUp OnPlayerCollidersSetUp;

        [SerializeField] Transform cameraRig;
        [SerializeField] Rigidbody cameraRigRb;

        [SerializeField] Transform playerLegsTransform, playerTorsoTransform;
        [SerializeField] CapsuleCollider playerLegsCollider, playerTorsoCollider;

        [SerializeField] bool leftIsMovement = true;
        [SerializeField] float speed = 10f;
        [SerializeField] float rotationSpeed = 50f;
        [Range (0f,1f)]
        [SerializeField] float deadzone = 0.2f;

        [SerializeField] float footstepVolume = 0.15f;
        [SerializeField] AudioSource legsAudioSource, torsoAudioSource, rotationAudioSource;
        [SerializeField] AudioClip[] footsteps, jumpSounds, landingSounds, rotationClips;

        VRTK_ControllerEvents leftEvents, rightEvents;
        Transform head = null;

        bool isGrounded;
        float horizontal = 0;
        float fwd = 0;

        void Start () {
        }

        void OnEnable () {
            VRTK_SDKManager.instance.LoadedSetupChanged += Instance_LoadedSetupChanged;
            leftEvents = GameObject.FindGameObjectWithTag ("Left").GetComponent<VRTK_ControllerEvents> ();
            rightEvents = GameObject.FindGameObjectWithTag ("Right").GetComponent<VRTK_ControllerEvents> ();
        }

        void OnDisable () {
            VRTK_SDKManager.instance.LoadedSetupChanged -= Instance_LoadedSetupChanged;
        }

        void Instance_LoadedSetupChanged (VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
            //target for rigidbody and rotation movement is sender.loadedSetup.transform.GetChild (0)
            //aka the camera rig
            //target for the player collider is a new child of that object
            cameraRig = sender.loadedSetup.actualBoundaries.transform;
            cameraRigRb = cameraRig.gameObject.AddComponent<Rigidbody> ();
            cameraRigRb.constraints = RigidbodyConstraints.FreezeRotation;
            cameraRigRb.mass = 25;
            cameraRigRb.useGravity = true;

            CreateColliders (cameraRig);

            head = sender.loadedSetup.actualHeadset.transform;
        }

        void CreateColliders (Transform cameraRig) {
            GameObject legsGameObject = Instantiate (new GameObject ("Player Legs Collider"), gameObject.transform.position, Quaternion.identity, cameraRig);
            GameObject upperBodyGameObject = Instantiate (new GameObject ("Player Torso Collider"), gameObject.transform.position, Quaternion.identity, cameraRig);

            legsGameObject.AddComponent<Hitbox> ();
            upperBodyGameObject.AddComponent<Hitbox> ();

            legsAudioSource = legsGameObject.AddComponent<AudioSource> ();
            legsAudioSource.loop = false;
            legsAudioSource.playOnAwake = false;
            legsAudioSource.volume = footstepVolume;
            torsoAudioSource = upperBodyGameObject.AddComponent<AudioSource> ();
            torsoAudioSource.loop = false;
            torsoAudioSource.playOnAwake = false;
            torsoAudioSource.volume = footstepVolume;

            playerLegsTransform = legsGameObject.transform;
            playerTorsoTransform = upperBodyGameObject.transform;

            playerLegsCollider = legsGameObject.AddComponent<CapsuleCollider> ();
            playerLegsCollider.radius = 0.1f;
            playerTorsoCollider = upperBodyGameObject.AddComponent<CapsuleCollider> ();
            playerTorsoCollider.direction = 2;
            playerTorsoCollider.radius = 0.17f;

            legsGameObject.layer = gameObject.layer;
            legsGameObject.tag = "Player";
            upperBodyGameObject.layer = gameObject.layer;
            upperBodyGameObject.tag = "Player";

            if (OnPlayerCollidersSetUp != null) {
                OnPlayerCollidersSetUp.Invoke (playerTorsoTransform, playerLegsTransform);
            }
        }

        Vector3 moveDirection = Vector3.zero;
        bool attemptJump = false;
        bool playRotationSound = false;

        void Update () {
            if (Levels.Level01.Instance.canMove) {
                if (head != null) {
                    if (leftIsMovement) {
                        GetMovementInputs (leftEvents, rightEvents);
                    } else {
                        GetMovementInputs (rightEvents, leftEvents);
                    }

                    moveDirection = (horizontal * head.right + fwd * head.forward);

                    if (attemptJump && isGrounded) {
                        cameraRigRb.AddForce (0, 8f, 0, ForceMode.Impulse);
                        torsoAudioSource.clip = jumpSounds[Random.Range (0, jumpSounds.Length)];
                        torsoAudioSource.Play ();
                    }
                    if (playRotationSound) {
                        var f = Random.Range (0f, 1f);
                        if (f < 0.4f) {
                            rotationAudioSource.clip = rotationClips[Random.Range (0, rotationClips.Length)];
                            rotationAudioSource.Play ();
                        }
                    }
                }
            }
        }

        float movementX = 0;
        float movementY = 0;
        float rotationX = 0;
        float rotationY = 0;
        void GetMovementInputs (VRTK_ControllerEvents movement, VRTK_ControllerEvents rotation) {
            if (rotationX == 0 && (rotation.GetTouchpadAxis ().x > 0.3f || rotation.GetTouchpadAxis ().x < 0.3f)) {
                playRotationSound = true;
            } else {
                playRotationSound = false;
            }

            movementX = movement.GetTouchpadAxis ().x;
            movementY = movement.GetTouchpadAxis ().y;
            rotationX = rotation.GetTouchpadAxis ().x;
            rotationY = rotation.GetTouchpadAxis ().y;

            if (Mathf.Abs (movementX) > deadzone)
                horizontal = movement.GetTouchpadAxis ().x * speed;
            else
                horizontal = 0;

            if (Mathf.Abs (movementY) > deadzone)
                fwd = movement.GetTouchpadAxis ().y * speed;
            else
                fwd = 0;

            if (Mathf.Abs (rotationX) > deadzone) {
                cameraRig.RotateAround (new Vector3 (head.position.x, 0, head.position.z), Vector3.up, rotationX * rotationSpeed * Time.deltaTime);
            }

            //We only want the player to jump on a positive Y-axis movement
            if (rotationY > deadzone)
                attemptJump = true;
            else
                attemptJump = false;
        }

        [SerializeField] Vector3 positionBeforeLean;
        private bool previousGroundedState = true;
        void FixedUpdate () {
            if (head != null && Levels.Level01.Instance.canMove) {
                isGrounded = Physics.Raycast (new Vector3 (head.position.x, playerLegsCollider.transform.position.y + .43f * head.localPosition.y, head.position.z), Vector3.down, .65f * head.localPosition.y, 1 << 0);

                Footsteps ();

                //If the head is tilted, don't move the player legs under the head
                if (    (head.localRotation.eulerAngles.x > 15f && head.localRotation.eulerAngles.x < 345f) ||
                        (head.localRotation.eulerAngles.z > 15f && head.localRotation.eulerAngles.z < 345f)) {
                    if (Vector3.Distance (new Vector3 (positionBeforeLean.x, 0, positionBeforeLean.z), new Vector3 (head.localPosition.x, 0, head.localPosition.z)) > 0.45f) {
                        positionBeforeLean = head.localPosition;
                    }

                    playerLegsTransform.localPosition = new Vector3 (positionBeforeLean.x, 0, positionBeforeLean.z);
                    playerLegsCollider.center = new Vector3 (0, 0.21f * head.localPosition.y, 0);

                    playerTorsoTransform.localPosition = new Vector3 (head.localPosition.x / 2f, 0.42f * head.localPosition.y, head.localPosition.z / 2f);
                    var targetRot = Quaternion.LookRotation (head.position - playerTorsoTransform.position);
                    playerTorsoTransform.rotation = targetRot * Quaternion.Euler (0, 0, head.rotation.eulerAngles.y);
                    //Debug.Log (head.rotation.eulerAngles.y);
                    //Debug.Log (playerTorsoTransform.rotation.eulerAngles.y);

                    //playerTorsoTransform.LookAt (head.position, -playerTorsoTransform.up);
                    //playerTorsoTransform.rotation = Quaternion.Euler (playerTorsoTransform.rotation.x, playerTorsoTransform.rotation.y, head.rotation.eulerAngles.y);
                } else {
                    positionBeforeLean = head.localPosition;
                    playerLegsTransform.localPosition = new Vector3 (head.localPosition.x, 0, head.localPosition.z);
                    playerLegsCollider.center = new Vector3 (0, 0.21f * head.localPosition.y, 0);

                    playerTorsoTransform.localPosition = new Vector3 (head.localPosition.x, 0.42f * head.localPosition.y, head.localPosition.z);
                    playerTorsoCollider.center = new Vector3 (0, 0, 0.21f * head.localPosition.y);
                    playerTorsoTransform.localRotation = Quaternion.identity * Quaternion.Euler (-90, 0, head.rotation.eulerAngles.y);
                }

                playerLegsTransform.localRotation = Quaternion.Euler (0, head.rotation.eulerAngles.y, 0);

                playerLegsCollider.height = 0.42f * head.localPosition.y;
                playerTorsoCollider.height = 0.42f * head.localPosition.y;

                cameraRigRb.velocity = new Vector3 (moveDirection.x, cameraRigRb.velocity.y, moveDirection.z);
                timeSinceLastFootstep += Time.fixedDeltaTime;
                
                if (!previousGroundedState && isGrounded) {
                    //If we weren't grounded and now we are, play sound effect if y velocity < -3f
                    if (cameraRigRb.velocity.y < -3f) {
                        torsoAudioSource.clip = landingSounds[Random.Range (0, landingSounds.Length)];
                        torsoAudioSource.Play ();
                    }
                }

                previousGroundedState = isGrounded;
            }
        }

        float timeSinceLastFootstep = 0;
        void Footsteps () {
            if (new Vector3 (moveDirection.x, 0, moveDirection.z).sqrMagnitude > 0.8f) {
                if (isGrounded) {
                    if (timeSinceLastFootstep > 0.6f) {
                        legsAudioSource.clip = footsteps[Random.Range (0, footsteps.Length)];
                        legsAudioSource.Play ();
                        timeSinceLastFootstep = 0;
                    }
                }
            }
        }
    }

}
