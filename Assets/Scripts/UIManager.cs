using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform votingUIParent;

    private VotingUI[] votingUIs;

    private PlayerInputManager inputManager;

    private void Awake()
    {
        inputManager = FindObjectOfType<PlayerInputManager>();

        votingUIs = new VotingUI[votingUIParent.childCount];
        for (int i = 0; i < votingUIParent.childCount; i++)
        {
            votingUIs[i] = votingUIParent.GetChild(i).GetComponentInChildren<VotingUI>();
        }
    }

    private void OnEnable()
    {
        inputManager.onPlayerJoined += RegisterPlayer;
    }

    private void OnDisable()
    {
        inputManager.onPlayerJoined -= RegisterPlayer;
    }

    private void RegisterPlayer(PlayerInput playerInput)
    {
        foreach (VotingUI ui in votingUIs)
        {
            if (!ui.gameObject.activeInHierarchy)
            {
                // Player registration should probably happen beforehand and all the UI should already be enabled
                ui.gameObject.SetActive(true);
                ui.RegisterPlayer(playerInput);
                break;
            }
        }
    }
}
