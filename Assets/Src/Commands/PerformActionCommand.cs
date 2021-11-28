using System;
using UnityEngine;

public struct PerformActionCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var mouseCellCoords = MouseDataProvider.Instance.MouseCellCoords;
        var audioManager = AudioManager.Instance;

        var actionResult = false;
        switch (gameStateModel.ActionState)
        {
            case ActionStateName.PlacingNewShopObject:
                actionResult = PlaceNewShopObject();
                break;
            case ActionStateName.MovingShopObject:
                actionResult = PlaceMovingShopObject();
                break;
            case ActionStateName.PlacingNewFloor:
                actionResult = PlaceNewDecoration(mouseCellCoords, ShopDecorationObjectType.Floor, gameStateModel.PlacingDecorationNumericId);
                break;
            case ActionStateName.PlacingNewWall:
                actionResult = PlaceNewDecoration(mouseCellCoords, ShopDecorationObjectType.Wall, gameStateModel.PlacingDecorationNumericId);
                break;
            case ActionStateName.PlacingNewWindow:
                actionResult = PlaceNewDecoration(mouseCellCoords, ShopDecorationObjectType.Window, gameStateModel.PlacingDecorationNumericId);
                break;
            case ActionStateName.MovingWindow:
                actionResult = PlaceMovingDecoration(mouseCellCoords, ShopDecorationObjectType.Window, gameStateModel.PlacingDecorationNumericId);
                break;
            case ActionStateName.PlacingNewDoor:
                actionResult = PlaceNewDecoration(mouseCellCoords, ShopDecorationObjectType.Door, gameStateModel.PlacingDecorationNumericId);
                break;
            case ActionStateName.MovingDoor:
                actionResult = PlaceMovingDecoration(mouseCellCoords, ShopDecorationObjectType.Door, gameStateModel.PlacingDecorationNumericId);
                break;
            case ActionStateName.PlacingProductPlayer:
            case ActionStateName.PlacingProductFriend:
                actionResult = PlaceProductOnHighlightedShelf();
                break;
            case ActionStateName.FriendShopTakeProduct:
                actionResult = TakeProductOnFriendHighlightedShelf();
                break;
            case ActionStateName.FriendShopAddUnwash:
                actionResult = AddUnwashOnFriendsShop();
                break;
        }

        if (actionResult)
        {
            if (gameStateModel.ActionState == ActionStateName.PlacingProductPlayer)
            {
                audioManager.PlaySound(SoundNames.ProductPut);
            }
            else
            {
                audioManager.PlaySound(SoundNames.Place);
            }
        }
    }

    private bool AddUnwashOnFriendsShop()
    {
        var result = false;

        var playerModel = PlayerModelHolder.Instance.UserModel;
        var viewingUserModel = GameStateModel.Instance.ViewingUserModel;
        var viewingShopModel = viewingUserModel.ShopModel;
        var friendActionsDataModel = playerModel.FriendsActionsDataModels.GetFriendShopActionsModel(viewingUserModel.Uid);
        var gameStateModel = GameStateModel.Instance;
        var mouseCellCoordsProvider = MouseDataProvider.Instance;
        var actionId = FriendShopActionId.AddUnwash;
        var actionData = friendActionsDataModel.ActionsById[actionId];
        var screenCalculator = ScreenCalculator.Instance;
        var dispatcher = Dispatcher.Instance;
        var coords = mouseCellCoordsProvider.MouseCellCoords;
        var loc = LocalizationManager.Instance;

        if (viewingShopModel.ShopDesign.IsCellInside(coords))
        {
            if (viewingShopModel.Unwashes.ContainsKey(coords))
            {
                var screenCoords = screenCalculator.CellToScreenPoint(coords);
                dispatcher.UIRequestFlyingText(screenCoords, loc.GetLocalization(LocalizationKeys.FlyingTextUnwashAlreadyAdded));
            }
            else if (viewingShopModel.Grid.TryGetValue(coords, out var gridItem)
                && gridItem.buildState > 0)
            {
                var screenCoords = screenCalculator.CellToScreenPoint(coords);
                dispatcher.UIRequestFlyingText(screenCoords, loc.GetLocalization(LocalizationKeys.FlyingTextUnwashCantBePlaced));
            }
            else if (viewingShopModel.AddUnwash(coords, 1))
            {
                var cleanerEndWorkTime = viewingUserModel.ShopModel.PersonalModel.GetMaxEndWorkTimeForPersonalType(PersonalType.Cleaner);
                if (cleanerEndWorkTime < gameStateModel.ServerTime)
                {
                    viewingUserModel.ExternalActionsModel.AddAction(new ExternalActionAddUnwash(playerModel.Uid, coords));
                }
                actionData.SetAmount(actionData.RestAmount - 1);
                result = true;
            }

            if (actionData.RestAmount <= 0)
            {
                ResetActionData(viewingUserModel.Uid, actionId);
                gameStateModel.ResetActionState();
            }
        }
        else
        {
            var screenCoords = screenCalculator.CellToScreenPoint(coords);
            dispatcher.UIRequestFlyingText(screenCoords, loc.GetLocalization(LocalizationKeys.FlyingTextUnwashCantBePlacedOutside));
        }

        return result;
    }

    private bool TakeProductOnFriendHighlightedShelf()
    {
        var result = false;

        var dispatcher = Dispatcher.Instance;
        var gameStateModel = GameStateModel.Instance;
        var viewingUserModel = gameStateModel.ViewingUserModel;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var playerWarehouseModel = playerModel.ShopModel.WarehouseModel;
        var playerActionsDataModel = playerModel.FriendsActionsDataModels.GetFriendShopActionsModel(viewingUserModel.Uid);
        var highlightState = gameStateModel.HighlightState;
        var highlightedShopObject = highlightState.HighlightedShopObject;
        var actionId = FriendShopActionId.TakeProduct;
        var actionData = playerActionsDataModel.ActionsById[actionId];
        var screenCalculator = ScreenCalculator.Instance;
        var loc = LocalizationManager.Instance;

        if (highlightState.IsHighlighted
            && highlightedShopObject != null
            && highlightedShopObject.Type == ShopObjectType.Shelf
            && actionData.RestAmount > 0)
        {
            var shelf = highlightedShopObject as ShelfModel;
            var hasProduct = false;
            foreach (var slot in shelf.Slots)
            {
                if (slot.HasProduct)
                {
                    hasProduct = true;
                    var productConfig = slot.Product.Config;
                    var amountToTake = 1;
                    var takenAmount = playerWarehouseModel.AddProduct(productConfig, amountToTake);
                    if (takenAmount > 0)
                    {
                        var securityEndWorkTime = viewingUserModel.ShopModel.PersonalModel.GetMaxEndWorkTimeForPersonalType(PersonalType.Security);
                        slot.ChangeProductAmount(-amountToTake);
                        if (securityEndWorkTime < gameStateModel.ServerTime)
                        {
                            viewingUserModel.ExternalActionsModel.AddAction(new ExternalActionTakeProduct(playerModel.Uid, shelf.Coords, productConfig, amountToTake));
                        }
                        actionData.SetAmount(actionData.RestAmount - 1);
                        break;
                    }
                    else
                    {
                        gameStateModel.ResetActionState();
                        var screenCoords = screenCalculator.CellToScreenPoint(shelf.Coords);
                        dispatcher.UIRequestFlyingText(screenCoords, loc.GetLocalization(LocalizationKeys.FlyingTextWarehouseDoesntHaveFreeSpace));
                        break;
                    }
                }
            }

            if (hasProduct == false)
            {
                var screenCoords = screenCalculator.CellToScreenPoint(shelf.Coords);
                dispatcher.UIRequestFlyingText(screenCoords, loc.GetLocalization(LocalizationKeys.FlyingTextNothingToTakeFromShelf));
            }
        }

        if (actionData.RestAmount <= 0)
        {
            ResetActionData(viewingUserModel.Uid, actionId);
            gameStateModel.ResetActionState();
        }

        return result;
    }

    private void ResetActionData(string friendUId, FriendShopActionId actionId)
    {
        var gameStateModel = GameStateModel.Instance;
        var actionsDataModel = PlayerModelHolder.Instance.UserModel.FriendsActionsDataModels.GetFriendShopActionsModel(friendUId);
        var actionsConfig = GameConfigManager.Instance.FriendActionsConfig;

        actionsDataModel.ActionsById[actionId].SetEndCooldownTime(actionsConfig.GetDefaultActionCooldownMinutes(actionId) * 60 + gameStateModel.ServerTime);
        actionsDataModel.ActionsById[actionId].SetAmount(actionsConfig.GetDefaultActionAmount(actionId));
    }

    private bool PlaceProductOnHighlightedShelf()
    {
        var result = false;
        var dispatcher = Dispatcher.Instance;
        var gameStateModel = GameStateModel.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var viewingUserModel = gameStateModel.ViewingUserModel;
        var viewingShopModel = gameStateModel.ViewingShopModel;
        var highlightState = gameStateModel.HighlightState;
        var highlightedShopObject = highlightState.HighlightedShopObject;
        var loc = LocalizationManager.Instance;
        var screenCalculator = ScreenCalculator.Instance;
        var audioManager = AudioManager.Instance;
        var isOnFriendShop = gameStateModel.GameState == GameStateName.ShopFriend;

        if (highlightState.IsHighlighted
            && highlightedShopObject != null
            && highlightedShopObject.Type == ShopObjectType.Shelf)
        {
            var shelf = highlightedShopObject as ShelfModel;
            string flyingTextToShow = null;

            var placingProductSlot = playerModel.ShopModel.WarehouseModel.Slots[gameStateModel.PlacingProductWarehouseSlotIndex];
            if (placingProductSlot.HasProduct)
            {
                for (var i = 0; i < shelf.PartsCount; i++)
                {
                    flyingTextToShow = null;

                    var placedAmount = new PutWarehouseProductOnShelfCommand().Execute(gameStateModel.PlacingProductWarehouseSlotIndex, viewingUserModel, shelf, i);
                    result = placedAmount > 0;
                    if (placedAmount > 0)
                    {
                        break;
                    }
                    else
                    {
                        flyingTextToShow = loc.GetLocalization(LocalizationKeys.FlyingTextShelfDoesntHaveFreeSpace);
                    }
                }
            }

            if (placingProductSlot.HasProduct == false)
            {
                gameStateModel.ResetActionState();
            }

            if (flyingTextToShow != null)
            {
                dispatcher.UIRequestFlyingText(screenCalculator.CellToScreenPoint(shelf.Coords), flyingTextToShow);
                audioManager.PlaySound(SoundNames.Negative1);
            }
        }

        return result;
    }

    private bool PlaceNewShopObject()
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
        var shopObject = gameStateModel.PlacingShopObjectModel;
        var screenCalculator = ScreenCalculator.Instance;
        var dispatcher = Dispatcher.Instance;
        var localiationManager = LocalizationManager.Instance;
        var audioManager = AudioManager.Instance;

        var screenCoords = screenCalculator.CellToScreenPoint(shopObject.Coords);
        if (shopModel.CanPlaceShopObject(shopObject))
        {
            var price = shopObject.Price;
            if (TrySpendMoneyOrBlink(price))
            {
                dispatcher.UIRequestFlyingPrice(screenCoords, price.IsGold, -price.Value);

                var clonedShopObject = gameStateModel.PlacingShopObjectModel.Clone();
                shopModel.PlaceShopObject(clonedShopObject);
                return true;
            }
            else
            {
                gameStateModel.ResetActionState();
                dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextInsufficientFunds));
            }
        }
        else
        {
            dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextWrongPlace));
            audioManager.PlaySound(SoundNames.Negative1);
        }
        return false;
    }

    private bool PlaceMovingShopObject()
    {
        var gameStateModel = GameStateModel.Instance;
        var shopObject = gameStateModel.PlacingShopObjectModel;
        var shopModel = gameStateModel.ViewingShopModel;
        var audioManager = AudioManager.Instance;
        var screenCalculator = ScreenCalculator.Instance;
        var dispatcher = Dispatcher.Instance;
        var screenCoords = screenCalculator.CellToScreenPoint(shopObject.Coords);
        var localiationManager = LocalizationManager.Instance;

        if (shopModel.CanPlaceShopObject(shopObject))
        {
            shopModel.PlaceShopObject(shopObject);
            gameStateModel.ResetActionState();
            return true;
        }
        else
        {
            dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextWrongPlace));
            audioManager.PlaySound(SoundNames.Negative1);
        }
        return false;
    }

    private bool PlaceNewDecoration(Vector2Int coords, ShopDecorationObjectType decorationType, int numericId)
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
        var mainConfig = GameConfigManager.Instance.MainConfig;
        var screenCalculator = ScreenCalculator.Instance;
        var decorationConfig = mainConfig.GetDecorationConfigBuNumericId(decorationType, numericId);
        var dispatcher = Dispatcher.Instance;
        var localiationManager = LocalizationManager.Instance;
        var audioManager = AudioManager.Instance;

        var screenCoords = screenCalculator.CellToScreenPoint(coords);
        if (shopModel.CanPlaceDecoration(decorationType, coords, numericId))
        {
            var price = decorationConfig.Price;
            if (TrySpendMoneyOrBlink(price))
            {
                dispatcher.UIRequestFlyingPrice(screenCoords, price.IsGold, -price.Value);
                return shopModel.TryPlaceDecoration(decorationType, coords, numericId);
            }
            else
            {
                gameStateModel.ResetActionState();
                dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextInsufficientFunds));
            }
        }
        else
        {
            dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextWrongPlace));
            audioManager.PlaySound(SoundNames.Negative1);
        }

        return false;
    }

    private bool PlaceMovingDecoration(Vector2Int coords, ShopDecorationObjectType decorationType, int numericId)
    {
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;

        var screenCalculator = ScreenCalculator.Instance;
        var dispatcher = Dispatcher.Instance;
        var screenCoords = screenCalculator.CellToScreenPoint(coords);
        var localiationManager = LocalizationManager.Instance;
        var audioManager = AudioManager.Instance;

        if (shopModel.TryPlaceDecoration(decorationType, coords, numericId))
        {
            gameStateModel.ResetActionState();
            return true;
        }
        else
        {
            dispatcher.UIRequestFlyingText(screenCoords, localiationManager.GetLocalization(LocalizationKeys.FlyingTextWrongPlace));
            audioManager.PlaySound(SoundNames.Negative1);
        }
        return false;
    }

    private bool TrySpendMoneyOrBlink(Price price)
    {
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var shopModel = playerModel.ShopModel;
        if (playerModel.TrySpendMoney(price))
        {
            return true;
        }
        else
        {
            Dispatcher.Instance.UIRequestBlinkMoney(price.IsGold);
            return false;
        }
    }
}
