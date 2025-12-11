using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/ScoreBuff")]
public class ScoreBuff : PowerUpEffect
{
    public int scoreAmount = 500;
    public float duration = 5f;

    public override void Apply(GameObject target)
    {
        // etsitään UIManager
        UIManager ui = Object.FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.AddScore(scoreAmount);
            ui.ShowPowerupText($"+{scoreAmount} SCORE", duration);
        }
    }
}