using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class VotingUI : MonoBehaviour
{
    [SerializeField] private GameObject[] choiceObjects;

    [SerializeField] private Transform iconParent;
    [SerializeField] private GameObject[] iconAreas;
    [SerializeField] private GameObject iconPrefab;

    [SerializeField] private Animator aspectBottom;

    [Space]
    [SerializeField] private TMP_Text[] choiceTexts;
    [SerializeField] private Animator selectionAUnderline;
    [SerializeField] private Animator selectionBUnderline;
    [SerializeField] private Animator[] selectionUnderlines;

    [Space]
    [SerializeField] private Image decisionIcon;
    [SerializeField] private Sprite unanimousIcon;
    [SerializeField] private Sprite majorityIcon;

    [Space]
    [SerializeField] private Animator timerBar;

    [Space]
    [SerializeField] private GameObject focusGroupNoteObject;
    [SerializeField] private GameObject[] focusGroupIcons;

    [SerializeField] private NarrativeHandler narrative;

    

    private Dictionary<int, PlayerIcon> idsToIcons;

    private Coroutine currentRoutine;

    private Decision currentDecision;

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

        decisionIcon.enabled = false;
    }

    private void OnEnable()
    {
        VotingManager.Instance.onUpdateFinalVote += SetSelectionUnderline;
        VotingManager.Instance.onCastFinalVote += FinishChoice;
        VotingManager.Instance.onNextPlayer += NextPlayerIcon;

        foreach (PlayerVote playerVote in VotingManager.Instance.idsToPlayerVotes.Values)
        {
            playerVote.onStartCastVote += SetPlayerIconLocation;
        }

        narrative.onPresentChoice += PresentChoice;
    }

    private void OnDisable()
    {
        VotingManager.Instance.onUpdateFinalVote -= SetSelectionUnderline;
        VotingManager.Instance.onCastFinalVote -= FinishChoice;
        VotingManager.Instance.onNextPlayer -= NextPlayerIcon;

        foreach (PlayerVote playerVote in VotingManager.Instance.idsToPlayerVotes.Values)
        {
            playerVote.onStartCastVote -= SetPlayerIconLocation;
        }

        narrative.onPresentChoice -= PresentChoice;
    }

    public void PresentChoice(Decision decision)
    {
        currentDecision = decision;

        // Enable the icon areas
        for (int i = 0; i < choiceObjects.Length; i++)
        {
            choiceObjects[i].SetActive(i < decision.choices.Length);

            if (i < decision.choices.Length)
                choiceTexts[i].text = decision.choices[i];
        }

        aspectBottom.SetBool("IsPresenting", true);

        if (decision.decisionMode == DecisionMode.Unanimous)
            decisionIcon.sprite = unanimousIcon;
        else if (decision.decisionMode == DecisionMode.Majority)
            decisionIcon.sprite = majorityIcon;
        decisionIcon.enabled = true;

        if (decision.useFocusGroup)
        {
            focusGroupNoteObject.SetActive(true);
            focusGroupIcons[0].SetActive(true);
        }
    }

    private void FinishChoice(Choice choice)
    {
        foreach (PlayerIcon icon in idsToIcons.Values)
        {
            icon.transform.SetParent(iconParent);
            icon.transform.position = new Vector3(0, -650, 0);
        }

        //selectionAUnderline.SetBool("IsSelected", false);
        //selectionBUnderline.SetBool("IsSelected", false);
        foreach (Animator selectionUnderline in selectionUnderlines)
        {
            if (selectionUnderline.gameObject.activeInHierarchy)
                selectionUnderline.SetBool("IsSelected", false);
        }

        aspectBottom.SetBool("IsPresenting", false);

        decisionIcon.enabled = false;

        if (currentDecision.useFocusGroup)
        {
            focusGroupNoteObject.SetActive(false);
            focusGroupIcons[focusGroupIcons.Length - 1].SetActive(false);
        }
    }

    private void SetSelectionUnderline(Choice choice)
    {
        Debug.Log(choice);
        for (int i = 0; i < selectionUnderlines.Length; i++)
        {
            Animator selectionUnderline = selectionUnderlines[i];
            if (selectionUnderline.gameObject.activeInHierarchy)
                selectionUnderline.SetBool("IsSelected", choice == (Choice)i);
        }
    }

    private void SetPlayerIconLocation(int playerId, Choice choice)
    {
        if (currentDecision.useFocusGroup && playerId != VotingManager.Instance.CurrentPlayer)
            return;

        // I need to access the player icon of the given id and move it under the correct icon area
        Transform iconAreaParent = iconAreas[(int)choice].transform.GetChild(0);
        if (iconAreaParent.gameObject.activeInHierarchy)
            idsToIcons[playerId].transform.SetParent(iconAreaParent);
    }

    private void NextPlayerIcon(int previousPlayer)
    {
        // Basically finish choice logic but the choice isn't actually done
        PlayerIcon icon = idsToIcons[previousPlayer];
        icon.transform.SetParent(iconParent);
        icon.transform.position = new Vector3(0, -650, 0);

        foreach (Animator selectionUnderline in selectionUnderlines)
        {
            if (selectionUnderline.gameObject.activeInHierarchy)
                selectionUnderline.SetBool("IsSelected", false);
        }

        focusGroupIcons[previousPlayer - 1].SetActive(false);
        focusGroupIcons[previousPlayer].SetActive(true);
    }
}
