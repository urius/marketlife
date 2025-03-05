using Src.Managers;
using Src.Model;

namespace Src.Commands
{
    public struct RotatePlacingObjectCommand
    {
        public void Execute(bool isRightRotation)
        {
            var gameStateModel = GameStateModel.Instance;
            var audioManager = AudioManager.Instance;
            if (gameStateModel.PlacingShopObjectModel != null)
            {
                gameStateModel.PlacingShopObjectModel.Side += isRightRotation ? 1 : -1;
                audioManager.PlaySound(SoundNames.Rotate);
            }
        }
    }
}
