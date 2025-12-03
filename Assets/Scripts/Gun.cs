using UnityEngine;

// HUGE THANKS: https://www.youtube.com/watch?v=chIFtkuFnVw

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
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

        //Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bulletPrefab, muzzle.position, transform.rotation);
        }
    }
}