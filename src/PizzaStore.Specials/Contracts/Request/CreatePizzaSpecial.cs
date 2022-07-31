using System.ComponentModel.DataAnnotations;
/// <summary>
/// Represents the request body for a pizza special being created
/// </summary>
record CreatePizzaSpecial([Required] string Name, [Range(1, 20)] decimal BasePrice, string Description, string ImageUrl);