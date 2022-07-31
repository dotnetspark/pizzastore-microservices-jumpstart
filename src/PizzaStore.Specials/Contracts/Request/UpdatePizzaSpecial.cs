using System.ComponentModel.DataAnnotations;
/// <summary>
/// Represents a request body for a pizza special being updated
/// </summary>
record UpdatePizzaSpecial([Required] string Name, [Range(1, 20)] decimal BasePrice, string Description, string ImageUrl);