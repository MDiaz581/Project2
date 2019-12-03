 using UnityEngine;
 using System.Collections;

public class GetLeftHit : MonoBehaviour
{
    
    public bool leftTriggered;

    private void Start()
    {
       leftTriggered = false;
    }

    private void OnTriggerStay(Collider other)
    {
        leftTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {


        leftTriggered = false;

    }
}