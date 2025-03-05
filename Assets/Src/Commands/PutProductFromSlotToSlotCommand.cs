using System;
using Src.Common;
using Src.Model;

namespace Src.Commands
{
    public struct PutProductFromSlotToSlotCommand
    {
        public int Execute(ProductSlotModel fromSlot, ProductSlotModel toSlot)
        {
            var gameStateModel = GameStateModel.Instance;
            var placedProductsCount = 0;

            if (fromSlot.HasProduct && fromSlot.Product.DeliverTime <= gameStateModel.ServerTime)
            {
                var placingProduct = fromSlot.Product;
                if (toSlot.HasProduct && toSlot.Product.NumericId == fromSlot.Product.NumericId)
                {
                    var maxSlotAmount = CalculationHelper.GetAmountForProductInVolume(placingProduct.Config, toSlot.Volume);
                    var amountToAdd = Math.Min(maxSlotAmount - toSlot.Product.Amount, placingProduct.Amount);
                    if (amountToAdd > 0)
                    {
                        toSlot.ChangeProductAmount(amountToAdd);
                        fromSlot.ChangeProductAmount(-amountToAdd);
                        placedProductsCount = amountToAdd;
                    }
                }
                else if (toSlot.HasProduct == false)
                {
                    var maxSlotAmount = CalculationHelper.GetAmountForProductInVolume(placingProduct.Config, toSlot.Volume);
                    var amountToAdd = Math.Min(maxSlotAmount, placingProduct.Amount);
                    if (amountToAdd > 0)
                    {
                        toSlot.SetProduct(new ProductModel(placingProduct.Config, amountToAdd));
                        fromSlot.ChangeProductAmount(-amountToAdd);
                        placedProductsCount = amountToAdd;
                    }
                }
            }

            return placedProductsCount;
        }
    }
}
