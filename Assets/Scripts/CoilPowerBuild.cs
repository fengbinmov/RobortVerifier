using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class CoilPowerBuild : MonoBehaviour
{
    public BodyStatus bodyStatus = BodyStatus.Null;
    public Transform[] coilPowers;
    public Transform[] partComp;
    public TextMeshPro textMeshPro;

    public int coilCount { get {
            int n = 0;
            foreach (var item in coilPowers)
            {
                n += item.gameObject.activeSelf ? 1 : 0;
            }
            return n;
        }
    }
    public BodyStatus LocalStatus()
    {

        var landChunk = transform.parent.GetComponent<LandChunk>();

        float localTimeProcess = ShaderGlobal.inst.LoopTimeProcess(landChunk.posValue + transform.localPosition.z);

        return ShaderGlobal.ProcessToBodyStatus(localTimeProcess);
    }
    private void Start()
    {
        GameRes.inst.builds.Add(this);
    }
    private void CreateAIRobot(string name = "AIPlayer") {

        var obj = Instantiate(GameRes.inst.prefabs[name], WorldManager.inst.world_);

        obj.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        var com = obj.GetComponent<AFPrototypeRobot>();

        com.mover.speed = Random.Range(1, 2f) * 2f;
        com.takeSpeed = Random.Range(0.75f, 1.25f);
        int powerNum = Mathf.FloorToInt(Mathf.Pow(Random.Range(0, 1f), 2) * 5);

        for (int i = 0; i <= powerNum; i++)
        {
            com.partComp[2].GetChild(i).gameObject.SetActive(true);
        }

        com.init = true;
        obj.SetActive(true);

        obj.GetComponent<SimpleAIRobot>().StartUp();

        GameRes.inst.CacheRobot(com);
    }

    public LandChunk landChunk;

    private float localTimeProcess;

    private void FixedUpdate()
    {
        if (landChunk == null) landChunk = transform.parent.GetComponent<LandChunk>();

        localTimeProcess = ShaderGlobal.inst.LoopTimeProcess(landChunk.posValue + transform.localPosition.z);


        if (textMeshPro != null)
        {
            textMeshPro.text = ShaderGlobal.ProcessToString(localTimeProcess);
        }

        ChangeBodyStatus(ShaderGlobal.ProcessToBodyStatus(localTimeProcess));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    [Button("ChangeBodyStatus")]
    public void ChangeBodyStatus(BodyStatus bodyStatus_)
    {
        if (bodyStatus != bodyStatus_)
        {
            StopAllCoroutines();
            partComp[0].gameObject.SetActive(false);
            partComp[1].gameObject.SetActive(false);

            if (bodyStatus_ == BodyStatus.Day)
            {
                partComp[0].gameObject.SetActive(true);
                partComp[1].gameObject.SetActive(true);

                CreateAIRobot();
                waitT = new Vector2(4, 8);
                StartCoroutine("MakeCoilPower");
            }
            else if(bodyStatus_ == BodyStatus.Night)
            {
                partComp[1].gameObject.SetActive(true);
                partComp[0].gameObject.SetActive(true);

                waitT = new Vector2(2, 4);
                StartCoroutine("MakeCoilPower");
            }
            bodyStatus = bodyStatus_;
        }
    }
    public bool TakeOne() {

        foreach (var item in coilPowers)
        {
            if (item.gameObject.activeSelf) {
                item.gameObject.SetActive(false);
                return true;
            }
        }
        return false;
    }

    Vector2 waitT = new Vector2(4, 8);
    IEnumerator MakeCoilPower() {

        var wait = new WaitForSeconds(Random.Range(waitT.x, waitT.y));
        while (true)
        {
            Vector3 p = transform.position + new Vector3(Random.Range(-10, 10f), 0, Random.Range(-10f, 10f));

            if (p.x > -5f && p.x < 5f && GameRes.inst.props.Count < 100) {

                var obj = Instantiate(GameRes.inst.prefabs["Coil Spring"], WorldManager.inst.world_);
                obj.transform.position = p;
                obj.SetActive(true);

                GameRes.inst.CacheProp(obj);
            }

            yield return wait;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("类型:建筑");
        sb.AppendLine("名称:供能站 "+Mathf.Abs(GetInstanceID()));
        sb.AppendLine("效率:" + waitT.x +" ~ "+ waitT.y +" s");
        sb.AppendLine("范围:" + "10x10");
        sb.AppendLine();
        sb.AppendLine("局部时间:" + ShaderGlobal.ProcessToString(localTimeProcess));
        sb.AppendLine("局部时辰:" + bodyStatus.ToString());
        sb.AppendLine();
        sb.AppendLine("提示:机器人的守护站");
        sb.AppendLine("详情:每个清晨提供新的机器人，并分发资源");
        sb.AppendLine("不可再生");

        return sb.ToString();
    }
}
