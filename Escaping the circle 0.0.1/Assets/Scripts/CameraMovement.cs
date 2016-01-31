using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Tobii.EyeX.Framework;

public class CameraMovement : MonoBehaviour
{
    public float _timeNeededToFindFlaw;
    public float _sensitivityOfViewPort;
    public float _SpeedOfRotation;
    public int _nextScene;

    private Transform _tr;
    private Camera _camera;
    private Animator _handAnim;
    private Transform _handTrans;

    private List<GameObject> _flawGO = new List<GameObject>();
    private Vector3[] _prevMousePos = new Vector3[10];
    private int _index = 0;
    private Vector3[] _prevMousePosViewPort = new Vector3[10];
    private float mouseTimer = 0.0f;
    private const float mouseTimerLimit = 0.5f;
    private bool mouseActive;
    private Vector3 inputPosition;
    private GazePointDataComponent gaze;
    private EyeXHost _eyexHost;
	private ClueManager clueManager;
    private Ray _mouseToWorldRay;
    private RaycastHit _mouseRayHit;
    private RaycastHit _prevMouseRayHit;
    private int _flawId = -1;
    private Vector2 _averagePos;
    private Vector2 _prevAveragePos;
    private float _timeAnim;
    private bool _isMoving;
    private float _timeAnimStart;
    private GameObject _tagetFlaw;


   
    // Use this for initialization
    void Start()
    {
        _tr = GetComponent<Transform>();
        _camera = Camera.main;
        _flawGO.AddRange(GameObject.FindGameObjectsWithTag("Flaw"));
        gaze = GetComponent<GazePointDataComponent>();
        _eyexHost = EyeXHost.GetInstance();
        _handAnim = gameObject.GetComponentInChildren<Animator>();
        gameObject.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.clear, 0.3f);
        _handTrans = _tr.GetChild(0);
		clueManager = new ClueManager ();
    }

    // Update is called once per frame
    void Update()
    {
        //----------Loading array for both smoothening purpus, and to check if mouse/eyes are forcused on one spot
        if (_index >= _prevMousePos.Length - 1)
        { _index = 0; }
        else { _index++; }
        isMouseActive();
        _prevMousePos[_index] = _camera.ScreenToWorldPoint(getInputPosition());
        _prevMousePosViewPort[_index] = _camera.ScreenToViewportPoint(getInputPosition());
        _mouseToWorldRay = _camera.ScreenPointToRay(getInputPosition());
        Physics.Raycast(_mouseToWorldRay, out _mouseRayHit, 100f);
        Debug.DrawRay(_mouseToWorldRay.origin, _mouseToWorldRay.direction, Color.red);
        //Debug.Log(getInputPosition());

        //----------Standard Update Voids
        ViewPortMovement();

        FindFlaw();

        HandAnimation();

		FindClue ();

        //----------Load next level when no Flaws left
        if (_flawGO.Count == 0)
		{ 
			Cursor.visible = true;
			SceneManager.LoadSceneAsync(_nextScene);
		}

    }

    #region Animation
    void HandAnimation()
    {
        _averagePos = Vector2.zero;
        if (_isMoving)
        { _averagePos = new Vector2(9999, 9999); _prevAveragePos = new Vector2(9999, 9999); }
        else
        {
            for (int i = 0; i < _prevMousePosViewPort.Length; i++)
            { _averagePos += new Vector2(_prevMousePosViewPort[i].x, _prevMousePosViewPort[i].y); }
            _averagePos /= (_prevMousePosViewPort.Length);
        }
        if (_timeAnim < -1.2f)
        { _prevAveragePos = new Vector2(9999, 9999); }
        _timeAnim -= Time.deltaTime;

        if (_handAnim.GetBool("ReachOut") && _handAnim.GetCurrentAnimatorStateInfo(0).IsName("HandReachoutAnim"))
        { _handAnim.SetBool("ReachOut", false); }

        if (_timeAnim < -0.85f)
        {
            if (Vector2.Distance(_averagePos, new Vector2(_prevMousePosViewPort[_index].x, _prevMousePosViewPort[_index].y)) < 0.2f &&
                !(Vector2.Distance(_averagePos, _prevAveragePos) < 0.2f))
            { _timeAnimStart += Time.deltaTime; }
            else
            { _timeAnimStart -= Time.deltaTime; }
        }
        if (_timeAnimStart < 0)
        { _timeAnimStart = 0; }

        if (_timeAnimStart > 0.5f && !(_handAnim.GetCurrentAnimatorStateInfo(0).IsName("HandReachoutAnim")))
        {
            _prevMouseRayHit = _mouseRayHit;
            _handAnim.SetBool("ReachOut", true);
            _prevAveragePos = _averagePos;
            _timeAnim = 1f;
            _timeAnimStart = 0;
        }

        float _ang;
        if (_handAnim.GetCurrentAnimatorStateInfo(0).IsName("HandReachoutAnim"))
        {
            _ang = Angle
            (
                _prevAveragePos.x - 0.5f,
                Magnitude(_prevAveragePos.x - 0.5f, _prevAveragePos.y),
                _prevAveragePos.y
            );
            if (float.IsNaN(_ang)) { _ang = 90f; }
            _ang += (_prevAveragePos.x - 0.5f) * 0.8f;
        }
        else
        {
            _ang = -90 * Mathf.Deg2Rad;
        }

        _handTrans.localEulerAngles = new Vector3(31, 0, -_ang * Mathf.Rad2Deg - 90);
    }
    #endregion

    #region InputHandling
    public void isMouseActive()
    {
		if (_eyexHost == null || !_eyexHost.enabled) {
			mouseActive = true;
			return;
		}
			
		var eyeTrackerDeviceStatus = _eyexHost.EyeTrackingDeviceStatus;
		if (eyeTrackerDeviceStatus == EyeXDeviceStatus.Tracking) {
			if (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0) {
				mouseTimer = 0;
			}
			mouseTimer += Time.deltaTime;
			if (mouseTimer >= mouseTimerLimit) {
				Cursor.visible = false;
				mouseActive = false;
			} else {
				Cursor.visible = true;
				mouseActive = true;
			}
		} else {
			Cursor.visible = true;
			mouseActive = true;
			mouseTimer = 0;
			Debug.Log ("NotTraking");
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

    #region FindFlaw
    void FindFlaw()
    {
        if (_handAnim.GetCurrentAnimatorStateInfo(0).IsName("HandReachoutAnim"))
        {
            if (_prevMouseRayHit.collider != null)
            {
                if (_prevMouseRayHit.collider.tag == "Flaw")
                {
                    _flawId = _prevMouseRayHit.collider.GetInstanceID();
                    _tagetFlaw = _prevMouseRayHit.collider.gameObject;
                }
            }
        }
        else if (_flawId > 0)
        {
            _flawGO.Remove(_tagetFlaw);
            Destroy(_tagetFlaw);
            _flawId = -1;
        }
    }
    #endregion

	#region FindClue
	void FindClue()
	{
		if (GameObject.FindGameObjectWithTag("Clue") != null){
			if (_handAnim.GetCurrentAnimatorStateInfo(0).IsName("HandReachoutAnim"))
			{
				if (_prevMouseRayHit.collider != null) {
					var obj = _prevMouseRayHit.collider.gameObject;
					if (clueManager.isClue (obj)) {
						clueManager.addPlayerClue (obj);
						obj.GetComponentInChildren<Light> (true).enabled = true;
						if (clueManager.isClueOrderCorrect ()) {
							Debug.Log ("Victory");
							var lights = clueManager.getClueContainer ().GetComponentsInChildren<Light> ();
							foreach (var l in lights) {
								l.color = Color.green;
							}
							Sleepy ();
							foreach (var l in lights) {
								l.enabled = false;
							}
							// GET TO THE CHOPPA! (fault lights up)
						} else {
							Debug.Log ("Fail");
							var lights = clueManager.getClueContainer ().GetComponentsInChildren<Light> ();
							foreach (var l in lights) {
								l.color = Color.red;
							}
							Sleepy ();
							foreach (var l in lights) {
								l.enabled = false;
							}
						}
					}
				}
			}
		}
	}

	IEnumerator Sleepy()
	{
		yield return new WaitForSeconds(3f);
	}

	#endregion

    #region ViewPort Movement
    void ViewPortMovement()
    {
        if (_handAnim.GetCurrentAnimatorStateInfo(0).IsName("HandReachoutAnim"))
        { return; }
        _isMoving = false;
        if (_prevMousePosViewPort[_index].x > 1f - _sensitivityOfViewPort) //RightSide
        {
            _tr.Rotate(Vector3.up * Time.deltaTime * _SpeedOfRotation * ((_prevMousePosViewPort[_index].x - (1f - _sensitivityOfViewPort)) / (_sensitivityOfViewPort)), Space.World);
            //Debug.Log("GoRight");
            _isMoving = true;
        }
        if (_prevMousePosViewPort[_index].x < _sensitivityOfViewPort) //LeftSide
        {
            _tr.Rotate(Vector3.up * Time.deltaTime * _SpeedOfRotation * -1 * ((_sensitivityOfViewPort - _prevMousePosViewPort[_index].x) / _sensitivityOfViewPort), Space.World);
            //Debug.Log("GoLeft");
            _isMoving = true;
        }
        if (_prevMousePosViewPort[_index].y > 1f - _sensitivityOfViewPort) //Up
        {
            //Debug.Log("GoUp");
            _isMoving = true;
        }
        if (_prevMousePosViewPort[_index].y < _sensitivityOfViewPort) //Down
        {
            //Debug.Log("GoDown");
            _isMoving = true;
        }
    }
    #endregion

    #region MathFunctions
    public float Angle(float A, float B, float C)
    {
        return (
                Mathf.Acos
                (
                    (
                        Mathf.Pow(A, 2) +
                        Mathf.Pow(B, 2) -
                        Mathf.Pow(C, 2)
                    )
                    /
                    (2 * A * B)
                )
                * -Mathf.Sign(C));
    }

    public float Magnitude(float A, float B)
    {
        return Mathf.Sqrt
             (
             Mathf.Pow(A, 2)
             +
             Mathf.Pow(B, 2)
             );
    }
    #endregion
}
