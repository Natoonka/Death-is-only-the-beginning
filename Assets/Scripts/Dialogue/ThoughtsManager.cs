using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThoughtsManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Dialogue dialogue;
    [SerializeField]
    public int index;

    private Queue<string> sentences;
    void Start()
    {
        sentences = new Queue<string>();
        StartThought(dialogue);
    }
    
    public void StartThought(Dialogue dialogue)
    {
        Cursor.visible = true;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextLine();
    }
    public void DisplayNextLine()
    {
        if (sentences.Count == 0)
        {
            EndThought();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    public void EndThought()
    {
        FindObjectOfType<LevelLoader>().LoadNextLevel(index);
    }
}