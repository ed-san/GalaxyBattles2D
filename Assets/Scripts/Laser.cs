using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _projectileSpeed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        CalculateLaserMovement();
        
        // if Y-Axis position is greater than 9.5, destroy laser object
        if (transform.position.y >= 9.5f)
        {
            if (gameObject.CompareTag("Laser"))
            {
                Destroy(gameObject);
            }

            if (transform.parent.CompareTag("TripleShot"))
            {
                Destroy(transform.parent.gameObject);
            } 
   
           
            
        }
    }

    void CalculateLaserMovement()
    {
        transform.Translate(Time.deltaTime * _projectileSpeed * Vector3.up );
    }
    
}
