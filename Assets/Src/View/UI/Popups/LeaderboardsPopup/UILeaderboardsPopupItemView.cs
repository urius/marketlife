using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.LeaderboardsPopup
{
    public class UILeaderboardsPopupItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _rankText;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private Image _bgImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private Image _typeImage;

        public void SetRankText(string text)
        {
            _rankText.text = text;
        }

        public void SetAvatar(Sprite sprite)
        {
            _avatarImage.sprite = sprite;
        }

        public void SetNameText(string text)
        {
            _nameText.text = text;
        }

        public void SetValueText(string text)
        {
            _valueText.text = text;
        }

        public void SetTypeImageSprite(Sprite sprite)
        {
            _typeImage.sprite = sprite;
        }

        public void SetBgImageColor(Color color)
        {
            _bgImage.color = color;
        }
    }
}
