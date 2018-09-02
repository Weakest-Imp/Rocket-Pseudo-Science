using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour {

	[SerializeField] float bulletSpeed;
	[SerializeField] Vector2 direction;
	[SerializeField] float timeToDie;
	float timeAlive;

	[SerializeField] float damage;

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

	void Explode () {
		//DealDamage();
		Destroy(gameObject);
	}

	public void SetDirection(Vector2 newDirection) {
		direction = newDirection;
		//Needs to rotate bullet
	}

}
