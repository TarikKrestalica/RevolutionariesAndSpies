using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// https://youtu.be/_nRzoTzeyxU
[System.Serializable]
public class DialogueBox
{
    public GameObject box;
    public TextMeshProUGUI text_box;
}

// Keep track of the current dialogue we are on!
public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences = new Queue<string>();
    [SerializeField] private int currentLine;  // How far am I in the story?

    [SerializeField] private DialogueBox dialogueBox;

    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject doneButton;
    [SerializeField] private GameObject repeatButton;
    private bool isStoryFinished;

    private float textSpeed = 0.05f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (textSpeed > 0.01f)
                SetTextSpeed(textSpeed -= 0.01f);
            else if (textSpeed <= 0.0025f)
                SetTextSpeed(textSpeed -= 0.025f);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        currentLine = 0;
        isStoryFinished = false;
        dialogueBox.box.SetActive(true);
        sentences.Clear();  // Remove old story

        // Add each sentence to manager
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplaySentence();
    }

    public void DisplaySentence()
    {
        continueButton.SetActive(false);
        if (sentences.Count == 0)
        {
            isStoryFinished = true;
            EndDialogue();
            return;
        }
        currentLine++;
   
        string sentence = sentences.Dequeue();
        if (sentence.Length == 0)
        {
            return;
        }

        SetTextSpeed(0.05f);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueBox.text_box.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueBox.text_box.text += letter;
            yield return new WaitForSeconds(GetTextSpeed());  // Wait some time for the subsequent characters to be typed out!
        }

        continueButton.SetActive(true);  
    }

    public virtual void EndDialogue()
    {
        doneButton.SetActive(true);
        repeatButton.SetActive(true);
    }

    public void ClearDialogue()
    {
        dialogueBox.text_box.text = "";
    }

    public int GetCurrentLine()
    {
        return currentLine;
    }

    public bool StoryFinished()
    {
        return isStoryFinished;
    }

    public Queue<string> GetSentences()
    {
        return sentences;
    }

    public GameObject GetDoneButton()
    {
        return doneButton;
    }

    public GameObject GetRepeatButton()
    {
        return repeatButton;
    }

    public GameObject GetBox()
    {
        return dialogueBox.box;
    }

    public GameObject GetContinueButton()
    {
        return continueButton;
    }

    public TextMeshProUGUI GetTextBox()
    {
        return dialogueBox.text_box;
    }

    public void SetTextBox(string text)
    {
        dialogueBox.text_box.text = text.ToString();
    }

    public void GoToTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    void SetTextSpeed(float speed)
    {
        textSpeed = speed;
    }

    float GetTextSpeed()
    {
        return textSpeed;
    }
}