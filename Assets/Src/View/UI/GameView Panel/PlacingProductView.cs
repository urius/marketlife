using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacingProductView : MonoBehaviour
{
    [SerializeField] private Image _productImage;
    [SerializeField] private TMP_Text _amountText;

    public void SetImageSprite(Sprite sprite)
    {
        _productImage.sprite = sprite;
    }

    public void SetText(string text)
    {
        _amountText.text = text;
    }
}
