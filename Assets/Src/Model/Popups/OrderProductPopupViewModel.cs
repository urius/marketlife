using System;
using System.Collections.Generic;
using Src.Model.Configs;

namespace Src.Model.Popups
{
    public class OrderProductPopupViewModel : PopupViewModelBase
    {
        public event Action<int> TabSelected = delegate { };

        public override PopupType PopupType => PopupType.OrderProduct;

        public int SelectedTabIndex { get; private set; } = 0;
        public int TargetWarehouseSlotIndex { get; private set; }
        public int[] TabIds { get; private set; }
        public Dictionary<int, ProductConfig[]> ProductsByGroupId { get; private set; }

        public void Setup(int targetWarehouseSlotIndex, int[] tabIds, Dictionary<int, ProductConfig[]> productsByGroupId)
        {
            TargetWarehouseSlotIndex = targetWarehouseSlotIndex;
            TabIds = tabIds;
            ProductsByGroupId = productsByGroupId;
        }

        public ProductConfig[] GetProductsByTabIndex(int tabIndex)
        {
            if (tabIndex < TabIds.Length)
            {
                return ProductsByGroupId[TabIds[tabIndex]];
            }
            return new ProductConfig[0];
        }

        public void OnViewTabClicked(int tabIndex)
        {
            SetSelectedTabIndex(tabIndex);
        }

        private void SetSelectedTabIndex(int tabIndex)
        {
            SelectedTabIndex = tabIndex;
            TabSelected(SelectedTabIndex);
        }
    }
}
