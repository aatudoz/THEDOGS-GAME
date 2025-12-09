using UnityEngine;
using System.Collections;
// HUGE THANKS: https://www.youtube.com/watch?v=chIFtkuFnVw
public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private AudioClip ShootSoundClip;
    [Header("Ammo & Reload")]
    [SerializeField] private int maxMagAmmo = 12; //bullet capacity
    [SerializeField] private float reloadTime = 1.5f;
    [HideInInspector] public int currentMagAmmo;
    [HideInInspector] public bool isReloading = false;
    public int MaxMagAmmo => maxMagAmmo;
    public float ReloadTime => reloadTime;
    private Vector3 originalScale;
    void Start()
    {
        originalScale = transform.localScale;
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

        //manual reload with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            //only reload if not already reloading and magazine is not full
            if (!isReloading && currentMagAmmo < maxMagAmmo)
            {
                StartReload();
            }
        }

        //Shoot logic modified
        if (Input.GetMouseButtonDown(0))
        {
            if (!isReloading && currentMagAmmo > 0)
            {
                Instantiate(bulletPrefab, muzzle.position, transform.rotation);
                currentMagAmmo--; // Decrease ammo
                // Play sound effect
                SoundFXManager.Instance.PlaySoundFXClip(ShootSoundClip, transform, 1f);
                //Auto-reload if magazine is now empty
                if (currentMagAmmo == 0)
                {
                    StartReload();
                }
            }
        }
    }
    private void StartReload()
    {
        if (isReloading) return;
        isReloading = true;
        StartCoroutine(ReloadCoroutine());
    }
    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        currentMagAmmo = maxMagAmmo;
        isReloading = false;
    }
}