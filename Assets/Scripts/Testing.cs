using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Testing : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public UnityEvent onPresentChoice;

    private void Start()
    {
        StartCoroutine(TestNarrative());
    }

    private IEnumerator TestNarrative()
    {
        text.text = "ooo aah luffy image";

        yield return new WaitForSeconds(1f);

        text.text = "what's he doing???";

        yield return new WaitForSeconds(1f);

        text.text = "now it's time to choose: A or B?";

        yield return new WaitForSeconds(0.5f);

        onPresentChoice?.Invoke();
    }
}
