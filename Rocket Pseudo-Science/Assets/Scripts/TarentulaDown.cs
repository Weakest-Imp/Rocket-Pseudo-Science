using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarentulaDown : MonoBehaviour {

	[SerializeField] GameObject player;
	[SerializeField] float detectDistanceX;
	[SerializeField] float detectDistanceY;

	[SerializeField] float speed;
	[SerializeField] Vector2[] moveSpots;
	[SerializeField] float waitTime;
	bool wait;
	int targetSpot;
	float reachedDistance;

	bool targetDetected = false;

	void Start () {
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

	void OnCollision2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
		
		}
		if (other.gameObject.tag == "Bullet") {

		}
	}
}
