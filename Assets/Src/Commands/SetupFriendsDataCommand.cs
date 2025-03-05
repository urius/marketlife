using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using Src.Common_Utils;
using Src.Managers;
using Src.Model;
using Src.Net;

namespace Src.Commands
{
    public struct SetupFriendsDataCommand
    {
        private List<GetVisitTimesResponseItemDto> _deserializedDataDto;

        public async void Execute(IEnumerable<FriendData> friendDatas)
        {
            var friendsDataHolder = FriendsDataHolder.Instance;
            var avatarsManager = AvatarsManager.Instance;
            var analyticsManager = AnalyticsManager.Instance;
            var friendDatasArray = friendDatas.ToArray();

            await LoadLastVisitTimesAsync(friendDatasArray);

            foreach (var friendData in friendDatasArray)
            {
                avatarsManager.SetupAvatarSettings(friendData.Uid, friendData.Picture50Url);
            }

            friendsDataHolder.SetupFriendsData(friendDatasArray);

            analyticsManager.SetupMetaParameter(AnalyticsManager.NumFriendsParamName, friendDatasArray.Length);
            analyticsManager.SetupMetaParameter(AnalyticsManager.NumAppFriendsParamName, friendDatas.Where(f => f.IsApp).Count());
        }

        private async UniTask LoadLastVisitTimesAsync(FriendData[] friendDatasArray)
        {
            var ids = friendDatasArray.Select(f => f.Uid).ToArray();
            var idsStr = string.Join(",", ids);
            var url = string.Format(Urls.GetVisitTimesURL, idsStr);
            var resultOperation = await new WebRequestsSender().GetAsync(url);
            if (resultOperation.IsSuccess)
            {
                var responseDto = JsonConvert.DeserializeObject<CommonResponseDto>(resultOperation.Result);
                if (responseDto.response != null)
                {
                    _deserializedDataDto = JsonConvert.DeserializeObject<List<GetVisitTimesResponseItemDto>>(responseDto.response);
                    var visitTimesDict = _deserializedDataDto.ToDictionary(d => d.uid);
                    foreach (var friendData in friendDatasArray)
                    {
                        if (visitTimesDict.TryGetValue(friendData.Uid, out var visitTimeValue))
                        {
                            friendData.SetLastVisitTime(visitTimeValue.last_visit_time);
                        }
                    }
                }
            }
        }
    }

    public struct GetVisitTimesResponseItemDto
    {
        public string uid;
        public int last_visit_time;
    }
}