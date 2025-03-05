using System.Collections.Generic;

namespace Src.Systems.PlatformModules
{
    public class DebugEditorLogicModule : PlatformSpecificLogicModuleBase
    {
        private readonly SocialUsersData _socialUsersData = SocialUsersData.Instance;
        private readonly AvatarsManager _avatarsManager = AvatarsManager.Instance;
        private readonly MockDataProvider _mockDataProvider = MockDataProvider.Instance;

        public override void Start()
        {
            new SetupDebugSystemsCommand().Execute();
            new SetVkFriendsDataCommand().Execute(_mockDataProvider.GetMockFriends());
            
            Activate();
        }

        private void Activate()
        {
            _socialUsersData.NewUidsRequested += OnNewUidsRequested;
        }

        private void OnNewUidsRequested()
        {
            var result = new List<UserSocialData>();
            foreach (var uid in _socialUsersData.RequestedUids)
            {
                _avatarsManager.SetupAvatarSettings(uid, _mockDataProvider.GetMockAvatarUrl());
                result.Add(new UserSocialData(uid, $"-{uid}-", "Lastname", null));
            }

            _socialUsersData.FillRequestedSocialData(result);
        }
    }
}