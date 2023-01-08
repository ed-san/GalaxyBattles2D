using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerUpSpeed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PowerupMovement();
        if (transform.position.y < -7.0f)
        {
            Destroy(gameObject);
        }
        //move down at a speed of 3 and make this visible in inspector
        //destroy object when it leaves the screen
    }

    //OnTriggerCollision
    //be collectable by player
    //destroy after collected
    //update tripleShotActive to true 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.TripleShotActive();
               Destroy(this.gameObject);
            }
        }
    }
    void PowerupMovement()
    {
        transform.Translate(Time.deltaTime * _powerUpSpeed * Vector3.down);
    }
}
