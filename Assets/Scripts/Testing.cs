using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Testing : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction transAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        transAction = playerInput.actions.FindAction("Transition");
    }

    private void OnEnable()
    {
        transAction.performed += NextScene;
    }

    private void OnDisable()
    {
        transAction.performed -= NextScene;
    }

    private void NextScene(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(1);
    }
}
