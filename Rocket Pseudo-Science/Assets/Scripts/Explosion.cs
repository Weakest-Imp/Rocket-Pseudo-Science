using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	[SerializeField] float boomTime;
	[SerializeField] float waveLength;
	Vector2 positionDeviation;
	public int damage = 1;


	void Start () {
		StartCoroutine ("Boom");
		positionDeviation = Vector2.zero;
		//StartCoroutine ("Vibration");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator Boom () {
		yield return new WaitForSeconds (boomTime);
		Destroy (this.gameObject);
	}

	IEnumerator Vibration () {
		while (true) {
			UndoDeviation (positionDeviation);
			positionDeviation = randomDeviation ();
			DeviateTransform (positionDeviation);
			yield return  null;
		}

	}

	Vector2 randomDeviation () {
		float deviationX = Random.Range (-1 * waveLength, waveLength);
		float deviationY = Random.Range (-1 * waveLength, waveLength);
		return new Vector2 (deviationX, deviationY);
	}

	void DeviateTransform (Vector2 positionDeviation) {
		float newTransformX = this.transform.position.x + positionDeviation.x;
		float newTransformY = this.transform.position.y + positionDeviation.y;
		this.transform.position = new Vector2 (newTransformX, newTransformY);
	}

	void UndoDeviation (Vector2 positionDeviation) {
		float newTransformX = this.transform.position.x - positionDeviation.x;
		float newTransformY = this.transform.position.y - positionDeviation.y;
		this.transform.position = new Vector2 (newTransformX, newTransformY);
	}

	public void SetDamage (int newDamage) {
		damage = newDamage;
	}
}
