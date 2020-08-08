using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;

public class Unit : MonoBehaviourPun, IPunObservable
{
	//Display Data
	public float healthBarY;
	public Animator anim;
	public Animator passiveAnim;
	public float projectileHeight = 1;
    [HideInInspector] public AudioSource sound;

	//Unit Data
	public Faction faction;
	public bool flying = false;
	public UnitType type;
	public UnitSpeed speed;
	public UnitSize size;
	public int baseHealth;
	public float aggroRange;
	public float attackRange;
	public int baseAttack;
	public float attackDelay;
	public float attackLength;
	public Targeting targeting;

    public UnitAction attack;
    public UnitMove move;
    public UnitAbility[] abilities;
    public List<UnitBuff> buffs;
    public UnitAction onSummon;
    public UnitAction onDeath;
    public UnitTakeDamage onTakeDamage;







    //State
    [HideInInspector] public int state;
    [HideInInspector] public TargetInfo targetInfo;
    [HideInInspector] public int level;
	[HideInInspector] public int teamID;
    [HideInInspector] public bool AI;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int health;
    [HideInInspector] public int attackDamage;
	[HideInInspector] public Unit enemy;
	[HideInInspector] public float radius;
	[HideInInspector] public NavMeshAgent nav;
	[HideInInspector] public CapsuleCollider capsule;

    [HideInInspector] public HealthBar healthBar;

    [HideInInspector] public Vector3 masterPosition;
    [HideInInspector] public Quaternion masterRotation;

    int currentHealthCheck;
    int serializeCount;

    [PunRPC]
    public virtual void SetRPC(int teamID, int level)
	{
        this.teamID = teamID;
		this.level = level;

        AI = true;

        if(move == null)
        {
            move = gameObject.AddComponent<UnitMove>();
        }

        targetInfo = new TargetInfo();

        maxHealth = LevelandTypeBonus(baseHealth);
        health = maxHealth;
		attackDamage = LevelandTypeBonus(baseAttack);

        state = 0;

        if (anim != null && type == UnitType.Building && Battle.state.unitRotation != null)
        {
            if(teamID == 0)
            {
                anim.transform.rotation = Battle.state.unitRotation.rotation;
            }
            else
            {
                anim.transform.rotation = Quaternion.Inverse(Battle.state.unitRotation.rotation);
            }
        }

		GameObject go = Resources.Load<GameObject>("UI/HealthBar");
		healthBar = Instantiate(go, transform).GetComponent<HealthBar>();

		healthBar.SetHealthBar(level, health, baseHealth, teamID, healthBarY, false, type);
		currentHealthCheck = health;

        sound = GetComponent<AudioSource>();
		nav = GetComponent<NavMeshAgent>();
		capsule = GetComponent<CapsuleCollider>();

        switch (size)
		{
			case UnitSize.Tiny:
				radius = 0.25f;
				nav.avoidancePriority = 80;
                break;
			case UnitSize.Small:
				radius = 0.35f;
				nav.avoidancePriority = 60;
                break;
			case UnitSize.Medium:
				radius = 0.45f;
				nav.avoidancePriority = 40;
                break;
			case UnitSize.Large:
				radius = 0.65f;
				nav.avoidancePriority = 10;
                break;
			case UnitSize.Giant:
				radius = 1.0f;
				nav.avoidancePriority = 5;
				break;
			case UnitSize.Building:
				radius = 1.3f;
				nav.avoidancePriority = 3;
				break;
            case UnitSize.Stronghold:
                radius = 2f;
                nav.avoidancePriority = 2;
                break;
        }
        if(flying)
        {
            capsule.radius = radius / 2;
            nav.radius = radius / 2;
			nav.enabled = false;
        }
        else
        {
            capsule.radius = radius;
            nav.radius = radius;
        }
		switch (speed)
		{
			case UnitSpeed.Slow:
				nav.speed = 1.2f;
				break;
			case UnitSpeed.Normal:
				nav.speed = 1.8f;
				break;
			case UnitSpeed.Fast:
				nav.speed = 3.4f;
				break;
			case UnitSpeed.Woah:
				nav.speed = 5f;
				break;
			case UnitSpeed.Immobile:
				nav.speed = 0;
				break;
		}
        Battle.state.units.Add(this);
        
        masterPosition = transform.position;
        masterRotation = transform.rotation;
        serializeCount = 0;
        if (!PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }

        if (onSummon != null)
        {
            onSummon.Trigger(this, targetInfo, ActionType.OnSummon, 0);
        }
    }


