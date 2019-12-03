 using UnityEngine;
 using System.Collections;

public class GetSideHit : MonoBehaviour
{
    
    public bool triggered;

    private void Start()
    {
        triggered = false;
    }

    private void OnTriggerStay(Collider other)
    {              
        triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        

        triggered = false;
    }
}