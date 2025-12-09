using UnityEngine;
using System.Collections; // Still needed for the reload coroutine

// HUGE THANKS: https://www.youtube.com/watch?v=chIFtkuFnVw

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private AudioClip ShootSoundClip;

    // NEW: Ammo and Reload Variables
    [Header("Ammo & Reload")]
    [SerializeField] private int maxMagAmmo = 12; // 12-bullet capacity
    [SerializeField] private float reloadTime = 1.5f;

    // Public properties/variables for the UIManager to read
    [HideInInspector] public int currentMagAmmo; // Hidden in Inspector, but publically readable
    [HideInInspector] public bool isReloading = false;
    public int MaxMagAmmo => maxMagAmmo; // Read-only access to max ammo
    public float ReloadTime => reloadTime; // Read-only access to reload time

    private Vector3 originalScale;


    void Start()
    {
        originalScale = transform.localScale;
        // NEW: Initialize ammo to full
        currentMagAmmo = maxMagAmmo;
    }

    void Update()
    {
        //Aim at mouse
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouse - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float rotation = transform.eulerAngles.z;
        if (rotation > 90f && rotation < 270f)
        {
            transform.localScale = new Vector3(
                originalScale.x,
                -Mathf.Abs(originalScale.y),
                originalScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                originalScale.x,
                Mathf.Abs(originalScale.y),
                originalScale.z
            );
        }

        //Shoot logic modified
        if (Input.GetMouseButtonDown(0))
        {
            // NEW: Only shoot if not reloading and magazine is NOT empty
            if (!isReloading && currentMagAmmo > 0)
            {
                Instantiate(bulletPrefab, muzzle.position, transform.rotation);
                currentMagAmmo--; // Decrease ammo

                // Play sound effect
                SoundFXManager.Instance.PlaySoundFXClip(ShootSoundClip, transform, 1f);

                // NEW: Auto-reload if magazine is now empty
                if (currentMagAmmo == 0)
                {
                    StartAutoReload();
                }
            }
        }
    }

    // NEW: Method to begin the reload process
    private void StartAutoReload()
    {
        if (isReloading) return;

        isReloading = true;

        // Start the timed reload
        StartCoroutine(ReloadCoroutine());
    }

    // NEW: Coroutine for the reload delay and refill
    private IEnumerator ReloadCoroutine()
    {
        // Wait for the specified reload time
        yield return new WaitForSeconds(reloadTime);

        // Refill the magazine
        currentMagAmmo = maxMagAmmo;

        isReloading = false;
    }
}