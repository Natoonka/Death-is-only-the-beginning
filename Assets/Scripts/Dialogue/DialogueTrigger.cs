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
    public void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<DialogueManager>().ShowThatInteractable();
    }
    public void OnTriggerStay(Collider other)
    {
        if(inputManager.PlayerInteracts())
        {
            TriggerDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FindObjectOfType<DialogueManager>().HideThatInteractable();
    }

    virtual public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
