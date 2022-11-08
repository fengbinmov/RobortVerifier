using UnityEngine;
using System.Runtime.CompilerServices;
/// <summary>
/// UI面板的基类
/// </summary>
public class BaseUI : MonoBehaviour
{
    private UIManager m_uIMgr;

    private bool inited = false;

    public string uiName => name;
    public int ID => GetInstanceID();
    public UIManager uIMgr => m_uIMgr;

    /// <summary>
    /// 初始化界面
    /// </summary>
    public virtual void OnInit()
    {

    }

    public virtual void OnShow(ITuple tuple = null)
    {
        if (!inited)
        {
            inited = true;
            OnInit();
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    public virtual void OnClose()
    {
        OnClear();

        gameObject.SetActive(false);

        uIMgr?.Close(this);
    }

    /// <summary>
    /// 清理界面
    /// </summary>
    public virtual void OnClear()
    {
    }

    private void OnDestroy()
    {
        OnClear();
    }

    public virtual void PlaySound(string audioName)
    {
    }
}
