using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.Left_Panel
{
    public class UIDailyMissionsButtonView : MonoBehaviour
    {
        public event Action OnClick = delegate { };

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _notificationPointImage;
        [SerializeField] private Image _notificationCheckImage;

        private Vector2 _defaultAnchoredPosition;
        private CancellationTokenSource _jumpingCts;

        private void Awake()
        {
            _defaultAnchoredPosition = _rectTransform.anchoredPosition;
            
            SetNotificationVisibility(false);
            SetNotificationCheckVisibility(false);
            _button.AddOnClickListener(OnButtonClicked);
        }

        public void SetIconImageAlpha(float alpha)
        {
            _iconImage.color = _iconImage.color.SetAlpha(alpha);
        }

        public void SetNotificationVisibility(bool isVisible)
        {
            _notificationPointImage.gameObject.SetActive(isVisible);
        }

        public void SetNotificationColor(Color color)
        {
            _notificationPointImage.color = color;
        }

        public void SetNotificationCheckVisibility(bool isVisible)
        {
            _notificationCheckImage.gameObject.SetActive(isVisible);
        }

        public void SetJumpAnimationState(bool isEnabled)
        {
            if (isEnabled)
            {
                EnableJumping();
            }
            else
            {
                DisableJumping();
            }
        }
 
        private void EnableJumping()
        {
            DisableJumping();
            _jumpingCts = new CancellationTokenSource();

            HandleJumpAnimation(_jumpingCts.Token).Forget();
        }

        private void DisableJumping()
        {
            _jumpingCts?.Cancel();
            _jumpingCts = null;
            _rectTransform.anchoredPosition = _defaultAnchoredPosition;
        }

        private async UniTaskVoid HandleJumpAnimation(CancellationToken stopToken)
        {
            if (stopToken.IsCancellationRequested) return;

            await LeanTweenHelper.BounceYAsync(_rectTransform, 50, stopToken).SuppressCancellationThrow();
            
            if (stopToken.IsCancellationRequested) return;

            await UniTask.Delay(1000, cancellationToken: stopToken).SuppressCancellationThrow();
            
            if (stopToken.IsCancellationRequested) return;

            HandleJumpAnimation(stopToken).Forget();
        }

        private void OnButtonClicked()
        {
            OnClick();
        }
    }
}
