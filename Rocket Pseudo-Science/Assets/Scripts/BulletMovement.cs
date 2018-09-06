using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour {

	[SerializeField] float bulletSpeed =25;
	[SerializeField] Vector2 direction;
	[SerializeField] float timeToDie = 3;
	float timeAlive;

	public int damage = 1;

	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		direction.Normalize ();
		rb.velocity = direction * bulletSpeed;
		timeAlive = 0;
	}

	void FixedUpdate () {
		if (timeAlive > timeToDie) {
			Explode ();
		} else {
			timeAlive += Time.deltaTime;
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag != "Player") {
			Explode ();
		}
	}

	public void Explode () {
		//Boom Animation
		Destroy(gameObject);
	}

	public void SetDirection(Vector2 newDirection) {
		direction = newDirection;
		//Needs to rotate bullet
	}

}
