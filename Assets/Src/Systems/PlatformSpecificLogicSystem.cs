using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using Src.Systems.PlatformModules;
using UnityEngine;

namespace Src.Systems
{
    public class PlatformSpecificLogicSystem
    {
        private readonly Dispatcher _dispatcher = Dispatcher.Instance;
        //
        private PlatformSpecificLogicModuleBase _module;
        private PlayerModelHolder _playerModelHolder;
        private LocalizationManager _localizationManager;

        public void Start()
        {
            _playerModelHolder = PlayerModelHolder.Instance;
            _localizationManager = LocalizationManager.Instance;

            SetupDisabledLogicFlags();
            
            Activate();
        }

        private void SetupDisabledLogicFlags()
        {
            if (MirraSdkWrapper.IsYandexGames)
            {
                DisabledLogicFlags.IsFriendsLogicDisabled = true;
            }
        }

        private void Activate()
        {
            if (MirraSdkWrapper.IsMirraSdkUsed)
            {
                SetupMirraPlatform();
            }
            else
            {
                _dispatcher.JsIncomingMessage += OnJsIncomingMessage;
            }
#if UNITY_EDITOR
            InitDebugEditorModule();
#endif
        }

        private void SetupMirraPlatform()
        {
            InitViaMirraSdk().Forget();
        }

        private async UniTaskVoid InitViaMirraSdk()
        {
            MirraSdkWrapper.Log("InitViaMirraSdk");
            MirraSdkWrapper.Log("CurrentLanguage: " + MirraSdkWrapper.CurrentLanguage);

            var playerId = await MirraSdkWrapper.GetPlayerId();
            MirraSdkWrapper.Log("PlayerId: " + playerId);

            SetupLanguage();

            if (MirraSdkWrapper.IsYandexGames)
            {
                Urls.UpdateBasePathPostfix("/marketYG");
                _playerModelHolder.SetInitialData(playerId, SocialType.YG);
            }
        }

        private void SetupLanguage()
        {
            if (MirraSdkWrapper.IsRussianLanguage)
            {
                _localizationManager.SetLocale(LocaleType.Ru);
            }
            else if (MirraSdkWrapper.IsEnglishLanguage)
            {
                _localizationManager.SetLocale(LocaleType.En);
            }
        }

        private void OnJsIncomingMessage(string message)
        {
            var deserialized = JsonConvert.DeserializeObject<JsCommonCommandDto>(message);
            Debug.Log($"Incoming Js command {message}");
            switch (deserialized.command)
            {
                case "SetVkPlatformData":
                    InitModuleVK();
                    SetVkPlatformDataCommand.Execute(message);
                    break;
            }
        }

        private void InitDebugEditorModule()
        {
            _module = new DebugEditorLogicModule();
            _module.Start();
        }

        private void InitModuleVK()
        {
            _module = new VkLogicModule();
            _module.Start();
        }
    }

    public struct JsCommonCommandDto
    {
        public string command;
    }
}