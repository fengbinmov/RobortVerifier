using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class UIGameScene : BaseUI
{
    public TextMeshProUGUI tex_TimeDate;
    public Transform tran_Clock_shiZ;
    public Image tran_Clock_shiZ_Fill;
    public TextMeshProUGUI tex_TimeProcess;

    public TextMeshProUGUI tex_Message;

    public override void OnInit()
    {
        base.OnInit();

    }
    public override void OnShow(ITuple tuple = null)
    {
        base.OnShow(tuple);

        StartCoroutine("UIUpdate");
    }

    public override void OnClear()
    {
        base.OnClear();

        StopAllCoroutines();
    }

    IEnumerator UIUpdate() {

        var wait = new WaitForSeconds(0.1f);

        while (true)
        {
            tex_TimeDate.text = ShaderGlobal.inst.timeLabel;

            tran_Clock_shiZ.transform.eulerAngles = new Vector3(0, 0, - ShaderGlobal.inst.sky_Process * 720f);

            tex_TimeProcess.text = ShaderGlobal.inst.timeLabel;
            yield return wait;
        }
    }

    public void ShowText(string mess) {

        tex_Message.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(mess));
        tex_Message.text = mess;
    }

    public List<int> keyCodes = new List<int>();

    public void OnButtonDown(int keyCode) {

        if (!keyCodes.Contains(keyCode)) {

            keyCodes.Add(keyCode);
        }
    }
    public void OnButtonUp(int keyCode)
    {
        if (keyCodes.Contains(keyCode))
        {
            keyCodes.Remove(keyCode);
        }
    }
}
