using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRes : MonoBehaviour
{
    private static GameRes _ins;
    public static GameRes inst => _ins;

    public StringPairs<GameObject> prefabs = new StringPairs<GameObject>();

    public List<Transform> props = new List<Transform>();
    public List<Transform> propsTemp = new List<Transform>();

    public List<AFPrototypeRobot> roborts = new List<AFPrototypeRobot>();
    public List<AFPrototypeRobot> robortsTemp = new List<AFPrototypeRobot>();

    public List<CoilPowerBuild> builds = new List<CoilPowerBuild>();

    public void CacheRobot(AFPrototypeRobot obj)
    {
        roborts.Add(obj);
    }
    public void CacheProp(GameObject obj) {
        props.Add(obj.transform);
    }

    public CoilPowerBuild GetNearBuild(Transform self,bool isDay = true)
    {
        float d = 99999f;
        CoilPowerBuild resut = null;
        for (int i = builds.Count - 1; i >= 0; i--)
        {
            if (builds[i] == null) { builds.RemoveAt(i); continue; }
            if (isDay)
            {
                if (builds[i].LocalStatus() == BodyStatus.Night) continue;
            }
            else
            {
                if (builds[i].LocalStatus() == BodyStatus.Day) continue;
            }

            float t = WorldManager.inst.landGroup.Distance(self, builds[i].transform);

            if (t < d)
            {
                d = t;
                resut = builds[i];
            }
        }
        return resut;
    }
    public AFPrototypeRobot GetNearDayRobot(Transform self)
    {
        float d = 99999f;
        AFPrototypeRobot resut = null;
        for (int i = roborts.Count - 1; i >= 0; i--)
        {
            if (roborts[i] == null) { roborts.RemoveAt(i); continue; }
            if (roborts[i].init && roborts[i].bodyStatus == BodyStatus.Day) {

                float t = WorldManager.inst.landGroup.Distance(self, roborts[i].transform);

                if (t < d)
                {
                    d = t;
                    resut = roborts[i];

                    robortsTemp.Add(resut);

                    roborts.RemoveAt(i);
                }
            }

        }
        return resut;
    }
    public Transform GetNearProp(Transform self) {

        float d = 99999f;
        Transform resut = null;
        for (int i = props.Count - 1; i >= 0; i--)
        {
            if (props[i] == null) {props.RemoveAt(i);continue;}

            float t = WorldManager.inst.landGroup.Distance(self, props[i]);

            if (t < d) {
                d = t;
                resut = props[i];

                propsTemp.Add(resut);

                props.RemoveAt(i);
            }
        }
        return resut;
    }

    private void Awake()
    {
        _ins = this;
    }

    private void Start()
    {
        StartCoroutine("PropBack");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator PropBack() {

        var wait = new WaitForSeconds(5);
        while (true)
        {
            for (int i = propsTemp.Count - 1; i >= 0; i--)
            {
                if (propsTemp[i] == null) { continue; }
                props.Add(propsTemp[i]);
            }
            propsTemp.Clear();

            for (int i = robortsTemp.Count - 1; i >= 0; i--)
            {
                if (robortsTemp[i] == null) { continue; }
                roborts.Add(robortsTemp[i]);
            }
            robortsTemp.Clear();

            yield return wait;
        }
    }
}
