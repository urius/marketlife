using System;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Humans;
using Src.Model.Missions;
using Src.Model.Popups;
using Src.Systems;
using UnityEngine;

namespace Src.Common
{
    public class Dispatcher
    {
        public Action<Vector3> CameraMoved = delegate { };
        public Action UIMouseMoved = delegate { };
        public Action UIGameViewMouseMoved = delegate { };
        public Action UIGameViewMouseDown = delegate { };
        public Action UIGameViewMouseUp = delegate { };
        public Action UIGameViewMouseClick = delegate { };
        public Action UIGameViewMouseEnter = delegate { };
        public Action UIGameViewMouseExit = delegate { };
        public Action UIBottomPanelPointerEnter = delegate { };
        public Action UIBottomPanelPointerExit = delegate { };
        public Action UIRequestBlockRaycasts = delegate { };
        public Action UIRequestUnblockRaycasts = delegate { };
        public Action UITopPanelLevelUpAnimationFinished = delegate { };
        public Action<bool> UITopPanelAddMoneyClicked = delegate { };
        public Action UITopPanelRequestOpenLeaderboardsClicked = delegate { };    
        public Action<Vector2, int> UIRequestAddCashFlyAnimation = delegate { };
        public Action<Vector2, int> UIRequestAddGoldFlyAnimation = delegate { };
        public Action<Vector2, int> UIRequestAddExpFlyAnimation = delegate { };    
        public Action UIDispatchBillboardClick = delegate { };
        public Action<Vector3, float> UIRequestMoveCamera = delegate { };
        public Action<SaveField> RequestMarkToSaveField = delegate { };
        public Action RequestTriggerSave = delegate { };    

        public Action<int> UIBottomPanelPlaceShelfClicked = delegate { };
        public Action<int> UIBottomPanelPlaceFloorClicked = delegate { };
        public Action<int> UIBottomPanelPlaceWallClicked = delegate { };
        public Action<int> UIBottomPanelPlaceWindowClicked = delegate { };
        public Action<int> UIBottomPanelPlaceDoorClicked = delegate { };
        public Action<int> UIBottomPanelWarehouseSlotClicked = delegate { };
        public Action UIBottomPanelExpandWarehouseClicked = delegate { };
        public Action<int> UIBottomPanelWarehouseQuickDeliverClicked = delegate { };
        public Action<int> UIBottomPanelWarehouseRemoveProductClicked = delegate { };
        public Action<FriendData> UIBottomPanelFriendClicked = delegate { };
        public Action UIBottomPanelInviteFriendsClicked = delegate { };
        public Action UIBottomPanelAutoPlaceClicked = delegate { };
        public Action<FriendShopActionId> UIBottomPanelFriendShopActionClicked = delegate { };
        public Action<FriendShopActionId> UIBottomPanelBuyFriendShopActionClicked = delegate { };
        public Action<Vector3> UIFriendShopBottomPanelUserViewCreated = delegate { };
        public Action UIMuteAudioClicked = delegate { };
        public Action UIMuteMusicClicked = delegate { };
        public Action UIScaleInClicked = delegate { };
        public Action UIScaleOutClicked = delegate { };

        public Action UIActionsRotateRightClicked = delegate { };
        public Action UIActionsRotateLeftClicked = delegate { };
        public Action UIActionsMoveClicked = delegate { };
        public Action UIActionsRemoveClicked = delegate { };
        public Action<bool> UIConfirmPopupResult = delegate { };

        public Action<bool> UIRequestBlinkMoney = delegate { };
        public Action<Vector2, bool, int> UIRequestFlyingPrice = delegate { };
        public Action<Vector2, string> UIRequestFlyingText = delegate { };
        public Action<Vector2, string, int> UIRequestFlyingProduct = delegate { };
        public Action<Vector2, int> UIRequestFlyingExp = delegate { };
        public Action UIRequestRemoveCurrentPopup = delegate { };
        public Action<RectTransform, Vector2, ProductConfig> UIOrderProductClicked = delegate { };
        public Action UIOrderPopupAppeared = delegate { };
        public Action<RectTransform, Vector2, int, ProductModel> UIRequestOrderProductAnimation = delegate { };
        public Action<ShelfContentPopupViewModel, int> UIShelfContentAddProductClicked = delegate { };
        public Action<ShelfContentPopupViewModel, int> UIShelfContentRemoveProductClicked = delegate { };
        public Action<int> UIWarehousePopupSlotClicked = delegate { };
        public Action<UpgradesPopupItemViewModelBase> UIUpgradePopupBuyClicked = delegate { };
        public Action TutorialActionPerformed = delegate { };
        public Action<BankConfigItem> UIBankItemClicked = delegate { };
        public Action<bool> UIBankAdsItemClicked = delegate { };
        public Action UIGetBonusButtonClicked = delegate { };
        public Action UIDailyMissionsButtonClicked = delegate { };
        public Action<DailyMissionModel, Vector3> UITakeDailyMissionRewardClicked = delegate { };
        public Action<Vector3[]> UIDailyBonusTakeClicked = delegate { };
        public Action<Vector3> UILevelUpShareClicked = delegate { };
        public Action<Vector3> UIOfflineReportShareClicked = delegate { };
        public Action UIShareSuccessCallback = delegate { };
        public Action<string> UIBillboardPopupApplyTextClicked = delegate { };
        public Action<int> UIDailyBonusDoubleClicked = delegate { };
        public Action UIOfflineReportPopupViewAdsClicked = delegate { };
        public Action<int> UIChangeCashDeskManHairClicked = delegate { };
        public Action<int> UIChangeCashDeskManGlassesClicked = delegate { };
        public Action<int> UIChangeCashDeskManDressClicked = delegate { };

        public Action<CustomerModel> CustomerAnimationEnded = delegate { };

        public Action<Vector2Int> MouseCellCoordsUpdated = delegate { };
        public Action RequestForceMouseCellPositionUpdate = delegate { };

        public Action BottomPanelFriendsClicked = delegate { };
        public Action BottomPanelWarehouseClicked = delegate { };
        public Action BottomPanelInteriorClicked = delegate { };
        public Action BottomPanelManageButtonClicked = delegate { };
        public Action BottomPanelBackClicked = delegate { };
        public Action BottomPanelInteriorShelfsClicked = delegate { };
        public Action BottomPanelInteriorFloorsClicked = delegate { };
        public Action BottomPanelInteriorWallsClicked = delegate { };
        public Action BottomPanelInteriorWindowsClicked = delegate { };
        public Action BottomPanelInteriorDoorsClicked = delegate { };

        public Action BottomPanlelFinishPlacingClicked = delegate { };
        public Action BottomPanelRotateRightClicked = delegate { };
        public Action BottomPanelRotateLeftClicked = delegate { };
        public Action NotifyNotEnoughtMoney = delegate { };

        public Action RequestShowAdvert = delegate { };
        public Action<string> RequestNotifyInactiveFriend = delegate { };

        public Action<bool> SaveStateChanged = delegate { };
        public Action<bool> TutorialSaveStateChanged = delegate { };

        public Action<string> JsIncomingMessage = delegate { };
        public Action<bool, SaveField> SaveCompleted = delegate { };
        public Action<bool> SaveExternalDataCompleted = delegate { };

        public Action<ProductModel> CustomerBuyProduct = delegate { };

        public Action<int> DailyMissionsSecondsLeftAmountUpdated = delegate { };

        private static Lazy<Dispatcher> _instance = new();
        public static Dispatcher Instance => _instance.Value;
    }
}
