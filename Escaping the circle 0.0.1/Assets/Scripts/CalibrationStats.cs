using UnityEngine;
using System.Collections;

public class CalibrationStats : MonoBehaviour
{
    public float _calibrationSize;

    void Awake()
    { DontDestroyOnLoad(gameObject); }
}
