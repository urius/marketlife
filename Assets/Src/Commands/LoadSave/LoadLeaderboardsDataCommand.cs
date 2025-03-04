using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;

public struct LoadLeaderboardsDataCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var leaderboardsDataHolder = LeaderboardsDataHolder.Instance;
        if (leaderboardsDataHolder.IsLeaderboardsSet == false)
        {
            var scocialUsersData = SocialUsersData.Instance;
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
                    scocialUsersData.RequestedDataFilled += OnRequestedDataFilled;
                    scocialUsersData.AddRequestedUids(uids);
                    await requestedDataFilledTcs.Task;
                    scocialUsersData.RequestedDataFilled -= OnRequestedDataFilled;

                    SetupLeaderboards(expItems, friendsItems, cashItems, goldItems);
                }
            }
        }

        return leaderboardsDataHolder.IsLeaderboardsSet;
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
            .Select((d, i) => new LeaderboardUserData(i + 1, d.Value, socialUsersData.GetUserData(d.Uid)))
            .ToArray();
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
