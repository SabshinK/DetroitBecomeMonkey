using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private List<VotingChoice> playerVotes;

    private int playersReady = 0;

    private void Awake()
    {
        playerVotes = new List<VotingChoice>();
    }

    private void OnEnable()
    {
        PlayerManager.Instance.InputManager.onPlayerJoined += RegisterPlayer;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.InputManager.onPlayerJoined -= RegisterPlayer;

        foreach (VotingChoice playerVote in playerVotes)
        {
            playerVote.onCastVote.RemoveListener(TallyAndCheck);
            playerVote.onCancelVote.RemoveListener(CancelVote);
        }
    }

    private void RegisterPlayer(PlayerInput playerInput)
    {
        VotingChoice playerVote = playerInput.GetComponent<VotingChoice>();

        playerVote.onCastVote.AddListener(TallyAndCheck);
        playerVote.onCancelVote.AddListener(CancelVote);
    }

    private void TallyAndCheck(Choice choice)
    {
        playersReady++;

        Debug.Log(playersReady);

        if (playersReady == PlayerManager.Instance.PlayerCount)
            StartCoroutine(NextScene(2f));
    }

    private void CancelVote(Choice choice)
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
