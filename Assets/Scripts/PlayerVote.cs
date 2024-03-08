using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerVote : MonoBehaviour
{
    public int PlayerId { get; set; }
    public Choice LastChoice { get; private set; }

    public delegate void CastVote(int playerId, Choice choice);

    public event CastVote onStartCastVote;
    public event CastVote onFinishCastVote;
    public event CastVote onCancelCastVote;

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

        LastChoice = Choice.C;
        choiceCode = Choice.C;
    }

    private void OnEnable()
    {
        voteAAction.started += StartCastVote;
        voteBAction.started += StartCastVote;
        voteAAction.performed += FinishCastVote;
        voteBAction.performed += FinishCastVote;
        voteAAction.canceled += CancelCastVote;
        voteBAction.canceled += CancelCastVote;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        voteAAction.started -= StartCastVote;
        voteBAction.started -= StartCastVote;
        voteAAction.performed -= FinishCastVote;
        voteBAction.performed -= FinishCastVote;
        voteAAction.canceled -= CancelCastVote;
        voteBAction.canceled -= CancelCastVote;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Testing test = FindObjectOfType<Testing>();
        if (test != null)
            test.onPresentChoice.AddListener(() => { 
                LastChoice = Choice.C;
                choiceCode = Choice.C;
            });
    }

    private void StartCastVote(InputAction.CallbackContext context)
    {
        // Only call this function when there is a new vote
        Choice potentialChoice = ActionToEnum(context.action);
        LastChoice = choiceCode;
        if (potentialChoice != LastChoice)
        {
            choice = context.action;
            choiceCode = potentialChoice;

            onStartCastVote?.Invoke(PlayerId, choiceCode);
        }
    }

    private void FinishCastVote(InputAction.CallbackContext context)
    {
        Choice code = ActionToEnum(context.action);
        if (choiceCode == code)
        {
            didCast = true;
            onFinishCastVote?.Invoke(PlayerId, choiceCode);
        }
    }

    private void CancelCastVote(InputAction.CallbackContext context)
    {
        if (didCast)
        {
            onCancelCastVote?.Invoke(PlayerId, choiceCode);
            didCast = false;
        }
    }

    private Choice ActionToEnum(InputAction action)
    {
        // Turn the choice action into an easily readable enum
        if (action == voteAAction)
            return Choice.A;
        else if (action == voteBAction)
            return Choice.B;
        else
            return Choice.C;
    }
}

public enum Choice
{
    A = 1,
    B = 2,
    C = 3
}