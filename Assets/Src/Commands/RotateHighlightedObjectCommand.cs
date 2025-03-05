using Src.Managers;
using Src.Model;

namespace Src.Commands
{
    public struct RotateHighlightedObjectCommand
    {
        public void Execute(int deltaSide)
        {
            var gameStateMOdel = GameStateModel.Instance;
            var shopModel = gameStateMOdel.ViewingShopModel;
            var higlightState = gameStateMOdel.HighlightState;
            var audioManager = AudioManager.Instance;

            if (higlightState.HighlightedShopObject != null)
            {
                var shopObjectModel = higlightState.HighlightedShopObject;
                if (shopModel.CanRotateShopObject(shopObjectModel, deltaSide))
                {
                    shopModel.RotateShopObject(shopObjectModel, deltaSide);
                    audioManager.PlaySound(SoundNames.Rotate);
                }
                else if (shopModel.CanRotateShopObject(shopObjectModel, 2 * deltaSide))
                {
                    shopModel.RotateShopObject(shopObjectModel, 2 * deltaSide);
                    audioManager.PlaySound(SoundNames.Rotate);
                }
                else
                {
                    //TODO: "Can't rotate" Sound
                }
            }
        }
    }
}
