using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    [SerializeField] private Image iconSprite;
    [SerializeField] private TMP_Text text;

    [SerializeField] private Image selectionStatus;

    private InputAction voteAAction;
    private InputAction voteBAction;

    private Coroutine currentRoutine;

    private float timer;

    private void Awake()
    {
        selectionStatus.fillAmount = 0f;
        timer = 0f;
    }

    private void OnDisable()
    {
        voteAAction.started -= StartTimer;
        voteBAction.started -= StartTimer;
        voteAAction.canceled -= CancelTimer;
        voteBAction.canceled -= CancelTimer;
    }

    public void RegisterPlayer(PlayerInput playerInput, int playerId)
    {
        voteAAction = playerInput.actions.FindAction("Vote A");
        voteBAction = playerInput.actions.FindAction("Vote B");
        
        voteAAction.started += StartTimer;
        voteBAction.started += StartTimer;
        voteAAction.canceled += CancelTimer;
        voteBAction.canceled += CancelTimer;

        text.text = playerId.ToString();
        //iconSprite.color = iconColor;
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

        selectionStatus.fillAmount = 0f;
        timer = 0f;
    }

    private IEnumerator FillBar(float holdDuration)
    {
        while (timer < holdDuration)
        {
            // Update the timer
            timer += Time.deltaTime;

            // Update UI
            selectionStatus.fillAmount = timer / holdDuration;

            yield return new WaitForEndOfFrame();
        }

        selectionStatus.fillAmount = 1f;
    }

    /*
     * Web as opposed to having people install apps-- of course
     * 
     * remember firewalls
     * html web javascript-- potentially CSS
     * 
     * wifi isn't an issue
     * 
     * Look into JS libraries for mobile development
     * Lots js libraries for web games, look into this
     */
}
