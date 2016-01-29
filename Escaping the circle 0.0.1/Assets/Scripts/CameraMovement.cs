using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    private Vector3[] _prevMousePos = new Vector3[12];
    private int _index = 0;
    private Vector3[] _prevMousePosViewPort = new Vector3[12];
    public float _sensitivityOfViewPort;
    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_index >= _prevMousePos.Length-1)
        { _index = 0; }
        else { _index++; }
        _prevMousePos[_index] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _prevMousePosViewPort[_index] = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (_prevMousePosViewPort[_index].x > 1f- _sensitivityOfViewPort) //RightSide
        {
            Debug.Log("GoRight");
        }
        if (_prevMousePosViewPort[_index].x < _sensitivityOfViewPort) //LeftSide
        {
            Debug.Log("GoLeft");
        }
        if (_prevMousePosViewPort[_index].y > 1f- _sensitivityOfViewPort) //Up
        {
            Debug.Log("GoUp");
        }
        if (_prevMousePosViewPort[_index].y < _sensitivityOfViewPort) //Down
        {
            Debug.Log("GoDown");
        }
    }
}
