using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerCam, character, centerPoint;

    private float mouseX, mouseY;
    public float mouseSensitivity = 10f;
    public Vector3 offset;
    public Vector3 characterRotation;
    public Quaternion characterQuaternion;
    public float characterRotationX;
    public float characterRotationY;
    public float characterRotationZ;

    public float zoom;
    public float zoomSpeed = 2;
    public float zoomMin = -2f;
    public float zoomMax = -10f;

    public float rotationSpeed = 5f;

    void Start()
    {
        zoom = zoomMin;
    }

    void LateUpdate()
    {

        characterRotationX = character.rotation.x;
        characterRotationY = character.rotation.y;
        characterRotationZ = character.rotation.z;

        characterRotation = new Vector3(characterRotationX, characterRotationY, characterRotationZ);

        characterQuaternion = Quaternion.Euler(characterRotationX, characterRotationY, characterRotationZ);

        zoom += Input.GetAxis("CameraZoom") * zoomSpeed * Time.deltaTime;
        if(zoom > zoomMin)
        {
            zoom = zoomMin;
        }
        if (zoom < zoomMax)
        {
            zoom = zoomMax;
        }
        playerCam.transform.localPosition = new Vector3(0, 0, zoom);

        mouseX += Input.GetAxis("CameraHorizontal") * mouseSensitivity;
        //mouseY -= Input.GetAxis("Mouse Y");

        
        playerCam.LookAt(centerPoint);

        centerPoint.localRotation = Quaternion.Euler(10, mouseX, 0) * character.rotation;


        centerPoint.position = new Vector3(character.position.x, character.position.y, character.position.z) + offset;

        //Quaternion turnAngle = Quaternion.Euler(0, centerPoint.eulerAngles.y, 0);

        mouseX = Mathf.Clamp(mouseX, -60f - centerPoint.rotation.y, 60 + centerPoint.rotation.y); //This restricts camera moving it clamping it at -60 to 60


    }
}
