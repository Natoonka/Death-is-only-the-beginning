using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpening : MonoBehaviour
{
    public Animator door;
    virtual public void OnTriggerEnter(Collider other)
    {
        door.SetBool("inProximity", true);
    }
    public void OnTriggerExit(Collider other)
    {
        door.SetBool("inProximity", false);
    }
}
