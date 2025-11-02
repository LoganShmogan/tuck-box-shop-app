namespace tuck_box_shop.Models
{
    public class User
    {
        public string FirebaseKey { get; set; } // Firebase UID
        public string Email { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public List<DeliveryAddress> Addresses { get; set; } = new List<DeliveryAddress>();
    }
    
    public class DeliveryAddress
    {
        public string Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public bool IsDefault { get; set; }
    }
}