using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandChunk : MonoBehaviour
{
    public LandChunk top;
    public LandChunk dow;
    public Vector2 td;
    public float posValue;


    public void UpdatePosition(float off)
    {
        //transform.position = new Vector3(0, 0, posValue * 10 + off);
        transform.position = new Vector3(0, 0, transform.position.z + off);
    }

    public void UpdatePosition(bool alignAtDown = true)
    {
        if (alignAtDown)
        {
            transform.position = new Vector3(0, 0, dow.transform.position.z + dow.td.y - td.x);
        }
        else
        {
            transform.position = new Vector3(0, 0, top.transform.position.z + top.td.x - td.y);
        }
    }

    public bool InCenter() {

        return transform.position.z + td.y >= 0 && transform.position.z + td.x <= 0;
    }

    public bool InChunk(float z) {
        return z > transform.position.z + td.x && z <= transform.position.z + td.y;
    }

    public void AddChild(Transform transf,float z) {

        if (transf.parent == transform) return;

        var pos = transf.position;

        pos.z = z;

        transf.transform.position = pos;

        transf.SetParent(transform,true);
    }
}

