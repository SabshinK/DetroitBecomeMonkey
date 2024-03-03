using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VotingChoice : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction voteAAction;
    InputAction voteBAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        voteAAction = playerInput.actions.FindAction("Vote A");
        voteBAction = playerInput.actions.FindAction("Vote B");
    }

    private void OnEnable()
    {
        voteAAction.performed += OnVoteA;
        voteBAction.performed += OnVoteB;
    }

    private void OnDisable()
    {
        voteAAction.performed -= OnVoteA;
        voteBAction.performed -= OnVoteB;
    }

    private void OnVoteA(InputAction.CallbackContext context)
    {
        Debug.Log("lmao");
    }

    private void OnVoteB(InputAction.CallbackContext context)
    {
        Debug.Log("lol");
    }
}
