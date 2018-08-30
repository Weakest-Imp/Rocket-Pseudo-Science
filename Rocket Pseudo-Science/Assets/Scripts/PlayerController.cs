using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float groundSpeed = 13;
	[SerializeField] private float jumpForce = 30;
	[SerializeField] private float jumpBoost = 1.5f;
	[SerializeField] private float airSpeed = 7;
	[SerializeField] private float airJumpForce = 20;
	//Gravity is 10 G in Rigidbody2D

	private float moveInput;
	private float moveInputRaw;
	private float jumpInput;
	[SerializeField] private int totalAirJumps;
	private int airJumpAvailable;
	[SerializeField] private float jumpCooldown;
	private float currentJumpCooldown;

	private bool facingRight = true;
	private bool isGrounded;
	private bool wasGrounded;
	[SerializeField] private Transform groundCheck1;
	[SerializeField] private Transform groundCheck2;
	[SerializeField] private float checkRadius = 0.144f;
	[SerializeField] private LayerMask whatIsGround;

	private Rigidbody2D rb;

	[Header("Bullet")]
	[SerializeField] GameObject bullet;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		airJumpAvailable = totalAirJumps;
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


	void GroundMove ()
	//How to move when grounded
	{
		moveInput = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector2 (moveInput * groundSpeed, rb.velocity.y);
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
			Vector2 deviation = new Vector2 (moveInput * airSpeed, 0f);
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
		bool ground1 = Physics2D.OverlapCircle (groundCheck1.position, checkRadius, whatIsGround);
		bool ground2 = Physics2D.OverlapCircle (groundCheck2.position, checkRadius, whatIsGround);
		return (ground1 || ground2);
	}

	void JumpFromGround () {
		if (currentJumpCooldown > 0) {
			currentJumpCooldown -= Time.deltaTime;
			return;
		}
		jumpInput = Input.GetAxisRaw ("Jump");
		moveInputRaw = Input.GetAxisRaw ("Horizontal");
		if (jumpInput > 0) {
			float xVelocity = groundSpeed * moveInputRaw * jumpBoost;
			float yVelocity = jumpForce;
			rb.velocity = new Vector2 (xVelocity, yVelocity);
			currentJumpCooldown = jumpCooldown;
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
			float xVelocity = groundSpeed * moveInputRaw * jumpBoost;
			float yVelocity = airJumpForce;
			rb.velocity = new Vector2 (xVelocity, yVelocity);
			airJumpAvailable--;
			currentJumpCooldown = jumpCooldown/2f;
		}
	}

	void FastFall () {
		rb.velocity = new Vector2 (0, -2 * airJumpForce);
	}

	void RestoreAirJump () {
		airJumpAvailable = totalAirJumps;
	}

	void ShootOnGround () {
		if (Input.GetAxisRaw ("Fire1") == 1) {
			GameObject.Instantiate (bullet);
		}
	}
}
