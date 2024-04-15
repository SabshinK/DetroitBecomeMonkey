using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public PlayerInputManager InputManager { get; private set; }

    public int PlayerCount => transform.childCount;

    public readonly Dictionary<int, PlayerInput> idsToPlayers = new Dictionary<int, PlayerInput>();

    private void Awake()
    {
        InputManager = GetComponent<PlayerInputManager>();

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }        
    }

    private void OnEnable()
    {
        InputManager.onPlayerJoined += RegisterPlayer;
    }

    private void OnDisable()
    {
        InputManager.onPlayerJoined -= RegisterPlayer;
    }

    private void RegisterPlayer(PlayerInput playerInput)
    {
        // Parent the new player instance to the player manager
        playerInput.transform.parent = transform;

        // Add the player and its id to the dictionary
        idsToPlayers.Add(transform.childCount, playerInput);

        // Initialize player vote
        PlayerVote playerVote = playerInput.GetComponent<PlayerVote>();
        playerVote.PlayerId = transform.childCount;
        VotingManager.Instance.RegisterPlayer(transform.childCount, playerVote);

        //Debug.Log(transform.childCount);
    }
}
