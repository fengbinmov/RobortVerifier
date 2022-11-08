using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T inst => _instance;

    public virtual void Awake()
    {
        if (_instance == null) _instance = this as T;
        else
        {
            Debug.LogWarning("SingletonMono hav single:[InstanceID:" + inst.GetInstanceID() + "][name," + inst.name +
         "] not use this [InstanceID:" + GetInstanceID() + "][name," + name + "]");
        }
    }

    public virtual void OnInit() { }
}