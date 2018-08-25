using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField] float groundSpeed;
	[SerializeField] float jumpForce;

	float moveInput;

	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		GroundMove ();
	}


	//How to move when grounded
	void GroundMove ()
	{
		moveInput = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector2 (moveInput * groundSpeed, rb.velocity.y);
	}
}
