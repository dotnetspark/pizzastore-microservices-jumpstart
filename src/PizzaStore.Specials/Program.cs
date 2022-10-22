using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "OAuth2.0 Auth Code with PKCE",
        Name = "oauth2",
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(configuration.GetValue<string>("AuthorizationUrl")!),
                TokenUrl = new Uri(configuration.GetValue<string>("TokenUrl")!),
                Scopes = new Dictionary<string, string>
                {
                    { "https://blazzingpizza.onmicrosoft.com/pizzaspecials-api/read", "Read access to pizza specials"},
                    { "https://blazzingpizza.onmicrosoft.com/pizzaspecials-api/write", "Write access to pizza specials"},
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[]
            {
                "https://blazzingpizza.onmicrosoft.com/pizzaspecials-api/read",
                "https://blazzingpizza.onmicrosoft.com/pizzaspecials-api/write"
            }
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.OAuthClientId(configuration.GetValue<string>("SwaggerClientId"));
        c.OAuthUsePkce();
        c.OAuthScopeSeparator(" ");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

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

app.MapGet("/", (HttpContext context) => 
{
    context.VerifyUserHasAnyAcceptedScope("read");

    return Results.Ok(specials);
})
.WithName("GetAll")
.WithOpenApi(o =>
{
    o.Summary = "Get all pizza special";
    o.Description = "Returns all pizza specials";
    o.Responses["200"].Description = "Collection of pizza specials";
    o.Responses.Add(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse { Description = "Unauthorize access to this resource" });

    return o;
})
.RequireAuthorization()
;

app.MapGet("/{id:Guid}", (HttpContext context, Guid id) =>
{
    context.VerifyUserHasAnyAcceptedScope("read");

    if (id == Guid.Empty) return Results.NotFound();

    var special = specials.FirstOrDefault(s => s.Id == id);
    if (special is null) return Results.NotFound();

    return Results.Ok(special);
})
.WithName("GetById")
.WithOpenApi(o =>
{
    o.Summary = "Remove pizza special";
    o.Description = "Get a pizza special by its unique identifier";
    o.Responses["200"].Description = "A pizza special";
    o.Responses.Add(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse { Description = "Unauthorize access to this resource" });
    o.Responses.Add(StatusCodes.Status404NotFound.ToString(), new OpenApiResponse { Description = "The given id could not be found" });

    return o;
})
.RequireAuthorization();

//app.MapPost("/", async ([FromServices] IMapper mapper, CreatePizzaSpecial contract) =>
//{
//    var special = mapper.Map<PizzaSpecial>(contract);
//    specials.Add(special);

//    return Results.Created($"/{special.Id}", mapper.Map<PizzaSpecialCreated>(special));
//})
//.WithName("Create")
//.WithDescription("Creates a pizza special")
//.WithOpenApi()
//.Produces<CreatedResult>();

app.MapDelete("/specials/{id:Guid}", (HttpContext context, Guid id) =>
{
    context.VerifyUserHasAnyAcceptedScope("write");

    if (id == Guid.Empty) return Results.NotFound();

    var pizzaSpecial = specials.FirstOrDefault(s => s.Id == id);
    if (pizzaSpecial is null) return Results.NotFound();

    specials.Remove(pizzaSpecial);

    return Results.Ok();
})
.WithOpenApi(o =>
{
    o.Summary = "Remove pizza special";
    o.Description = "Removes a pizza special by its unique identifier";
    o.Responses["200"].Description = "Pizza special successfully removed";
    o.Responses.Add(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse { Description = "Unauthorize access to this resource" });
    o.Responses.Add(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse { Description = "Forbidden access to this resource" });
    o.Responses.Add(StatusCodes.Status404NotFound.ToString(), new OpenApiResponse { Description = "The given id could not be found" });

    return o;
})
.RequireAuthorization();

app.Run();