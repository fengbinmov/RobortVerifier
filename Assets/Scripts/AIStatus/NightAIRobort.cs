using SAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class NightAIRobort : SimpleAIRobot
{
    public AFPrototypeRobot self;

    class Idle : Behavior
    {
        public readonly static int hash = "Idle".GetHashCode();

        public AFPrototypeRobot moveTarget;

        public AFPrototypeRobot attackTarget;

        public Walk walk;

        public NightAIRobort aIRobort;
        public bool goHome;

        public override void Entry()
        {
            walk = GetBehavior<Walk>(Walk.hash);

            if (aIRobort == null) aIRobort = mono as NightAIRobort;
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
            else if (aIRobort.self.bodyStatus == BodyStatus.Matinal)
            {
                var build = GameRes.inst.GetNearBuild(transform, false);

                walk.target = build.transform;

                goHome = true;

                ChangeStatus(Walk.hash);

            }
            else if (moveTarget != null)
            {
                walk.target = moveTarget.transform;

                attackTarget = moveTarget;

                moveTarget = null;

                ChangeStatus(Walk.hash);
            }
            else if (attackTarget != null) {

                GetBehavior<Take>(Take.hash).target = attackTarget;

                attackTarget = null;

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

        public NightAIRobort aIRobort;
        public override void Entry()
        {
            if (aIRobort == null) aIRobort = mono as NightAIRobort;
        }

        public override void Update()
        {
            if (aIRobort.self.LocalStatus() == BodyStatus.Night)
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

        public Take take;

        public override void Entry()
        {
            take = GetBehavior<Take>(Take.hash);
        }

        public override void Update()
        {
            if (target != null)
            {
                Vector3 dir = WorldManager.inst.landGroup.GetDirection(transform, target);

                (mono as NightAIRobort).self.mover.PlayerMove(new Vector2(dir.x, dir.z));

                if (WorldManager.inst.landGroup.Distance(transform, target) < 0.3f)
                {
                    (mono as NightAIRobort).self.mover.PlayerMove(Vector2.zero);

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
            idle.moveTarget = GameRes.inst.GetNearDayRobot(transform);

            ChangeStatus(Idle.hash);
        }
    }
    class Take : Behavior
    {

        public readonly static int hash = "Take".GetHashCode();

        public AFPrototypeRobot target;

        public override void Update()
        {
            if (target != null)
            {
                target.power = Mathf.Max(0, target.power - (mono as NightAIRobort).self.coilValue * 3f);

                (mono as NightAIRobort).self.power += (mono as NightAIRobort).self.coilValue;

                target.Injured();
                (mono as NightAIRobort).self.Attack();

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
        WaitHome,
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
