using Src.Common;

namespace Src.Commands
{
    public struct NotEnoughtMoneySequenceCommand
    {
        public void Execute(bool isGold)
        {
            var dispatcher = Dispatcher.Instance;

            dispatcher.UIRequestBlinkMoney(isGold);

            new OpenBankPopupCommand().Execute(isGold);
        }
    }
}
