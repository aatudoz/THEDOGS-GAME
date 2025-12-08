using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "PowerUps/SpeedBuff")]
public class SpeedBuff : PowerUpEffect
{
    public float amount;
    public float duration = 10f;

    public override void Apply(GameObject target)
    {
        PlayerController pc = target.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.StartCoroutine(ApplySpeedBoost(pc));
        }
    }

    private IEnumerator ApplySpeedBoost(PlayerController pc)
    {
        pc.moveSpeed += amount;
        yield return new WaitForSeconds(duration);
        pc.moveSpeed -= amount; //palauta nopeus
    }
}
