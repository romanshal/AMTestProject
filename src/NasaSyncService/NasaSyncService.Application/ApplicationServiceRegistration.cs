using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NasaSyncService.Application.Decorators;
using NasaSyncService.Application.ValidationBehaviours;
using System.Reflection;

namespace NasaSyncService.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
                conf.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });

            services.Decorate(typeof(IRequestHandler<,>), typeof(CachingDecorator<,>));

            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
