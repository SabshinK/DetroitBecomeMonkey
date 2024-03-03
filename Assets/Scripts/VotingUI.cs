using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class VotingUI : MonoBehaviour
{
    [SerializeField] private Slider statusBar;
    [SerializeField] private TMP_Text statusText;

    private InputAction voteAAction;
    private InputAction voteBAction;

    private Coroutine currentRoutine;
    private float timer;

    private void Awake()
    {
        statusText.enabled = false;

        statusBar.value = 0f;
        timer = 0f;
    }

    private void OnDisable()
    {
        voteAAction.started -= StartTimer;
        voteBAction.started -= StartTimer;
        voteAAction.canceled -= CancelTimer;
        voteBAction.canceled -= CancelTimer;
    }

    public void RegisterPlayer(PlayerInput playerInput)
    {
        voteAAction = playerInput.actions.FindAction("Vote A");
        voteBAction = playerInput.actions.FindAction("Vote B");

        voteAAction.started += StartTimer;
        voteBAction.started += StartTimer;
        voteAAction.canceled += CancelTimer;
        voteBAction.canceled += CancelTimer;
    }

    private void StartTimer(InputAction.CallbackContext context)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        var interaction = context.interaction as HoldInteraction;
        currentRoutine = StartCoroutine(FillBar(interaction.duration));
    }

    private void CancelTimer(InputAction.CallbackContext context)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        var interaction = context.interaction as HoldInteraction;
        currentRoutine = StartCoroutine(CancelFillBar(interaction.duration));
    }

    private IEnumerator FillBar(float holdDuration)
    {
        while (timer < holdDuration)
        {
            // Update the timer
            timer += Time.deltaTime;

            // Update UI
            statusBar.value = timer / holdDuration;

            yield return new WaitForEndOfFrame();
        }

        statusBar.value = 1;
        statusText.enabled = true;
    }

    private IEnumerator CancelFillBar(float holdDuration)
    {
        statusText.enabled = false;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            statusBar.value = timer / holdDuration;

            yield return new WaitForEndOfFrame();
        }

        statusBar.value = 0;
        timer = 0;
    }
}
