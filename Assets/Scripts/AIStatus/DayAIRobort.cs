using SAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAIRobort : SimpleAIRobot
{
    public AFPrototypeRobot self;

    class Idle : Behavior
    {
        public readonly static int hash = "Idle".GetHashCode();

        public Transform moveTarget;

        public Walk walk;

        public DayAIRobort aIRobort;
        public bool goHome;
        public Transform takeObj;

        public override void Entry()
        {
            walk = GetBehavior<Walk>(Walk.hash);

            if (aIRobort == null) aIRobort = mono as DayAIRobort;
        }
        public override void Update()
        {
            if (goHome)
            {
                goHome = false;

                aIRobort.self.init = false;
                aIRobort.self.enabled = false;

                ChangeStatus(WaitHome.hash);
            }
            else if (aIRobort.self.bodyStatus != BodyStatus.Day)
            {
                var build = GameRes.inst.GetNearBuild(transform, true);

                goHome = true;

                walk.target = build.transform;

                ChangeStatus(Walk.hash);

            }
            else if (moveTarget != null)
            {
                walk.target = moveTarget;

                takeObj = moveTarget;

                moveTarget = null;

                ChangeStatus(Walk.hash);
            }
            else if (takeObj != null) {

                GetBehavior<Take>(Take.hash).target = takeObj;

                takeObj = null;

                ChangeStatus(Take.hash);
            }
            else
            {
                ChangeStatus(FindTarget.hash);
            }
        }
    }

    class WaitHome : Behavior
    {
        public readonly static int hash = "WaitHome".GetHashCode();

        public DayAIRobort aIRobort;
        public override void Entry()
        {
            if (aIRobort == null) aIRobort = mono as DayAIRobort;
        }

        public override void Update()
        {
            if (aIRobort.self.LocalStatus() == BodyStatus.Day)
            {
                aIRobort.self.init = true;
                aIRobort.self.enabled = true;

                ChangeStatus(Idle.hash);
            }
        }
    }
    class Walk : Behavior
    {
        public readonly static int hash = "Walk".GetHashCode();

        public Transform target;

        public override void Entry()
        {
        }

        public override void Update()
        {
            if (target != null)
            {

                Vector3 dir = WorldManager.inst.landGroup.GetDirection(transform, target);

                (mono as DayAIRobort).self.mover.PlayerMove(new Vector2(dir.x, dir.z));

                if (WorldManager.inst.landGroup.Distance(transform, target) < 0.3f)
                {
                    (mono as DayAIRobort).self.mover.PlayerMove(Vector2.zero);

                    ChangeStatus(Idle.hash);
                }
            }
            else
            {
                ChangeStatus(Idle.hash);
            }
        }
    }
    class FindTarget : Behavior
    {
        public readonly static int hash = "FindTarget".GetHashCode();

        public Idle idle;

        public override void Entry()
        {
            idle = GetBehavior<Idle>(Idle.hash);
        }

        public override void Update()
        {
            idle.moveTarget = GameRes.inst.GetNearProp(transform);

            ChangeStatus(Idle.hash);
        }
    }
    class Take : Behavior
    {
        public readonly static int hash = "Take".GetHashCode();

        public Transform target;

        public override void Update()
        {
            if (target != null)
            {
                Destroy(target.gameObject);

                (mono as DayAIRobort).self.power += (mono as DayAIRobort).self.coilValue;

                ChangeStatus(Idle.hash);
            }
            else
            {
                ChangeStatus(Idle.hash);
            }
        }
    }

    public enum Status
    {
        Idle,
        Walk,
        FindTarget,
        Take,
        WaitHome
    }

    public Status status = Status.Idle;

    protected Dictionary<string, Status> pairs = new Dictionary<string, Status>();

    public override void SwithStatus(int type)
    {
        status = pairs[type.ToString()];

        base.SwithStatus(type);
    }

    public override T GetBehavior<T>(int type) 
    {
        Behavior temp = behaviors.ContainsKey(type) ? behaviors[type] : null;

        switch (pairs[type.ToString()])
        {
            case Status.Idle:
                if (temp == null) behaviors.Add(type, new Idle() { mono = this });
                break;
            case Status.Walk:
                if (temp == null) behaviors.Add(type, new Walk() { mono = this });
                break;
            case Status.FindTarget:
                if (temp == null) behaviors.Add(type, new FindTarget() { mono = this });
                break;
            case Status.Take:
                if (temp == null) behaviors.Add(type, new Take() { mono = this });
                break;
            case Status.WaitHome:
                if (temp == null) behaviors.Add(type, new WaitHome() { mono = this });
                break;
        }
        return behaviors.ContainsKey(type) ? behaviors[type] as T : null;
    }

    public override void StartUp()
    {
        base.StartUp();

        pairs.Add(Idle.hash.ToString(), Status.Idle);
        pairs.Add(Walk.hash.ToString(), Status.Walk);
        pairs.Add(FindTarget.hash.ToString(), Status.FindTarget);
        pairs.Add(Take.hash.ToString(), Status.Take);
        pairs.Add(WaitHome.hash.ToString(), Status.WaitHome);

        gameObject.SetActive(true);

        SwithStatus(Idle.hash);
    }

    public override string ToString()
    {
        return status.ToString();
    }
}
