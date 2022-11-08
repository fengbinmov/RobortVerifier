using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public WorldManager world;
    public List<string> tags = new List<string>();

    public Transform select;
    public Transform origin;
    UIGameScene uIGameScene;

    private void Start()
    {
        uIGameScene = UIManager.inst.GetLoadedUI<UIGameScene>();
    }
    public List<Transform> content = new List<Transform>();

    public MonoBehaviour mono;


    private void Update()
    {

        if (Input.anyKey || uIGameScene.keyCodes.Count > 0) {

            content.Clear();
            foreach (var item in world.objects)
            {
                if (item == null) continue;

                if (item.position.z > -15 && item.position.z < 5 && tags.Contains(item.tag))
                {
                    content.Add(item);
                }
            }

            origin = content.Count > 0 ? content[content.Count - 1] : null;
        }

        if (content.Count == 0) return;

        if (select != null) {

            if (select.position.z > 5 || select.position.z <= -15)
            {
                foreach (var item in select.GetComponentsInChildren<Transform>())
                {
                    item.gameObject.layer = 0;
                }
                select = null;
            }
        }

        if (select == null)
        {
            float d = float.MaxValue;

            foreach (var item in content)
            {
                if (item.position.magnitude < d)
                {
                    d = item.position.magnitude;
                    select = item;
                }
            }

            foreach (var item in select.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = 8;
            }
        }
        else
        {
            foreach (var item in select.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = 0;
            }
            if (Input.GetKeyDown(KeyCode.W) || uIGameScene.keyCodes.Contains((int)KeyCode.W)) {

                float d = float.MaxValue;
                foreach (var item in content)
                {
                    if (item.position.z > select.position.z && Mathf.Abs(item.position.z - select.position.z) < d)
                    {
                        d = Mathf.Abs(item.position.z - select.position.z);
                        select = item;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.S) || uIGameScene.keyCodes.Contains((int)KeyCode.S))
            {
                float d = float.MaxValue;
                foreach (var item in content)
                {
                    if (item.position.z < select.position.z && Mathf.Abs(item.position.z - select.position.z) < d)
                    {
                        d = Mathf.Abs(item.position.z - select.position.z);
                        select = item;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.D) || uIGameScene.keyCodes.Contains((int)KeyCode.D))
            {
                float d = float.MaxValue;
                foreach (var item in content)
                {
                    if (item.position.x > select.position.x && Mathf.Abs(item.position.x - select.position.x) < d)
                    {
                        d = Mathf.Abs(item.position.x - select.position.x);
                        select = item;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.A) || uIGameScene.keyCodes.Contains((int)KeyCode.A))
            {
                float d = float.MaxValue;
                foreach (var item in content)
                {
                    if (item.position.x < select.position.x && Mathf.Abs(item.position.x - select.position.x) < d)
                    {
                        d = Mathf.Abs(item.position.x - select.position.x);
                        select = item;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.X) || uIGameScene.keyCodes.Contains((int)KeyCode.X))
            {
                Destroy(select.gameObject);
                uIGameScene.ShowText(null);
                return;
            }
            foreach (var item in select.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = 8;
            }

            if (select.CompareTag("build"))
            {
                mono = select.GetComponent<CoilPowerBuild>();
            }
            else
            {
                mono = select.GetComponent<AFPrototypeRobot>();
            }
            uIGameScene.ShowText(mono != null ? mono.ToString() : null);
        }

    }
}
