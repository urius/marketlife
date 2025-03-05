using Src.Commands.LoadSave;
using Src.Common;
using Src.Managers;
using Src.View.Gameplay.Human;
using UnityEngine;
using UnityEngine.UI;

namespace Src.Debug_Scripts
{
    public class debugFaceAnimations : MonoBehaviour
    {
        [SerializeField] private Button _loadHumanButton;
        [SerializeField] private Button _changeSideButton;
        [SerializeField] private Text _changeSideButtonText;

        [SerializeField] private Button _switchAnimationButton;
        [SerializeField] private Text _animationIndexText;
        [SerializeField] private int _animationIndexMax;
        [SerializeField] private Button _switchFaceAnimationButton;
        [SerializeField] private Text _faceAnimationIndexText;
        [SerializeField] private int _faceAnimationIndexMax;

        [SerializeField] private Button _hairButton;
        [SerializeField] private Text _hairIndexText;
        private int _hairNumMax;
        [SerializeField] private Button _glassesButton;
        [SerializeField] private Text _glassesIndexText;
        private int _glassesNumMax;
        [SerializeField] private Button _topClothesButton;
        [SerializeField] private Text _topClothesIndexText;
        private int _topClothesNumMax;
        [SerializeField] private Button _bottomClothesButton;
        [SerializeField] private Text _bottomClothesIndexText;
        private int _bottomClothesNumMax;

        private string _animationParamName = "animation_index";

        private int _animationIndex = 0;
        private int _faceAnimationIndex = 0;
        private int _hairNum = 0;
        private int _glassesNum = 0;
        private int _topClothesNum = 0;
        private int _bottomClothesNum = 0;    
        private int _side = 0;

        async void Start()
        {
            Debug.Log("debugFaceAnimations start");

            await new LoadConfigsCommand().ExecuteAsync();
            await new LoadAssetsCommand().ExecuteAsync();

            _hairNumMax = GraphicsManager.Instance.GetAllIncrementiveSprites(SpriteAtlasId.GameplayAtlas, SpritesProvider.HUMAN_HAIR_PREFIX).Length;
            _glassesNumMax = GraphicsManager.Instance.GetAllIncrementiveSprites(SpriteAtlasId.GameplayAtlas, SpritesProvider.HUMAN_GLASSES_PREFIX).Length;
            _topClothesNumMax = GraphicsManager.Instance.GetAllIncrementiveSprites(SpriteAtlasId.GameplayAtlas, SpritesProvider.CLOTHES_PREFIX).Length;
            _bottomClothesNumMax = GraphicsManager.Instance.GetAllIncrementiveSprites(SpriteAtlasId.GameplayAtlas, HumanView.FOOT_CLOTHES_PREFIX).Length;

            _loadHumanButton.onClick.AddListener(OnLoadHumanClicked);
            _changeSideButton.onClick.AddListener(OnChangeSideClicked);

            _switchAnimationButton.onClick.AddListener(OnSwitchAnimationClicked);
            _switchFaceAnimationButton.onClick.AddListener(OnSwitchFaceAnimationClicked);

            _hairButton.onClick.AddListener(OnHairButtonClicked);
            _glassesButton.onClick.AddListener(OnGlassesButtonClicked);
            _topClothesButton.onClick.AddListener(OnTopClothesButtonClicked);
            _bottomClothesButton.onClick.AddListener(OnBottomClothesButtonClicked);
        }

        private void OnLoadHumanClicked()
        {
            var headGO = Instantiate(PrefabsHolder.Instance.Human);
            new DebugHumanMediator(headGO);
        }

        private void OnSwitchAnimationClicked()
        {
            _animationIndex++;
            if (_animationIndex > _animationIndexMax)
            {
                _animationIndex = 0;
            }
            _animationIndexText.text = "animation:" + _animationIndex;

            DebugDispatcher.Instance.ShowHumanAnimation(_animationIndex);
        }

        private void OnChangeSideClicked()
        {
            _side++;
            if (_side > 4) _side = 1;
            _changeSideButtonText.text = "side:" + _side;

            DebugDispatcher.Instance.ShowHumanSide(_side);
        }

        private void OnSwitchFaceAnimationClicked()
        {
            _faceAnimationIndex++;
            if (_faceAnimationIndex > _faceAnimationIndexMax)
            {
                _faceAnimationIndex = 0;
            }
            _faceAnimationIndexText.text = "animation:" + _faceAnimationIndex;

            DebugDispatcher.Instance.ShowFaceAnimation(_faceAnimationIndex);
        }

        private void OnHairButtonClicked()
        {
            _hairNum++;
            if (_hairNum > _hairNumMax)
            {
                _hairNum = 1;
            }
            _hairIndexText.text = "hair:" + _hairNum;

            DebugDispatcher.Instance.ShowHair(_hairNum);
        }

        private void OnGlassesButtonClicked()
        {
            _glassesNum++;
            if (_glassesNum > _glassesNumMax)
            {
                _glassesNum = 1;
            }
            _glassesIndexText.text = "glasses:" + _glassesNum;

            DebugDispatcher.Instance.ShowGlasses(_glassesNum);
        }

        private void OnTopClothesButtonClicked()
        {
            _topClothesNum++;
            if (_topClothesNum > _topClothesNumMax)
            {
                _topClothesNum = 1;
            }
            _topClothesIndexText.text = "top clothes:" + _topClothesNum;

            DebugDispatcher.Instance.ShowTopClothes(_topClothesNum);
        }

        private void OnBottomClothesButtonClicked()
        {
            _bottomClothesNum++;
            if (_bottomClothesNum > _bottomClothesNumMax)
            {
                _bottomClothesNum = 1;
            }
            _bottomClothesIndexText.text = "bottom clothes:" + _bottomClothesNum;

            DebugDispatcher.Instance.ShowBottomClothes(_bottomClothesNum);
        }
    }
}
