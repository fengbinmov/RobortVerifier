using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed = 1;
    public bool lookForward = true;


    public Camera cam;
    private Vector3 positionLast;

    [ReadOnly] public float angleOff;
    [ReadOnly] public Vector3 velocity;
    [ReadOnly] public float magnitude;

    public List<string> wallTags = new List<string>();

    private void Awake()
    {
        positionLast = transform.position;
    }

    private void Start()
    {
        if (cam != null)
        {
            Vector3 viewForward = cam.transform.forward;
            viewForward.y = 0;
            viewForward = viewForward.normalized;

            angleOff = Vector2.SignedAngle(Vector2.up, new Vector2(viewForward.x, viewForward.z));
        }
    }

    private void Update()
    {
        velocity = transform.position - positionLast;
        positionLast = transform.position;
        magnitude = velocity.magnitude;
    }

    public void LiZheng()
    {
        positionLast = transform.position;
        velocity = transform.position - positionLast;
        magnitude = velocity.magnitude;
    }

    public void PlayerMove(Vector2 mont)
    {
        var target = transform.position + new Vector3(mont.x, 0, mont.y) * speed * Time.deltaTime;
        if (lookForward)
        {
            transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
        }

        if (target.x <= -5 && target.x >= 5)
        {
            return;
        }
        transform.position = target;
    }
    public void PlayerMoveToTarget(Vector3 targetPos)
    {
        Vector3 distance = targetPos - transform.position;
        Vector3 moveLen = distance.normalized * speed * Time.deltaTime;
        var target = transform.position + moveLen;
        if (lookForward && transform.position != targetPos)
        {
            transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
        }
        if (moveLen.magnitude >= distance.magnitude)
        {
            transform.position = targetPos;
        }
        else
        {
            transform.position = target;
        }
    }
}
