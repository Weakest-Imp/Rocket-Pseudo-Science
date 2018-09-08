using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager Instance { get; private set; }

	[SerializeField] Canvas gameOverCanvas;

	void Awake () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}
	
	public IEnumerator GameOver () {
		gameOverCanvas.enabled = true;
		yield return new WaitForSeconds (3);
		SceneManager.LoadScene ("Training Room");
	}
}
