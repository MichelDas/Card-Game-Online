using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlerHand : MonoBehaviour
{
    List<Card> list = new List<Card>();

    public bool IsEmpty { get => list.Count == 0; }

    public void Add(Card card)
    {
        list.Add(card);
        card.transform.SetParent(transform);
    }

    public void Remove(Card card)
    {
        list.Remove(card);
    }

    // 手札を整頓する
    public void ResetPosition()
    {
        // Sort：Numberの小さい順に並べる
        list.Sort((card0, card1) => card0.Base.Number - card1.Base.Number);
        for (int i = 0; i < list.Count; i++)
        {
            float posX = (i - list.Count/2f) * 1.4f;
            list[i].transform.localPosition = new Vector3(posX, 0);
        }
    }

    public Card RandomRemove()
    {
        int r = Random.Range(0, list.Count);
        Card card = list[r];
        Remove(card);
        return card;
    }

    public Card RemoveCardOfNumber(int number)
    {
        Card card = list.Find(x => x.Base.Number == number);
        Remove(card);
        return card;
    } 

}
