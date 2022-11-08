using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Text;

public enum BodyStatus
{
    Day,    //����
    Night,  //ҹ��
    Gloam,  //�ƻ�
    Matinal, //�峿
    Null
}
public class AFPrototypeRobot : RobotBase
{
    public float coilValue = 60;


    public bool init;

    public BodyStatus bodyStatus;
    public Transform[] partComp;

    public float power;

    public float takeSpeed = 1;

    public int coilCount;

    public Mover mover;

    public Animator animator;


    private void Awake()
    {
        coilCount = partComp[2].GetComponentsInChildren<Transform>().Length - 1;

        power = coilCount * coilValue;
    }

    float localTimeProcess;
    public void CheckBodyStatus() {

        var landChunk = transform.parent.GetComponent<LandChunk>();

        localTimeProcess = ShaderGlobal.inst.LoopTimeProcess(landChunk.posValue + transform.localPosition.z);

        ChangeBodyStatus(ShaderGlobal.ProcessToBodyStatus(localTimeProcess));
    }

    [Button("ChangeBodyStatus")]
    public void ChangeBodyStatus(BodyStatus bodyStatus_) {
        if (bodyStatus != bodyStatus_) {

            if (power > coilValue)
            {
                power -= takeSpeed * 4 * Time.deltaTime;
                return;
            } 
            bodyStatus = bodyStatus_;

            if (bodyStatus == BodyStatus.Day || bodyStatus == BodyStatus.Gloam || bodyStatus == BodyStatus.Matinal)
            {
                partComp[0].localPosition = new Vector3(0, 1.5f, 0);
                partComp[1].localPosition = new Vector3(0, 0.5f, 0);

                if (GetComponent<NightAIRobort>() != null) {
                    Destroy(GetComponent<NightAIRobort>());

                    var script = gameObject.AddComponent<DayAIRobort>();
                    script.behaviorWait = new Vector2(0.25f, 2f);
                    script.self = this;
                    script.StartUp();
                }
            }
            else if (bodyStatus == BodyStatus.Night)
            {
                partComp[0].localPosition = new Vector3(0, 0.5f, 0);
                partComp[1].localPosition = new Vector3(0, 1.5f, 0);

                if (GetComponent<DayAIRobort>() != null)
                {
                    Destroy(GetComponent<DayAIRobort>());

                    var script = gameObject.AddComponent<NightAIRobort>();
                    script.behaviorWait = new Vector2(0.25f, 2f);
                    script.self = this;
                    script.StartUp();
                }
            }
        }
    }

    private void Update()
    {
        if (!init) return;

        power -= takeSpeed * Time.deltaTime;
        power = power <= 0 ? 0 : power;

        switch (bodyStatus)
        {
            case BodyStatus.Day:
                partComp[2].Rotate(Vector3.up, (Mathf.Sin(Time.realtimeSinceStartup * Mathf.PI) * 0.45f + 1f) * power * Time.deltaTime);
                break;
            case BodyStatus.Night:
                break;
        }

        int num = Mathf.CeilToInt(power / coilValue);

        int i = 0;
        for (; i < num && i < partComp[2].childCount; i++)
        {
            partComp[2].GetChild(i).gameObject.SetActive(true);
        }
        for (; i < partComp[2].childCount; i++)
        {
            partComp[2].GetChild(i).gameObject.SetActive(false);
        }

        //float v = Input.GetAxis("Vertical");
        //float h = Input.GetAxis("Horizontal");

        //mover.PlayerMove(new Vector2(h, v));

        //animator.SetFloat("Speed", mover.magnitude);

        if (power == 0) {
            Die();
        }
    }

    public void Attack() {

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

    }

    public void Injured()
    {
        animator.ResetTrigger("Injured");
        animator.SetTrigger("Injured");
    }


    public void Die() {
        init = false;
        animator.enabled = false;
        var mesho = GetComponentsInChildren<MeshFilter>();

        foreach (var item in mesho)
        {
            if (item.name.Equals("brainlowest")) item.transform.localScale = new Vector3(0.1f,1,  1);
            item.transform.SetParent(WorldManager.inst.world_);
            item.gameObject.AddComponent<MeshCollider>().convex = true;
            item.gameObject.AddComponent<Rigidbody>();
        }

        GetComponent<SimpleAIRobot>().enabled = false;

        this.TimeDelay(4f, () => {

            foreach (var item in mesho)
            {
                item.gameObject.layer = 7;
            }

            this.TimeDelay(1f, () => {

                foreach (var item in mesho)
                {
                    Destroy(item.gameObject);
                }
                Destroy(gameObject);
            });
        });
    }

    private void FixedUpdate()
    {
        CheckBodyStatus();
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        bool isnh = GetComponent<NightAIRobort>() != null;
        var behv = GetComponent<SimpleAIRobot>();
        if (isnh)
        {
            sb.AppendLine("����:������");
            sb.AppendLine("����:???������-" + Mathf.Abs(GetInstanceID()));
            sb.AppendLine();
            sb.AppendLine("��Դ:" + power.ToString());
            sb.AppendLine("�ٶ�:" + mover.speed.ToString());
            sb.AppendLine("����:" + takeSpeed.ToString());
            sb.AppendLine("״̬:" + behv.ToString());
            sb.AppendLine();
            sb.AppendLine("�ֲ�ʱ��:" + ShaderGlobal.ProcessToString(localTimeProcess));
            sb.AppendLine("�ֲ�ʱ��:" + bodyStatus.ToString());
            sb.AppendLine();
            sb.AppendLine("��ʾ:����ƻ�..");
            sb.AppendLine("����:�ռ��ַ���ѵ����Դ��Ϊ�Լ�����");
            sb.AppendLine("������");
        }
        else
        {
            sb.AppendLine("����:������");
            sb.AppendLine("����:�ռ�������-" + Mathf.Abs(GetInstanceID()));
            sb.AppendLine("��Դ:" + power.ToString());
            sb.AppendLine("�ٶ�:" + mover.speed.ToString());
            sb.AppendLine("����:" + takeSpeed.ToString());
            sb.AppendLine("״̬:" + behv.ToString());
            sb.AppendLine();
            sb.AppendLine("�ֲ�ʱ��:" + ShaderGlobal.ProcessToString(localTimeProcess));
            sb.AppendLine("�ֲ�ʱ��:" + bodyStatus.ToString());
            sb.AppendLine();
            sb.AppendLine("��ʾ:�Ѳ���ƻ�..");
            sb.AppendLine("����:�ռ��ַ�����Դ��Ϊ�Լ�����");
            sb.AppendLine("������");
        }

        return sb.ToString();
    }
}
