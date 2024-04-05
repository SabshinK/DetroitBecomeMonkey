using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinPrompt : MonoBehaviour
{
    private Animator anim;
    private TMP_Text text;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        PlayerManager.Instance.InputManager.onPlayerJoined += NextPrompt;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.InputManager.onPlayerJoined -= NextPrompt;
    }

    private void NextPrompt(PlayerInput playerInput)
    {
        if (PlayerManager.Instance.PlayerCount == 2)
        {
            //anim.SetTrigger("All Joined");
            text.text = "All Players Press and Hold Any Button to Continue";
        }
    }
}
