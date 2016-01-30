using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

//This script is a collection different actions typical for a button
// Mattias Tronslien 2016

public class ButtonActions : MonoBehaviour {

	public int _PreviousScene;
	private GazeAwareComponent _gazeAware;

	void Start(){
		_gazeAware = GetComponent<GazeAwareComponent> ();
	}

    public void ChangeLevel(int IndexOfScene) {
        SceneManager.LoadScene(IndexOfScene);
    } 

	public void Exit () {
		Application.Quit();
	}
	void Update () {
		Debug.Log (_gazeAware.HasGaze);
		if(Input.GetButtonDown("Cancel") || _gazeAware.HasGaze)
		{
			SceneManager.LoadScene(_PreviousScene);
		}

	}



}
