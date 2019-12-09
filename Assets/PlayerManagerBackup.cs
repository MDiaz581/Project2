using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManagerBackup : MonoBehaviour
{
    [Header("Physics")]

    public Rigidbody rb;

    public GetSideHit triggerScript;

    public GetLeftHit leftTriggerScript;

    public GetRightHit rightTriggerScript;

    


    public ParticleSystem psLeft;

    public ParticleSystem psRight;

    public ParticleSystem psBoost;

    public int currentCheckpoint;

    public int currentLap;

    public int maxLap;

    public Text LapText;

    [Header("Speed Variables")]

    [Tooltip("Changes the max speed of the player")]
    public float maxSpeed;


    [Tooltip("Speed will decrease by this value every second")]
    public float deceleration;

    [Tooltip("Speed will increase by this value every second")]
    public float acceleration;

    [Tooltip("This cannot be changed, this just for testing purposes")]
    //Keep public for testing purposes ie. watching deceleration and accelteration rate
    public float speed;

    private float translation;

    [Tooltip("This changes how fast you decelerate when braking by multiplying the value. A higher number allows you to brake faster. This value should be greater than 0.")]
    public float brakeRate;

    public float bounce;

    private float bounceTranslation;

    public bool rightActive;

    public bool leftActive;



    [Header("Boost Variables")]
    [Tooltip("Time in seconds you must drift before the player can boost")]
    public float driftTimer;

    public float boostSpeed;

    public bool driftBoosting;

    public bool isBoosting;

    private bool addSpeed;

    public float boostDuration;

    private float initialBoostDuration;

    private float extraSpeed;

    private float initialBoostTimer;

    [Header("Handling Variables")]

    [Tooltip("Determines how fast the player will turn. Higher values increase the turn rate")]
    public float handling;

    private float initialHandling;

    //This is the float that stores the calculation for handling + drift 
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

    [Tooltip("Divides handling by this value while in the air. This reduces handling but still gives some control. Handling in the air is divided by this value so the higher the value the less handling. Values between 0 and 1 increase handling")]
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

    [Header("Controller Number")]
    public int playerNumber;

    [Header("Testing Bool")]
    //This is for testing purposes
    public bool onEnabled;

    
    

    // Start is called before the first frame update
    void Start()
    {
        initialHandling = handling;

        initialBoostTimer = driftTimer;

        initialBoostDuration = boostDuration;

        //Calculates the handling while drifting
        driftHandling = handling + drift;

        driftBoosting = false;

        currentLap = 1;

    }

    private void Update()
    {
        //These check the local Eular angle it grabs value based off of 360 rotation not -180 to 180
        eulerAngX = transform.localEulerAngles.x;
        eulerAngY = transform.localEulerAngles.y;
        eulerAngZ = transform.localEulerAngles.z;

        LapText.text = ("Lap: " + currentLap.ToString() + " / " + maxLap.ToString());

        //Debug.Log("Y " + eulerAngY);
        //Debug.Log("X " + eulerAngX);
        //Debug.Log("Z " + eulerAngZ);

        /* -------------------------------------------------------- Bounce Manager -------------------------------------------------- */

        if (triggerScript.triggered)
        {
            Debug.Log("Hit Front");
            speed = -speed/2f - 5;
        }


        if (rightTriggerScript.rightTriggered)
        {
            bounce -= (acceleration * Mathf.Abs(speed)) * Time.deltaTime;

            speed = speed / 1.25f;

            Debug.Log("Hit Right");

            rightActive = true;
        }
        else
        {
            if (bounce < 0 && rightActive)
            {
                bounce += (deceleration * 3) * Time.deltaTime;
            }
            else if (bounce >= 0 && rightActive)
            {
                bounce = 0;

                rightActive = false;
            }

        }
        

        if (leftTriggerScript.leftTriggered)
        {

            bounce += (acceleration * Mathf.Abs(speed)) * Time.deltaTime;

            speed = speed / 1.25f;

            Debug.Log("Hit left");

            leftActive = true;
        }
        else
        {
            if (bounce > 0 && leftActive)
            {
                bounce -= (deceleration * 3) * Time.deltaTime;
            }
            else if (bounce <= 0 && leftActive)
            {
                bounce = 0;
                leftActive = false;
            }

        }

        if (bounce < -20)
        {
            Debug.Log("Hit - Max");

            bounce = -20;
        }

        if (bounce > 20)
        {
            Debug.Log("Hit + Max");

            bounce = 20;
        }




        /* -------------------------------------------------------- Bounce Manager END -------------------------------------------------- */

        //This checks if the player is boosting, if so it boosts.
        if (isBoosting)
        {
            Boost();
        }

        //This prevents the player from going over their max speed
        if(speed > maxSpeed + extraSpeed && isBoosting)
        {
            speed = maxSpeed + extraSpeed;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {



        /* -------------------------------------------------------- Movement Manager -------------------------------------------------- */

        //This Drives
        if (Input.GetAxis("Drive" + playerNumber) >= 1 && Input.GetAxis("Brake" + playerNumber) < 1 && onGround)
        {
            
            moving = true;
            if(speed < maxSpeed + extraSpeed)
            {
                speed += (acceleration) * Time.deltaTime;
            } else
            {
                speed -= deceleration * Time.deltaTime;
            }
        }
        //This Brakes and Reverses
        if (Input.GetAxis("Brake" + playerNumber) >= 1 && onGround)
        {

            moving = true;

            //Reversing is slower than driving forward therefore is calculated differently than normal deceleration
            if (speed > -maxSpeed / 4)
            {
               // speed -= (acceleration - (acceleration / 1000)) * Time.deltaTime;
                speed -= (deceleration * brakeRate) * Time.deltaTime;
            }
        }

        //This decelerates when neither driving or braking
        if (Input.GetAxis("Drive" + playerNumber) < 1 && Input.GetAxis("Brake" + playerNumber) < 1 || !onGround)
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

        bounceTranslation = bounce;

        bounceTranslation *= Time.deltaTime;

        //This float sets the speed to a new float this is necessary I don't remember why
        translation = speed;

        //This sets translation to be based of seconds not frames, again necessary even though speed is already * Time.deltaTime
        translation *= Time.deltaTime;

        //This moves the players transform to the new translation, which is its local Z axis, in other words forward.
        transform.Translate(bounceTranslation, 0, translation);

        /* -------------------------------------------------------- Movement Manager END-------------------------------------------------- */


        /* -------------------------------------------------------- Handling and Drift Manager -------------------------------------------------- */

        //If right trigger is pressed, then allow increased handling except while in air
        if (Input.GetAxis("Drift" + playerNumber) >= 1 && onGround)
        {
            handling = driftHandling;
            if (Input.GetAxis("Horizontal" + playerNumber) > 0 && !isDriftingLeft && speed > 0)
            {
                
                isDriftingRight = true;


            }
            if (Input.GetAxis("Horizontal" + playerNumber) < 0  && !isDriftingRight && speed > 0)
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

        //Prevents the ability to drift if not moving
        if(speed <= 0)
        {
            isDriftingLeft = false;
            isDriftingRight = false;
        }

        //Here we'll handle the drift boost

        //If the player is drifting left or right, start a timer.
        if (isDriftingRight)
        {   
            //This timer is based on how hard you push towards the drift direction, in this case right.
            if(Input.GetAxis("Horizontal" + playerNumber) > 0.2f)
            {
                
                driftTimer -= (1.5f + Input.GetAxis("Horizontal" + playerNumber)) * Time.deltaTime;


            } else
            {

                driftTimer -= Time.deltaTime;
            }

        }
        if (isDriftingLeft)
        {
            //This timer is based on how hard you push towards the drift direction, in this case left.
            if (Input.GetAxis("Horizontal" + playerNumber) < -0.2f)
            {

                driftTimer -= (1.5f - Input.GetAxis("Horizontal" + playerNumber)) * Time.deltaTime;

            }
            else
            {
            driftTimer -= Time.deltaTime;
            }

        }
        
        //if that timer is 0 the player can now drift
        if (driftTimer <= 0)
        {
            Debug.Log("CanDrift");

            //The player now must let go of the drift and be on the ground to actually boost.
            if (Input.GetAxis("Drift" + playerNumber) == 0 && onGround)
            {

                //It'll tell the game that it is now drift boosting 
                driftBoosting = true;

                //It'll reset the boost duration so the player can chain boost
                boostDuration = initialBoostDuration;


            }
        }

        if (Input.GetAxis("Drift" + playerNumber) == 0 && onGround)
        {
            //This resets the drift timer if you let go of right trigger
            driftTimer = initialBoostTimer;
        }


        if (driftBoosting)
        {
            isBoosting = true;
        }
        

        //Only allow the player to turn if speed is > 1 or -1 OR if right trigger is pressed
        if (Mathf.Abs(speed) > 0 || Input.GetAxis("Drift" + playerNumber) >= 1)
        {
            //Handles normally if on the ground
            if (onGround)
            {
                //This float adds a value depending on the left sticks left (-1) and right position (1) and multiply by handling.
                horizontalRotation = Input.GetAxis("Horizontal" + playerNumber) * handling;
            }
            //Handling is severly reduced while in air, but still gives slight control.
            else
            {
                horizontalRotation = (Input.GetAxis("Horizontal" + playerNumber) * handling) / airHandling;
            }


            //This sets that rotation to be based on time rather than frames
            horizontalRotation *= Time.deltaTime;

            //This actually rotates the player on the Y axis determined by the float.
            if (isDriftingRight)
            {
                transform.Rotate(0, horizontalRotation/2 + 1.30f, 0);



                //This changes the particle Color based on the boost timer
                if (driftTimer <= 0)
                {
                    var emitParams = new ParticleSystem.EmitParams();

                    emitParams.startColor = Color.red;

                    psRight.Emit(emitParams, 1);
                }
                else
                {
                    var emitParams = new ParticleSystem.EmitParams();

                    emitParams.startColor = Color.white;

                    psRight.Emit(emitParams, 1);
                }
            }
            else if (isDriftingLeft)
            {
                transform.Rotate(0, horizontalRotation/2 - 1.30f, 0);


                //This changes the particle Color based on the boost timer
                if (driftTimer <= 0)
                {
                    var emitParams = new ParticleSystem.EmitParams();

                    emitParams.startColor = Color.red;

                    psLeft.Emit(emitParams, 1);
                }
                else
                {
                    var emitParams = new ParticleSystem.EmitParams();

                    emitParams.startColor = Color.white;

                    psLeft.Emit(emitParams, 1);
                }

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
            rotationX = (Input.GetAxis("Vertical" + playerNumber) * airControl) * Time.deltaTime;

            transform.Rotate(rotationX, 0, 0);

            
        }


        /* -------------------------------------------------------- Air Control Manager END-------------------------------------------------- */



        /*       ------------------------------------------ Rotation Manager ------------------------------------------------------------  */


        //This grabs the transform.localEulerAngles.z and checks if its the right half of the rotation
        if (eulerAngZ > 180 && eulerAngZ < 360 - maxZrotation)
        {

           //Debug.Log("transform.localEulerAngles.Z > 180" + " &&  < " + (360 - maxZrotation));

            Quaternion desiredRotation = Quaternion.Euler(eulerAngX, eulerAngY, -maxZrotation);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothTime);
        }

        //This grabs the transform.localEulerAngles.z and checks if its the left half of the rotation
        if (eulerAngZ > maxZrotation && eulerAngZ < 180)
        {
           //Debug.Log("transform.localEulerAngles.Z > " + maxZrotation + " && < 180 ");

            //This rotates the player to the desired rotation which is whatever its local x rotation and local Y rotation so it doesn't return those to 0.
            Quaternion desiredRotation = Quaternion.Euler(eulerAngX, eulerAngY, maxZrotation);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothTime);
        }

        //This grabs the transform.localEulerAngles.x and checks if its the upper half of the rotation
        if (eulerAngX > 180 && eulerAngX < 360 - maxXrotation)
        {

            //Debug.Log("transform.localEulerAngles.x > 180" + " &&  < " + (360 - maxXrotation));

            Quaternion desiredRotation = Quaternion.Euler(-maxXrotation, eulerAngY, eulerAngZ);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothTime);
        }

        //This grabs the transform.localEulerAngles.x and checks if its the bottom half of the rotation
        //This allows you to tilt downwards slightly more than tilting upwards
        if (eulerAngX > maxXrotation + 10 && eulerAngX < 180)
        {
            //Debug.Log("transform.localEulerAngles.x > " + (maxXrotation + 10) + " && < 180 ");

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

                //Checks if the player is on the ground, this affects the players deceleration, and ability to move.
                onGround = true;

        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
           
            onGround = false;
        }
    }


    private void OnTriggerEnter(Collider target)
    {
        if (target.tag == "Boost")
        {
            boostDuration = initialBoostDuration;

            isBoosting = true;

        }

        checkpointTrigger pos = target.GetComponent<checkpointTrigger>();

        if (pos != null)
        {
            
            if(pos.position > currentCheckpoint && pos.position < currentCheckpoint + 2)
            {
                currentCheckpoint = pos.position;
                Debug.Log("Hit " + pos.position);
                Debug.Log("You are now " + currentCheckpoint);

            } else if(currentCheckpoint == 5 && pos.position == 0)
            {
                Debug.Log("Hit " + pos.position);
                Debug.Log("Lapped!");

                
                currentLap += 1;

                currentCheckpoint = pos.position;
            }
        }
        
    }

    private void OnTriggerStay(Collider target)
    {
        if (target.tag == "Boost")
        {
            if (!isBoosting)
            {
                boostDuration = initialBoostDuration;

                isBoosting = true;
            }

        }
    }

    //This function handles the boost mechanics
    private void Boost()
    {   

        extraSpeed = boostSpeed;

        if (boostDuration >= initialBoostDuration) 
        {
            psBoost.Emit(50);

            if (driftBoosting)
            {
                speed += boostSpeed/2;
            } else
            {
                speed += (maxSpeed/2) + boostSpeed;
            }

            
        }
            
        boostDuration -= Time.deltaTime;

        if (boostDuration <= 0)
        {

            extraSpeed = 0;

            isBoosting = false;

            driftBoosting = false;

            boostDuration = initialBoostDuration;
        }
        
    }

}
