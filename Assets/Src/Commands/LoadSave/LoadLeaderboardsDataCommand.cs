using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using Src.Common_Utils;
using Src.Model;
using Src.Model.Leaderboards;
using Src.Net;

namespace Src.Commands.LoadSave
{
    public struct LoadLeaderboardsDataCommand
    {
        public async UniTask<bool> ExecuteAsync()
        {
            var leaderboardsDataHolder = LeaderboardsDataHolder.Instance;
            
            if (leaderboardsDataHolder.IsLeaderboardsSet) return leaderboardsDataHolder.IsLeaderboardsSet;
            
            if (MirraSdkWrapper.IsMirraSdkUsed)
            {
                await LoadLeaderboardDataFromPlatform();
            }
            else
            {
                await LoadLeaderboardDataFromServer();
            }

            return leaderboardsDataHolder.IsLeaderboardsSet;
        }

        private async UniTask LoadLeaderboardDataFromPlatform()
        {
            var leaderboardsDataHolder = LeaderboardsDataHolder.Instance;
            
            var scoreTableDataExp = await MirraSdkWrapper.GetScoreTable(Constants.ScoreTagExp);
            //var scoreTableDataCash = await MirraSdkWrapper.GetScoreTable(Constants.ScoreTagCash);
            //var scoreTableDataGold = await MirraSdkWrapper.GetScoreTable(Constants.ScoreTagGold);
            
            var tempLBItemsConverted = ConvertLeaderboardItems(scoreTableDataExp);
            leaderboardsDataHolder.SetLeaderboardData(LeaderboardType.Exp, tempLBItemsConverted);
        }

        private LeaderboardUserData[] ConvertLeaderboardItems(MirraLeaderboardUserData[] scoreTableDataExp)
        {
            var leaderboardUserData = new LeaderboardUserData[scoreTableDataExp.Length];
            for (var i = 0; i < scoreTableDataExp.Length; i++)
            {
                var item = scoreTableDataExp[i];
                leaderboardUserData[i] = new LeaderboardUserData(
                    null, item.Rank, item.Score, item.PlayerName, item.PictureURL);
            }

            return leaderboardUserData;
        }

        private async UniTask LoadLeaderboardDataFromServer()
        {
            var socialUsersData = SocialUsersData.Instance;
            var playerUid = PlayerModelHolder.Instance.Uid;
            var url = string.Format(Urls.GetLeaderboardsURL, playerUid);

            var resultOperation = await new WebRequestsSender().GetAsync(url);
            if (resultOperation.IsSuccess)
            {
                var responseDto = JsonConvert.DeserializeObject<CommonResponseDto>(resultOperation.Result);
                if (responseDto.response != null
                    && ResponseValidator.Validate(responseDto))
                {
                    var requestedDataFilledTcs = new UniTaskCompletionSource();

                    void OnRequestedDataFilled()
                    {
                        requestedDataFilledTcs.TrySetResult();
                    }

                    var deserialized = JsonConvert.DeserializeObject<LeaderboardsResponseDto>(responseDto.response);
                    var expItems = ConvertToItemsDto(deserialized.exp);
                    var friendsItems = ConvertToItemsDto(deserialized.friends);
                    var cashItems = ConvertToItemsDto(deserialized.cash);
                    var goldItems = ConvertToItemsDto(deserialized.gold);
                    var uids = GetUids(expItems, friendsItems, cashItems, goldItems);
                    socialUsersData.RequestedDataFilled += OnRequestedDataFilled;
                    socialUsersData.AddRequestedUids(uids);
                    await requestedDataFilledTcs.Task;
                    socialUsersData.RequestedDataFilled -= OnRequestedDataFilled;

                    SetupLeaderboards(expItems, friendsItems, cashItems, goldItems);
                }
            }
        }

        private void SetupLeaderboards(LeaderboardItemDto[] expDtos, LeaderboardItemDto[] friendsDtos, LeaderboardItemDto[] cashDtos, LeaderboardItemDto[] goldDtos)
        {
            var leaderboardsDataHolder = LeaderboardsDataHolder.Instance;

            var tempLBItemsConverted = ConvertLeaderboardItems(expDtos);
            leaderboardsDataHolder.SetLeaderboardData(LeaderboardType.Exp, tempLBItemsConverted);

            tempLBItemsConverted = ConvertLeaderboardItems(friendsDtos);
            leaderboardsDataHolder.SetLeaderboardData(LeaderboardType.Friends, tempLBItemsConverted);

            tempLBItemsConverted = ConvertLeaderboardItems(cashDtos);
            leaderboardsDataHolder.SetLeaderboardData(LeaderboardType.Cash, tempLBItemsConverted);

            tempLBItemsConverted = ConvertLeaderboardItems(goldDtos);
            leaderboardsDataHolder.SetLeaderboardData(LeaderboardType.Gold, tempLBItemsConverted);
        }

        private LeaderboardUserData[] ConvertLeaderboardItems(LeaderboardItemDto[] dtos)
        {
            var socialUsersData = SocialUsersData.Instance;
            return dtos
                .Where(d => socialUsersData.Contains(d.Uid))
                .Select(ToLeaderboardUserData)
                .ToArray();
        }

        private LeaderboardUserData ToLeaderboardUserData(LeaderboardItemDto itemDto, int positionIndex)
        {
            var socialData = SocialUsersData.Instance.GetUserData(itemDto.Uid);
            return new LeaderboardUserData(
                itemDto.Uid, positionIndex + 1, itemDto.Value, $"{socialData.FirstName} {socialData.LastName}", null);
        }

        private IEnumerable<string> GetUids(params LeaderboardItemDto[][] items)
        {
            return items.SelectMany(items => items.Select(i => i.Uid));
        }

        public LeaderboardItemDto[] ConvertToItemsDto(string[] itemsStr)
        {
            var result = new LeaderboardItemDto[itemsStr.Length];
            for (var i = 0; i < itemsStr.Length; i++)
            {
                result[i] = ConvertToItemDto(itemsStr[i]);
            }
            return result;
        }

        public LeaderboardItemDto ConvertToItemDto(string itemStr)
        {
            var splitted = itemStr.Split(':');
            return new LeaderboardItemDto
            {
                Uid = splitted[0],
                Value = int.Parse(splitted[1]),
            };
        }
    }

    public struct LeaderboardsResponseDto
    {
        public string[] cash;
        public string[] gold;
        public string[] exp;
        public string[] friends;
    }

    public struct LeaderboardItemDto
    {
        public string Uid;
        public int Value;
    }
}