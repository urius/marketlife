using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITextLocalizator : MonoBehaviour
{
    [SerializeField] private string _localizationKey;
    [SerializeField] private Text _text;
    [SerializeField] private TMP_Text _tmpText;

    public async void Start()
    {
        await GameStateModel.Instance.GameDataLoadedTask;

        var textStr = LocalizationManager.Instance.GetLocalization(_localizationKey);

        if (_text != null)
        {
            _text.text = textStr;
        }

        if (_tmpText != null)
        {
            _tmpText.text = textStr;
        }

        Destroy(this);
    }
}
