namespace TourManagement.Web.Helpers;

public static class PriceCalculator
{
    /// <summary>
    /// Tính toán tổng tiền booking dựa trên giá schedule, số khách và khuyến mãi.
    /// </summary>
    public static (decimal subtotal, decimal discount, decimal surcharge, decimal total) Calculate(
        decimal unitPrice,
        int adults,
        int children,
        decimal childDiscountPercent = 30,
        decimal? minOrderValue = null,
        decimal? discountValue = null,
        int? discountType = null,
        decimal? maxDiscount = null,
        decimal surcharge = 0)
    {
        var adultPrice = unitPrice * adults;
        var childPrice = unitPrice * (100 - childDiscountPercent) / 100m * children;
        var subtotal = adultPrice + childPrice;

        decimal discount = 0;
        if (discountValue.HasValue && discountType.HasValue)
        {
            if ((minOrderValue ?? 0) <= subtotal)
            {
                discount = discountType.Value == 1
                    ? Math.Min(subtotal * discountValue.Value / 100m, maxDiscount ?? decimal.MaxValue)
                    : Math.Min(discountValue.Value, maxDiscount ?? decimal.MaxValue);
            }
        }

        var total = subtotal - discount + surcharge;
        return (subtotal, discount, surcharge, total);
    }
}