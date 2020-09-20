using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;

	public float xOffset;
	public float smoothing;

	void LateUpdate() {
		float targetX = target.position.x + xOffset;
		float delta = (targetX - transform.position.x) * smoothing;
		transform.Translate(delta, 0, 0);
	}
}
