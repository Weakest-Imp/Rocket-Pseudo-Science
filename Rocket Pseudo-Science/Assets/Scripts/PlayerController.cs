using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float groundSpeed = 10;
	[SerializeField] private float jumpForce = 25;
	[SerializeField] private float jumpBoost = 1.5f;
	//Gravity is 10 G

	private float moveInput;
	private float moveInputRaw;
	private float jumpInput;
	private bool facingRight = true;
	private bool isGrounded;
	[SerializeField] private Transform groundCheck1;
	[SerializeField] private Transform groundCheck2;
	[SerializeField] private float checkRadius = 0.144f;
	[SerializeField] private LayerMask whatIsGround;

	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		isGrounded = Grounded ();
		if (isGrounded) {
			GroundMove ();
			JumpFromGround ();
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
		jumpInput = Input.GetAxisRaw ("Jump");
		moveInputRaw = Input.GetAxisRaw ("Horizontal");
		if (jumpInput > 0) {
			float xVelocity = groundSpeed * moveInputRaw * jumpBoost;
			float yVelocity = jumpForce;
			rb.velocity = new Vector2 (xVelocity, yVelocity);
		}
	}
}
