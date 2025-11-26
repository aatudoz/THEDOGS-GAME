using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 300f;

    void Update()
    {
        //Move bullet
        transform.position += transform.right * speed * Time.deltaTime;
    }

    void OnBecameInvisible()
    {
        //Destroy bullet when it goes off screen
        Destroy(gameObject);
    }
}