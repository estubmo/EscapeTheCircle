using UnityEngine;
using System.Collections;

public class logoanimScript : MonoBehaviour {
    Transform tr;

	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        tr.position = new Vector3(tr.position.x, tr.position.y + Mathf.Sin(Time.realtimeSinceStartup) * 0.5f, 0);
	}
}
