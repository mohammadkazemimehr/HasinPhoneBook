using Hasin.Application.Services;
using Hasin.Infrastructure.Repositories;
using Hasin.Model.Repositories;

namespace Hasin.Api.Registrations;

public static partial class Registration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {

        services.AddSingleton<IContactRepository, ContactRepository>();
        services.AddSingleton<ITagRepository, TagRepository>();

        services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));

        return services;
    }

    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<ITagService, TagService>();

        return services;
    }
}