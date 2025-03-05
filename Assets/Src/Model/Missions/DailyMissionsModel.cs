using System;
using System.Collections.Generic;

namespace Src.Model.Missions
{
    public class DailyMissionsModel
    {
        public event Action<DailyMissionModel> MissionAdded = delegate { };
        public event Action<DailyMissionModel> MissionRemoved = delegate { };

        private readonly List<DailyMissionModel> _missionsList = new List<DailyMissionModel>();

        public DailyMissionsModel()
        {
        }

        public IReadOnlyList<DailyMissionModel> MissionsList => _missionsList;

        public void AddMission(DailyMissionModel mission)
        {
            _missionsList.Add(mission);
            MissionAdded(mission);
        }

        public void RemoveMission(DailyMissionModel mission)
        {
            if (_missionsList.Remove(mission))
            {
                MissionRemoved(mission);
            }
        }

        public void Clear()
        {
            while (_missionsList.Count > 0)
            {
                RemoveMission(_missionsList[0]);
            }
        }
    }
}
