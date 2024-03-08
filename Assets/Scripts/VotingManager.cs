using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VotingManager : MonoBehaviour
{
    public static VotingManager Instance { get; private set; }

    public readonly Dictionary<int, PlayerVote> idsToPlayerVotes = new Dictionary<int, PlayerVote>();

    public delegate void FinalVoteEvent(Choice choice);

    public event FinalVoteEvent onUpdateMajorityVote;
    public event FinalVoteEvent onCastFinalVote;
    
    [SerializeField] private Testing testing;

    [SerializeField] private float voteTime = 10f;
    
    private Dictionary<Choice, int> choiceTallies;
    private Choice majorityVote;
    private int playersReady = 0;

    private bool shouldVote;

    private void Awake()
    {
        // Singleton logic
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        // Populate player vote dictionary
        foreach (KeyValuePair<int, PlayerInput> player in PlayerManager.Instance.idsToPlayers)
        {
            PlayerVote playerVote = player.Value.GetComponent<PlayerVote>();
            idsToPlayerVotes.Add(player.Key, playerVote);
        }

        choiceTallies = new Dictionary<Choice, int>();
    }

    private void OnEnable()
    {
        testing.onPresentChoice.AddListener(InitializedDecision);

        foreach (PlayerVote playerVote in idsToPlayerVotes.Values)
        {
            playerVote.onStartCastVote += RecordVote;
            playerVote.onFinishCastVote += SubmitVote;
            playerVote.onCancelCastVote += CancelVote;
        }
    }

    private void OnDisable()
    {
        testing.onPresentChoice.RemoveListener(InitializedDecision);

        foreach (PlayerVote playerVote in idsToPlayerVotes.Values)
        {
            playerVote.onStartCastVote -= RecordVote;
            playerVote.onFinishCastVote -= SubmitVote;
            playerVote.onCancelCastVote -= CancelVote;
        }
    }

    // These two functions are just getting what the current voting status is like, players have to all hold buttons to submit
    private void RecordVote(int playerId, Choice choice)
    {
        if (shouldVote)
        {
            // This should be done somewhere else, there should be a function that sets the dictionary when a choice is presented
            if (!choiceTallies.ContainsKey(choice))
                choiceTallies.Add(choice, 0);            

            choiceTallies[choice]++;

            // Discard the last vote of that player
            PlayerVote playerVote = idsToPlayerVotes[playerId];
            if (choiceTallies.ContainsKey(playerVote.LastChoice))
                choiceTallies[playerVote.LastChoice]--;

            // Check to see what is currently highest voted
            CheckVote();
        }
    }

    private void CheckVote()
    {
        // Find the current highest vote, also have a boolean to check and make sure the values aren't all the same
        // This checking method is based on majority, not percentile
        bool differentValues = false;
        KeyValuePair<Choice, int> highestVote = choiceTallies.ToArray()[0];
        foreach (KeyValuePair<Choice, int> vote in choiceTallies)
        {
            if (vote.Value != highestVote.Value || choiceTallies.Count() == 1)
            {
                differentValues = true;
                if (vote.Value > highestVote.Value)
                    highestVote = vote;
            }
        }

        // Broadcast the current highest vote, or not if there is no consensus
        majorityVote = differentValues ? highestVote.Key : Choice.C;
        onUpdateMajorityVote?.Invoke(majorityVote);
    }

    // For these two methods we don't care what player voted or what they chose, just that they are holding and ready
    private void SubmitVote(int playerId, Choice choice)
    {
        if (shouldVote)
        {
            playersReady++;

            if (playersReady == PlayerManager.Instance.PlayerCount)
            {
                StopAllCoroutines();
                onCastFinalVote?.Invoke(majorityVote);
            }
        }
    }

    private void CancelVote(int playerId, Choice choice)
    {
        if (shouldVote)
            playersReady = playersReady > 0 ? playersReady - 1 : 0;
    }

    private void InitializedDecision()
    {
        //if (isTimed)
        //    StartCoroutine(TimedVote(voteTime));

        majorityVote = Choice.C;
        onUpdateMajorityVote?.Invoke(majorityVote);
        playersReady = 0;
        shouldVote = true;
    }

    private IEnumerator TimedVote(float voteTime)
    {
        yield return new WaitForSeconds(voteTime);

        // Cast the final vote regardless of whether players are ready
        onCastFinalVote?.Invoke(majorityVote);
    }
}
