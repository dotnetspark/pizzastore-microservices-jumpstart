/// <summary>
/// Represents a pizza special offered by a pizza store and a user can order
/// </summary>
public record PizzaSpecial(Guid Id, string Name, decimal BasePrice, string Description, string ImageUrl)
{
    string GetFormattedBasePrice() => BasePrice.ToString("0.00");
}