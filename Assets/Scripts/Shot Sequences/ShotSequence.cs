using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shot Sequence", menuName = "ScriptableObjects/ShotSequence", order = 1)]
public class ShotSequence : ScriptableObject
{
    public Sprite shot;
    public string[] dialogue;

    public string[] choices;
}
