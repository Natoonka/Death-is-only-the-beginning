using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    private PlayerControls playerControls;

    public Animator animatorD, animatorE;

    private Queue<string> sentences;
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void ShowThatInteractable()
    {
        animatorE.SetBool("canInteract", true);
    }
    public void HideThatInteractable()
    {
        animatorE.SetBool("canInteract", false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Cursor.visible = true;
        animatorD.SetBool("isOpen", true);
        animatorE.SetBool("canInteract", false);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextLine();
    }
    public void DisplayNextLine()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    public void EndDialogue()
    {
        Cursor.visible = false;
        animatorD.SetBool("isOpen", false);
    }
}
