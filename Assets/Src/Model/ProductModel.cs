
public class ProductModel
{
    public readonly int ProductId;

    public int Amount = 1;
    public int Time = -1;

    public ProductModel(int productId)
    {
        ProductId = productId;
    }

    public ProductModel(int productId, int amount) : this(productId)
    {
        Amount = amount;
    }

    public ProductModel(int productId, int amount, int time) : this(productId, amount)
    {
        Time = time;
    }
}
