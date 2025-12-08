using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 2f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;
    [SerializeField] private AudioClip LoseSound;
    [SerializeField] private AudioClip PlayerDamageSound;
    private SpriteRenderer spriteRenderer;

    //ui manager
    private UIManager uiManager;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        //hakee ui managerin
        uiManager = FindFirstObjectByType<UIManager>();

        //paivita sydamet alussa
        uiManager.UpdateHearts(currentHealth);
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;

            //Flicker effect
            spriteRenderer.enabled = (Time.time * 10) % 2 < 1;

            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                spriteRenderer.enabled = true;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        //paivita sydamet
        uiManager.UpdateHearts(currentHealth);
        //Sound FX for damage taken
        SoundFXManager.Instance.PlaySoundFXClip(PlayerDamageSound, transform, 1f);




        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Start invincibility
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    void Die()
    {
        //nayttaa deathscreenin uimanagerin kautta
        uiManager.ShowDeathScreen();

        //Sound FX
        SoundFXManager.Instance.PlaySoundFXClip(LoseSound, transform, 1f);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}