namespace Cairo_Cinema.API.DTOs.Response
{
    public class CartResponse
    {
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total => Subtotal - Discount;
        public string? CouponCode { get; set; }
    }
}