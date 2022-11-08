using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunShake : MonoBehaviour
{
    public Vector3[] rang;
    public float strength = 1f;

    private Vector3 origin;

    private void Awake()
    {
        origin = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = origin + Vector3.Lerp(rang[0], rang[1], Random.Range(0, 1f)) * strength;
    }
}
