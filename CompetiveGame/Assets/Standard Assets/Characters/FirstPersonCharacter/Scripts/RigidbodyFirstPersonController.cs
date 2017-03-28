using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
	        public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}
#if !MOBILE_INPUT
	            if (Input.GetKey(RunKey))
	            {
		            CurrentTargetSpeed *= RunMultiplier;
		            m_Running = true;
	            }
	            else
	            {
		            m_Running = false;
	            }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();
        public GameObject m_gun;
        public string JumpInputString = "Player1Jump";
        public string HorizontalMoveAxisInputString = "Player1MoveHorizontal";
        public string VerticalMoveAxisInputString = "Player1MoveVertical";
        public string HorizontalLookAxisInputString = "Player1LookHorizontal";
        public string VerticalLookAxisInputString = "Player1LookVertical";
        public string ShootAxisInputString = "Player1Shoot";
        public float airMoveSpeed = 1.0f;
        public float BounceAmount = 50.0f;
        public float m_MaxBounceTime = 20.0f;

        private float m_BounceTimer = 0.0f;
        private bool m_bouncing = false;
        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;

        public float Look_Multiplier_Default = 0.4f;
        private bool is_looking_X = false;
        private float X_Look_Multiplier = 0.2f;
        private bool is_looking_Y = false;
        private float Y_Look_Multiplier = 0.2f;
        [HideInInspector]
        public bool Deactivated = true;
        public GameObject VerticalLookTarget;

        public AudioSource JumpSound;
        public AudioSource[] Step;
        public AudioSource Falling;

        bool canStep = true;
        bool isFalling = false;

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
 #if !MOBILE_INPUT
				return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init (transform, cam.transform);
        }


        private void Update()
        {
            if (Deactivated) return;
            //RotateView();
            JoyStickLook();

            if (CrossPlatformInputManager.GetButtonDown(JumpInputString) && !m_Jump)
            {
                m_Jump = true;
            }
        }


        private void FixedUpdate()
        {
            if (Deactivated) return;
            if(m_bouncing)
            {
                m_BounceTimer -= Time.fixedDeltaTime;
                if(m_BounceTimer <= 0)
                {
                    m_bouncing = false;
                }
                else
                {
                    return;
                }
            }
            GroundCheck();
            
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward*input.y + cam.transform.right*input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal);

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed*movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove*SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (m_IsGrounded)
            {
               if(canStep && GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
                {
                    canStep = false;
                     Step[(int) UnityEngine.Random.Range(0, 3)].Play();
                    StartCoroutine(StepCooldown());
                }
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    JumpSound.Play();
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                m_RigidBody.drag = 0f;
                desiredMove.x = desiredMove.x * airMoveSpeed;
                desiredMove.z = desiredMove.z * airMoveSpeed;
                desiredMove.y = desiredMove.y * airMoveSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }

                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;

            if(CrossPlatformInputManager.GetAxis(ShootAxisInputString) < -0.5f)
            {
                m_gun.BroadcastMessage("onShoot", SendMessageOptions.DontRequireReceiver);
            }
        }

        global::System.Collections.IEnumerator StepCooldown()
        {
            yield return new WaitForSeconds(0.5f);
            canStep = true;
        }

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius, Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {

            Vector2 input = new Vector2(
                CrossPlatformInputManager.GetAxis(HorizontalMoveAxisInputString),
                CrossPlatformInputManager.GetAxis(VerticalMoveAxisInputString)                        
                                        );
            movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }

        private void JoyStickLook()
        {
            //Used for Joy look
            float joysensitivityX = 3F;
            float joysensitivityY = 3F;

            float minimumY = -60F;
            float maximumY = 60F;

            float Xon = Mathf.Abs(CrossPlatformInputManager.GetAxis(HorizontalLookAxisInputString));
            float Yon = Mathf.Abs(CrossPlatformInputManager.GetAxis(VerticalLookAxisInputString));
            //print (Xon);
            if (Xon > .01)
            {
                if(!is_looking_X)
                {
                    //StartCoroutine(X_Look());
                    is_looking_X = true;
                }
                transform.Rotate(0, CrossPlatformInputManager.GetAxis(HorizontalLookAxisInputString) * joysensitivityX, 0);
                float spin = transform.eulerAngles.y;
                spin += CrossPlatformInputManager.GetAxis(HorizontalLookAxisInputString) * joysensitivityX;
                Vector3 newAngle = new Vector3(transform.eulerAngles.x, spin, transform.eulerAngles.z);
                transform.eulerAngles = newAngle;
            }
            else
            {
                X_Look_Multiplier = Look_Multiplier_Default;
                is_looking_X = false;
            }

            if (Yon > .01)
            {
                if (!is_looking_Y)
                {
                    //StartCoroutine(Y_Look());
                    is_looking_Y = true;
                }
                m_YRotation += CrossPlatformInputManager.GetAxis(VerticalLookAxisInputString) * joysensitivityY;
            }
            else
            {
                Y_Look_Multiplier = Look_Multiplier_Default;
                is_looking_Y = false;
            }

            m_YRotation += CrossPlatformInputManager.GetAxis(VerticalLookAxisInputString) * joysensitivityY;
            m_YRotation = Mathf.Clamp(m_YRotation, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-m_YRotation, transform.localEulerAngles.y, 0);
        }



        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius, Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }

        void Bounce(Vector3 direction)
        {
            if (!m_bouncing)
            {
                m_Jumping = true;
                m_RigidBody.AddForce(BounceAmount * (direction + new Vector3(0,1,0)), ForceMode.Impulse);
                m_bouncing = true;
                m_BounceTimer = m_MaxBounceTime;
            }
        }

        
    }
}
