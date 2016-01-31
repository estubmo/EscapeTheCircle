using UnityEngine;
using System.Collections;

public class persist : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(gameObject);
	}

}
