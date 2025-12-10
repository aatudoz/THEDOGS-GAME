using UnityEngine;
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 3;
    [SerializeField] private float lifetime = 3f; //destroyed after 3s

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        //destroy bullet if game is paused
        if (Time.timeScale == 0f)
        {
            Destroy(gameObject);
            return;
        }

        //Move bullet
        transform.position += transform.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Hit enemy
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            BossController boss = other.GetComponent<BossController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (boss != null) 
            { 
                boss.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}