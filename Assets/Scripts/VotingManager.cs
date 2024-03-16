using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VotingManager : MonoBehaviour
{
    public static VotingManager Instance { get; private set; }

    public readonly Dictionary<int, PlayerVote> idsToPlayerVotes = new Dictionary<int, PlayerVote>();

    public delegate void FinalVoteEvent(Choice choice);

    public event FinalVoteEvent onUpdateMajorityVote;
    public event FinalVoteEvent onCastFinalVote;

    public bool ShouldVote { get; private set; }
    
    [SerializeField] private INarrative narrative;

    [SerializeField] private float voteTime = 10f;
    
    private Dictionary<Choice, int> choiceTallies;
    private Choice majorityVote;
    private int playersReady = 0;

    //private KeyValuePair<Choice, int> highestVote;

    private void Awake()
    {
        // Singleton logic
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Populate player vote dictionary
        //foreach (KeyValuePair<int, PlayerInput> player in PlayerManager.Instance.idsToPlayers)
        //{
        //    PlayerVote playerVote = player.Value.GetComponent<PlayerVote>();
        //    idsToPlayerVotes.Add(player.Key, playerVote);
        //}

        choiceTallies = new Dictionary<Choice, int>();
    }

    private void OnEnable()
    {
        if (narrative != null)
            narrative.onPresentChoice += InitializeDecision;

        //foreach (PlayerVote playerVote in idsToPlayerVotes.Values)
        //{
        //    playerVote.onStartCastVote += RecordVote;
        //    playerVote.onFinishCastVote += SubmitVote;
        //    playerVote.onCancelCastVote += CancelVote;
        //}

        onCastFinalVote += (Choice choice) => { ShouldVote = false; };

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        //narrative.onPresentChoice -= InitializeDecision;

        foreach (PlayerVote playerVote in idsToPlayerVotes.Values)
        {
            playerVote.onStartCastVote -= RecordVote;
            playerVote.onFinishCastVote -= SubmitVote;
            playerVote.onCancelCastVote -= CancelVote;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Get all INarratives in the scene, 
        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
        INarrative[] narratives = (from r_narratives in monoBehaviours where r_narratives.GetType().GetInterfaces().Any(k => k == typeof(INarrative)) select (INarrative)r_narratives).ToArray();

        if (narratives.Length > 0)
            narratives[0].onPresentChoice += InitializeDecision;
    }

    public void RegisterPlayer(int playerId, PlayerVote playerVote)
    {
        idsToPlayerVotes.Add(playerId, playerVote);

        playerVote.onStartCastVote += RecordVote;
        playerVote.onFinishCastVote += SubmitVote;
        playerVote.onCancelCastVote += CancelVote;
    }

    // These two functions are just getting what the current voting status is like, players have to all hold buttons to submit
    private void RecordVote(int playerId, Choice choice)
    {
        // We don't care about the choice if it's not in the dictionary
        if (!choiceTallies.ContainsKey(choice))
            return;

        choiceTallies[choice]++;

        // Discard the last vote of that player
        PlayerVote playerVote = idsToPlayerVotes[playerId];
        if (choiceTallies.ContainsKey(playerVote.LastChoice))
            choiceTallies[playerVote.LastChoice]--;

        // Check to see what is currently highest voted
        CheckVote();
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
        majorityVote = differentValues ? highestVote.Key : Choice.Default;
        onUpdateMajorityVote?.Invoke(majorityVote);
    }

    private void CheckVoteUnanimous()
    {

    }

    // For these two methods we don't care what player voted or what they chose, just that they are holding and ready
    private void SubmitVote(int playerId, Choice choice)
    {
        playersReady++;

        if (playersReady == PlayerManager.Instance.PlayerCount)
        {
            StopAllCoroutines();
            onCastFinalVote?.Invoke(majorityVote);
        }
    }

    private void CancelVote(int playerId, Choice choice)
    {
        playersReady = playersReady > 0 ? playersReady - 1 : 0;
    }

    private void InitializeDecision(Decision decision)
    {
        if (decision.isTimed)
            StartCoroutine(TimedVote(voteTime));

        // Reset state for everything
        majorityVote = Choice.Default;
        onUpdateMajorityVote?.Invoke(majorityVote);
        playersReady = 0;

        // Create the dictionary
        choiceTallies = new Dictionary<Choice, int>();
        if (decision.choices != null)
        {
            for (int i = 0; i < decision.choices.Length; i++)
                choiceTallies.Add((Choice)i, 0);
        }


        foreach (PlayerVote playerVote in idsToPlayerVotes.Values)
            playerVote.ResetVote();

        // Time to vote!
        ShouldVote = true;
    }

    private IEnumerator TimedVote(float voteTime)
    {
        yield return new WaitForSeconds(voteTime);

        // Cast the final vote regardless of whether players are ready
        onCastFinalVote?.Invoke(majorityVote);
    }
}

public enum DecisionMode
{
    Unanimous,
    Majority,
    Percentile
}
