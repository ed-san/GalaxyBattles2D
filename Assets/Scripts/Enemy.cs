using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Time.deltaTime * _enemySpeed * Vector3.down);
        
        // if enemy object is at the bottom of screen
        // respawn at top with a random X-Axis position
        if (transform.position.y <= -10.5f)
        { 
            float randomX = Random.Range(-9.46f,9.46f);
            transform.position = new Vector3(randomX, 10.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            //Damage player on collision
            Player player = other.transform.GetComponent<Player>();
            //Add a NULL check for component
            if (player != null)
            {
                player.Damage();
            }
            Destroy(this.gameObject);
        }
        
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
    
}
