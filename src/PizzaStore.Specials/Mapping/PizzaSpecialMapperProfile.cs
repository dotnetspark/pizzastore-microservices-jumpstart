using AutoMapper;

public class PizzaSpecialMapperProfile : Profile
{
    public PizzaSpecialMapperProfile()
    {
        CreateMap<CreatePizzaSpecial, PizzaSpecial>()
        .ConstructUsing((contract, model) => new PizzaSpecial(
            Guid.NewGuid(),
            contract.Name,
            contract.BasePrice,
            contract.Description,
            contract.ImageUrl
        ));
    }
}