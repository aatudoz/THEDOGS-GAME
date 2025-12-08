using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUp : MonoBehaviour
{
    public PowerUpEffect effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (!collision.CompareTag("Player"))
            return;
        Destroy(gameObject); //poistaa powerupin kun se on kerätty
        effect.Apply(collision.gameObject);
    }
    
    
}
