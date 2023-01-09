using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerUpSpeed = 3.0f;

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
                player.TripleShotActive();      
            }

            Destroy(this.gameObject);
        }
    }
    void PowerupMovement()
    {
        transform.Translate(Time.deltaTime * _powerUpSpeed * Vector3.down);
    }
}
