﻿using UnityEngine;
using System.Collections;

public class DropThroughOnDown : MonoBehaviour {

    private bool playerOnObject = true; // Platform should initially be 'solid'

	void Update () {
		if (playerOnObject)
            GetComponent<Collider2D>().isTrigger = Input.GetAxis("Vertical") < 0;
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.tag == GameConstants.PlayerTag)
			playerOnObject = true;
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.tag == GameConstants.PlayerTag)
			playerOnObject = false;
	}
}
