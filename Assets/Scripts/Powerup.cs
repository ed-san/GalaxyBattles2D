using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerUpSpeed = 3.0f;
    [SerializeField] //ID "0" = TripleShot | "1" = Speed Boost | "2" = Shield
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
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    default:
                        Debug.Log("Undetected Powerup picked up!");
                        break;
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
