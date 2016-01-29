using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    private Vector3[] _prevMousePos = new Vector3[12];
    private int _index = 0;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        _prevMousePos[_index] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(_index >= _prevMousePos.Length)
        { _index = 0; }else { _index++; }


	}
}
