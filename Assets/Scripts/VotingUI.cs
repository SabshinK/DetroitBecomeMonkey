using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class VotingUI : MonoBehaviour
{
    [SerializeField] private Slider statusBarA;
    [SerializeField] private Slider statusBarB;
    [SerializeField] private TMP_Text statusText;

    private InputAction voteAAction;
    private InputAction voteBAction;

    private Coroutine currentRoutineA;
    private Coroutine currentRoutineB;

    private float[] timers = new float[2];

    private void Awake()
    {
        statusText.enabled = false;

        statusBarA.value = 0f;
        statusBarB.value = 0f;
        timers[0] = 0f;
        timers[1] = 0f;
    }

    private void OnDisable()
    {
        voteAAction.started -= StartTimerA;
        voteBAction.started -= StartTimerB;
        voteAAction.canceled -= CancelTimerA;
        voteBAction.canceled -= CancelTimerB;
    }

    public void RegisterPlayer(PlayerInput playerInput)
    {
        voteAAction = playerInput.actions.FindAction("Vote A");
        voteBAction = playerInput.actions.FindAction("Vote B");

        voteAAction.started += StartTimerA;
        voteBAction.started += StartTimerB;
        voteAAction.canceled += CancelTimerA;
        voteBAction.canceled += CancelTimerB;
    }

    private void StartTimerA(InputAction.CallbackContext context)
    {
        if (voteBAction.IsInProgress())
            /*
             * I'm being sneaking here, I'm passing the context for B to A because it's only useful for
             * getting the hold duration, so it doesn't matter where it came from
             */
            CancelTimerB(context);  

        if (currentRoutineA != null)
            StopCoroutine(currentRoutineA);

        var interaction = context.interaction as HoldInteraction;
        currentRoutineA = StartCoroutine(FillBar(interaction.duration, statusBarA, 0));
    }

    private void StartTimerB(InputAction.CallbackContext context)
    {
        if (voteAAction.IsInProgress())
            CancelTimerA(context);

        if (currentRoutineB != null)
            StopCoroutine(currentRoutineB);

        var interaction = context.interaction as HoldInteraction;
        currentRoutineB = StartCoroutine(FillBar(interaction.duration, statusBarB, 1));
    }

    private void CancelTimerA(InputAction.CallbackContext context)
    {
        if (currentRoutineA != null)
            StopCoroutine(currentRoutineA);

        var interaction = context.interaction as HoldInteraction;
        currentRoutineA = StartCoroutine(CancelFillBar(interaction.duration, statusBarA, 0));
    }

    private void CancelTimerB(InputAction.CallbackContext context)
    {
        if (currentRoutineB != null)
            StopCoroutine(currentRoutineB);

        var interaction = context.interaction as HoldInteraction;
        currentRoutineB = StartCoroutine(CancelFillBar(interaction.duration, statusBarB, 1));
    }

    private IEnumerator FillBar(float holdDuration, Slider statusBar, int timerIndex)
    {
        while (timers[timerIndex] < holdDuration)
        {
            // Update the timer
            timers[timerIndex] += Time.deltaTime;

            // Update UI
            statusBar.value = timers[timerIndex] / holdDuration;

            yield return new WaitForEndOfFrame();
        }

        statusBar.value = 1;
        statusText.enabled = true;
    }

    private IEnumerator CancelFillBar(float holdDuration, Slider statusBar, int timerIndex)
    {
        statusText.enabled = false;

        while (timers[timerIndex] > 0)
        {
            timers[timerIndex] -= Time.deltaTime;

            statusBar.value = timers[timerIndex] / holdDuration;

            yield return new WaitForEndOfFrame();
        }

        statusBar.value = 0;
        timers[timerIndex] = 0;
    }
}
