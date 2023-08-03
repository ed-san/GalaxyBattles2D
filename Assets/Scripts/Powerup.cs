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
    private Animator _anim;
    [SerializeField] 
    private AudioSource[] _audioSource;
    public bool attracted = false;
    private GameObject _player;
    private string[] _powerupNames = new string[] {"TripleShot", "Speed", "Shield", "Ammo", "Heal", "Homing"};
    private float attractSpeed = 5.0f;

    private void Start()
    {
        _audioSource = GetComponents<AudioSource>();
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Animator component is NULL!");
        }
        else
        {
            if (_powerupID < 6)
            {
                _anim.SetTrigger("Play" + _powerupNames[_powerupID] + "Powerup");
            }
        }
        
        if (_audioSource == null)
        {
            Debug.LogError("Enemy prefab doesn't have audio source component!");
        }
    }

    void Update()
    {
        if (attracted && _player != null)
        {
            Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
            transform.Translate(directionToPlayer * attractSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            PowerupMovement();
        }

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
                        player.HomingShotActive();
                        break;
                    case 6:
                        player.DrainEnergyActive();
                        break;
                    case 7:
                        player.TakeDamageActive();
                        break;
                    case 8:
                        player.SpecialShotActive();
                        break;
                    default:
                        Debug.Log("Undetected Powerup picked up!");
                        break;
                }

            }

            Destroy(this.gameObject);
        }
        
        
        if (other.CompareTag("Laser") && gameObject.layer == LayerMask.NameToLayer("Positive Powerup"))
        {
            // Check that the laser is from an enemy or boss
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // This if-statement cleans up the Laser object
                if (other != null)
                {
                    Debug.Log("Has Entered the Powerups Collision Block!");
                    Destroy(GetComponent<Collider2D>());
                    Destroy(other.gameObject);
                }

                _powerUpSpeed = 0;
                
                if (_powerupID == 8) // Special Shot Powerup Destroyed
                {
                    // Immediately destroy the GameObject for the Special Shot
                    Destroy(this.gameObject);
                    _audioSource[0].Play();
                }
                else
                {
                    // For other powerups, wait for 2.2 seconds before destroying
                    _audioSource[0].Play();
                    _anim.SetTrigger("DestroyPowerup");
                    Destroy(this.gameObject, 2.2f);
                }
            }

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

    public void SetAttract(bool attract, GameObject player)
    {
        attracted = attract;
        _player = player;
    }
    
}
