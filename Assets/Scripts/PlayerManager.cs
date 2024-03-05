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

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        InputManager.onPlayerJoined += ParentPlayer;
    }

    private void OnDisable()
    {
        InputManager.onPlayerJoined -= ParentPlayer;
    }

    private void ParentPlayer(PlayerInput playerInput)
    {
        playerInput.transform.parent = transform;
    }
}
