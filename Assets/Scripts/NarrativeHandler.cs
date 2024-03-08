using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeHandler : MonoBehaviour
{
    public delegate void ChoiceEvent(string[] choices);
    public event ChoiceEvent onPresentChoice;

    [SerializeField] private ShotSequence[] shotSequences;

    [SerializeField] private Image shot;
    [SerializeField] private TMP_Text dialogueBox;

    private int currentSequence;
    private int dialogueIndex;

    private void Awake()
    {
        dialogueIndex = 0;
    }

    private void OnEnable()
    {
        VotingManager.Instance.onCastFinalVote += ChooseBranch;
    }

    private void OnDisable()
    {
        VotingManager.Instance.onCastFinalVote -= ChooseBranch;
    }

    private void Start()
    {
        shot.sprite = shotSequences[0].shot;
        dialogueBox.text = shotSequences[0].dialogue[dialogueIndex];
    }

    public void NextLine()
    {
        dialogueIndex++;

        if (dialogueIndex == shotSequences[currentSequence].dialogue.Length)
            dialogueIndex = shotSequences[currentSequence].dialogue.Length - 1;

        // If this is the last dialogue, do stuff
        if (dialogueIndex == shotSequences[currentSequence].dialogue.Length - 1)
        {
            // If there are choices
            if (shotSequences[currentSequence].choices.Length > 0)
            {
                // Theoretically disable the buttons too


                onPresentChoice?.Invoke(shotSequences[currentSequence].choices);
            }
            else
            {
                currentSequence++;
                SetShotSequence();
            }
        }
        else
        {
            dialogueBox.text = shotSequences[currentSequence].dialogue[dialogueIndex];
        }
    }

    public void PreviousLine()
    {
        if (dialogueIndex > 0)
        {
            dialogueIndex--;
            dialogueBox.text = shotSequences[currentSequence].dialogue[dialogueIndex];
        }
        else
        {
            // set the shot sequence to the previous sequence
        }
    }

    private void SetShotSequence()
    {
        dialogueIndex = 0;

        shot.sprite = shotSequences[currentSequence].shot;
        dialogueBox.text = shotSequences[currentSequence].dialogue[0];
    }
    
    private void ChooseBranch(Choice choice)
    {
        throw new NotImplementedException();
    }
}
