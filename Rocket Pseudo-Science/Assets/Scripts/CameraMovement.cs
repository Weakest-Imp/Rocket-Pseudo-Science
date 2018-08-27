using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	[SerializeField] GameObject player;

	void Update () {
		float x = player.transform.position.x;
		float y = player.transform.position.y;

		float z = transform.position.z;
		transform.position = new Vector3 (x, y, z);
	}
}
