using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSystem : BaseSystem
{
	public override void Run(BattleState s)
	{
		for(int i = 0; i < s.projectiles.Count; i++)
		{
            if(s.projectiles[i] is AOEProjectile && s.projectiles[i].enemy == null)
            {
                if (Vector3.Distance(s.projectiles[i].transform.position, s.projectiles[i].enemyPos) < 0.5f)
                {
                    s.projectiles[i].Impact();
                    Destroy(s.projectiles[i].gameObject);
                    s.projectiles.RemoveAt(i);
                    i--;
                }
                else
                {
                    MoveProjectile(s, s.projectiles[i], s.projectiles[i].enemyPos);
                }
            }
            else
            {
                if (s.projectiles[i].enemy == null)
                {
                    Destroy(s.projectiles[i].gameObject);
                    s.projectiles.RemoveAt(i);
                    i--;
                }
                else if (Vector3.Distance(s.projectiles[i].transform.position, s.projectiles[i].enemy.transform.position) < s.projectiles[i].enemy.radius)
                {
                    s.projectiles[i].Impact();
                    Destroy(s.projectiles[i].gameObject);
                    s.projectiles.RemoveAt(i);
                    i--;
                }
                else
                {
                    MoveProjectile(s, s.projectiles[i], s.projectiles[i].enemy.transform.position);
                }
            }
        }
    }

    void MoveProjectile(BattleState s, Projectile p, Vector3 targetPosition)
    {
		if(!p.lob)
		{
			p.transform.position = Vector3.MoveTowards(p.transform.position, targetPosition, Time.deltaTime * p.speed);
			p.displayObject.transform.localPosition = new Vector3(0f, (Vector3.MoveTowards(p.displayObject.transform.position, targetPosition + new Vector3(0f, p.enemyHeight, 0f), Time.deltaTime * p.speed) - p.transform.position).y, 0);
		}
		else
		{
			p.transform.position = Vector3.MoveTowards(p.transform.position, targetPosition, Time.deltaTime * p.speed);
			float distance = Vector3.Distance(p.transform.position, p.sourcePos) / Vector3.Distance(targetPosition, p.sourcePos);
			if (distance < 0.5f)
			{
				//float y = (Vector3.Slerp(p.displayObject.transform.position, targetPosition + new Vector3(0f, p.lobHeight, 0f), Time.deltaTime * p.speed) - p.transform.position).y;
				//p.displayObject.transform.localPosition = new Vector3(0f, y, 0);

				//p.displayObject.transform.localPosition = Vector3.Slerp(p.displayObject.transform.localPosition, new Vector3(p.displayObject.transform.localPosition.x, p.lobHeight, p.displayObject.transform.localPosition.z), distance);

				p.displayObject.transform.localPosition = new Vector3(p.displayObject.transform.localPosition.x, Mathf.Lerp(p.sourceHeight, p.lobHeight * Vector3.Distance(p.sourcePos, p.enemyPos) / p.range, Mathf.Sqrt(2 * distance)), p.displayObject.transform.localPosition.z);
			}
			else
			{
				//float y = (Vector3.Slerp(p.displayObject.transform.position, targetPosition + new Vector3(0f, p.enemyHeight, 0f), Time.deltaTime * p.speed) - p.transform.position).y;
				//p.displayObject.transform.localPosition = new Vector3(0f, y, 0);

				//float y = Vector3.Slerp(p.displayObject.transform.localPosition, new Vector3(p.displayObject.transform.localPosition.x, p.lobHeight, p.displayObject.transform.localPosition.z), distance - 0.5f).y;
				//p.displayObject.transform.localPosition = p.displayObject.transform.localPosition - new Vector3(0, y, 0);

				//p.displayObject.transform.localPosition = Vector3.Slerp(p.displayObject.transform.localPosition, new Vector3(p.displayObject.transform.localPosition.x, p.enemyHeight, p.displayObject.transform.localPosition.z), distance - 0.5f);

				p.displayObject.transform.localPosition = new Vector3(p.displayObject.transform.localPosition.x, Mathf.Lerp(p.lobHeight * Vector3.Distance(p.sourcePos, p.enemyPos) / p.range, p.enemyHeight, (2 * (distance - 0.5f)) * (2 * (distance - 0.5f))), p.displayObject.transform.localPosition.z);
			}
		}
    }
}
