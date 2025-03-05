using System;
using System.Collections.Generic;
using System.Linq;

namespace Src.Model
{
    public class SocialUsersData
    {
        public static SocialUsersData Instance = new SocialUsersData();

        public event Action NewUidsRequested = delegate { };
        public event Action RequestedDataFilled = delegate { };

        private string[] _requestedUids;
        private Dictionary<string, UserSocialData> _data = new Dictionary<string, UserSocialData>();

        public string[] RequestedUids => _requestedUids;

        public void AddRequestedUids(IEnumerable<string> uids)
        {
            var newUids = uids.Where(uid => _data.ContainsKey(uid) == false);
            var uniqueUidsToAdd = new HashSet<string>(newUids);
            _requestedUids = uniqueUidsToAdd.ToArray();
            NewUidsRequested();
        }

        public void FillRequestedSocialData(IEnumerable<UserSocialData> usersData)
        {
            foreach (var userData in usersData)
            {
                _data[userData.Uid] = userData;
            }
            _requestedUids = null;
            RequestedDataFilled();
        }

        public UserSocialData GetUserData(string uid)
        {
            _data.TryGetValue(uid, out var result);
            return result;
        }

        public bool Contains(string uid)
        {
            return _data.ContainsKey(uid);
        }
    }
}
