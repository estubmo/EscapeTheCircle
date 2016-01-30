using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Calibration : MonoBehaviour
{
    public GameObject _gazePoint;

    private Transform _tr;
    private List<GameObject> _gazePointList = new List<GameObject>();
    private float _timer = 3f;
    private int _index;
	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_timer > 0) return; //Initiali startUp time
       //GameObject go = new GameObject("GazePoint " + );

	}
}
