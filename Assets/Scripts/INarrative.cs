using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INarrative
{
    public delegate void ChoiceEvent(string[] choices);
    public event ChoiceEvent onPresentChoice;
}
