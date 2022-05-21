using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    // Create Cards


    [SerializeField] CardBase[] cardBases;
    [SerializeField] Card cardPrefab;

    public Card Spawn(int number)
    {

        Card card = Instantiate(cardPrefab);
        card.name = cardBases[number].Name + "Card";
        card.Set(cardBases[number]);
        return card;
    }
}
