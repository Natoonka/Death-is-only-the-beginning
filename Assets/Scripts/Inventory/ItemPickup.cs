using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : DialogueTrigger
{
    public Item item;
    public override void TriggerDialogue()
    {
        base.TriggerDialogue();
        PickUp();
    }
    void PickUp()
    {
        Inventory.instance.Add(item);
        Destroy(gameObject);
    }
}
