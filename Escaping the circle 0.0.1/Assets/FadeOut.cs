using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {
    public float _timeOut;
    public bool _fadeOut;
    public float _time;

    private bool _audioPlaying;

    public GameObject _AudioDemonLaugh;
    // Use this for initialization
    void Start () {
        _audioPlaying = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_fadeOut) { 
            _time += Time.deltaTime;
            if (_time < _timeOut + 1f)
            {
                if (!_audioPlaying) {
                    GameObject Source = Instantiate<GameObject>(_AudioDemonLaugh) as GameObject;
                    Debug.Log("The audio should now have been spawned!");
                    _audioPlaying = true;
                }
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<Renderer>().material.color =
                        Color.Lerp(Color.white, Color.clear, _time / _timeOut);
                }
            }
        }
	}
}
