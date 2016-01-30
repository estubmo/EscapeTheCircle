using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Calibration : MonoBehaviour
{
    public GameObject _gazePoint;
	private EyeXHost _eyeXHost;
	private IEyeXDataProvider<EyeXGazePoint> _gazePointProvider;
    private Transform _tr;
	public BlueDot blueDotPrefab;
    //private List<GameObject> _gazePointList = new List<GameObject>();
	private List<EyeXGazePoint> gazePointList = new List<EyeXGazePoint>();
    private float _timer = 3f;
    private int _index;
	// Use this for initialization
	void Start ()
    {
		_eyeXHost = EyeXHost.GetInstance ();
		_gazePointProvider = _eyeXHost.GetGazePointDataProvider (Tobii.EyeX.Framework.GazePointDataMode.LightlyFiltered);
		onEnable ();
	}

	// Update is called once per frame
	void Update ()
    {
        //if (_timer > 0) return; //Initiali startUp time
        //GameObject go = new GameObject("GazePoint " + );
		//Debug.Log(Time.deltaTime);




	}
	public void addGazePointToList(){
		if (_gazePointProvider.Last.IsValid) {
			var gazePoint = _gazePointProvider.Last;
			GameObject go = (GameObject)Instantiate (Resources.Load ("BlueDot"));
			go.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (gazePoint.Screen.x, gazePoint.Screen.y, 0.0f));
			go.transform.position = new Vector3 (go.transform.position.x, go.transform.position.y, -1.0f);
			Debug.Log (go.transform.position);
			Debug.Log (string.Format ("X: {0} Y:{1}", gazePoint.Display.x, gazePoint.Display.y));
		}
		//gazePointList.Add (gazePoint);

	}
	public void onEnable(){
		_gazePointProvider.Start ();
		InvokeRepeating ("addGazePointToList", 0.0f, 0.1f);
	}

	public void onDisable(){
		CancelInvoke ("addGazePointToList");
		_gazePointProvider.Stop ();
	}

}
