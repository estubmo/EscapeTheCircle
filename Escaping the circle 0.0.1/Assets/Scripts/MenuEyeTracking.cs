using UnityEngine;
using System.Collections;
using Tobii.EyeX.Framework;

public class MenuEyeTracking : MonoBehaviour {
	private GazePointDataComponent gaze;
	private EyeXHost _eyexHost;

	private float mouseTimer = 0;
	private bool mouseActive = false;
	private const float mouseTimerLimit = 0.5f;
	// Use this for initialization
	void Start () {
		gaze = GetComponent<GazePointDataComponent>();
		_eyexHost = EyeXHost.GetInstance();
	}
	
	// Update is called once per frame
	void Update () {
		isMouseActive ();
		//Debug.Log ("Mouse active: " + mouseActive + " on position: " + getInputPosition ());
	}

	#region InputHandling
	public void isMouseActive()
	{
		var eyeTrackerDeviceStatus = _eyexHost.EyeTrackingDeviceStatus;
		if (eyeTrackerDeviceStatus == EyeXDeviceStatus.Tracking)
		{
			if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
			{
				mouseTimer = 0;
			}
			mouseTimer += Time.deltaTime;
			if (mouseTimer >= mouseTimerLimit)
			{
				Cursor.visible = false;
				mouseActive = false;
			}
			else {
				Cursor.visible = true;
				mouseActive = true;
			}
		}
		else {
			Cursor.visible = true;
			mouseActive = true;
			mouseTimer = 0;
		}

		//Debug.Log ("mouseTimer: " + mouseTimer + " mouseActive: " + mouseActive); 
	}

	public Vector3 getInputPosition()
	{
		if (mouseActive)
		{
			return Input.mousePosition;
		}
		else {
			return new Vector3(gaze.LastGazePoint.Screen.x, gaze.LastGazePoint.Screen.y, 0);
		}
	}

	#endregion

}
