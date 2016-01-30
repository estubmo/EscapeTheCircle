using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour
{
    public GameObject _gazePoint;
	private EyeXHost _eyeXHost;
	private IEyeXDataProvider<EyeXGazePoint> _gazePointProvider;
    private Transform _tr;
	public BlueDot blueDotPrefab;
    private List<GameObject> _gazePointList = new List<GameObject>();
	//private List<EyeXGazePoint> gazePointList = new List<EyeXGazePoint>();
    private float _timer = 3f;
    private int _index;
    private float _distance;

    private int _stage = 0;
    private float _multi = 10f;
    private float _add = 5f;
    private CalibrationStats _calStats;
	// Use this for initialization
	void Start ()
    {
		_eyeXHost = EyeXHost.GetInstance ();
		_gazePointProvider = _eyeXHost.GetGazePointDataProvider (Tobii.EyeX.Framework.GazePointDataMode.LightlyFiltered);
		onEnable ();
        _tr = transform;
        _calStats = GameObject.Find("CalibrationStats").GetComponent<CalibrationStats>();
	}

	// Update is called once per frame
	void Update ()
    {
        switch(_stage)
        {
            case 0: _tr.parent.position = new Vector3(0, 0, 0); break;
            case 1: _tr.parent.position = new Vector3(-6, 3, 0); break;
            case 2: _tr.parent.position = new Vector3(0, -3, 0); break;
            case 3: _tr.parent.position = new Vector3(6, 3, 0); break;
            case 4: SceneManager.LoadScene(0); break;
        }

        if(_gazePointList.Count > 30)
        {
            for (int i = 10; i <= 20;i++)
            {
                if(Vector3.Distance(_gazePointList[i].transform.position,_tr.position) > _distance)
                { _distance = Vector3.Distance(_gazePointList[i].transform.position, _tr.position); }
            }
            for(int i = 0; i < _gazePointList.Count;i++)
            { Destroy(_gazePointList[i]);}
            _gazePointList.Clear();
            _tr.localScale = new Vector3(_distance * _multi - _add, _distance * _multi - _add, _distance * _multi - _add);
            Debug.Log(_distance);
            _calStats._calibrationSize = _distance;
            _stage++;
            //SceneManager.LoadScene(0);
        }
	}
	public void addGazePointToList(){
        if (_gazePointProvider.Last.IsValid)
        {
            var gazePoint = _gazePointProvider.Last;
            GameObject go = (GameObject)Instantiate(Resources.Load("BlueDot"));
            go.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(gazePoint.Screen.x, gazePoint.Screen.y, 0.0f));
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -1.0f);
            //Debug.Log(go.transform.position);
            //Debug.Log(string.Format("X: {0} Y:{1}", gazePoint.Display.x, gazePoint.Display.y));
            _gazePointList.Add(go);
        }
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
