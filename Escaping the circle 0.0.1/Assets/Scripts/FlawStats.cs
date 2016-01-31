using UnityEngine;
using System.Collections;

public class FlawStats : MonoBehaviour
{
    //public float _timeToFindFlaw;
    private float _size;
    void Start()
    {
        if(GameObject.Find("CalibrationStats") != null)
        {
            _size = GameObject.Find("CalibrationStats").GetComponent<CalibrationStats>()._calibrationSize;
            transform.localScale = new Vector3(_size / 9f, _size / 9f, _size / 9f);
            Debug.Log("Calibrated");
        }
    }
}
