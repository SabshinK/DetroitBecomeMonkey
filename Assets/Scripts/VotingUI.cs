using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class VotingUI : MonoBehaviour
{
    [SerializeField] private Transform iconParent;
    [SerializeField] private GameObject iconAreaA;
    [SerializeField] private GameObject iconAreaB;
    [SerializeField] private GameObject iconPrefab;

    [SerializeField] private Animator aspectBottom;

    [Space]
    [SerializeField] private TMP_Text choiceAText;
    [SerializeField] private TMP_Text choiceBText;
    [SerializeField] private Animator selectionAUnderline;
    [SerializeField] private Animator selectionBUnderline;

    [Space]
    [SerializeField] private Animator timerBar;

    [SerializeField] private NarrativeHandler narrative;

    private Dictionary<int, PlayerIcon> idsToIcons;

    private Coroutine currentRoutine;

    private void Awake()
    {
        idsToIcons = new Dictionary<int, PlayerIcon>();

        // Need to make icons for each player
        foreach (KeyValuePair<int, PlayerInput> player in PlayerManager.Instance.idsToPlayers)
        {
            // Create the icon objects
            GameObject icon = GameObject.Instantiate(iconPrefab, iconParent);
            PlayerIcon iconScript = icon.GetComponent<PlayerIcon>();

            // Hand off player data to the icons
            iconScript.RegisterPlayer(player.Value.GetComponent<PlayerInput>(), player.Key);

            // Add icon to dictionary
            idsToIcons.Add(player.Key, iconScript);
        }
    }

    private void OnEnable()
    {
        VotingManager.Instance.onUpdateMajorityVote += SetSelectionUnderline;
        VotingManager.Instance.onCastFinalVote += FinishChoice;

        foreach (PlayerVote playerVote in VotingManager.Instance.idsToPlayerVotes.Values)
        {
            playerVote.onStartCastVote += SetPlayerIconLocation;
        }

        narrative.onPresentChoice += PresentChoice;
    }

    private void OnDisable()
    {
        VotingManager.Instance.onUpdateMajorityVote -= SetSelectionUnderline;
        VotingManager.Instance.onCastFinalVote -= FinishChoice;

        foreach (PlayerVote playerVote in VotingManager.Instance.idsToPlayerVotes.Values)
        {
            playerVote.onStartCastVote -= SetPlayerIconLocation;
        }

        narrative.onPresentChoice -= PresentChoice;
    }

    public void PresentChoice(string[] choices)
    {
        aspectBottom.SetBool("IsPresenting", true);
    }

    private void FinishChoice(Choice choice)
    {
        foreach (PlayerIcon icon in idsToIcons.Values)
        {
            icon.transform.SetParent(iconParent);
            icon.transform.position = new Vector3(0, -650, 0);
        }

        selectionAUnderline.SetBool("IsSelected", false);
        selectionBUnderline.SetBool("IsSelected", false);

        aspectBottom.SetBool("IsPresenting", false);
    }

    private void SetSelectionUnderline(Choice choice)
    {
        selectionAUnderline.SetBool("IsSelected", choice == Choice.A);
        selectionBUnderline.SetBool("IsSelected", choice == Choice.B);
    }

    private void SetPlayerIconLocation(int playerId, Choice choice)
    {
        // I need to access the player icon of the given id and move it under the correct icon area
        if (choice == Choice.A)
            idsToIcons[playerId].transform.SetParent(iconAreaA.transform.GetChild(0));
        else if (choice == Choice.B)
            idsToIcons[playerId].transform.SetParent(iconAreaB.transform.GetChild(0));
    }
}
