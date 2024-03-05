using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerJoinedInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;

    [SerializeField] private TMP_Text status;

    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color holdingColor;
    [SerializeField] private Color readyColor;

    [SerializeField] private PlayerIcon icon;

    private InputAction voteAAction;
    private InputAction voteBAction;

    public void RegisterPlayer(PlayerInput playerInput, int playerId)
    {
        playerName.text = $"Player {playerId}";

        SetStatus("Waiting", inactiveColor);

        voteAAction = playerInput.actions.FindAction("Vote A");
        voteBAction = playerInput.actions.FindAction("Vote B");

        voteAAction.started += HoldingStatus;
        voteBAction.started += HoldingStatus;
        voteAAction.performed += ReadyStatus;
        voteBAction.performed += ReadyStatus;
        voteAAction.canceled += InactiveStatus;
        voteBAction.canceled += InactiveStatus;

        icon.RegisterPlayer(playerInput, playerId);
    }

    private void OnDisable()
    {
        voteAAction.started -= HoldingStatus;
        voteBAction.started -= HoldingStatus;
        voteAAction.performed -= ReadyStatus;
        voteBAction.performed -= ReadyStatus;
        voteAAction.canceled -= InactiveStatus;
        voteBAction.canceled -= InactiveStatus;
    }

    private void InactiveStatus(InputAction.CallbackContext context)
    {
        SetStatus("Waiting", inactiveColor);
    }

    private void HoldingStatus(InputAction.CallbackContext context)
    {
        SetStatus("Holding...", holdingColor);
    }

    private void ReadyStatus(InputAction.CallbackContext context)
    {
        SetStatus("Ready!", readyColor);
    }

    private void SetStatus(string message, Color fontColor)
    {
        status.text = message;
        status.color = fontColor;
    }
}
