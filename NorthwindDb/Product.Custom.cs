namespace NorthwindDb;

public partial class Product
{
    public override string ToString() 
        => $"{ProductName}: {UnitPrice:##0.00}€ ({Category?.CategoryName ?? "No Category"})";
}