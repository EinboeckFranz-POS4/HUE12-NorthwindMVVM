namespace NorthwindDb;

public partial class OrderDetail
{
    public override string ToString() 
        => $"Order {OrderId}: {Quantity} x Product {ProductId}";
}