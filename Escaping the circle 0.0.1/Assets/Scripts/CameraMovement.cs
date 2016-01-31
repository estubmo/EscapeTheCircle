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
    public GameObject Circle;
	public bool puzzleSolved = false;

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
    private string _flawId = null;
    private Vector2 _averagePos;
    private Vector2 _prevAveragePos;
    private float _timeAnim;
    private bool _isMoving;
    private float _timeAnimStart;
    private GameObject _tagetFlaw;
    private float _timerClue = -1f;
    private bool _clueLighting;
    private SpriteRenderer _fadeout;
    private SpriteRenderer _fadeout2;

    void Awake()
    {

    }

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
		if (GameObject.FindGameObjectsWithTag ("Clue").Length < 1) {
			puzzleSolved = true;
		}

        GameObject _fade = new GameObject("Fadeout");
        Texture2D _tex2D = new Texture2D(1,1);
        _tex2D.SetPixel(0, 0, Color.white);
        _tex2D.Apply();
        _fadeout = _fade.AddComponent<SpriteRenderer>();
        _fadeout.sprite = Sprite.Create(_tex2D, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        _fadeout.material.color = Color.black;
        _fade.transform.localScale = new Vector3(999,999,999);
        _fade.transform.position = _tr.position + _tr.forward*0.5f;
        _fade.transform.eulerAngles = _camera.transform.eulerAngles;
        _fade.transform.parent = _tr;
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

        FindClue();

        //----------Load next level when no Flaws left
        if (_flawGO.Count == 0)
        {
            _isMoving = true;

            if (Circle != null)
            {
                Circle.GetComponent<FadeOut>()._fadeOut = true;
                if (Circle.GetComponent<FadeOut>()._time >= Circle.GetComponent<FadeOut>()._timeOut)
                {
                    if (_fadeout.material.color.a < 0.99f)
                    {
                        _fadeout.material.color = Color.Lerp(_fadeout.material.color, Color.black, Time.deltaTime * 2.5f);
                    }
                    else
                    {
                        Cursor.visible = true;
                        SceneManager.LoadSceneAsync(_nextScene);
                    }
                }
            }
            else
            {
                if (_fadeout.material.color.a < 0.99f)
                {
                    _fadeout.material.color = Color.Lerp(_fadeout.material.color, Color.black, Time.deltaTime * 2.5f);
                }
                else
                {
                    Cursor.visible = true;
                    SceneManager.LoadSceneAsync(_nextScene);
                }
            }
        }
        else
        {
            _fadeout.material.color = Color.Lerp(_fadeout.material.color, Color.clear, Time.deltaTime * 2.5f);
        }
        HandAnimation();
    }

    #region Animation
    void HandAnimation()
    {
        if (_handAnim.GetBool("ReachOut") && _handAnim.GetCurrentAnimatorStateInfo(0).IsName("HandReachoutAnim"))
        { _handAnim.SetBool("ReachOut", false); }

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

        _averagePos = Vector2.zero;

        _handTrans.localEulerAngles = new Vector3(31, 0, -_ang * Mathf.Rad2Deg - 90);

        if (_isMoving) // Case moving
        { return; }


        for (int i = 0; i < _prevMousePosViewPort.Length; i++)
        { _averagePos += new Vector2(_prevMousePosViewPort[i].x, _prevMousePosViewPort[i].y); }
        _averagePos /= (_prevMousePosViewPort.Length);
        if (_timeAnim < -1.2f)
        { _prevAveragePos = new Vector2(9999, 9999); }
        _timeAnim -= Time.deltaTime;

        

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
			//Debug.Log ("NotTraking");
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
            if (gaze.LastGazePoint.Screen.x >= -200 && gaze.LastGazePoint.Screen.x <= Screen.width+200 &&
                gaze.LastGazePoint.Screen.y >= -200 && gaze.LastGazePoint.Screen.y <= Screen.height+200)
            {
                return new Vector3(gaze.LastGazePoint.Screen.x, gaze.LastGazePoint.Screen.y, 0);
            }
            else
            {
                return Input.mousePosition;
            }
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
                Debug.Log("Hit: " + _prevMouseRayHit.collider.name);
				if (_prevMouseRayHit.collider.tag == "Flaw" && puzzleSolved)
                {
                    _flawId = _prevMouseRayHit.collider.name;
                    _tagetFlaw = _prevMouseRayHit.collider.gameObject;
                }
            }
        }
        else if (_flawId != null)
        {
            _flawGO.Remove(_tagetFlaw);
            Destroy(_tagetFlaw);
            _flawId = null;
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
                        if(clueManager.playerClueOrder.Count == clueManager.clueContainer.transform.childCount)
						if (clueManager.isClueOrderCorrect ()) {
							Debug.Log ("Victory");
							puzzleSolved = true;
							var lights = clueManager.getClueContainer ().GetComponentsInChildren<Light> ();
                            //_clueLighting = true;
                            if (_timerClue <= 0f)
                            {
                                foreach (var l in lights)
                                {
                                    l.color = Color.green;
                                }
                            }
							// GET TO THE CHOPPA! (fault lights up)
						} else {
							Debug.Log ("Fail");
							puzzleSolved = false;
							var lights = clueManager.getClueContainer ().GetComponentsInChildren<Light> ();
                            _clueLighting = true;
                            if (_timerClue <= 0f)
                            {
                                foreach (var l in lights)
                                {
                                    l.color = Color.red;
                                }
                            }
                        }
					}
				}
			}
		}

        

        if (_clueLighting)
        {
            _timerClue += Time.deltaTime;
            var lights = clueManager.getClueContainer().GetComponentsInChildren<Light>();
            if (_timerClue > 2f)
            {
                foreach (var l in lights)
                {
                    l.enabled = false;
                    l.color = Color.white;
                }
                _timerClue = -1f;
                clueManager.ResetPlayerClues();
                _clueLighting = false;
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
