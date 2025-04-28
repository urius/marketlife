using Newtonsoft.Json;
using Src.Commands.JsHandle;
using Src.Common;
using Src.Managers;
using Src.Systems.PlatformModules;
using UnityEngine;

namespace Src.Systems
{
    public class PlatformSpecificLogicSystem
    {
        private readonly Dispatcher _dispatcher = Dispatcher.Instance;
        //
        private PlatformSpecificLogicModuleBase _module;
        private LocalizationManager _localizationManager;

        public void Start()
        {
            _localizationManager = LocalizationManager.Instance;
            
            Activate();
        }

        private void Activate()
        {
            if (MirraSdkWrapper.IsMirraSdkUsed)
            {
                SetupMirraPlatform();
                return;
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
            MirraSdkWrapper.Log("InitViaMirraSdk");
            MirraSdkWrapper.Log("CurrentLanguage: " + MirraSdkWrapper.CurrentLanguage);
            
            SetupLanguage();

            if (MirraSdkWrapper.IsYandexGames)
            {
                _module = new MirraYandexGamesLogicModule();
                _module.Start();
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
                    InitModuleVk();
                    SetVkPlatformDataCommand.Execute(message);
                    break;
            }
        }

        private void InitDebugEditorModule()
        {
            _module = new DebugEditorLogicModule();
            _module.Start();
        }

        private void InitModuleVk()
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