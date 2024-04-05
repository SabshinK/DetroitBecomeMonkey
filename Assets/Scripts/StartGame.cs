using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour, INarrative
{
    public event INarrative.ChoiceEvent onPresentChoice;

    [Tooltip("The number of desired players.")]
    [SerializeField] private int playerTotal = 3;

    private List<PlayerVote> playerVotes;

    private int playersReady = 0;

    private void Awake()
    {
        playerVotes = new List<PlayerVote>();
    }

    private void OnEnable()
    {
        PlayerManager.Instance.InputManager.onPlayerJoined += RegisterPlayer;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.InputManager.onPlayerJoined -= RegisterPlayer;

        foreach (PlayerVote playerVote in playerVotes)
        {
            playerVote.onFinishCastVote -= TallyAndCheck;
            playerVote.onCancelCastVote -= CancelVote;
        }

        // Unsubscribe all listeners from onPresentChoice
        if (onPresentChoice != null)
        {
            foreach (Delegate d in onPresentChoice.GetInvocationList())
                onPresentChoice -= d as INarrative.ChoiceEvent;
        }
    }

    // Start is called after SceneManager.sceneLoaded
    private void Start()
    {
        onPresentChoice?.Invoke(new Decision(DecisionMode.Calibration, false, new ShotSequence[0], new string[0]));
    }

    private void RegisterPlayer(PlayerInput playerInput)
    {
        PlayerVote playerVote = playerInput.GetComponent<PlayerVote>();

        playerVote.onFinishCastVote += TallyAndCheck;
        playerVote.onCancelCastVote += CancelVote;

        // Add player vote to the list
        playerVotes.Add(playerVote);
    }

    private void TallyAndCheck(int playerId, Choice choice)
    {
        playersReady++;

        Debug.Log(playersReady);

        if (playersReady == playerTotal)
            StartCoroutine(NextScene(2f));
    }

    private void CancelVote(int playerId, Choice choice)
    {
        StopAllCoroutines();

        playersReady = playersReady > 0 ? playersReady - 1 : 0;

        Debug.Log(playersReady);
    }

    private IEnumerator NextScene(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(1);
    }
}
