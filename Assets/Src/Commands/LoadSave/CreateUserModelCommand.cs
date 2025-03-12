using Newtonsoft.Json;
using Src.Model;
using Src.Net;

namespace Src.Commands.LoadSave
{
    public struct CreateUserModelCommand
    {
        public UserModel Execute(string userDataStr)
        {
            UserModel result = null;
            if (userDataStr != null)
            {
                var deserializedData = JsonConvert.DeserializeObject<FullUserDataDto>(userDataStr);
                
                result = DataImporter.Instance.Import(deserializedData);
            }

            return result;
        }
    }
}
