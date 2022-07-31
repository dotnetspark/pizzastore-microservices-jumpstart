using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var specials = new List<PizzaSpecial>
{
    new PizzaSpecial(Guid.NewGuid(),"Basic Cheese Pizza", 9.99m,"It's cheesy and delicious. Why wouldn't you want one?","img/pizzas/cheese.jpg"),
    new PizzaSpecial(Guid.NewGuid(),"The Baconatorizor", 11.99m,"It has EVERY kind of bacon","img/pizzas/bacon.jpg"),
    new PizzaSpecial(Guid.NewGuid(),"Classic pepperoni",10.50m,"It's the pizza you grew up with, but Blazing hot!","img/pizzas/pepperoni.jpg"),
    new PizzaSpecial(Guid.NewGuid(),"Buffalo chicken",12.75m,"Spicy chicken, hot sauce and bleu cheese, guaranteed to warm you up","img/pizzas/meaty.jpg"),
    new PizzaSpecial(Guid.NewGuid(),"Mushroom Lovers",11.00m, "It has mushrooms. Isn't that obvious?","img/pizzas/mushroom.jpg"),
    new PizzaSpecial(Guid.NewGuid(),"The Brit",10.25m,"When in London...","img/pizzas/brit.jpg"),
    new PizzaSpecial(Guid.NewGuid(),"Veggie Delight",11.50m,"It's like salad, but on a pizza","img/pizzas/salad.jpg"),
    new PizzaSpecial(Guid.NewGuid(),"Margherita",9.99m,"Traditional Italian pizza with tomatoes and basil","img/pizzas/margherita.jpg")
};

app.MapGet("/", () => specials)
.WithName("GetAll")
.WithOpenApi()
.Produces<OkResult>();

app.MapGet("/{id}", (Guid id) =>
{
    if (Guid.Empty == id) return Results.NotFound();

    var special = specials.FirstOrDefault(s => s.Id == id);
    if (special is null) return Results.NotFound();

    return Results.Ok(special);
})
.WithName("GetById")
.WithDescription("Get a pizza special by its unique identifier")
.WithOpenApi()
.Produces<NotFoundResult>()
.Produces<OkResult>();

app.MapPost("/", async ([FromServices] IMapper mapper, CreatePizzaSpecial contract) =>
{
    var special = mapper.Map<PizzaSpecial>(contract);
    specials.Add(special);

    return Results.Created($"/{special.Id}", mapper.Map<PizzaSpecialCreated>(special));
})
.WithName("Create")
.WithDescription("Creates a pizza special")
.WithOpenApi()
.Produces<CreatedResult>();

app.MapDelete("/{id}", (Guid id) =>
{
    if (id == Guid.Empty) return Results.NotFound();

    var pizzaSpecial = specials.FirstOrDefault(s => s.Id == id);
    if (pizzaSpecial is null) return Results.NotFound();

    specials.Remove(pizzaSpecial);

    return Results.NoContent();
})
.WithName("Remove")
.WithDescription("Removes a pizza special by a given id")
.WithOpenApi()
.Produces<NotFoundResult>()
.Produces<NoContentResult>();

app.Run();