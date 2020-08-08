using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldCharacter : MonoBehaviour
{
    public float Radius { get { return nav.radius; }}

    [SerializeField] protected string attackAnim = "Attacking";

    [SerializeField] protected UnityEngine.AI.NavMeshAgent nav;
    [SerializeField] protected Animator anim;

    protected WorldCharacter enemyCharacter;
    protected bool moving = false;
    protected bool attacking = false;

    public virtual void Setup()
	{
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		anim = GetComponentInChildren<Animator>();
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    protected virtual void Update()
    {
        if (moving == true && Vector3.Distance(transform.position, nav.destination) < 0.5f)
        {
            moving = false;
        }

        if (anim != null)
        {
            //anim.SetFloat("Speed", nav.desiredVelocity.magnitude);
            anim.SetFloat("Speed", nav.velocity.magnitude);
        }
    }

    public void Move(Vector3 destination)
    {
        nav.SetDestination(destination);
        nav.isStopped = false;
        moving = true;
    }

    public void Stop()
    {
        nav.SetDestination(transform.position);
        nav.isStopped = true;
        moving = false;
    }

    protected virtual void AttackCharacter()
	{
		anim.SetTrigger(attackAnim);
		attacking = true;
	}

    protected void TurnTowardsEnemy(Vector3 enemyPosition)
    {
        Vector3 pos = enemyPosition - transform.position;
        Quaternion newRot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 10f * Time.deltaTime);
    }
}
