using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerUpSpeed = 3.0f;
    /*ID "0" = TripleShot | "1" = Speed Boost | "2" = Shield | "3" = Ammo | "4" = Heal | "5" = Special Shot
     | "6" = Drain | "7" = Take Damage
     */
    [SerializeField] 
    private int _powerupID;
    [SerializeField]
    private AudioClip _powerUpClip;
    

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
            AudioSource.PlayClipAtPoint(_powerUpClip, transform.position, 1.0f);

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
                        player.ShieldActive(); ;
                        break;
                    case 3:
                        player.AmmoReloadActive();
                        break;
                    case 4:
                        player.HealActive();
                        break;
                    case 5:
                        player.SpecialShotActive();
                        break;
                    case 6:
                        player.DrainEnergyActive();
                        break;
                    case 7:
                        player.TakeDamageActive();
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

    public int GetPowerupID()
    {
        return _powerupID;
    }
}
