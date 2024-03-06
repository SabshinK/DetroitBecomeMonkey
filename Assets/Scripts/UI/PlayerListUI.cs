using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerListUI : MonoBehaviour
{
    [SerializeField] private GameObject playerJoinedPrefab;

    private void OnEnable()
    {
        PlayerManager.Instance.InputManager.onPlayerJoined += CreatePlayerUI;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.InputManager.onPlayerJoined -= CreatePlayerUI;
    }

    private void CreatePlayerUI(PlayerInput playerInput)
    {
        GameObject playerJoined = GameObject.Instantiate(playerJoinedPrefab, transform);
        PlayerJoinedInfoUI infoScript = playerJoined.GetComponent<PlayerJoinedInfoUI>();

        infoScript.RegisterPlayer(playerInput, transform.childCount);
    }
}
