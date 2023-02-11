using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _projectileSpeed = 10.0f;

    void Update()
    {
        CalculateLaserMovement();
        
        // if Y-Axis position is greater than 9.5, destroy laser object
        if (transform.position.y >= 9.5f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    void CalculateLaserMovement()
    {
        transform.Translate(Time.deltaTime * _projectileSpeed * Vector3.up );
    }

}
