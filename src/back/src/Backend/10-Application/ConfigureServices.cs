using Application.Audit;
using Application.Behaviours;
using Application.FluentValidation;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Pcea.Core.Net.AuditTrail;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TrackDechetExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CurrentUserBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddPceaCoreNetAuditTrailServices<AuditService>();

        ValidatorOptions.Global.DisplayNameResolver = CustomDisplayNameResolver.Resolve;

        return services;
    }
}
