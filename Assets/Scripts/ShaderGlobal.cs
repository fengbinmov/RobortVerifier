using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderGlobal : MonoBehaviour
{
    private static ShaderGlobal _instance;
    public static ShaderGlobal inst => _instance;

    public float worldCurvature = 100;

    public Material sky_mat;

    [ColorUsage(false,true)] public Color[] sky_Days;
    [ColorUsage(false, true)] public Color[] sky_Nights;

    public Light light_directional;

    public AnimationCurve sky_ToNight_Curve;

    public Transform sumOrigin;

    public WorldManager world;


    [Range(0,1f)]
    public float sky_Process;


    private void Awake()
    {
        if (_instance == null) { _instance = this; }
    }

    public void UpdateSkyColor() {

        float process = sky_ToNight_Curve.Evaluate(sky_Process);

        RenderSettings.ambientGroundColor = Color.Lerp(sky_Days[0], sky_Nights[0], process);
        RenderSettings.ambientEquatorColor = Color.Lerp(sky_Days[1], sky_Nights[1], process);
        RenderSettings.ambientSkyColor = Color.Lerp(sky_Days[2], sky_Nights[2], process);

        sky_mat.SetFloat("_Process", process);

        light_directional.intensity = Mathf.Lerp(0.38f,0, process);

        sumOrigin.transform.eulerAngles = new Vector3((sky_Process -0.5f) * 360f, 0, 0);
    }

    private void OnValidate()
    {
        Shader.SetGlobalFloat("_WorldCurvature", worldCurvature);
        UpdateSkyColor();
    }

    public float nowTime = 0;
    public float loopTime;
    public float dayTime = 60;

    public string timeLabel;

    private void Start()
    {
        nowTime = dayTime / 2f;
    }

    private void Update()
    {
        nowTime += Time.deltaTime;

        loopTime = (nowTime + world.landGroup.offRadio * dayTime) % dayTime;

        sky_Process = ((loopTime >= 0 ? loopTime : (dayTime + loopTime)) / dayTime) % 1f;

        UpdateSkyColor();

        timeLabel = ProcessToString(sky_Process);
    }
    public float LoopTimeProcess(float z)
    {
        float offTime = WorldManager.inst.landGroup.LoopZ(z) / WorldManager.inst.landGroup.length;

        float loopTime_T = (nowTime - offTime * dayTime) % dayTime;

        return ((loopTime_T >= 0 ? loopTime_T : (dayTime + loopTime_T)) / dayTime) % 1f;
    }

    public static string ProcessToString(float p)
    {
        float h = Mathf.Lerp(0, 24f, p);

        return ((int)h).ToString("00.") + ":" + ((int)((h % 1f) * 60)).ToString("00.");
    }

    public static BodyStatus ProcessToBodyStatus(float p)
    {
        if (p >= 0.819f || p < 0.245f) return BodyStatus.Night;
        else if (p >= 0.245f && p < 0.35f) return BodyStatus.Matinal;
        else if (p >= 0.35f && p < 0.77f) return BodyStatus.Day;
        else return BodyStatus.Gloam;
    }

}
