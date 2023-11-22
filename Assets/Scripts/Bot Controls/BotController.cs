using UnityEngine;
using System.Collections;
using System;
// using UnityEditor.Scripting.Python;
using Python.Runtime;
// using UnityEditor;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class BotController : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;

		private DoubleTapDetector qTapDetector = new DoubleTapDetector();
		private DoubleTapDetector wTapDetector = new DoubleTapDetector();
		private DoubleTapDetector eTapDetector = new DoubleTapDetector();
		private DoubleTapDetector fTapDetector = new DoubleTapDetector();
		private DoubleTapDetector tabTapDetector = new DoubleTapDetector();

		public PlayerDamage playerAtm;
		public EnemyDamage enemyAtm;

		public PlayerNoTriggerCollision noTriggerCollision;
		public PlayerTriggerCollision triggerCollision;

		public String attackName;
		public bool dealingDamage = false;

		public Animator animator;
		public bool isBlocking = false;


		void Start()
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;

		}

		public void Update()
		{
			if(qTapDetector.CheckForDoubleTap(KeyCode.Q))
            {
                if (m_IsGrounded)
				{
					m_Animator.SetTrigger("UpStrong");
				}
				attackName = "UpStrong";
				StartCoroutine(CheckCollisionDurationStrongDamage(5.0f));
			}
            else if (qTapDetector.isSingleTapPending && qTapDetector.tapTimer <= 0)
            {
				if (m_IsGrounded)
				{
					m_Animator.SetTrigger("UpLight");
				}
				attackName = "UpLight";
                qTapDetector.isSingleTapPending = false;
				StartCoroutine(CheckCollisionDurationLightDamage(5.0f));
			}


			if(wTapDetector.CheckForDoubleTap(KeyCode.W))
            {
                if (m_IsGrounded)
				{
					m_Animator.SetTrigger("ForwardStrong");
				}
				attackName = "ForwardStrong";
				StartCoroutine(CheckCollisionDurationStrongDamage(5.0f));
            }
			else if(wTapDetector.isSingleTapPending && wTapDetector.tapTimer <= 0)
            {
                if (m_IsGrounded)
				{
					m_Animator.SetTrigger("ForwardLight");
				}
				attackName = "ForwardLight";
				wTapDetector.isSingleTapPending = false;
				StartCoroutine(CheckCollisionDurationLightDamage(5.0f));
            }

			if(eTapDetector.CheckForDoubleTap(KeyCode.E))
            {
                if (m_IsGrounded)
				{
					m_Animator.SetTrigger("DownStrong");
				}
				attackName = "DownStrong";
				StartCoroutine(CheckCollisionDurationStrongDamage(5.0f));
            }
            else if (eTapDetector.isSingleTapPending && eTapDetector.tapTimer <= 0)
            {
                if (m_IsGrounded)
				{
					m_Animator.SetTrigger("DownLight");
				}
				attackName = "DownLight";
                eTapDetector.isSingleTapPending = false;
				StartCoroutine(CheckCollisionDurationLightDamage(6.0f));
			}

			if(fTapDetector.CheckForDoubleTap(KeyCode.F))
            {
                return;
            }
            else if (fTapDetector.isSingleTapPending && fTapDetector.tapTimer <= 0)
            {
                if (m_IsGrounded)
				{
					m_Animator.SetTrigger("Ariel");
				}
				attackName = "Ariel";
                fTapDetector.isSingleTapPending = false;
				StartCoroutine(CheckCollisionDurationStrongDamage(3.0f));
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				isBlocking = true;
				animator.SetBool("IsBlocking", isBlocking);
			}

			if (Input.GetKeyUp(KeyCode.Tab))
			{
				isBlocking = false;
				animator.SetBool("IsBlocking", isBlocking);
			}


		}

				IEnumerator CheckCollisionDurationStrongDamage(float duration)
		{
			float startTime = Time.time;  // Remember the time we started the checking.

			// Keep checking within the duration.
			while (Time.time - startTime < duration)
			{
				if (noTriggerCollision.isCollided || triggerCollision.isCollided)  // If collided, do the action.
				{
					dealingDamage = true;
					playerAtm.DealDamage(enemyAtm.gameObject, 10);
					yield break;  // Exit the loop and coroutine since we've dealt damage.
				}
				yield return null;  // Wait for the next frame.
			}
			dealingDamage = false;
			attackName = "";

		}

		IEnumerator CheckCollisionDurationLightDamage(float duration)
		{
			float startTime = Time.time;  // Remember the time we started the checking.

			// Keep checking within the duration.
			while (Time.time - startTime < duration)
			{
				if (triggerCollision.isCollided || noTriggerCollision.isCollided)  // If collided, do the action.
				{
					dealingDamage = true;
					playerAtm.DealDamage(enemyAtm.gameObject, 5);
					yield break;  // Exit the loop and coroutine since we've dealt damage.
				}
				yield return null;  // Wait for the next frame.
			}
			dealingDamage = false;
			attackName = "";

		}



		public void Move(Vector3 move, bool crouch, bool jump)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);

			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}

			ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}


		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded);
			if (!m_IsGrounded)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded)
			{
				m_Animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}

		}


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;
				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}

			Vector3 newPosition = transform.position;
			newPosition.z = 0;
			transform.position = newPosition;
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}


	}

}
