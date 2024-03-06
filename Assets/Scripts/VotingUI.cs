using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class VotingUI : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Animator aspectBottom;

    [Space]
    [SerializeField] private TMP_Text choiceAText;
    [SerializeField] private TMP_Text choiceBText;
    [SerializeField] private Animator selectionAUnderline;
    [SerializeField] private Animator selectionBUnderline;

    [Space]
    [SerializeField] private Animator timerBar;

    private Coroutine currentRoutine;

    private void Awake()
    {
        // Need to make icons for each player
    }

    private void OnEnable()
    {
        VotingManager.Instance.onUpdateVote.AddListener(SetSelectionUnderline);
    }

    private void OnDisable()
    {
        VotingManager.Instance.onUpdateVote.RemoveListener(SetSelectionUnderline);
    }

    public void RegisterPlayer(PlayerInput playerInput)
    {
        
    }

    public void PresentChoice()
    {
        aspectBottom.SetBool("IsPresenting", true);
    }

    private void SetSelectionUnderline(Choice choice)
    {
        if (choice == Choice.A)
        {
            selectionAUnderline.SetBool("IsSelected", true);
            selectionBUnderline.SetBool("IsSelected", false);
        }
        else if (choice == Choice.B)
        {
            selectionAUnderline.SetBool("IsSelected", false);
            selectionBUnderline.SetBool("IsSelected", true);
        }
        else
        {
            selectionAUnderline.SetBool("IsSelected", false);
            selectionBUnderline.SetBool("IsSelected", false);
        }
    }
}
