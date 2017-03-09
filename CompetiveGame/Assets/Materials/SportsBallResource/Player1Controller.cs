﻿using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
	[RequireComponent(typeof (CharacterController))]
	[RequireComponent(typeof (AudioSource))]
	public class Player1Controller : MonoBehaviour
	{
		[SerializeField] private float m_Speed= 20.0f;
		[SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0.51f;
		[SerializeField] private float m_JumpSpeed = 40.0f;
		[SerializeField] private float m_StickToGroundForce = 4.0f;
		[SerializeField] private float m_GravityMultiplier = 10.0f;
		[SerializeField] private bool m_UseFovKick;
		[SerializeField] private float m_StepInterval = 5.0f;
		[SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
		[SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
		[SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [Range(0f, 1f)]
        public float AirControl = 0.2f;
        public float AirDegredationRate = 0.95f;
        public string JumpInputString = "Player1Jump";
        public string HorizontalMoveAxisInputString = "Player1MoveHorizontal";
        public string VerticalMoveAxisInputString = "Player1MoveVertical";
        public string HorizontalLookAxisInputString = "Player1LookHorizontal";
        public string VerticalLookAxisInputString = "Player1LookVertical";
        public string ShootAxisInputString = "Player1Shoot";
        public RocketLauncher m_gun;
        public float MoveRate = 5.0f;

        private bool GameOver = false;
		private Camera m_Camera;
		private bool m_Jump;
        private bool m_KnockBack = false;
		private float m_YRotation;
		private Vector2 m_Input;
		private Vector3 m_MoveDir = Vector3.zero;
		private CharacterController m_CharacterController;
		private CollisionFlags m_CollisionFlags;
		private bool m_PreviouslyGrounded;
		private Vector3 m_OriginalCameraPosition;
		private float m_StepCycle;
		private float m_NextStep;
		private bool m_Jumping;
		private AudioSource m_AudioSource;
		private Vector3 DownVector;
		private Vector3 StartingDownVector;
		private Vector3 m_PreviousMovement;
		private Quaternion m_DefaultRotation;
        private float m_knockbackTimer = 0.5f;
		
		float rotationY = 0F;
		
		// Use this for initialization
		private void Start(){
			m_CharacterController = GetComponent<CharacterController>();
			m_Camera = Camera.main;
			m_OriginalCameraPosition = m_Camera.transform.localPosition;
			m_Jumping = false;
			m_AudioSource = GetComponent<AudioSource>();
			m_PreviousMovement = Vector3.zero;
			DownVector = Vector3.zero;
			DownVector.y = -1;
			m_DefaultRotation = gameObject.transform.rotation;
		}

		void GameOverfun()
		{
			GameOver = true;
		}
		
		// Update is called once per frame
		private void Update()
		{
			if (GameOver) {
				return;
			}
			bool grounded = IsGrounded ();
			ControllerRotateView ();
			// the jump state needs to read here to make sure it is not missed
			m_Jump = CrossPlatformInputManager.GetButtonDown(JumpInputString);
			
			if (!m_PreviouslyGrounded && grounded)
			{
				//PlayLandingSound();
				m_MoveDir.y = 0f;
				m_Jumping = false;
			}
			if (!grounded && !m_Jumping && m_PreviouslyGrounded)
			{
				m_MoveDir.y = 0f;
			}
			
			m_PreviouslyGrounded = grounded;

            if(CrossPlatformInputManager.GetAxis(ShootAxisInputString) < -0.5)
            {
                m_gun.onShoot();

            }
		}
		
		
		private void PlayLandingSound()
		{
			m_AudioSource.clip = m_LandSound;
			m_AudioSource.Play();
			m_NextStep = m_StepCycle + .5f;
		}


        private void FixedUpdate()
        {
            float speed;

            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
            if (m_KnockBack)
            {
                if (m_knockbackTimer <= 0.0f)
                {
                    m_KnockBack = false;
                }
                m_PreviousMovement.x += desiredMove.x * speed * AirControl;
                m_PreviousMovement.z += desiredMove.z * speed * AirControl;
                m_PreviousMovement.y -= m_GravityMultiplier;
                m_PreviousMovement *= AirDegredationRate;
                m_MoveDir = m_PreviousMovement;
                m_knockbackTimer -= Time.fixedDeltaTime;
            }
            else if (m_Jumping) {
                m_PreviousMovement.x += desiredMove.x * speed * AirControl;
                m_PreviousMovement.z += desiredMove.z * speed * AirControl;
                m_PreviousMovement.y -= m_GravityMultiplier;
                m_PreviousMovement *= AirDegredationRate;
                m_MoveDir = m_PreviousMovement;
            }
            else {
                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;
            }
			
			bool grounded = IsGrounded ();
			if (grounded)
			{
				m_MoveDir.y = m_StickToGroundForce * DownVector.y;
				if (m_Jump)
				{
                    m_MoveDir += m_JumpSpeed * -DownVector;
                    m_Jump = false;
					m_Jumping = true;
				}
                else if(m_Jumping)
                {
                    m_Jumping = false;
                }
			}
			m_PreviousMovement = m_MoveDir;
			m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);
			
			ProgressStepCycle(speed);
		}
		
		private void ProgressStepCycle(float speed)
		{
			if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
			{
				m_StepCycle += (m_CharacterController.velocity.magnitude + (speed* m_RunstepLenghten))*
					Time.fixedDeltaTime;
			}
			
			if (!(m_StepCycle > m_NextStep))
			{
				return;
			}
			
			m_NextStep = m_StepCycle + m_StepInterval;
			
			//PlayFootStepAudio();
		}
		
		private void PlayFootStepAudio()
		{
			bool grounded = IsGrounded ();
			if (!grounded)
			{
				return;
			}
			// pick & play a random footstep sound from the array,
			// excluding sound at index 0
			int n = Random.Range(1, m_FootstepSounds.Length);
			m_AudioSource.clip = m_FootstepSounds[n];
			m_AudioSource.PlayOneShot(m_AudioSource.clip);
			// move picked sound to index 0 so it's not picked next time
			m_FootstepSounds[n] = m_FootstepSounds[0];
			m_FootstepSounds[0] = m_AudioSource.clip;
		}
		
		private void GetInput(out float speed)
		{
			// Read input
			float horizontal = CrossPlatformInputManager.GetAxis(HorizontalMoveAxisInputString);
			float vertical = CrossPlatformInputManager.GetAxis(VerticalMoveAxisInputString);
	
			// set the desired speed to be walking or running
			speed = m_Speed;
			m_Input = new Vector2(horizontal, vertical);
			
			// normalize input if it exceeds 1 in combined length:
			if (m_Input.sqrMagnitude > 1)
			{
				m_Input.Normalize();
			}
		}
		
		private void ControllerRotateView()
		{
			JoyStickLook ();
		}
		
		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			Rigidbody body = hit.collider.attachedRigidbody;
			//dont move the rigidbody if the character is on top of it
			if (m_CollisionFlags == CollisionFlags.Below)
			{
				return;
			}
			
			if (body == null || body.isKinematic)
			{
				return;
			}
			body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
		}
		
		public void FlipGravity()
		{
			DownVector = - DownVector;
		}
		
		private bool IsGrounded()
		{
			float height = m_CharacterController.height;
			RaycastHit info;
			if (Physics.Raycast (transform.position, DownVector, out info, height / 2 * 1.2f)) {
				if(info.collider.isTrigger == false)
					return true;
			}
			return false;
		}
		
		private void JoyStickLook ()
		{
			//Used for Joy look
			float joysensitivityX = 3F;
			float joysensitivityY = 3F;
			
			float minimumY = -60F;
			float maximumY = 60F;
			
			float Xon = Mathf.Abs (CrossPlatformInputManager.GetAxis(HorizontalLookAxisInputString));
			float Yon = Mathf.Abs (CrossPlatformInputManager.GetAxis(VerticalLookAxisInputString));
			//print (Xon);
			if (Xon>.05){
				transform.Rotate(0, CrossPlatformInputManager.GetAxis(HorizontalLookAxisInputString) * joysensitivityX, 0);
				float spin = transform.eulerAngles.y;
				spin += CrossPlatformInputManager.GetAxis(HorizontalLookAxisInputString) * joysensitivityX;
				Vector3 newAngle = new Vector3 (transform.eulerAngles.x, spin, transform.eulerAngles.z);
				transform.eulerAngles = newAngle;
			}
			
			if (Yon>.05){
				rotationY += CrossPlatformInputManager.GetAxis(VerticalLookAxisInputString) * joysensitivityY;
			}
			rotationY += CrossPlatformInputManager.GetAxis(VerticalLookAxisInputString) * joysensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}

        void Bounce (Vector3 Direction)
        {
            m_KnockBack = true;
            m_knockbackTimer = 0.5f;
            m_PreviousMovement += Direction * MoveRate;
        }


    }
	
	
}