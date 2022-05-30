using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingDoor : DoorOpening
{
    public Item item;
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (Inventory.instance.items.Contains(item))
        {
            door.SetBool("hasParkingKey", true);
        }
    }
}
