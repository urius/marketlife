using System;
using Src.Commands;
using Src.Managers;
using Src.Model;
using Src.Model.Debug;
using UnityEditor;
using UnityEngine;

namespace Src.Common
{
    public class InitScript : MonoBehaviour
    {
        [SerializeField] private string _debugUid;
        [SerializeField] private bool _disableSave;
        [SerializeField] private bool _disableTutorial;
        [SerializeField] private bool _loadDebugConfigVersion;
        [Space(20)]
        [SerializeField] private PrefabsHolder _prefabsHolder;
        [SerializeField] private GraphicsManager _graphicsManager;
        [SerializeField] private LocalizationManager _localizationManager;
        [SerializeField] private ColorsHolder _colorsHolder;
        [SerializeField] private BuiltinAssetsHolderSo _builtinAssetsHolder;

        private void Awake()
        {
            Application.targetFrameRate = 51;

            new InitializeSystemsCommand().Execute();

            Debug.Log($"Application.absoluteURL: {Application.absoluteURL}");
        }

        private async void Start()
        {
            await PlayerModelHolder.Instance.SetUidTask;
            await new InitializeAndLoadCommand().ExecuteAsync();
        }

        private void OnValidate()
        {
            DebugDataHolder.Instance.DebugUid = _debugUid;
            DebugDataHolder.Instance.IsSaveDisabled = _disableSave;
            DebugDataHolder.Instance.UseTestConfigFile = _loadDebugConfigVersion;
            DebugDataHolder.Instance.IsTutorialDisabled = _disableTutorial;
        }

        [ContextMenu("ResetLocalData")]
        private void ResetLocalData()
        {
            MirraSdkWrapper.DeleteKey(Constants.PlayerDataKey);
        }
    }
}
