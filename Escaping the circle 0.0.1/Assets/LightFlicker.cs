using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

    private Light _lt;
    public float _flickerStrength;
    private float _baseIntencisty;

	// Use this for initialization
	void Start () {
        _lt = GetComponent<Light>();
        _baseIntencisty = _lt.intensity;

	}
	
	// Update is called once per frame
	void Update () {
        _lt.intensity = Random.Range(_baseIntencisty -_flickerStrength, _baseIntencisty + _flickerStrength);
	}
}
