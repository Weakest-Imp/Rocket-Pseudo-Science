using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarentulaDown : MonoBehaviour {

	[SerializeField] int maxHealth = 1;
	private int health;

	[SerializeField] GameObject player;
	[SerializeField] float detectDistanceX = 20;
	[SerializeField] float detectDistanceY = 9;
	[SerializeField] int damageDealt = 1;

	[SerializeField] float speed = 5;
	[SerializeField] Vector2[] moveSpots;
	[SerializeField] float waitTime = 0.5f;
	bool wait;
	int targetSpot = 0;
	float reachedDistance = 0.05f;

	bool targetDetected = false;

	void Start () {
		health = maxHealth;

		targetDetected = false;
		wait = false;
		targetSpot = 0;
		reachedDistance = 0.05f;


		SetTargetSpots ();
		StartCoroutine ("DetectTarget");
	}

	void Update () {
		if (!wait) {
			transform.position = Vector2.MoveTowards (transform.position, 
				moveSpots [targetSpot], speed * Time.deltaTime);
			if (Vector2.Distance (transform.position, moveSpots [targetSpot]) < reachedDistance) {
				StartCoroutine ("SpotReached");
			}
		}
	}

	IEnumerator DetectTarget () {
		while (true) {
			float distanceX = Mathf.Abs(player.transform.position.x - this.transform.position.x);
			float distanceY = Mathf.Abs(player.transform.position.y - this.transform.position.y);

			targetDetected = (distanceX < detectDistanceX) && (distanceY < detectDistanceY);
			yield return new WaitForSeconds (0.1f);
		}
	}


	//Movement________________________________________________________________________________
	IEnumerator SpotReached () 
	{
		wait = true;
		yield return new WaitForSeconds (waitTime);
		NextSpot ();
		wait = false;
	}
	void NextSpot () {
		if (targetSpot == moveSpots.Length - 1) {
			targetSpot = 0;
		} else {
			targetSpot++;}
	}

	void SetTargetSpots () {
		Vector2 initialPosition = new Vector2(transform.position.x, transform.position.y);
		for (int i = 0; i < moveSpots.Length; i++) {
			moveSpots [i] += initialPosition;
		}
	}


	//Interactions________________________________________________________________________
	void OnCollisionEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Player") {
			PlayerController playerCon = (PlayerController) other.gameObject.GetComponent<PlayerController> ();
			playerCon.TakeDamage(damageDealt);
			playerCon.KnockBack (this.gameObject);
			playerCon.StartCoroutine("Invincibility");
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Bullet") {
			BulletMovement bullet = (BulletMovement) other.gameObject.GetComponent<BulletMovement> ();
			int damage = bullet.damage;
			bullet.Explode ();
			TakeDamage (damage);
		}
	}

	void TakeDamage (int damage) {
		health -= damage;
		if (health <= 0) {
			this.gameObject.SetActive (false);
		}
	}
}
