using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        /*
           All three lines of code below function the same but the last
           two are more efficient than the first. Mainly due to the order of multiplications.
           The last two don't have to execute as many multiplications over the x,y,z values of
           the Vector3 object. Otherwise you're multiplying each value by 5 instead of just the
           desired X value to move across the left/right planes.
        */
        //transform.Translate(Vector3.right * 5 * Time.deltaTime); 
        //transform.Translate(new Vector3(5 * Time.deltaTime,0,0));
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        //transform.Translate( Time.deltaTime * horizontalInput *_speed * Vector3.right);
        //transform.Translate(Time.deltaTime * verticalInput * _speed * Vector3.up);
        transform.Translate(Time.deltaTime * _speed * direction);
        
    }
}
