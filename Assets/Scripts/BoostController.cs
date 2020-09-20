using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostController : MonoBehaviour {

	public float boostSpeed = 3;

	void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Player") {
			other.gameObject.GetComponent<PlayerController>().boost(boostSpeed);
			Destroy(gameObject);
		}
	}
}