namespace Src.Model.Common
{
    public struct Price
    {
        public int Value;
        public bool IsGold;

        public Price(int value, bool isGold)
        {
            Value = value;
            IsGold = isGold;
        }

        public static Price FromString(string str)
        {
            var value = int.Parse(str.Split('g')[0]);
            var isGoldPrice = str.IndexOf('g') != -1;

            return new Price(value, isGoldPrice);
        }

        public static Price operator *(Price price, int i)
        {
            return new Price(price.Value * i, price.IsGold);
        }
    }
}
