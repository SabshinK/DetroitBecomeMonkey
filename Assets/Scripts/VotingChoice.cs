using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VotingChoice : MonoBehaviour
{
    public UnityEvent<Choice> onCastVote;
    public UnityEvent<Choice> onCancelVote;

    private InputAction voteAAction;
    private InputAction voteBAction;

    private InputAction choice;
    private Choice choiceCode;

    private bool didCast;

    private void Awake()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        voteAAction = playerInput.actions.FindAction("Vote A");
        voteBAction = playerInput.actions.FindAction("Vote B");
    }

    private void OnEnable()
    {
        voteAAction.started += RecordVote;
        voteBAction.started += RecordVote;
        voteAAction.performed += CastVote;
        voteBAction.performed += CastVote;
        voteAAction.canceled += CancelVote;
        voteBAction.canceled += CancelVote;
    }

    private void OnDisable()
    {
        voteAAction.started -= RecordVote;
        voteBAction.started -= RecordVote;
        voteAAction.performed -= CastVote;
        voteBAction.performed -= CastVote;
        voteAAction.canceled -= CancelVote;
        voteBAction.canceled -= CancelVote;
    }

    private void RecordVote(InputAction.CallbackContext context)
    {
        choice = context.action;
    }

    private void CastVote(InputAction.CallbackContext context)
    {
        if (context.action == choice)
        {
            // Turn the choice action into an easily readable enum
            if (choice == voteAAction)
                choiceCode = Choice.A;
            else if (choice == voteBAction)
                choiceCode = Choice.B;
            else
                choiceCode = Choice.C;

            didCast = true;
            onCastVote?.Invoke(choiceCode);
        }
    }

    private void CancelVote(InputAction.CallbackContext context)
    {
        if (didCast)
        {
            onCancelVote?.Invoke(choiceCode);
            didCast = false;
        }
    }
}

public enum Choice
{
    A = 1,
    B = 2,
    C = 3
}