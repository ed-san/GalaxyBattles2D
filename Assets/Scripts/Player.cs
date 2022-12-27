using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Subsystems;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 8.0f;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        calculateMovement();
    }

    void calculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(Time.deltaTime * _speed * direction);
        
       // The code below restricts Y-Axis movement between the ranges of -3.8f and 3.0f creating a vertical boundary for player.
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 3.0f),0);
        
        /*
           This if block allows the player to wrap around the screen
           when going off-screen on the left or right side along the X-Axis.
        */
        
        if (transform.position.x >= 11.3f)
        {
            float xPosOffScreenRight = transform.position.x * -1;
            float yPosOffScreenRight = transform.position.y * 1;
            
            transform.position = new Vector3(xPosOffScreenRight, yPosOffScreenRight, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            float xPosOffScreenLeft = transform.position.x * -1;
            float yPosOffScreenLeft = transform.position.y * 1;
            
            transform.position = new Vector3(xPosOffScreenLeft,  yPosOffScreenLeft, 0);
        }
    }
    
}
