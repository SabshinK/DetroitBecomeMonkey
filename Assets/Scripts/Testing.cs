using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Testing : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image img;
    [SerializeField] private Sprite enel;

    [SerializeField] bool randomizeTiebreaker;

    public UnityEvent onPresentChoice;

    private void OnEnable()
    {
        VotingManager.Instance.onCastFinalVote += (Choice choice) => { StartCoroutine(ChoiceHandler(choice)); };
    }

    private void Start()
    {
        StartCoroutine(TestNarrative());
    }

    private IEnumerator ChoiceHandler(Choice choice)
    {
        if (choice == Choice.A)
            text.text = "You chose A!";
        else if (choice == Choice.B)
            text.text = "You chose B!";
        else
        {
            if (randomizeTiebreaker)
            {
                int tiebraker = Random.Range(0, 2);
                text.text = $"Randomized Tiebreaker! The computer chose {(Choice)tiebraker}";
            }
            else
                text.text = "Secret third option??";
        }

        yield return new WaitForSeconds(3f);

        StartCoroutine(TestNarrative2());
    }

    private IEnumerator TestNarrative()
    {
        text.text = "ooo aah luffy image";

        yield return new WaitForSeconds(2f);

        text.text = "what's he doing???";

        yield return new WaitForSeconds(2f);

        text.text = "now it's time to choose: A or B?";

        yield return new WaitForSeconds(2f);

        onPresentChoice?.Invoke();
    }

    private IEnumerator TestNarrative2()
    {
        img.sprite = enel;

        text.text = "omg what";

        yield return new WaitForSeconds(2f);

        text.text = "rubber do not conduct electricity lmao";

        yield return new WaitForSeconds(2f);

        text.text = "choose again: A or B?";

        yield return new WaitForSeconds(2f);

        onPresentChoice?.Invoke();
    }
}
