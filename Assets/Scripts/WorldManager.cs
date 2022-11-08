using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class LandGroup {

    public LandChunk[] landChunks;

    public LandChunk top;
    public LandChunk bot;

    public int nowChunkId = 5;

    public float length;

    public float off;
    public float offRadio;

    public AnimationCurve offCurve;

    public void Init()
    {
        float dis = 0;
        for (int i = 0; i < landChunks.Length; i++)
        {
            dis += landChunks[i].td.y - landChunks[i].td.x;
        }
        length = dis;

        UpdatePosition();
    }

    public void UpdatePosition(float dalta = 0)
    {
        off = off - dalta;

        if (off > 60) off = 60f;
        if (off < -60) off = -60f;

        landChunks[nowChunkId].UpdatePosition(dalta);

        for (int i = 1; i < 7; i++)
        {
            int n = (nowChunkId - i + 14) % 14;
            landChunks[n].UpdatePosition();
        }

        for (int i = 1; i <= 7; i++)
        {
            int n = (nowChunkId + i) % 14;
            landChunks[n].UpdatePosition(false);
        }

        for (int i = 0; i < landChunks.Length; i++)
        {
            if (landChunks[i].InCenter())
            {
                nowChunkId = i;
                break;
            }
        }

        //float p = off >=0 ? off / length : (1f- off / length);

        //offRadio = (p <= 0.5f ? Mathf.Lerp(0,-1f,p) : Mathf.Lerp(-0.5f,0.5f,p - 0.5f));
        offRadio = -off / length;

        //offRadio = v * ShaderGlobal.inst.dayTime;

        UpdateTB();
    }

    private void UpdateTB() {
        float m = float.MinValue;
        float n = float.MaxValue;

        for (int i = 0; i < landChunks.Length; i++)
        {
            var item = landChunks[i];

            if (item.transform.position.z > m) {
                m = item.transform.position.z;
                top = item;
            }
            if (item.transform.position.z < n)
            {
                n = item.transform.position.z;
                bot = item;
            }
        }
    }

    public float LoopZ(float z)
    {
        UpdateTB();

        float max = top.transform.position.z + top.td.y;
        float min = bot.transform.position.z + bot.td.x;

        if (z > max)
        {
            z = z - 140f;
        }
        if (z <= min)
        {
            z = z + 140f;
        }
        return z;
    }

    public LandChunk InChunk(float z)
    {
        foreach (var item in landChunks)
        {
            if (item.InChunk(z)) {
                return item;
            }
        }

        var obj = GameObject.FindObjectOfType<AFPrototypeRobot>();

        float max = top.transform.position.z + top.td.y;
        float min = bot.transform.position.z + bot.td.x;

        throw new Exception("InChunk [z,"+z+"] not in any land z"+ obj.transform.position.z +" "+max +" "+min);
    }

    public float Distance(Transform a, Transform b) {

        float d = Mathf.Abs(a.position.z - b.position.z);

        if (d > 70f) {
            d = 140f - d;
            d = Mathf.Sqrt(Mathf.Pow(d, 2) + Mathf.Pow(Mathf.Abs(a.position.x - b.position.x), 2));
        }
        return d;
    }
    public Vector3 GetDirection(Transform a, Transform b)
    {
        float d = Mathf.Abs(a.position.z - b.position.z);

        if (d > 70f)
        {
            d = 140f - d;
            float dir = b.position.z - a.position.z >=0 ? -1 : 1;
            return new Vector3(b.position.x - a.position.x,0,d *dir).normalized;
        }
        return (b.position - a.position).normalized;
    }
}

public class WorldManager : MonoBehaviour
{
    private static WorldManager _ins;
    public static WorldManager inst => _ins;

    private void Awake()
    {
        _ins = this;
    }

    public LandGroup landGroup;
    public ShaderGlobal shaderGlobal;
    public Transform world_;
    public List<Transform> objects = new List<Transform>();

    public float speed = 1;
    UIGameScene uIGameScene;

    private void Start()
    {
        landGroup.Init();

        UIManager.inst.Show<UIGameScene>();

        for (int i = world_.childCount - 1; i >= 0; i--)
        {
            objects.Add(world_.GetChild(i));

            float z = landGroup.LoopZ(objects[objects.Count-1].transform.position.z);

            GetLandChunk(z).AddChild(objects[objects.Count - 1], z);
        }
        uIGameScene = UIManager.inst.GetLoadedUI<UIGameScene>();
    }

    public LandChunk GetLandChunk(float z) {

        return landGroup.InChunk(z);
    }

    private void Update()
    {
        float y2 = uIGameScene.keyCodes.Contains((int)KeyCode.E) ? 1 : (uIGameScene.keyCodes.Contains((int)KeyCode.Q) ? -1 : 0);

        float y = Input.GetAxis("VerticalT") + y2;

        if (y != 0) {

            //float mul = Input.GetKey(KeyCode.LeftShift) ? 2f : 1;
            float mul = 1;

            landGroup.UpdatePosition(-y * speed * Time.deltaTime * mul);
        }
    }

    private void FixedUpdate()
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if (objects[i] == null) {objects.RemoveAt(i); continue; }
            float z = landGroup.LoopZ(objects[i].transform.position.z);

            GetLandChunk(z).AddChild(objects[i], z);
        }

        for (int i = world_.childCount - 1; i >= 0; i--)
        {
            objects.Add(world_.GetChild(i));

            float z = landGroup.LoopZ(objects[objects.Count - 1].transform.position.z);

            GetLandChunk(z).AddChild(objects[objects.Count - 1], z);
        }
    }

    [ContextMenu("Test_SetLand")]
    private void Test_SetLand() {


        List<LandChunk> lands = new List<LandChunk>();

        for (int i = 0; i < 14; i++)
        {
            var obj = transform.Find("land_" + i).GetComponent<LandChunk>();

            lands.Add(obj);

            obj.top = transform.Find("land_" + (i + 13) % 14).GetComponent<LandChunk>();
            obj.dow = transform.Find("land_" + (i + 1) % 14).GetComponent<LandChunk>();
            obj.posValue = obj.transform.position.z;
        }

        landGroup.landChunks = lands.ToArray();
    }

    [ContextMenu("Test_SetLandStatus")]
    private void Test_SetLandStatus()
    {
        landGroup.UpdatePosition();
    }
}
