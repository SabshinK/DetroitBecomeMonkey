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
    [SerializeField] private ShotSequence currentSequence;

    [SerializeField] private Image shot;
    [SerializeField] private TMP_Text dialogueBox;

    //private Decision currentDecision;

    //private int currentSequence;
    private int dialogueIndex;
    private bool shouldProgress;

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
        SetShotSequence(currentSequence, 0);
        shouldProgress = true;
    }

    public void NextLine()
    {
        // This case is for if we are in a choice, the narrative shouldn't keep going
        if (!shouldProgress)
            return;

        // If this is the last dialogue, do stuff
        if (dialogueIndex == currentSequence.dialogue.Length - 1 || currentSequence.dialogue.Length == 0)
        {
            if (currentSequence.HasDecision)
            {
                // Theoretically disable the buttons too

                onPresentChoice?.Invoke(currentSequence.decision);
                shouldProgress = false;
            }
            else
            {
                SetShotSequence(currentSequence.nextSequence, 0);
            }
        }
        else
        {
            dialogueIndex++;
            dialogueBox.text = currentSequence.dialogue[dialogueIndex];
        }
    }

    // This function is pretty much unused unless we want it
    public void PreviousLine()
    {
        if (dialogueIndex > 0)
        {
            dialogueIndex--;
            dialogueBox.text = currentSequence.dialogue[dialogueIndex];
        }
        else
        {
            ShotSequence previousSequence = currentSequence.previousSequence;
            SetShotSequence(previousSequence, previousSequence.dialogue.Length - 1);
        }
    }

    private void SetShotSequence(ShotSequence sequence, int newDialogueIndex)
    {
        dialogueIndex = newDialogueIndex;

        currentSequence = sequence;

        shot.sprite = sequence.shot;
        if (sequence.dialogue.Length > 0)
            dialogueBox.text = sequence.dialogue[newDialogueIndex];
        else
            dialogueBox.text = string.Empty;
    }
    
    private void ChooseBranch(Choice choice)
    {
        SetShotSequence(currentSequence.decision.consequences[(int)choice], 0);
        shouldProgress = true;
    }
}
