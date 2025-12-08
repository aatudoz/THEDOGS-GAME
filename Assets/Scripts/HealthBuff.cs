using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "PowerUps/HealthBuff")]


public class HealthBuff : PowerUpEffect
{
    public float amount;
    public override void Apply(GameObject target)
    {
        var health = target.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.AddHealth((int)amount);
        }
    }
}
