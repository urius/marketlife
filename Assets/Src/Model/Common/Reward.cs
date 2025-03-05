namespace Src.Model.Common
{
    public struct Reward
    {
        public int Amount;
        public RewardType Type;

        public Reward(int amount, RewardType type)
        {
            Amount = amount;
            Type = type;
        }

        public static Reward FromString(string rewardStr)
        {
            var result = new Reward();
            if (rewardStr.IndexOf('g') >= 0)
            {
                result.Type = RewardType.Gold;
                result.Amount = int.Parse(rewardStr.Split('g')[0]);
            }
            else if (rewardStr.IndexOf('e') >= 0)
            {
                result.Type = RewardType.Exp;
                result.Amount = int.Parse(rewardStr.Split('e')[0]);
            }
            else
            {
                result.Type = RewardType.Cash;
                result.Amount = int.Parse(rewardStr);
            }

            return result;
        }

        public string SerializeToString()
        {
            var postfix = string.Empty;
            switch (Type)
            {
                case RewardType.Gold:
                    postfix = "g";
                    break;
                case RewardType.Exp:
                    postfix = "e";
                    break;
            }
            return $"{Amount}{postfix}";
        }
    }

    public enum RewardType
    {
        None,
        Cash,
        Gold,
        Exp,
    }
}