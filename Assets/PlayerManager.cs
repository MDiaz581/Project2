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

    public float Deceleration;
    public float acceleration;

    public bool moving;

    private float rotation;

    float rotationAngle = 0f;
    float smoothTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        initialHandling = handling;

        driftHandling = handling * drift;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //This Drives
        if (Input.GetAxis("Drive") >= 1 && Input.GetAxis("Brake") < 1)
        {
            Debug.Log("moving??");
            moving = true;
            if(speed < maxSpeed)
            {
                speed += acceleration * Time.deltaTime;
            }
        }
        //This Brakes and Reverses
        if (Input.GetAxis("Brake") >= 1)
        {
            Debug.Log("Braked");

            moving = true;

            if (speed > -maxSpeed / 4)
            {
                speed -= (acceleration - (acceleration / 4)) * Time.deltaTime;
            }
        }

        //This decelerates when neither driving or braking
        if (Input.GetAxis("Drive") < 1 && Input.GetAxis("Brake") < 1)
        {
            moving = false;
            if (speed > 1)
            {
                speed -= Deceleration * Time.deltaTime;
            }
            if (speed < -1)
            {
                speed += Deceleration * Time.deltaTime;
            }

        }

        //This sets speed to 0 when speed is near the value and not moving
        //This is mainly to prevent the player from moving when idle
        if (speed > -1 && speed < 1 && !moving)
        {
            speed = 0;
        }


        //If right trigger is pressed, then allow increased handling
        if (Input.GetAxis("Drift") >= 1)
        {
            handling = driftHandling;
        }
        //else we set it back to it's initial handling
        else
        {
            handling = initialHandling;
        }
        //Only allow the player to turn if speed is > 1 or -1 OR if right trigger is pressed
        if(Mathf.Abs(speed) > 0 || Input.GetAxis("Drift") >= 1)
        {
            //This float adds a value depending on the left sticks left (-1) and right position (1) and multiply by handling.
            rotation = Input.GetAxis("Horizontal") * handling;

            //This sets that rotation to be based on time rather than frames
            rotation *= Time.deltaTime;

            //This actually rotates the player on the Y axis determined by the float.
            transform.Rotate(0, rotation, 0);
        }

       /** if (Mathf.Abs(transform.rotation.z) > 0)
        {
            Debug.Log("Transform.Z > 0");
            Quaternion desiredRotation = Quaternion.Euler(0, 1, rotationAngle);

            //This defaults the character's position to 0 which I don't want.
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothTime);
        } **/

        //This float sets the speed to a new float this is necessary I don't remember why
        float translation = speed;

        //This sets translation to be based of seconds not frames, again necessary even though speed is already * Time.deltaTime
        translation *= Time.deltaTime;

        //This moves the players transform to the new translation, which is its local Z axis, in other words forward.
        transform.Translate(0, 0, translation);
    }

    //Here we're going to want the player to control the character only when touching the ground.
    //We're also going to want the player's speed to decelerate slower while in the air.
    private void OnCollisionStay(Collision collision)
    {
        
    }

    //Here we're going to check the X, Y, and Z directions and if there's a collision at that area we're going
    //to force the player in the opposite direction, like a bounce.
    //This is to prevent the player from getting stuck on walls, and instead of going full stop you'll just slow down.
    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
