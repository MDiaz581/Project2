using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public float maxSpeed;
    public float handling = 50.0f;
    private float initialHandling;
    public float driftHandling;
    public float drift;

    public float slowDown;

    // Start is called before the first frame update
    void Start()
    {
        initialHandling = handling;

        driftHandling = handling * drift;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("Submit") >= 1)
        {
            Debug.Log("Pressed");
            if(speed < maxSpeed)
            {
                speed += 30 * Time.deltaTime;
            }
        } else
        {
            Debug.Log("let go");
            if (speed > 0)
            {
                speed -= 20 * Time.deltaTime;
            }
        }
        if(speed < 0)
        {
            speed = 0;
        }

        if(Input.GetAxis("Drift") >= 1)
        {
            handling = driftHandling;
        }
        else
        {
            handling = initialHandling;
        }

        if(speed > 0 || Input.GetAxis("Drift") >= 1)
        {
            float rotation = Input.GetAxis("Horizontal") * handling;

            rotation *= Time.deltaTime;

            transform.Rotate(0, rotation, 0);
        }

        float translation = speed;

        translation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }
}
