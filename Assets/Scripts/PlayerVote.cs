using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerVote : MonoBehaviour
{
    public int PlayerId { get; set; }
    public Choice LastChoice { get; private set; }

    public delegate void CastVote(int playerId, Choice choice);

    public event CastVote onStartCastVote;
    public event CastVote onFinishCastVote;
    public event CastVote onCancelCastVote;

    private InputAction[] voteActions;

    private Choice choiceCode;
    private int totalChoices;

    private bool didCast;

    #region Unity Callbacks

    private void Awake()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        voteActions = new InputAction[] 
        {
            playerInput.actions.FindAction("Vote A"),
            playerInput.actions.FindAction("Vote B"),
            playerInput.actions.FindAction("Vote C"),
            playerInput.actions.FindAction("Vote D")
        };

        LastChoice = Choice.Default;
        choiceCode = Choice.Default;
    }

    private void OnEnable()
    {
        foreach (InputAction voteAction in voteActions)
        {
            voteAction.started += StartCastVote;
            voteAction.performed += FinishCastVote;
            voteAction.canceled += CancelCastVote;
        }

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        foreach (InputAction voteAction in voteActions)
        {
            voteAction.started -= StartCastVote;
            voteAction.performed -= FinishCastVote;
            voteAction.canceled -= CancelCastVote;
        }

        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Input Callbacks

    private void StartCastVote(InputAction.CallbackContext context)
    {
        if (VotingManager.Instance.ShouldVote)
        {
            // Only call this function when there is a new vote
            Choice potentialChoice = ActionToEnum(context.action);

            // Case where the button pressed isn't an available choice, in which case do nothing
            if ((int)potentialChoice >= totalChoices && totalChoices > 0)
                return;

            LastChoice = choiceCode;
            if (potentialChoice != LastChoice)
            {
                choiceCode = potentialChoice;
                onStartCastVote?.Invoke(PlayerId, choiceCode);
            }
        }
    }

    private void FinishCastVote(InputAction.CallbackContext context)
    {
        if (VotingManager.Instance.ShouldVote)
        {
            // Don't cast if the choice is invalid
            Choice code = ActionToEnum(context.action);
            if ((int)code >= totalChoices && totalChoices > 0)
                return;

            if (choiceCode == code)
            {
                didCast = true;
                onFinishCastVote?.Invoke(PlayerId, choiceCode);
            }
        }
    }

    private void CancelCastVote(InputAction.CallbackContext context)
    {
        if (VotingManager.Instance.ShouldVote)
        {
            if (didCast)
            {
                onCancelCastVote?.Invoke(PlayerId, choiceCode);
                didCast = false;
            }
        }
    }

    #endregion

    private Choice ActionToEnum(InputAction action)
    {
        // Cast the index as a vote choice if the action matches
        for (int i = 0; i < voteActions.Length; i++)
        {
            if (action == voteActions[i])
                return (Choice)i;
        }

        // If the action isn't a valid choice return default
        return Choice.Default;
    }

    public void ResetVoting(int totalChoices)
    {
        LastChoice = Choice.Default;
        choiceCode = Choice.Default;

        this.totalChoices = totalChoices;
    }
}

public enum Choice
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    Default = 4
}