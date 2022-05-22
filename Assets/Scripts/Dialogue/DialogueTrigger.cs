using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private InputManager inputManager;
    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    public void OnTriggerStay(Collider other)
    {
        if(inputManager.PlayerInteracts())
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
