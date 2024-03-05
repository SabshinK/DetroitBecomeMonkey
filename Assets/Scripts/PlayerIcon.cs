using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    [SerializeField] private Image iconSprite;
    [SerializeField] private TMP_Text text;

    private int playerId;

    private void Awake()
    {
        // Nothing here ...
    }

    public void RegisterPlayer(int playerId, Color iconColor)
    {
        this.playerId = playerId;
        text.text = playerId.ToString();
        iconSprite.color = iconColor;
    }
}
