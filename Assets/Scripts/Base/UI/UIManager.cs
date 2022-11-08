using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class UIManager : SingletonMono<UIManager>
{
    public Transform layer_Show;
    public Transform layer_Close;

    protected List<BaseUI> loadedUI = new List<BaseUI>();      //只对UIObject对象进行缓存处理
    protected List<int> listOpenUI = new List<int>();          //以InstanceID记录UIObject的层级顺序

    public List<int> openUISort { get { return new List<int>(listOpenUI); } }

    public override void Awake()
    {
        if (layer_Close == null) layer_Close = transform.Find("Layer_Close");
        if (layer_Show == null) layer_Show = transform.Find("Layer_Show");

        base.Awake();

        loadedUI.Clear();
        listOpenUI.Clear();
    }

    protected T GetOrLoadPanel<T>(string uiname = null) where T : BaseUI
    {
        if (string.IsNullOrEmpty(uiname)) uiname = typeof(T).Name;
        var panel = loadedUI.FirstOrDefault(x => x != null && x.name.Equals(uiname) && x is T);

        if (panel != null)
        {
            if (panel.uIMgr == null)
            {
                typeof(BaseUI).GetField("m_uIMgr", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(panel, this);
            }
        }
        else
        {
            GameObject panelObj = null;

            panelObj = Resources.Load<GameObject>("UI/" + uiname);

            if (panelObj != null)
            {
                panelObj = Instantiate(panelObj, layer_Close, false);
                panelObj.name = uiname;
                panelObj.transform.localPosition = Vector3.zero;
                (panelObj.transform as RectTransform).anchoredPosition = Vector2.zero;
                (panelObj.transform as RectTransform).anchorMax = Vector2.zero;
                (panelObj.transform as RectTransform).anchorMin = Vector2.zero;
                (panelObj.transform as RectTransform).anchorMax = Vector2.one;

                panel = panelObj.GetComponent<T>();
            }

            if (panel != null)
            {
                loadedUI.Add(panel);
            }
        }
        return panel == null ? null : panel as T;
    }

    protected void AddPanel(BaseUI uI)
    {

        int id = uI.ID;
        int index = listOpenUI.IndexOf(id);
        if (index < 0)
            listOpenUI.Add(id);
        else
        {
            var t = listOpenUI[listOpenUI.Count - 1];
            listOpenUI[listOpenUI.Count - 1] = listOpenUI[index];
            listOpenUI[index] = t;
        }
    }

    protected void RemovePanel(BaseUI uI)
    {
        int index = listOpenUI.IndexOf(uI.ID);
        if (index > -1)
        {
            listOpenUI.RemoveAt(index);
        }
    }

    public virtual void Show<T>(string uiname = null, ITuple tuple = null) where T : BaseUI
    {
        if (string.IsNullOrEmpty(uiname)) uiname = typeof(T).Name;

        T panel = GetOrLoadPanel<T>(uiname);

        if (panel == null)
        {
            throw new Exception("OnOpen: not hav " + uiname + " " + typeof(T));
            //return;
        }

        panel.transform.SetParent(layer_Show);
        panel.transform.SetAsLastSibling();
        AddPanel(panel);
        panel.OnShow(tuple);

        var panelBg = panel.transform.Find("bg");
        if (panelBg != null) panelBg.SetAsFirstSibling();
    }

    public virtual void Close<T>(string uiname = null) where T : BaseUI
    {
        if (string.IsNullOrEmpty(uiname)) uiname = typeof(T).Name;
        var panel = loadedUI.FirstOrDefault(x => x.name.Equals(uiname) && x is T);
        if (panel != null && panel.gameObject.activeSelf)
        {
            panel.OnClose();
            RemovePanel(panel);

            if (panel.transform.parent != layer_Close) panel.transform.SetParent(layer_Close);
        }
    }

    public virtual void Close<T>(T panel = null) where T : BaseUI
    {
        string uiname = panel == null ? uiname = typeof(T).Name : panel.name;

        if (panel == null)
        {
            var result = loadedUI.FirstOrDefault(x => x.name.Equals(uiname) && x is T);
            if (result != null) panel = result as T;
        }
        if (panel != null)
        {
            if (panel.gameObject.activeSelf) panel.OnClose();
            if (panel.transform.parent != layer_Close)
            {
                RemovePanel(panel);
                panel.transform.SetParent(layer_Close);
            }
        }
    }

    public virtual void DestroyPanel<T>(string uiname = null) where T : BaseUI
    {
        if (string.IsNullOrEmpty(uiname)) uiname = typeof(T).Name;
        var panel = loadedUI.FirstOrDefault(x => x.name.Equals(uiname) && x is T);
        if (panel != null)
        {
            if (panel.gameObject.activeSelf)
            {
                panel.OnClose();
            }
            loadedUI.Remove(panel);
            DestroyImmediate(panel.gameObject);
        }
    }

    public virtual T GetLoadedUI<T>(string uiname = null) where T : BaseUI
    {
        if (string.IsNullOrEmpty(uiname)) uiname = typeof(T).Name;
        var panel = loadedUI.FirstOrDefault(x => x.name.Equals(uiname) && x is T);
        if (panel != null)
        {
            return panel as T;
        }
        else
        {
            return null;
        }
    }
}