using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPriceLabelView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Image _image;

    private int _amount;
    private NumberFormatInfo _numberFormatInfo;

    public void Awake()
    {
        _numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
    }

    public void SetType(bool isGold)
    {
        _image.sprite = isGold ? SpritesProvider.Instance.GetGoldIcon() : SpritesProvider.Instance.GetCashIcon();
    }

    public int Value
    {
        get => _amount;
        set
        {
            _amount = value;
            _text.text = $"{value.ToString("# ### ##0", _numberFormatInfo)}";
        }
    }
}
