using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform votingUIParent;

    private VotingUI[] votingUIs;

    private void Awake()
    {
        votingUIs = new VotingUI[votingUIParent.childCount];
        for (int i = 0; i < votingUIParent.childCount; i++)
        {
            votingUIs[i] = votingUIParent.GetChild(i).GetComponentInChildren<VotingUI>();
        }
    }

    public void RegisterPlayer(PlayerInput playerInput)
    {
        foreach (VotingUI ui in votingUIs)
        {
            if (!ui.enabled)
            {
                ui.enabled = true;
                ui.RegisterPlayer(playerInput);
            }
        }
    }
}