    [PunRPC]
	public void UnitDied(bool animate)
    {
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
        Component.Destroy(nav);
        Component.Destroy(GetComponent<Collider>());
        Component.Destroy(GetComponent<Rigidbody>());
        if (Battle.state.hero[0] != null && Battle.state.hero[0] == this)
        {
            Battle.state.hero[0] = null;
        }
        else if(Battle.state.hero[1] != null && Battle.state.hero[1] == this)
        {
            Battle.state.hero[1] = null;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (onDeath != null)
            {
                onDeath.Trigger(this, targetInfo, ActionType.OnDeath, 0);
            }
            for (int i = 0; i < buffs.Count; i++)
            {
                RemoveBuff(buffs[i].name);
            }
        }
        if(animate)
        {
            if (anim != null)
            {
                anim.SetTrigger("Death");
            }
            Invoke("DestroyUnit", 2f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void DestroyUnit()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void AddBuff(string buff)
    {
        bool alreadyHaveBuff = false;
        for (int h = 0; h < buffs.Count; h++)
        {
            if (buffs[h].name == buff)
            {
                alreadyHaveBuff = true;
            }
        }
        if (!alreadyHaveBuff)
        {
            UnitBuff b = Instantiate(Resources.Load<GameObject>("Buffs/" + buff), transform).GetComponent<UnitBuff>();
            b.name = buff;
            b.Create(this);
        }
    }

    [PunRPC]
    public void RemoveBuff(string buff)
    {
        for(int i = 0; i < buffs.Count; i++)
        {
            if(buffs[i].name == buff)
            {
                buffs[i].End(this);
                Destroy(buffs[i].gameObject);
                buffs.RemoveAt(i);
            }
        }
    }

    [PunRPC]
    public void EnemyStronghold()
    {
        Battle.state.enemyObjectives.Add(this);
        PlayerNameAboveStronghold(PhotonNetwork.PlayerListOthers[0].NickName);
    }

    [PunRPC]
    public void RaidBoss()
    {
        Battle.state.enemyObjectives.Add(this);
    }

    [PunRPC]
    public void FriendlyStronghold()
    {
        //Battle.state.playerObjective = this;
        Battle.state.playerObjectives.Add(this);
        PlayerNameAboveStronghold(PhotonNetwork.PlayerListOthers[0].NickName);
    }

    public void PlayerNameAboveStronghold(string playerName)
    {
        GameObject go = Resources.Load<GameObject>("UI/PlayerName");
        GameObject playerNameObject = Instantiate(go, transform);
        Text playerNameText = playerNameObject.GetComponentInChildren<Text>();
        playerNameText.text = playerName;
        playerNameText.fontSize = 75;
        RectTransform r = playerNameObject.GetComponent<RectTransform>();
        r.anchoredPosition = new Vector2(r.anchoredPosition.x, healthBarY + 1.5f);
    }

    public int LevelandTypeBonus(int i)
	{
		float f = i;
		for (int h = 0; h < level; h++)
		{
			f = f * 1.1f;
		}

		if (teamID == 0 && type == UnitType.Unit && Battle.state.yourObjective != null && Battle.state.yourObjective.faction == faction)
		{
			i = Mathf.RoundToInt(f * 1.1f);
		}
		else
		{
            i = Mathf.RoundToInt(f);
		}

		return i;
    }

    [PunRPC]
    public void ResetCD(int index, int newCD)
    {
        abilities[index].CDTimer = newCD;
    }

    [PunRPC]
    public void TriggerAnimationRPC(int state)
	{
        if(anim != null)
        {
            if (state == 0)
            {
                anim.SetTrigger("Idle");
            }
            else if (state == 1)
            {
                anim.SetTrigger("Moving");
            }
            if (state == 2)
            {
                passiveAnim.SetTrigger("Stun");
            }
            else if (state == 3)
            {
                anim.SetTrigger("Attacking");
                if (sound != null)
                {
                    Invoke("PlaySound", attackDelay - 0.2f);
                }
            }
            else if (state > 3)
            {
                anim.SetTrigger("Cast" + (state - 3));
            }
        }
        
        if (passiveAnim != null && state == -1)
        {
            passiveAnim.SetTrigger("Open");
        }
    }

    [PunRPC]
    public virtual void AbilityDisplayPosition(ActionType type, int i, Vector3 position)
    {
        if (type == ActionType.Attack)
        {
            attack.PositionDisplay(position);
        }
        else if (type == ActionType.Ability)
        {
            abilities[i].action.PositionDisplay(position);
        }
        else if (type == ActionType.OnSummon)
        {
            onSummon.PositionDisplay(position);
        }
        else if (type == ActionType.OnDeath)
        {
            onDeath.PositionDisplay(position);
        }
        else if (type == ActionType.OnTakeDamage)
        {
            onTakeDamage.action.PositionDisplay(position);
        }
    }

    [PunRPC]
    public virtual void AbilityDisplayBool(ActionType type, int i, bool active)
    {
        if (type == ActionType.Attack)
        {
            attack.BoolDisplay(active);
        }
        else if (type == ActionType.Ability)
        {
            abilities[i].action.BoolDisplay(active);
        }
        else if (type == ActionType.OnSummon)
        {
            onSummon.BoolDisplay(active);
        }
        else if (type == ActionType.OnDeath)
        {
            onDeath.BoolDisplay(active);
        }
        else if (type == ActionType.OnTakeDamage)
        {
            if(onTakeDamage != null)
            {
                onTakeDamage.action.BoolDisplay(active);
            }
        }
    }

    [PunRPC]
    public virtual void SpawnProjectile(ActionType type, int i, int enemyID, Vector3 enemyPosition, int unitID)
    {
        if (type == ActionType.Attack)
        {
            attack.SpawnProjectile(enemyID, enemyPosition, unitID);
        }
        else if (type == ActionType.Ability)
        {
            abilities[i].action.SpawnProjectile(enemyID, enemyPosition, unitID);
        }
        else if (type == ActionType.BuffTick)
        {
            buffs[i].displayAction.SpawnProjectile(enemyID, enemyPosition, unitID);
        }
        else if (type == ActionType.OnSummon)
        {
            onSummon.SpawnProjectile(enemyID, enemyPosition, unitID);
        }
        else if (type == ActionType.OnDeath)
        {
            onDeath.SpawnProjectile(enemyID, enemyPosition, unitID);
        }
        else if (type == ActionType.OnTakeDamage)
        {
            onTakeDamage.action.SpawnProjectile(enemyID, enemyPosition, unitID);
        }
    }

    void PlaySound()
    {
        sound.Play();
    }

    public void TakeDamage(DamageData damage)
    {
        if(onTakeDamage.action == null)
        {
            health -= damage.amount;
        }
        else
        {
            onTakeDamage.Execute(this, damage);
        }
    }
    protected virtual void Awake()
    {
        if (type == UnitType.RaidBoss || type == UnitType.Building)
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i].CDTimer = abilities[i].cooldown + Random.Range(-abilities[i].randomness, abilities[i].randomness);
            }
        }
        else
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i].CDTimer = 0;
            }
        }
    }

    protected virtual void Update()
    {
        for (int h = 0; h < abilities.Length; h++)
        {
            if (abilities[h].CDTimer > 0)
            {
                abilities[h].CDTimer -= Time.deltaTime;
            }
        }

        if (health != currentHealthCheck && healthBar != null)
		{
			healthBar.UpdateHealth(health);
			currentHealthCheck = health;
		}

        if(!PhotonNetwork.IsMasterClient && !Battle.state.gameOver && type != UnitType.Building && nav != null)
        {
            transform.position = Vector3.Lerp(transform.position, masterPosition, Time.deltaTime * nav.speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, masterRotation, Time.deltaTime * 20);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (this == null)
            return;

        serializeCount++;

        if (stream.IsWriting)
        {
            stream.Serialize(ref health);

            masterPosition = transform.position;
            stream.Serialize(ref masterPosition);

            masterRotation = transform.rotation;
            stream.Serialize(ref masterRotation);
        }
        else
        {
            stream.Serialize(ref health);
            stream.Serialize(ref masterPosition);
            stream.Serialize(ref masterRotation);
        }

        if (serializeCount == 5)
            serializeCount = 0;
    }
}
