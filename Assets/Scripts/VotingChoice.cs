using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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

    //private void OnEnable()
    //{
    //    voteAAction.started += RecordVote;
    //    voteBAction.started += RecordVote;
    //    voteAAction.performed += CastVote;
    //    voteBAction.performed += CastVote;
    //}

    //private void OnDisable()
    //{
    //    voteAAction.started += RecordVote;
    //    voteBAction.started += RecordVote;
    //    voteAAction.performed -= CastVote;
    //    voteBAction.performed -= CastVote;
    //}

    //private void RecordVote(InputAction.CallbackContext context)
    //{
    //    choice = context.action;
    //}

    //private void CastVote(InputAction.CallbackContext context)
    //{
    //    if (context.action == choice)
    //        Debug.Log(context.action.name);
    //}

    //private void OnDisable()
    //{
    //    voteAAction.started -= StartTimer;
    //    voteBAction.started -= StartTimer;
    //    voteAAction.canceled -= CancelTimer;
    //    voteBAction.canceled -= CancelTimer;
    //}

    //private void StartTimer(InputAction.CallbackContext context)
    //{
    //    if (voteBAction.IsInProgress())

    //        if (currentRoutine != null)
    //            StopCoroutine(currentRoutine);

    //    var interaction = context.interaction as HoldInteraction;
    //    currentRoutineA = StartCoroutine(FillBar(interaction.duration, statusBar, 0));
    //}

    //private void CancelTimer(InputAction.CallbackContext context)
    //{
    //    if (currentRoutineA != null)
    //        StopCoroutine(currentRoutineA);

    //    var interaction = context.interaction as HoldInteraction;
    //    currentRoutineA = StartCoroutine(CancelFillBar(interaction.duration, statusBar, 0));
    //}

    //private IEnumerator FillBar(float holdDuration, Slider statusBar, int timerIndex)
    //{
    //    while (timers[timerIndex] < holdDuration)
    //    {
    //        // Update the timer
    //        timers[timerIndex] += Time.deltaTime;

    //        // Update UI
    //        statusBar.value = timers[timerIndex] / holdDuration;

    //        yield return new WaitForEndOfFrame();
    //    }

    //    statusBar.value = 1;
    //    statusText.enabled = true;
    //}
}
