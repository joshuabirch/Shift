using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	const float k_GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;

	public Animator animator;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	// Wall sliding edits

	private bool isTouchingWall;
	public float wallCheckDistance;
	public Transform wallCheck;
	private bool isWallSliding;
	public float WallSlideSpeed;

	// Ledge grab edits

	public Transform ledgeCheck;
	private bool isTouchingLedge;
	private bool canClimbLedge = false;
	private bool ledgeDetected;
	private Vector2 ledgePosBot;
	private Vector2 ledgePos1;
	private Vector2 ledgePos2;
	public float ledgeClimbXOffset1;
	public float ledgeClimbYOffset1;
	public float ledgeClimbXOffset2;
	public float ledgeClimbYOffset2;

	// Prevent player input
	
	public static bool freezeInput = false;

	// Dodge roll/dash edits

	private bool isDashing;
	public float dashTime;
	public float dashSpeed;
	public float dashCooldown;
	private float dashTimeLeft;
	private float lastDash = -100f;

	// falling edits

	private bool isFalling = false;


	void Update()
	{
		CheckIfWallSliding();
		CheckLedgeClimb();
		UpdateAnimations();
		checkDash();
		CheckIfFalling();
	}

	private void CheckIfFalling()
	{
		if (GetComponent<Rigidbody2D>().velocity.y < -1 && !isWallSliding)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
	}

	// Animation controller

	private void UpdateAnimations()
	{
		animator.SetBool("IsWallSliding", isWallSliding);
		animator.SetBool("IsGrounded", m_Grounded);
		if (isWallSliding == true)
		{
			animator.SetBool("IsJumping", false);
		}
		animator.SetBool("IsFalling", isFalling);
		if (isFalling == true)
		{
			animator.SetBool("IsJumping", false);
		}
	}

	// Wall sliding edits
	private void CheckIfWallSliding()
	{
		if(isTouchingWall && !m_Grounded && m_Rigidbody2D.velocity.y < 0 && !canClimbLedge)
		{
			isWallSliding = true;
		}
		else
		{
			isWallSliding = false;
		}
	}

	private void CheckLedgeClimb()
	{
		if (ledgeDetected && !canClimbLedge)
		{
			canClimbLedge = true;

			if (m_FacingRight)
			{
				ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
				ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2 + 0.78f, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2 + 1.59f);
			}
			else
			{
				ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
				ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2 -0.767f, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2 + 1.59f);
			}

			freezeInput = true;

			animator.SetBool("canClimbLedge", canClimbLedge);
		}

		if (canClimbLedge)
		{
			transform.position = ledgePos1;
		}
	}

	public void FinishLedgeClimb()
	{
		canClimbLedge = false;
		freezeInput = false;
		ledgeDetected = false;
		animator.SetBool("canClimbLedge", canClimbLedge);
		transform.position = ledgePos2;
	}


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

		isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, m_WhatIsGround);

		isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, m_WhatIsGround);

		if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
		{
			ledgeDetected = true;
			ledgePosBot = wallCheck.position;
		}

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump, bool roll)
	{

		// If crouching, check to see if the character can stand up
		if (crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			if (isWallSliding)
			{
				if (m_Rigidbody2D.velocity.y < -WallSlideSpeed)
				{
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -WallSlideSpeed);
				}
			}

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = true;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		if (m_Grounded && roll)
		{
			m_Grounded = true;
			attemptToDash();
		}
		
	}

	private void attemptToDash()
	{
		isDashing = true;
		dashTimeLeft = dashTime;
		lastDash = Time.time;
	}

	private void checkDash()
	{
		if (isDashing)
		{
			if (dashTimeLeft > 0)
			{
				freezeInput = true;
				animator.SetBool("IsRolling", true);
				if (m_FacingRight)
				{
					m_Rigidbody2D.velocity = new Vector2(dashSpeed * 1f, m_Rigidbody2D.velocity.y);
				}
				else
				{
					m_Rigidbody2D.velocity = new Vector2(dashSpeed * -1f, m_Rigidbody2D.velocity.y);
				}
				dashTimeLeft -= Time.deltaTime;
			}

			if (dashTimeLeft <= 0)
			{
				isDashing = false;
				StartCoroutine(EndRoll());
			}
			if (!m_Grounded)
			{
				isDashing = false;
				animator.SetBool("IsRolling", false);
				freezeInput = false;
			}
		}
	}

	private IEnumerator EndRoll()
	{
		animator.SetBool("IsRolling", false);
		m_Rigidbody2D.drag = 100;
		yield return new WaitForSeconds(1f);
		freezeInput = false;
		m_Rigidbody2D.drag = 0;
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0f, 180f, 0f);
	}

	private void OnDrawGizmos() 
	{
		Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
	}
}
