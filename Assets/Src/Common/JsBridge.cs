using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

namespace Src.Common
{
    public class JsBridge : MonoBehaviour
    {
        private Dispatcher _dispatcher;

        [DllImport("__Internal")]
        private static extern void SendToJs(string str);

        public static JsBridge Instance { get; private set; }

        public void Awake()
        {
            Debug.Log("JsBridge Awaken");

            Instance = this;
            _dispatcher = Dispatcher.Instance;
        }

        public void JsCommandMessage(string payload)
        {
            _dispatcher.JsIncomingMessage(payload);
        }

        public void SendCommandToJs(string command, object payload)
        {
            Debug.Log("Unity->Js: command = " + command);

            var dto = new UnityToJsCommonCommandDto()
            {
                command = command,
                payload = payload,
            };
            var dtoStr = JsonConvert.SerializeObject(dto);
            SendToJs(dtoStr);
        }
    }

    public struct UnityToJsCommonCommandDto
    {
        public string command;
        public object payload;
    }
}