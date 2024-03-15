using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour, INarrative
{
    public event INarrative.ChoiceEvent onPresentChoice;

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

        if (playersReady == PlayerManager.Instance.PlayerCount)
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
