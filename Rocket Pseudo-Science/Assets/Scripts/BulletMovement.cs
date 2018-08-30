using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour {

	[SerializeField] float bulletSpeed;
	[SerializeField] Vector2 direction;

	[SerializeField] float damage;

	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		direction.Normalize ();
		rb.velocity = direction * bulletSpeed;
	}

	void Update () {
		
	}

	void Explode () {
		//DealDamage();
		//this.Destroy();
	}

}
