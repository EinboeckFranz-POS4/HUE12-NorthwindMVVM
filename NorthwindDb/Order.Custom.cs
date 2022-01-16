namespace NorthwindDb;

public partial class Order
{
    public override string ToString() 
        => $"Order {OrderId} from {OrderDate:yyyy-MM-dd}";
}