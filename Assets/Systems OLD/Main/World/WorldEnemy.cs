using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnemy : WorldCharacter
{
    public EnemyNode node { get; protected set; }
    public int level { get; protected set; }

    [SerializeField] private string displayName;
    [SerializeField] private float infoYRect = 2.5f;
    [SerializeField] private string engageAnim = "Engage";
    [SerializeField] private float engageDelay = 1.5f;

    int moveDelay;
    float moveTimer;
    EnemyInfo info;
    float engageTimer = 0;

    public void Setup(EnemyNode node, string name, int level)
    {
        this.node = node;
        this.name = name;
        this.level = level;

        Setup();
    }

    public override void Setup()
	{
		base.Setup();
		CreateInfoDisplay();
    }

    public void Engage(WorldPlayer player)
    {
        enemyCharacter = player;
        info.foundEnemy.enabled = true;
        info.nameText.gameObject.SetActive(false);
        anim.SetTrigger(engageAnim);
        Stop();
    }

    public bool Aggroed(WorldManager w)
    {
        if(w.currentInteractiveNode == null && Vector3.Distance(w.player.transform.position, transform.position) < 8)
        {
            return true;
        }
        return false;
    }

    public void Roam(WorldManager w)
    {
        moveTimer += Time.deltaTime;

        if (node.roam && moveTimer > moveDelay)
        {
            moveTimer = 0;

            float x = Random.Range(-5f, 5f);
            float z = Random.Range(-5f, 5f);

            Move(new Vector3(node.transform.position.x + x, node.transform.position.y, node.transform.position.z + z));

            moveDelay = Random.Range(2, 7);
        }
    }

    void CreateInfoDisplay()
    {
        GameObject enemyInfoGO = Resources.Load<GameObject>("UI/EnemyInfo");
        enemyInfoGO = Instantiate(enemyInfoGO, transform);
        info = enemyInfoGO.GetComponent<EnemyInfo>();
        info.SetEnemyInfo(level, displayName, infoYRect, name);

    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 6);
    }

    protected override void Update()
    {
        base.Update();
        if (enemyCharacter != null)
        {
            BattleImpending();
        }
    }

    void BattleImpending()
    {
        TurnTowardsEnemy(enemyCharacter.transform.position);

        if (Vector3.Distance(enemyCharacter.transform.position, transform.position) - Radius - enemyCharacter.Radius <= 0.5f)
        {
            if (!attacking)
            {
                AttackCharacter();
                Stop();
            }

            if(WorldManager.instance != null && WorldManager.instance.receivedBattleData)
            {
                WorldManager.instance.Invoke("EnterBattle", 2f);
            }
        }
        else if (engageTimer > engageDelay)
        {
            Move(enemyCharacter.transform.position);
        }
        else
        {
            engageTimer += Time.deltaTime;
        }
    }
}
