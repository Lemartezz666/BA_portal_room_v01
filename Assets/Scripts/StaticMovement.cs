﻿using UnityEngine;

public class StaticMovement : MonoBehaviour {
	public Vector3 startOffset = new Vector3(0,0,0);
	public Vector3 endOffset = new Vector3(0,0,0);

	public float time = 1f;

	public bool loop = false;

	public AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);


	private Vector3 lastOffset = new Vector3(0,0,0);

	private float alpha = 0f;
	private bool toEnd = true;

	void Update() {
		alpha += Time.deltaTime / time * (toEnd ? 1 : -1);

		if(loop) {
			if(toEnd && alpha>1f) {
				alpha = 1f;
				toEnd = false;
			} else if(!toEnd && alpha<0f) {
				alpha = 0f;
				toEnd = true;
			}
		} else {
			alpha = Mathf.Clamp(alpha, 0f, 1f);
		}

		var offset = Vector3.Lerp(startOffset, endOffset, curve.Evaluate(alpha));
		transform.position += offset - lastOffset;
		lastOffset = offset;
	}
}