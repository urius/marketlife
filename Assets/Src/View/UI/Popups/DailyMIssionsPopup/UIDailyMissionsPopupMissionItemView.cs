using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Popups.DailyMIssionsPopup
{
    public class UIDailyMissionsPopupMissionItemView : MonoBehaviour
    {
        public event Action<UIDailyMissionsPopupMissionItemView> TakeRewardClicked = delegate { };
        public event Action<UIDailyMissionsPopupMissionItemView> TakeRewardX2Clicked = delegate { };

        [SerializeField] private Image _missionIcon;
        [SerializeField] private TMP_Text _missionDescriptionText;
        [SerializeField] private TMP_Text _rewardAmountText;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private Button _takeRewardButton;
        [SerializeField] private TMP_Text _takeRewardButtonText;
        [SerializeField] private Button _takeRewardX2Button;
        [SerializeField] private TMP_Text _takeRewardX2ButtonText;
        [SerializeField] private RectTransform _progressTransform;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private RectTransform _progressLineTransform;

        public Transform RewardButtonTransform => _takeRewardButton.transform;
        public Transform RewardX2ButtonTransform => _takeRewardX2Button.transform;

        private void Awake()
        {
            _takeRewardButton.AddOnClickListener(OnTakeRewardClick);
            _takeRewardX2Button.AddOnClickListener(OnTakeRewardX2Click);
        }

        public void SetMissionIconSprite(Sprite sprite)
        {
            _missionIcon.sprite = sprite;
        }

        public void SetMissionDescription(string description)
        {
            _missionDescriptionText.text = description;
        }

        public void SetTakeButtonText(string description)
        {
            _takeRewardButtonText.text = description;
        }

        public void SetTakeX2ButtonText(string description)
        {
            _takeRewardX2ButtonText.text = description;
        }

        public void SetRewardAmount(int amount)
        {
            _rewardAmountText.text = $"+{FormattingHelper.ToCommaSeparatedNumber(amount)}";
        }

        public void SetRewardTextColor(Color color)
        {
            _rewardAmountText.color = color;
        }

        public void SetRewardIconSprite(Sprite sprite)
        {
            _rewardIcon.sprite = sprite;
        }

        public void SetProgressText(int current, int target)
        {
            _progressText.text = $"{FormattingHelper.ToCommaSeparatedNumber(current)}/{FormattingHelper.ToCommaSeparatedNumber(target)}";
        }

        public void SetProgress(float progress)
        {
            var scaleX = Mathf.Clamp(progress, 0f, 1f);
            var scale = _progressLineTransform.transform.localScale;
            scale.x = scaleX;
            _progressLineTransform.transform.localScale = scale;
        }

        public void SetTakeButtonVisibility(bool isVisible)
        {
            _takeRewardButton.gameObject.SetActive(isVisible);
        }
        
        public void SetTakeX2ButtonVisibility(bool isVisible)
        {
            _takeRewardX2Button.gameObject.SetActive(isVisible);
        }

        public void SetTakeButtonsInteractable(bool isInteractable)
        {
            _takeRewardButton.interactable = isInteractable;
            _takeRewardX2Button.interactable = isInteractable;
        }

        public void SetProgressVisibility(bool isVisible)
        {
            _progressTransform.gameObject.SetActive(isVisible);
        }

        private void OnTakeRewardClick()
        {
            TakeRewardClicked(this);
        }

        private void OnTakeRewardX2Click()
        {
            TakeRewardX2Clicked(this);
        }
    }
}
