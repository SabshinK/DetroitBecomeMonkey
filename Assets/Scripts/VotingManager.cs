using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class VotingManager : MonoBehaviour
{
    public static VotingManager Instance { get; private set; }

    public UnityEvent<Choice> onUpdateVote;
    
    [SerializeField] private Testing testing;
    
    private Dictionary<Choice, int> choiceTallies;
    
    private bool shouldVote;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        choiceTallies = new Dictionary<Choice, int>();
    }

    private void OnEnable()
    {
        testing.onPresentChoice.AddListener(() => { shouldVote = true; });

        Transform playerParent = PlayerManager.Instance.transform;
        foreach (Transform player in playerParent)
        {
            VotingChoice playerVote = player.GetComponent<VotingChoice>();
            playerVote.onCastVote.AddListener(RecordVote);
            playerVote.onCancelVote.AddListener(DiscardVote);
        }
    }

    private void RecordVote(Choice choice)
    {
        if (shouldVote)
        {
            // This should be done somewhere else, there should be a function that sets the dictionary when a choice is presented
            if (!choiceTallies.ContainsKey(choice))
                choiceTallies.Add(choice, 0);

            choiceTallies[choice]++;
            Debug.Log($"{choice}: {choiceTallies[choice]}");

            CheckVote();
        }
    }

    private void DiscardVote(Choice choice)
    {
        if (shouldVote)
        {
            choiceTallies[choice]--;
            Debug.Log($"{choice}: {choiceTallies[choice]}");
        }
    }

    private void CheckVote()
    {
        // Find the current highest vote
        KeyValuePair<Choice, int> highestVote = choiceTallies.ToArray()[0];
        foreach (KeyValuePair<Choice, int> vote in choiceTallies)
        {
            if (vote.Value > highestVote.Value)
                highestVote = vote;
        }

        // Broadcast the current highest vote
        onUpdateVote?.Invoke(highestVote.Key);
    }
}
