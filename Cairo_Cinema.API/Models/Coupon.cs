namespace Cairo_Cinema.API.Models
        
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }   // مثلًا 10 = خصم 10%
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
