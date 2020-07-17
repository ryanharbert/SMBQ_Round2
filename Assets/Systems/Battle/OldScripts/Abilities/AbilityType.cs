using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class AbilityType : MonoBehaviourPun, IPunObservable
{
    //DATA
    public string displayName;
    public string shortDesc;
    public string longDesc;
    public Collider inputArea;
    public MeshRenderer inputAreaDisplay;
	public ValidPlayArea validPlayArea;
	public Sprite abilitySprite;
	public int cooldown;
	public float castDuration;
	public float castDelay;
    public bool channel = true;
    public string animName;

	//STATE
	protected HeroUnit u;
	protected Vector3 position;
	protected bool triggered;
	protected float durationTimer;
	protected IEnumerator casting;
    public float CDTimer;

    //AI
    public Targeting AITargeting;
    public float AITriggerDistance;

    protected virtual void Awake()
	{
		u = GetComponent<HeroUnit>();
        if(u.type != UnitType.RaidBoss)
        {
            CDTimer = 0;
        }
        else
        {
            CDTimer = cooldown;
        }
	}

    protected virtual void Update()
    {
        if(CDTimer > 0)
        {
            CDTimer -= Time.deltaTime;
        }
    }

    public virtual void StartCast(Vector3 abilityPosition)
    {
        CDTimer = cooldown;
        if (abilityPosition != Vector3.up)
        {
            position = abilityPosition;
        }
        else
        {
            position = u.transform.position;
        }
        durationTimer = 0;
        triggered = false;

        if(channel)
        {
            StartChanneling();
        }
        else
        {
            Effect();
        }
    }

    protected virtual void StartChanneling()
    {
        if (PhotonNetwork.IsMasterClient)
		{
			u.nav.velocity = Vector3.zero;
			u.nav.isStopped = true;
		}
		//u.state = UnitState.Casting;
        u.anim.SetTrigger(animName);
        casting = Casting();
        StartCoroutine(casting);
    }

	public virtual IEnumerator Casting()
	{
		while (durationTimer < castDuration)
        {
            durationTimer += Time.deltaTime;
            UnitSystem.TurnTowardsEnemy(u, position);
            if (durationTimer > castDelay)
            {
                Effect();
            }
			yield return null;
		}

		//u.state = UnitState.Idle;
	}

	public virtual void Effect()
	{

	}

    public virtual bool Decision(BattleState s, Unit u, TargetInfo t, int index)
    {
        if (CDTimer <= 0)
        {
            TargetInfo abilityTarget = new TargetInfo();
            if (AITargeting == u.targeting || AITriggerDistance == -1)
            {
                abilityTarget = t;
            }
            else
            {
                //abilityTarget = StratType.ClosestEnemy(s, u, abilityTarget, AITargeting);
            }

            if (t.targetUnit != null && (AITriggerDistance == -1 || (AITriggerDistance + u.radius) > abilityTarget.distance))
            {
                return AfterDistanceDecision(s, u, abilityTarget, index);
            }
        }
        return false;
    }

    protected virtual bool AfterDistanceDecision(BattleState s, Unit u, TargetInfo t, int index)
    {
        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{

	}
}
