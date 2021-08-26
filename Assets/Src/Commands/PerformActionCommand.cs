using System;
using UnityEngine;

public struct PerformActionCommand
{
    public void Execute()
    {
        var gameStateModel = GameStateModel.Instance;
        var mouseCellCoords = MouseCellCoordsProvider.Instance.MouseCellCoords;
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
            case ActionStateName.PlacingProduct:
                actionResult = PlaceProductOnHighlightedShelf();
                break;
            case ActionStateName.FriendShopTakeProduct:
                actionResult = TakeProductOnFriendHighlightedShelf();
                break;
        }

        if (actionResult)
        {
            if (gameStateModel.ActionState == ActionStateName.PlacingProduct)
            {
                audioManager.PlaySound(SoundNames.ProductPut);
            }
            else
            {
                audioManager.PlaySound(SoundNames.Place);
            }
        }
    }

    private bool TakeProductOnFriendHighlightedShelf()
    {
        var result = false;

        var dispatcher = Dispatcher.Instance;
        var playerModel = PlayerModelHolder.Instance.UserModel;
        var playerActionsDataModel = playerModel.ActionsDataModel;
        var gameStateModel = GameStateModel.Instance;
        var highlightState = gameStateModel.HighlightState;
        var highlightedShopObject = highlightState.HighlightedShopObject;
        var viewingUserModel = gameStateModel.ViewingUserModel;
        var actionId = FriendShopActionId.Take;
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
                    var takenAmount = playerModel.ShopModel.WarehouseModel.AddProduct(productConfig, amountToTake);
                    if (takenAmount > 0)
                    {
                        slot.ChangeProductAmount(-amountToTake);
                        actionData.SetAmount(actionData.RestAmount - 1);
                        viewingUserModel.ExternalActionsModel.AddAction(new ExternalActionTake(playerModel.Uid, shelf.Coords, productConfig, amountToTake));
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
            ResetActionData(FriendShopActionId.Take);
            gameStateModel.ResetActionState();
        }

        return result;
    }

    private void ResetActionData(FriendShopActionId actionId)
    {
        var gameStateModel = GameStateModel.Instance;
        var actionsDataModel = PlayerModelHolder.Instance.UserModel.ActionsDataModel;
        var mainConfig = GameConfigManager.Instance.MainConfig;

        actionsDataModel.ActionsById[actionId].SetEndCooldownTime(mainConfig.ActionDefaultCooldownMinutes * 60 + gameStateModel.ServerTime);
        actionsDataModel.ActionsById[actionId].SetAmount(mainConfig.ActionDefaultAmount);
    }

    private bool PlaceProductOnHighlightedShelf()
    {
        var result = false;
        var dispatcher = Dispatcher.Instance;
        var gameStateModel = GameStateModel.Instance;
        var shopModel = gameStateModel.ViewingShopModel;
        var highlightState = gameStateModel.HighlightState;
        var highlightedShopObject = highlightState.HighlightedShopObject;
        var loc = LocalizationManager.Instance;
        var screenCalculator = ScreenCalculator.Instance;
        var audioManager = AudioManager.Instance;

        if (highlightState.IsHighlighted
            && highlightedShopObject != null
            && highlightedShopObject.Type == ShopObjectType.Shelf)
        {
            var shelf = highlightedShopObject as ShelfModel;
            var placingProduct = shopModel.WarehouseModel.Slots[gameStateModel.PlacingProductWarehouseSlotIndex].Product;
            string flyingTextToShow = null;
            for (var i = 0; i < shelf.PartsCount; i++)
            {
                flyingTextToShow = null;
                if (shelf.TryGetProductAt(i, out var productOnShelf))
                {
                    if (productOnShelf.NumericId == placingProduct.NumericId)
                    {
                        var neededAmount = Math.Min(shelf.GetRestAmountOn(i), placingProduct.Amount);
                        if (neededAmount > 0)
                        {
                            productOnShelf.Amount += neededAmount;
                            placingProduct.Amount -= neededAmount;
                            result = true;
                            break;
                        }
                        else
                        {
                            flyingTextToShow = loc.GetLocalization(LocalizationKeys.FlyingTextShelfDoesntHaveFreeSpace);
                        }
                    }
                    else
                    {
                        flyingTextToShow = loc.GetLocalization(LocalizationKeys.FlyingTextShelfDoesntHaveFreeSpace);
                    }
                }
                else
                {
                    var neededAmount = Math.Min(shelf.PartVolume / placingProduct.Config.Volume, placingProduct.Amount);
                    var productToPlace = new ProductModel(placingProduct.Config, neededAmount);
                    if (shelf.TrySetProductOn(i, productToPlace))
                    {
                        placingProduct.Amount -= neededAmount;
                        result = true;
                        break;
                    }
                }
            }

            if (placingProduct.Amount <= 0)
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
