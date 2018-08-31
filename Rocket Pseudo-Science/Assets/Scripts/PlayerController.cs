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

		public Transform groundCheck1;
		public Transform groundCheck2;
		public float checkRadius = 0.144f;
		public LayerMask whatIsGround;
	}
	[SerializeField] private PlayerMovement movement;	

	private float moveInput;
	private float moveInputRaw;
	private float jumpInput;
	private int airJumpAvailable;
	private float currentJumpCooldown;

	private bool facingRight = true;
	private bool isGrounded;
	private bool wasGrounded;

	[SerializeField] GameObject bullet;
	[SerializeField] float shootCooldown;
	float currentShootCooldown;


	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		airJumpAvailable = movement.totalAirJumps;
		currentJumpCooldown = 0;
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
			}
			JumpFromAir ();
			if (Input.GetAxisRaw ("Vertical") == -1) {
				FastFall ();
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
		bool ground1 = Physics2D.OverlapCircle (movement.groundCheck1.position, movement.checkRadius, movement.whatIsGround);
		bool ground2 = Physics2D.OverlapCircle (movement.groundCheck2.position, movement.checkRadius, movement.whatIsGround);
		return (ground1 || ground2);
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

	void JumpFromAir () {
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
			currentJumpCooldown = movement.jumpCooldown/2f;
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
			//Shoot Inertia

			currentShootCooldown = shootCooldown;
		}
	}

	void CreateBullet (Vector2 direction) {
		direction.Normalize ();
		GameObject shot = GameObject.Instantiate (bullet, this.transform.position, Quaternion.Euler(0, 0, 0));
		BulletMovement bm = shot.GetComponent<BulletMovement> ();
		bm.SetDirection (direction);
	}


}
