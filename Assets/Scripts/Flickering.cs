﻿using UnityEngine;

[RequireComponent(typeof(Light))]
public class Flickering : MonoBehaviour {
	[Tooltip("Minimum factor multiplied with original light intensity")]
	[Range(0, 2)]
	public float minIntensityFactor = 0.0f;
	[Tooltip("Maximum factor multiplied with original light intensity")]
	[Range(0, 2)]
	public float maxIntensityFactor = 1.4f;

	[Tooltip("Pick a random intensity between min/max or switch directly between them")]
	public bool randomIntensity = false;

	[Tooltip("Interpolate between intensities or hard switches")]
	public bool lerp = true;
	[Tooltip("Maximum seconds it takes to interpolate between intensities")]
	[Range(0, 1)]
	public float maxLerpTime = 0.04f;

	[Tooltip("Curve used for intensity interpolation")]
	public AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0,0,1,1);

	[Tooltip("Minimum delay between flickers")]
	[Range(0, 1)]
	public float minDelay = 0.02f;
	[Tooltip("Maximum delay between flickers")]
	[Range(0, 1)]
	public float maxDelay = 0.08f;

	[Tooltip("Time the light should stay 'on'")]
	[Range(0, 120)]
	public float stayOnTime = 2f;
	[Tooltip("Static 'on' intensity (0 to keep it off most of the time)")]
	[Range(0, 2)]
	public float stayOnIntensity = 1f;
	[Tooltip("Probability that the light stays 'on' after a flicker")]
	[Range(0, 1)]
	public float stayOnProbability = 0.01f;

	private float lerpTime = 0f;
	private float lerpTimeLeft = 0f;
	private float delayLeft = 0f;
	private Light light;
	private float intensity;
	private float srcIntensity;
	private float targetIntensity;
	private bool turningOff = true;

	void Start() {
		light = GetComponent<Light>();
		intensity = light.intensity;
	}

	void Update() {
		delayLeft -= Time.deltaTime;
		lerpTimeLeft -= Time.deltaTime;

		if(delayLeft <= 0f) {
			srcIntensity = light.intensity;
			if(randomIntensity) {
				targetIntensity = intensity * Random.Range(minIntensityFactor, maxIntensityFactor);
			} else {
				targetIntensity = intensity *(turningOff ? minIntensityFactor : maxIntensityFactor);
				turningOff = !turningOff;
			}

			delayLeft = Random.Range(minDelay, maxDelay);

			if(Random.value < stayOnProbability) {
				delayLeft = stayOnTime;
				targetIntensity = intensity * stayOnIntensity;
			}

			lerpTimeLeft = lerpTime = Mathf.Min(delayLeft, maxLerpTime);
		}

		light.intensity = lerp ? Mathf.Lerp(targetIntensity, srcIntensity, lerpCurve.Evaluate(lerpTime<=0.000001f ? 0 : lerpTimeLeft/lerpTime))
			: targetIntensity;
	}
}