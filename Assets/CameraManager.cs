using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Transform playerCam, character, centerPoint;

    //Moves centerpoint away from the player, this means the camera looks slightly above the player rather than at the player
    public Vector3 offset;

    //Angles Camera slightly due to the LookAt function
    public float cameraYpos;

    //Speed at which the camera smooths the rotation
    public float speed = 5f;


    void LateUpdate()
    {

        //Takes camera position and moves it on the Z position, we use the cameraYpos to angle it slightly
        playerCam.transform.localPosition = new Vector3(0, cameraYpos, -8);

        //Forces camera to constantly "Look at" the centerpoint
        playerCam.LookAt(centerPoint);

        //Rotates the centerpoint to the same rotation as the player, but making it buttery smooth
        centerPoint.localRotation = Quaternion.Lerp(centerPoint.rotation, character.rotation, Time.deltaTime * speed);

        //Moves the centerpoint with the player character's position + an offset
        centerPoint.position = new Vector3(character.position.x, character.position.y, character.position.z) + offset;


    }
}
