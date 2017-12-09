using Autofac;
using Autofac.Core;
using DriveDrop.Api.Application.DomainEventHandlers.OrderStartedEvent;
using DriveDrop.Api.Application.Validations;
using DriveDrop.Api.Infrastructure.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.eShopOnContainers.Services.DriveDrop.Api.Application.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DriveDrop.Api.Infrastructure.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // Register all the Command classes (they implement IAsyncRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(typeof(CreateShippingCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IAsyncRequestHandler<,>));

            // Register all the event classes (they implement IAsyncNotificationHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(typeof(ValidateOrAddCustomerAggregateWhenShippingStartedDomainEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IAsyncNotificationHandler<>));


            builder
                .RegisterAssemblyTypes(typeof(CreateShippingCommandValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();


            builder.Register<SingleInstanceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            builder.Register<MultiInstanceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();

                return t =>
                {
                    var resolved = (IEnumerable<object>)componentContext.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
                    return resolved;
                };
            });

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

        }
    }
}
