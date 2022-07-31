/// <summary>
/// Represents a response body for pizza special created
/// </summary>
record PizzaSpecialCreated(Guid Id, string Name, decimal BasePrice, string Description, string ImageUrl)
{
    string GetFormattedBasePrice() => BasePrice.ToString("0.00");
}