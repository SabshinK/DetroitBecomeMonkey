using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VotingChoice : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction voteAAction;
    private InputAction voteBAction;

    private InputAction choice;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        voteAAction = playerInput.actions.FindAction("Vote A");
        voteBAction = playerInput.actions.FindAction("Vote B");
    }

    private void OnEnable()
    {
        voteAAction.started += RecordVote;
        voteBAction.started += RecordVote;
        voteAAction.performed += CastVote;
        voteBAction.performed += CastVote;
    }

    private void OnDisable()
    {
        voteAAction.started += RecordVote;
        voteBAction.started += RecordVote;
        voteAAction.performed -= CastVote;
        voteBAction.performed -= CastVote;
    }

    private void RecordVote(InputAction.CallbackContext context)
    {
        choice = context.action;
    }

    private void CastVote(InputAction.CallbackContext context)
    {
        if (context.action == choice)
            Debug.Log(context.action.name);
    }
}
