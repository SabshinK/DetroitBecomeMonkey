using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeHandler : MonoBehaviour, INarrative
{
    public event INarrative.ChoiceEvent onPresentChoice;
    
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
        // Create a local for simplicity purposes
        ShotSequence current = shotSequences[currentSequence];

        dialogueIndex++;

        if (dialogueIndex == current.dialogue.Length)
            dialogueIndex = current.dialogue.Length - 1;

        // If this is the last dialogue, do stuff
        if (dialogueIndex == current.dialogue.Length - 1)
        {
            // If there are choices
            if (current.HasDecision)
            {
                // Theoretically disable the buttons too

                onPresentChoice?.Invoke(current.decision);
            }
            else
            {
                currentSequence++;
                SetShotSequence();
            }
        }
        else
        {
            dialogueBox.text = current.dialogue[dialogueIndex];
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
