using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Card : MonoBehaviour
{
    // This is Visual part of the cards
    [SerializeField] Text nameText;
    [SerializeField] Text numberText;
    [SerializeField] Image icon;
    [SerializeField] Text description;


    public CardBase Base { get; private set; }
    public UnityAction<Card> OnClickCard;

    public void Set(CardBase cardBase)
    {
        Base = cardBase;
        nameText.text = cardBase.Name;
        numberText.text = cardBase.Number.ToString();
        icon.sprite = cardBase.Icon;
        description.text = cardBase.Description;

    }

    public void OnClick()
    {
        OnClickCard?.Invoke(this);
    }
}
