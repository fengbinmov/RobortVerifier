using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBase : MonoBehaviour
{

    public BodyStatus LocalStatus() {

        var landChunk = transform.parent.GetComponent<LandChunk>();

        float localTimeProcess = ShaderGlobal.inst.LoopTimeProcess(landChunk.posValue + transform.localPosition.z);

        return ShaderGlobal.ProcessToBodyStatus(localTimeProcess);
    }
}
