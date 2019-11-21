using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Physics")]

    public Rigidbody rb;

    [Header("Speed Variables")]

    [Tooltip("Changes the max speed of the player")]
    public float maxSpeed;

    [Tooltip("Speed will decrease by this value every second")]
    public float deceleration;

    [Tooltip("Speed will increase by this value every second")]
    public float acceleration;

    //Keep public for testing purposes ie. watching deceleration and accelteration rate
    public float speed;

    private float translation;

    

    [Header("Handling Variables")]

    [Tooltip("Determines how fast the player will turn. Higher values increase the turn rate")]
    public float handling;

    private float initialHandling;

    private float driftHandling;

    [Tooltip("Determines how fast the player will turn while drifting. This is added to the handling value. Higher values increase the turn rate")]
    public float drift;

    private float horizontalRotation;


    [Header("Checking Bools")]
    public bool onGround;
    public bool moving;

    //Determines which direction the player is drifting in and locks them to it
    public bool isDriftingLeft;
    public bool isDriftingRight;



    [Header("Air Control Variables")]

    //Determines how fast you can rotate the player forward or backwards while in the air before being restricted
    [Tooltip("Determines how fast you can rotate the player forward or backwards while in the air before being restricted")]
    public float airControl;

    [Tooltip("Divides handling by this value while in the air. This reduces handling but still gives some control. The higher the value the less handling")]
    public float airHandling;

    private float rotationX;



    [Header("Rotation Restricters")]

    //This value is subtracted from 360 and added from 0 depending on which side of the rotatation you want. This is degrees.
    [Tooltip("Maximum Degrees on the Z axis the player can rotate")]
    public float maxZrotation;

    [Tooltip("Maximum Degrees on the X axis the player can rotate")]
    public float maxXrotation;

    //The smooth time of the Quaternion.Slerp for restricting rotation
    
    private float smoothTime = 0.1f;

    [Header("Local Rotations")]
    //We use this to take the local rotation of the object and apply it to a float which we can manipulate/restrict. See private void Update() on how it's stored

    private float eulerAngX;

    private float eulerAngY;

    private float eulerAngZ;

    [Header("Testing Bool")]
    //This is for testing purposes
    public bool onEnabled;

    // Start is called before the first frame update
    void Start()
    {
        initialHandling = handling;

        //Calculates the handling while drifting
        driftHandling = handling + drift;

    }

    private void Update()
    {

        //These check the local Eular angle it grabs value based off of 360 rotation not -180 to 180
        eulerAngX = transform.localEulerAngles.x;
        eulerAngY = transform.localEulerAngles.y;
        eulerAngZ = transform.localEulerAngles.z;

        //Debug.Log("Y " + eulerAngY);
        //Debug.Log("X " + eulerAngX);
        //Debug.Log("Z " + eulerAngZ);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        /* -------------------------------------------------------- Movement Manager -------------------------------------------------- */


        //This Drives
        if (Input.GetAxis("Drive") >= 1 && Input.GetAxis("Brake") < 1 && onGround)
        {
            Debug.Log("moving??");
            moving = true;
            if(speed < maxSpeed)
            {
                speed += acceleration * Time.deltaTime;
            }
        }
        //This Brakes and Reverses
        if (Input.GetAxis("Brake") >= 1 && onGround)
        {
            Debug.Log("Braked");

            moving = true;

            //Reversing is slower than driving forward therefore is calculated differently than normal deceleration
            if (speed > -maxSpeed / 4)
            {
                speed -= (acceleration - (acceleration / 10)) * Time.deltaTime;
            }
        }

        //This decelerates when neither driving or braking
        if (Input.GetAxis("Drive") < 1 && Input.GetAxis("Brake") < 1 || !onGround)
        {
            moving = false;

            //Decelaration Calculation while on ground
            if (onGround)
            {
                if (speed > 1)
                {
                    speed -= deceleration * Time.deltaTime;
                }
                if (speed < -1)
                {
                    speed += deceleration * Time.deltaTime;
                }
            }
            //Decelaration Calculation while in air
            else
            {
                if (speed > 1)
                {
                    speed -= deceleration/2 * Time.deltaTime;
                }
                if (speed < -1)
                {
                    speed += deceleration/2 * Time.deltaTime;
                }
            }

        }

        //This sets speed to 0 when speed is near the value and not moving
        //This is mainly to prevent the player from moving when idle
        if (speed > -1 && speed < 1 && !moving)
        {
            speed = 0;
        }


        //This float sets the speed to a new float this is necessary I don't remember why
        translation = speed;

        //This sets translation to be based of seconds not frames, again necessary even though speed is already * Time.deltaTime
        translation *= Time.deltaTime;

        //This moves the players transform to the new translation, which is its local Z axis, in other words forward.
        transform.Translate(0, 0, translation);

        /* -------------------------------------------------------- Movement Manager END-------------------------------------------------- */


        /* -------------------------------------------------------- Handling and Drift Manager -------------------------------------------------- */

        //If right trigger is pressed, then allow increased handling except while in air
        if (Input.GetAxis("Drift") >= 1 && onGround)
        {
            handling = driftHandling;
            if (Input.GetAxis("Horizontal") > 0 && !isDriftingLeft && speed > 0)
            {
                isDriftingRight = true;
            }
            if (Input.GetAxis("Horizontal") < 0  && !isDriftingRight && speed > 0)
            {
                isDriftingLeft = true;

            }

        }
        //else we set it back to it's initial handling
        else
        {
            handling = initialHandling;
            isDriftingLeft = false;
            isDriftingRight = false;
        }
        //Only allow the player to turn if speed is > 1 or -1 OR if right trigger is pressed
        if (Mathf.Abs(speed) > 0 || Input.GetAxis("Drift") >= 1)
        {
            //Handles normally if in the air
            if (onGround)
            {
                //This float adds a value depending on the left sticks left (-1) and right position (1) and multiply by handling.
                horizontalRotation = Input.GetAxis("Horizontal") * handling;
            }
            //Handling is severly reduced while in air, but still gives slight control.
            else
            {
                horizontalRotation = (Input.GetAxis("Horizontal") * handling) / airHandling;
            }


            //This sets that rotation to be based on time rather than frames
            horizontalRotation *= Time.deltaTime;

            //This actually rotates the player on the Y axis determined by the float.
            if (isDriftingRight)
            {
                transform.Rotate(0, horizontalRotation/2 + 2, 0);
            }
            else if (isDriftingLeft)
            {
                transform.Rotate(0, horizontalRotation/2 - 2, 0);
            }
            else
            {
                transform.Rotate(0, horizontalRotation, 0);
            }
            

        }
        /* -------------------------------------------------------- Handling and Drift Manager END -------------------------------------------------- */
        
        
        /* -------------------------------------------------------- Air Control Manager -------------------------------------------------- */

        if (!onGround)
        {
            //This allows you to tilt forward or backwards while in the air.
            rotationX = (Input.GetAxis("Vertical") * airControl) * Time.deltaTime;

            transform.Rotate(rotationX, 0, 0);

            
        }


        /* -------------------------------------------------------- Air Control Manager END-------------------------------------------------- */



        /*       ------------------------------------------ Rotation Manager ------------------------------------------------------------  */


        //This grabs the transform.localEulerAngles.z and checks if its the right half of the rotation
        if (eulerAngZ > 180 && eulerAngZ < 360 - maxZrotation)
        {

            Debug.Log("transform.localEulerAngles.Z > 180" + " &&  < " + (360 - maxZrotation));

            Quaternion desiredRotation = Quaternion.Euler(eulerAngX, eulerAngY, -maxZrotation);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothTime);
        }

        //This grabs the transform.localEulerAngles.z and checks if its the left half of the rotation
        if (eulerAngZ > maxZrotation && eulerAngZ < 180)
        {
            Debug.Log("transform.localEulerAngles.Z > " + maxZrotation + " && < 180 ");

            //This rotates the player to the desired rotation which is whatever its local x rotation and local Y rotation so it doesn't return those to 0.
            Quaternion desiredRotation = Quaternion.Euler(eulerAngX, eulerAngY, maxZrotation);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothTime);
        }

        //This grabs the transform.localEulerAngles.x and checks if its the upper half of the rotation
        if (eulerAngX > 180 && eulerAngX < 360 - maxXrotation)
        {

            Debug.Log("transform.localEulerAngles.x > 180" + " &&  < " + (360 - maxXrotation));

            Quaternion desiredRotation = Quaternion.Euler(-maxXrotation, eulerAngY, eulerAngZ);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothTime);
        }

        //This grabs the transform.localEulerAngles.x and checks if its the bottom half of the rotation
        //This allows you to tilt downwards slightly more than tilting upwards
        if (eulerAngX > maxXrotation + 10 && eulerAngX < 180)
        {
            Debug.Log("transform.localEulerAngles.x > " + (maxXrotation + 10) + " && < 180 ");

            //This rotates the player to the desired rotation which is whatever its local Y rotation and local Z rotation so it doesn't return those to 0.
            Quaternion desiredRotation = Quaternion.Euler(maxXrotation, eulerAngY, eulerAngZ);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothTime);
        }
        
        /*       ------------------------------------------Rotation Manager END----------------------------------                */


    }



    //Collisions

    private void OnCollisionStay(Collision collision)
    {     
        if (collision.gameObject.tag == "Ground")
        {
            
            if (collision.transform.position.y < transform.position.y)
            {
                //Checks if the player is on the ground, affects whether
                onGround = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
           
            onGround = false;
        }
    }

    //Here we're going to check the X, Y, and Z directions and if there's a collision at that area we're going
    //to force the player in the opposite direction, like a bounce.
    //This is to prevent the player from getting stuck on walls, and instead of going full stop you'll just slow down.
    private void OnCollisionEnter(Collision collision)
    {
/**
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("Entered");


                Quaternion desiredRotation = Quaternion.Euler(0, eulerAngY, 0);

                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, 1f);
            
        }
    **/
        //This is forward
        if (collision.transform.position.z > transform.position.z)
        {
            if (collision.gameObject.tag != "Ground")
            {

                translation = -translation;

            }
        }
        
    }
}
