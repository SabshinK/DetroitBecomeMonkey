using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INarrative
{
    public delegate void ChoiceEvent(Decision decision);
    public event ChoiceEvent onPresentChoice;
}
