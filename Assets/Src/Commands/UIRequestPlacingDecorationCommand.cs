using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Common;

namespace Src.Commands
{
    public struct UIRequestPlacingDecorationCommand
    {
        public void Execute(ShopDecorationObjectType decorationType, int numericId)
        {
            var gameStateModel = GameStateModel.Instance;
            var audioManager = AudioManager.Instance;
            if (gameStateModel.ActionState != ActionStateName.None) return;

            Dispatcher.Instance.RequestForceMouseCellPositionUpdate();
            switch (decorationType)
            {
                case ShopDecorationObjectType.Floor:
                    var floorConfig = GameConfigManager.Instance.MainConfig.GetFloorConfigByNumericId(numericId);
                    if (CheckCanSpendOrNotEnoughtMoneySequence(floorConfig.Price))
                    {
                        gameStateModel.SetPlacingFloor(numericId);
                    }
                    break;
                case ShopDecorationObjectType.Wall:
                    var wallConfig = GameConfigManager.Instance.MainConfig.GetWallConfigByNumericId(numericId);
                    if (CheckCanSpendOrNotEnoughtMoneySequence(wallConfig.Price))
                    {
                        gameStateModel.SetPlacingWall(numericId);
                    }
                    break;
                case ShopDecorationObjectType.Window:
                    var windowConfig = GameConfigManager.Instance.MainConfig.GetWindowConfigByNumericId(numericId);
                    if (CheckCanSpendOrNotEnoughtMoneySequence(windowConfig.Price))
                    {
                        gameStateModel.SetPlacingWindow(numericId);
                    }
                    break;
                case ShopDecorationObjectType.Door:
                    var doorConfig = GameConfigManager.Instance.MainConfig.GetDoorConfigByNumericId(numericId);
                    if (CheckCanSpendOrNotEnoughtMoneySequence(doorConfig.Price))
                    {
                        gameStateModel.SetPlacingDoor(numericId);
                    }
                    break;
            }
        }

        private bool CheckCanSpendOrNotEnoughtMoneySequence(Price price)
        {
            var playerModel = PlayerModelHolder.Instance.UserModel;
            if (playerModel.CanSpendMoney(price))
            {
                return true;
            }
            else
            {
                new NotEnoughtMoneySequenceCommand().Execute(price.IsGold);
                return false;
            }
        }
    }
}
