using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToEnd : MonoBehaviour
{
    public List<Item> items;
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggering Ending");
        EndGame();
    }

    public void EndGame()
    {
        foreach(Item item in items)
        {
            if(!Inventory.instance.items.Contains(item))
            {
                FindObjectOfType<LevelLoader>().LoadNextLevel(3);
                return;
            }
        }

        FindObjectOfType<LevelLoader>().LoadNextLevel(4);
    }
}
