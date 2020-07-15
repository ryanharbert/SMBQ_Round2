using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    public virtual void Damage(Unit self, int damage)
    {
        self.health -= damage;
    }
}