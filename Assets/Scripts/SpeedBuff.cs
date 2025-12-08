using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/SpeedBuff")]
public class SpeedBuff : PowerUpEffect
{
    public float amount;
    public float duration = 10f;

    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().moveSpeed += amount;
    }
}
