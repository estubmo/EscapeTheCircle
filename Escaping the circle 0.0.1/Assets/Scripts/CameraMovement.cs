using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    private Transform _tr;
    private Camera _camera;
    private Animator _handAnim;
    private Transform _handTrans;

    private List<GameObject> _flawGO = new List<GameObject>();
    private Vector3[] _prevMousePos = new Vector3[12];
    private int _index = 0;
    private Vector3[] _prevMousePosViewPort = new Vector3[12];
    private Ray _mouseToWorldRay;
    private RaycastHit _mouseRayHit;
    private string _flawName;
    private string _prevName;
    private float _flawTimer;
    private Vector2 _averagePos;
    private Vector2 _prevAveragePos;
    private float _timeAnim;
    private bool _isMoving;


    public float _timeNeededToFindFlaw;
    public float _sensitivityOfViewPort;
    public float _SpeedOfRotation;

    // Use this for initialization
    void Start ()
    {
        _tr = GetComponent<Transform>();
        _camera = Camera.main;
        _flawGO.AddRange(GameObject.FindGameObjectsWithTag("Flaw"));
        _handAnim = gameObject.GetComponentInChildren<Animator>();
        _handTrans = _tr.GetChild(0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Loading array for both smoothening purpus, and to check if mouse/eyes are forcused on one spot
        if (_index >= _prevMousePos.Length-1)
        { _index = 0; }
        else { _index++; }
        _prevMousePos[_index] = _camera.ScreenToWorldPoint(Input.mousePosition);
        _prevMousePosViewPort[_index] = _camera.ScreenToViewportPoint(Input.mousePosition);
        _mouseToWorldRay = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(_mouseToWorldRay, out _mouseRayHit, 100f);
        Debug.DrawRay(_mouseToWorldRay.origin, _mouseToWorldRay.direction, Color.red);

        //Debug.Log(_mouseRayHit.collider.name);

        ViewPortMovement();

        FindFlaw();
        
        _averagePos = Vector2.zero;
        if (_isMoving)
        { _averagePos = new Vector2(9999, 9999); }

        for (int i = 0; i < _prevMousePosViewPort.Length;i++)
        {_averagePos += new Vector2(_prevMousePosViewPort[i].x,_prevMousePosViewPort[i].y);}
        _averagePos /= (_prevMousePosViewPort.Length);

        //Debug.Log(_averagePos.ToString() + " : " + _prevMousePosViewPort[_index].ToString());
        _timeAnim -= Time.deltaTime;

        if (_handAnim.GetBool("ReachOut") && _timeAnim <= 0) { _handAnim.SetBool("ReachOut", false); }
        if (Vector2.Distance(_averagePos, new Vector2(_prevMousePosViewPort[_index].x,_prevMousePosViewPort[_index].y)) < 0.1f &&
            !(Vector2.Distance(_averagePos,_prevAveragePos) < 0.1f))
        { _handAnim.SetBool("ReachOut",true); _prevAveragePos = _averagePos; _timeAnim = 0.2f; Debug.Log("Hey"); }

        //_handTrans.rotation = Quaternion.LookRotation(Vector3.RotateTowards(_handTrans.forward, _mouseRayHit.barycentricCoordinate - _handTrans.position, 10f,0));
        //_handTrans.eulerAngles = new Vector3(_handTrans.eulerAngles.x+90,_handTrans.eulerAngles.y,_handTrans.eulerAngles.z);
        if(_flawGO.Count == 0)
        {
            SceneManager.LoadSceneAsync(1);
        }

    }

    #region FindFlaw
    void FindFlaw()
    {
        if (_mouseRayHit.collider.tag == "Flaw")
        {
            //_timeNeededToFindFlaw = _mouseRayHit.collider.gameObject.GetComponent<FlawStats>()._timeToFindFlaw;

            if (_mouseRayHit.collider.name != _flawName || _flawTimer > _timeNeededToFindFlaw)
            {
                _flawName = _mouseRayHit.collider.name;
                _flawTimer = _timeNeededToFindFlaw;
            }
        }
        if(_flawName == _mouseRayHit.collider.name)
        { _flawTimer -= Time.deltaTime; }
        else
        { _flawTimer += Time.deltaTime; }
        if (_flawTimer <= 0f)
        { Debug.Log("Found Flaw"); _flawGO.Remove(_mouseRayHit.collider.gameObject); Destroy(_mouseRayHit.collider.gameObject); }
    }
    #endregion

    #region ViewPort Movement
    void ViewPortMovement()
    {
        _isMoving = false;
        if (_prevMousePosViewPort[_index].x > 1f - _sensitivityOfViewPort) //RightSide
        {
            _tr.Rotate(Vector3.up * Time.deltaTime * _SpeedOfRotation * ((_prevMousePosViewPort[_index].x - (1f - _sensitivityOfViewPort)) / (_sensitivityOfViewPort)), Space.World);
            Debug.Log("GoRight");
            _isMoving = true;
        }
        if (_prevMousePosViewPort[_index].x < _sensitivityOfViewPort) //LeftSide
        {
            _tr.Rotate(Vector3.up * Time.deltaTime * _SpeedOfRotation * -1 * ((_sensitivityOfViewPort - _prevMousePosViewPort[_index].x) / _sensitivityOfViewPort), Space.World);
            Debug.Log("GoLeft");
            _isMoving = true;
        }
        if (_prevMousePosViewPort[_index].y > 1f - _sensitivityOfViewPort) //Up
        {
            Debug.Log("GoUp");
            _isMoving = true;
        }
        if (_prevMousePosViewPort[_index].y < _sensitivityOfViewPort) //Down
        {
            Debug.Log("GoDown");
            _isMoving = true;
        }
    }
    #endregion
}
