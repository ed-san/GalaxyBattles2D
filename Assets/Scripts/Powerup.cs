using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerUpSpeed = 3.0f;

    //ID for powerups
    // 0 = tripleshot
    // 1 = speed
    // 2 = shield
    [SerializeField]
    private int _powerupID;

    // Update is called once per frame
    void Update()
    {
        PowerupMovement();
        if (transform.position.y < -7.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                if (_powerupID == 0)
                {
                    player.TripleShotActive();
                }
               else if (_powerupID == 1)
                {
                    Debug.Log("Player has picked up speed powerup!");
                }
                else if (_powerupID == 2)
                {
                    Debug.Log("Player has picked up a shield powerup!");
                }

            }

            Destroy(this.gameObject);
        }
    }
    void PowerupMovement()
    {
        transform.Translate(Time.deltaTime * _powerUpSpeed * Vector3.down);
    }
}
