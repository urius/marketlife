using System;
using System.Collections.Generic;
using System.Linq;
using Src.Common;
using Src.Model.Configs;

namespace Src.Model
{
    public class PlayerOfflineReportHolder
    {
        public static PlayerOfflineReportHolder Instance => _instance.Value;
        private static Lazy<PlayerOfflineReportHolder> _instance = new Lazy<PlayerOfflineReportHolder>();

        public UserOfflineReportModel PlayerOfflineReport { get; private set; }
        public void SetReport(UserOfflineReportModel report)
        {
            PlayerOfflineReport = report;
        }
    }

    public class UserOfflineReportModel
    {
        public readonly float CalculationHours;
        public readonly int CalculationMinutes;
        public readonly Dictionary<ProductConfig, int> SoldFromShelfs;
        public readonly Dictionary<ProductConfig, int> SoldFromWarehouse;
        public readonly int SellProfit;
        public readonly int ExpFromSell;
        public readonly bool HasSellInfo;
        public readonly bool HasPersonalInfo;
        public readonly bool HasActivityInfo;
        public readonly int UnwashesCleanedAmount;
        public readonly IEnumerable<GuestOfflineReportActionModel> GuestOfflineActionModels;

        public UserOfflineReportModel(
            float calculationHours,
            Dictionary<ProductConfig, int> soldFromShelfs,
            Dictionary<ProductConfig, int> soldFromWarehouse,
            int unwashesCleanedAmount,
            IEnumerable<GuestOfflineReportActionModel> guestOfflineActionModels)
        {
            CalculationHours = calculationHours;
            CalculationMinutes = (int)(60 * calculationHours);
            SoldFromShelfs = soldFromShelfs;
            SoldFromWarehouse = soldFromWarehouse;
            UnwashesCleanedAmount = unwashesCleanedAmount;
            GuestOfflineActionModels = guestOfflineActionModels;
            SellProfit = CalculateSellProfit(SoldFromWarehouse) + CalculateSellProfit(SoldFromShelfs);
            ExpFromSell = CalculationHelper.CalculateExpToAdd(SoldFromWarehouse) + CalculationHelper.CalculateExpToAdd(SoldFromShelfs);

            var hasSoldFromWarehouse = SoldFromWarehouse.Any(kvp => kvp.Value > 0);
            HasSellInfo = SoldFromShelfs.Any(kvp => kvp.Value > 0) || hasSoldFromWarehouse;
            HasPersonalInfo = (UnwashesCleanedAmount > 0) || hasSoldFromWarehouse;
            HasActivityInfo = guestOfflineActionModels.Any();
        }

        public bool IsEmpty => !HasSellInfo && !HasPersonalInfo && !HasActivityInfo; //TODO add activity info


        private int CalculateSellProfit(Dictionary<ProductConfig, int> soldProducts)
        {
            var result = 0;
            foreach (var kvp in soldProducts)
            {
                result += kvp.Key.GetSellPriceForAmount(kvp.Value);
            }
            return result;
        }
    }

    public class GuestOfflineReportActionModel
    {
        public readonly string UserId;
        public readonly IEnumerable<ProductModel> TakenProducts;
        public readonly IEnumerable<ProductModel> AddedProducts;
        public readonly int AddedUnwashes;

        public GuestOfflineReportActionModel(
            string userId,
            IEnumerable<ProductModel> takenProducts,
            IEnumerable<ProductModel> addedProducts,
            int addedUnwashes)
        {
            UserId = userId;
            TakenProducts = takenProducts;
            AddedProducts = addedProducts;
            AddedUnwashes = addedUnwashes;
        }
    }
}