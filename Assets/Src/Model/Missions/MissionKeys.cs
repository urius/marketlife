namespace Src.Model.Missions
{
    public static class MissionKeys
    {
        public const string AddFriends = "add_friends";
        public const string AddShelfs = "add_shelfs";
        public const string AddGold = "add_gold";
        public const string AddCash = "add_cash";
        public const string ChangeBillboard = "change_billboard";
        public const string ChangeCashman = "change_cashman";
        public const string GiftToFriend = "gift_to_friend";
        public const string RepaintFloors = "repaint_floors";
        public const string RepaintWalls = "repaint_walls";
        public const string SellProduct = "sell_product";
        public const string CleanUnwashes = "clean_unwashes";
        public const string VisitFriends = "visit_friends";
        public const string ExpandShop = "expand_shop";
        public const string UpgradeWarehouseVolume = "upgrade_wh_volume";
        public const string AddWarehouseCells = "add_wh_cells";
        
        public static bool IsFriendsRelatedMission(string key)
        {
            return key is GiftToFriend or AddFriends or VisitFriends;
        } 
    }
}
