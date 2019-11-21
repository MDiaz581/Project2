using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    /** This Script will not be used, it may contain useful code for something, but not for the camera I wanted **/

    
    public Transform playerCam, character, centerPoint;

    private float centerpointYRotation;

    //Speed at which the camera will rotate when using right stick left and right
    public float rotationSpeed = 10f;

    //Moves centerpoint away from the player, this means the camera looks slightly above the player rather than at the player
    public Vector3 offset;

    //Angles Camera slightly
    public float cameraYpos;

    //Zoom variables, determines how far or close the camera is to the center point
    private float zoom;
    public float zoomSpeed = 2;
    public float zoomMin = -2f;
    public float zoomMax = -10f;


    void Start()
    {
        //Automatically set zoom to the minimum value
        zoom = zoomMin;
    }

    void LateUpdate()
    {
        //Takes the up and down of the right stick multiplies it by speed and time, down zooms our, up zooms out.
        zoom += Input.GetAxis("CameraZoom") * zoomSpeed * Time.deltaTime;

        //These prevent zoom from going past the max and min
        if(zoom > zoomMin)
        {
            zoom = zoomMin;
        }
        if (zoom < zoomMax)
        {
            zoom = zoomMax;
        }

        //Takes camera position and moves it on the Z position, we use the cameraYpos to angle it slightly
        playerCam.transform.localPosition = new Vector3(0, cameraYpos, zoom);

        //Changes the centerpoint's Y axis based off of the right stick's left and right movement
        centerpointYRotation += Input.GetAxis("CameraHorizontal") * rotationSpeed * Time.deltaTime;

        //Forces camera to constantly "Look at" the centerpoint
        playerCam.LookAt(centerPoint);

        //rotates the centerpoint based off of the centerpointYRotation and the player character's rotation
        centerPoint.localRotation = Quaternion.Euler(0, centerpointYRotation, 0) * character.rotation;

        //Moves the centerpoint with the player character's position + an offset
        centerPoint.position = new Vector3(character.position.x, character.position.y, character.position.z) + offset;

        //This restricts camera moving it clamping it at -60 to 60
        centerpointYRotation = Mathf.Clamp(centerpointYRotation, -60f - centerPoint.rotation.y, 60 + centerPoint.rotation.y); 


    }
}
