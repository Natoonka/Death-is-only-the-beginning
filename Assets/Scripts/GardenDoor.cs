using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenDoor : DoorOpening
{
    public Item item;
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (Inventory.instance.items.Contains(item))
        {
            door.SetBool("hasGardenKey", true);
        }
    }
}
