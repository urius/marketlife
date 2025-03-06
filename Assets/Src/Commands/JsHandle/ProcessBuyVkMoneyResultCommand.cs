using Newtonsoft.Json;

namespace Src.Commands.JsHandle
{
    public struct ProcessBuyVkMoneyResultCommand
    {
        public void Execute(string message)
        {
            var deserialized = JsonConvert.DeserializeObject<BuyVkMoneyResultDto>(message);

            new ProcessBuyChargedBankItemResultCommand().Execute(deserialized.data.is_success);
        }
    }

    public struct BuyVkMoneyResultDto
    {
        public string command;
        public BuyVkMoneyResultPayloadDto data;
    }

    public struct BuyVkMoneyResultPayloadDto
    {
        public bool is_success;
        public string order_id;
        public bool is_user_cancelled;
    }
}