using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class Card : MonoBehaviour
{
    // This is Visual part of the cards
    [SerializeField] Text nameText;
    [SerializeField] Text numberText;
    [SerializeField] Image icon;
    [SerializeField] Text description;
    [SerializeField] GameObject hidePanel;

    public CardBase Base { get; private set; }
    public UnityAction<Card> OnClickCard;

    public void Set(CardBase cardBase, bool isEnemy)
    {
        Base = cardBase;
        nameText.text = cardBase.Name;
        numberText.text = cardBase.Number.ToString();
        icon.sprite = cardBase.Icon;
        description.text = cardBase.Description;
        hidePanel.SetActive(isEnemy);
    }

    public void OnClick()
    {
        OnClickCard?.Invoke(this);
    }

    public void OnPonterEnter()
    {
        transform.localScale =  Vector3.one * 1.1f;
        transform.position += Vector3.up * 0.3f;
        GetComponentInChildren<Canvas>().sortingLayerName = "overlay";
    }

    public void OnPointerExit()
    {
        transform.localScale = Vector3.one;
        transform.position -= Vector3.up * 0.3f;
        GetComponentInChildren<Canvas>().sortingLayerName = "Default";

    }

    public void Open()
    {
        if(hidePanel.activeSelf)
            StartCoroutine(OpenAnim());
    }

    IEnumerator OpenAnim()
    {
        yield return  transform.DORotate(new Vector3(0, 90, 0), 0.2f).WaitForCompletion();
        hidePanel.SetActive(false);
        yield return transform.DORotate(new Vector3(0, 0, 0), 0.2f).WaitForCompletion();

    }
}
