using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Shot Sequence", menuName = "ScriptableObjects/ShotSequence", order = 1)]
public class ShotSequence : ScriptableObject
{
    public Sprite shot;
    public string[] dialogue;

    public Decision decision;

    public ShotSequence previousSequence;
    public ShotSequence nextSequence;

    public bool HasDecision => !(decision.choices == null || decision.choices.Length == 0);
}

[Serializable]
public struct Decision
{
    public string[] choices;
    public ShotSequence[] consequences;

    [Space]
    public DecisionMode decisionMode;
    public bool isTimed;

    public Decision(DecisionMode decisionMode, bool isTimed, ShotSequence[] consequences, string[] choices)
    {
        this.choices = choices;
        this.decisionMode = decisionMode;
        this.consequences = consequences;
        this.isTimed = isTimed;
    }
}
