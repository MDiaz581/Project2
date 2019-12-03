 using UnityEngine;
 using System.Collections;

public class GetRightHit : MonoBehaviour
{
    
    public bool rightTriggered;

    private void Start()
    {
       rightTriggered = false;
    }

    private void OnTriggerStay(Collider other)
    {
        rightTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {


        rightTriggered = false;
    }
}