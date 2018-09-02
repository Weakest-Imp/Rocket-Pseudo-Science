using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[System.Serializable]
	public class PlayerMovement {
		public float groundSpeed = 13;
		public float jumpForce = 30;
		public float jumpBoost = 1.5f;
		public float airSpeed = 7;
		public float airJumpForce = 20;
		//Gravity is 10 G in Rigidbody2D

		public int totalAirJumps = 1;
		public float jumpCooldown = 0.3f;
		public float coyoteTime = 0.1f;

		public Transform groundCheck;
		public float checkRadius = 0.15f;
		public LayerMask whatIsGround;
	}
	[SerializeField] private PlayerMovement movement;	

	private float moveInput;
	private float moveInputRaw;
	private float jumpInput;
	private int airJumpAvailable;
	private float currentJumpCooldown;
	private bool coyoteTimeActive;

	private bool facingRight = true;
	private bool isGrounded;
	private bool wasGrounded;

	[System.Serializable]
	public class PlayerBullet {
		public GameObject prefab;
		public float shootCooldown = 0.5f;
		public float recoilForce = 10;
		public float recoilTime = 0.5f;
	}
	[SerializeField] PlayerBullet bullet;

	float currentShootCooldown;


	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		airJumpAvailable = movement.totalAirJumps;
		currentJumpCooldown = 0;
		coyoteTimeActive = false;
	}

	void FixedUpdate () {
		wasGrounded = isGrounded;
		isGrounded = Grounded ();
		if (isGrounded) {
			if (!wasGrounded) {
				//Upon landing
				StopCoroutine("AirMove");
				RestoreAirJump ();
			}
			GroundMove ();
			ShootOnGround ();
			JumpFromGround ();
		} else {
			if (wasGrounded) {
				//Upon quitting ground
				StartCoroutine ("AirMove");
				StartCoroutine ("CoyoteTime");
			}
			ShootInAir ();
			if (coyoteTimeActive) {
				JumpFromGround ();
			} else {
				JumpFromAir ();
			}
		}
	}

	//Moving with arrows___________________________________________________________________

	void GroundMove ()
	//How to move when grounded
	{
		moveInput = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector2 (moveInput * movement.groundSpeed, rb.velocity.y);
		if (facingRight && moveInput < 0) {
			//Facing rihght & going left
			Flip ();
		}
		if (!facingRight && moveInput > 0) {
			//Faing left & going right
			Flip ();
		}
	}

	IEnumerator AirMove ()
	//How to move when not grounded
	{
		while (!isGrounded) {
			moveInput = Input.GetAxis ("Horizontal");
			Vector2 deviation = new Vector2 (moveInput * movement.airSpeed, 0f);
			rb.velocity += deviation;

			yield return null;
			rb.velocity -= deviation;

		}
	}

	void Flip () 
	//Makes the character change the direction it looks
	{
		facingRight = !facingRight;
		Vector3 Scaler = transform.localScale;
		Scaler.x *= -1;
		transform.localScale = Scaler;
	}

	bool Grounded ()
	//returns whether the player is on the ground or not
	{
		return (Physics2D.OverlapCircle (movement.groundCheck.position, movement.checkRadius, movement.whatIsGround));
	}

	//Jumps_______________________________________________________________________________

	void JumpFromGround () {
		if (currentJumpCooldown > 0) {
			currentJumpCooldown -= Time.deltaTime;
			return;
		}
		jumpInput = Input.GetAxisRaw ("Jump");
		moveInputRaw = Input.GetAxisRaw ("Horizontal");
		if (jumpInput > 0) {
			float xVelocity = movement.groundSpeed * moveInputRaw * movement.jumpBoost;
			float yVelocity = movement.jumpForce;
			rb.velocity = new Vector2 (xVelocity, yVelocity);
			currentJumpCooldown = movement.jumpCooldown;
		}
	}

	IEnumerator CoyoteTime () {
		coyoteTimeActive = true;
		yield return new WaitForSeconds (movement.coyoteTime);
		coyoteTimeActive = false;
	}

	void JumpFromAir () {
		//Checks for FastFall, then for jumping
		//Can FastFall even with no jumps left
		if (Input.GetAxisRaw ("Vertical") < 0) {
			jumpInput = Input.GetAxisRaw ("Jump");
			if (jumpInput > 0) {
				FastFall ();
			}

			//Recovers from cooldown anyway
			if (currentJumpCooldown > 0) {
				currentJumpCooldown -= Time.deltaTime;
			}
		}
		else {
			//Normal double jump
			if ((airJumpAvailable <= 0) || (currentJumpCooldown > 0)) {
				currentJumpCooldown -= Time.deltaTime;
				return;
			}
			jumpInput = Input.GetAxisRaw ("Jump");
			moveInputRaw = Input.GetAxisRaw ("Horizontal");
			if (jumpInput > 0) {
				float xVelocity = movement.groundSpeed * moveInputRaw * movement.jumpBoost;
				float yVelocity = movement.airJumpForce;
				rb.velocity = new Vector2 (xVelocity, yVelocity);
				airJumpAvailable--;
				CreateBullet (-1 * rb.velocity);
				currentJumpCooldown = movement.jumpCooldown / 2f;
			}
		}
	}

	void FastFall () {
		if (currentJumpCooldown > 0) {
			return;
		}
		rb.velocity = new Vector2 (0, -2 * movement.airJumpForce);
		CreateBullet (-1 * rb.velocity);
		currentJumpCooldown = movement.jumpCooldown/2f;
	}

	void RestoreAirJump () {
		airJumpAvailable = movement.totalAirJumps;
	}

	//Shooting____________________________________________________________________________

	void ShootOnGround () {
		if (currentShootCooldown > 0) {
			currentShootCooldown -= Time.deltaTime;
			return;
		}

		if (Input.GetAxisRaw ("Fire1") == 1) {
			//First evaluates which direction to shoot, then shoots
			float forward = 0;
			float up = -1;

			if (facingRight) {
				forward = 1;
			} else {forward = -1;}
				
			if (Input.GetAxisRaw ("Vertical") == 1) {
				up = 1;
			} else {up = 0;}

			Vector2 direction = new Vector2 (forward, up);

			CreateBullet (direction);
			StartCoroutine ("RecoilInertia", Vector2.zero );

			currentShootCooldown = bullet.shootCooldown;
		}
	}

	void ShootInAir () {
		if (currentShootCooldown > 0) {
			currentShootCooldown -= Time.deltaTime;
			return;
		}

		if (Input.GetAxisRaw ("Fire1") == 1) {
			//First evaluates which direction to shoot, then shoots
			float forward = 0;
			float up = -1;

			if (facingRight) {
				forward = 1;
			} else {forward = -1;}

			up = Input.GetAxisRaw ("Vertical");

			Vector2 direction = new Vector2 (forward, up);

			CreateBullet (direction);
			StartCoroutine ("RecoilInertia", -1 * direction );

			currentShootCooldown = bullet.shootCooldown;
		}
	}

	void CreateBullet (Vector2 direction) {
		direction.Normalize ();
		GameObject shot = GameObject.Instantiate (bullet.prefab, this.transform.position, Quaternion.Euler(0, 0, 0));
		BulletMovement bm = shot.GetComponent<BulletMovement> ();
		bm.SetDirection (direction);
	}

	IEnumerator RecoilInertia (Vector2 direction) {
		this.enabled = false;
		direction.Normalize ();
		rb.velocity = bullet.recoilForce * direction;
		float timeStopped = 0;
		while (timeStopped < bullet.recoilTime) {
			wasGrounded = isGrounded;
			isGrounded = Grounded ();
			if (isGrounded) {
				if (!wasGrounded) {
					//Upon landing
					StopCoroutine ("AirMove");
					RestoreAirJump ();
					rb.velocity = Vector2.zero;
				}
			}
			timeStopped += Time.deltaTime;
			yield return null;
		}
		this.enabled = true;

	}


}
