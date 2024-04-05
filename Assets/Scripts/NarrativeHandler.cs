using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NarrativeHandler : MonoBehaviour, INarrative
{
    public event INarrative.ChoiceEvent onPresentChoice;
    
    [SerializeField] private ShotSequence[] shotSequences;
    [SerializeField] private ShotSequence currentSequence;

    [SerializeField] private Image shot;
    [SerializeField] private TMP_Text dialogueBox;

    [SerializeField] private GameObject instruction;

    //private Decision currentDecision;

    //private int currentSequence;
    private int dialogueIndex;
    private bool shouldProgress = false;

    private void Awake()
    {
        dialogueIndex = 0;
    }

    private void OnEnable()
    {
        VotingManager.Instance.onCastFinalVote += ChooseBranch;

        foreach (PlayerInput playerInput in PlayerManager.Instance.idsToPlayers.Values)
        {
            InputAction prevAction = playerInput.actions.FindAction("Previous");
            InputAction nextAction = playerInput.actions.FindAction("Next");

            prevAction.performed += PreviousLine;
            nextAction.performed += NextLine;
            nextAction.performed += ClearInstructions;
        }
    }

    private void OnDisable()
    {
        VotingManager.Instance.onCastFinalVote -= ChooseBranch;

        foreach (PlayerInput playerInput in PlayerManager.Instance.idsToPlayers.Values)
        {
            InputAction prevAction = playerInput.actions.FindAction("Previous");
            InputAction nextAction = playerInput.actions.FindAction("Next");

            prevAction.performed -= PreviousLine;
            nextAction.performed -= NextLine;
        }
    }

    public void NextLine(InputAction.CallbackContext context)
    {
        // This case is for if we are in a choice, the narrative shouldn't keep going
        if (!shouldProgress)
            return;

        // If this is the last dialogue, do stuff
        if (dialogueIndex >= currentSequence.dialogue.Length - 1)
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

            if (currentSequence.HasDecision)
            {
                onPresentChoice?.Invoke(currentSequence.decision);
                shouldProgress = false;
            }
        }
    }

    // This function is pretty much unused unless we want it
    public void PreviousLine(InputAction.CallbackContext context)
    {
        if (!shouldProgress)
            return;

        if (dialogueIndex > 0)
        {
            dialogueIndex--;
            dialogueBox.text = currentSequence.dialogue[dialogueIndex];
        }
        else if (currentSequence.previousSequence != null)
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

        if (currentSequence.HasDecision && currentSequence.dialogue.Length <= 1)
        {
            onPresentChoice?.Invoke(currentSequence.decision);
            shouldProgress = false;
        }
    }
    
    private void ChooseBranch(Choice choice)
    {
        SetShotSequence(currentSequence.decision.consequences[(int)choice], 0);
        shouldProgress = true;
    }

    private void ClearInstructions(InputAction.CallbackContext context)
    {
        instruction.SetActive(false);

        foreach (PlayerInput playerInput in PlayerManager.Instance.idsToPlayers.Values)
        {
            InputAction nextAction = playerInput.actions.FindAction("Next");
            nextAction.performed -= ClearInstructions;
        }

        SetShotSequence(currentSequence, 0);
        shouldProgress = true;
    }
}
