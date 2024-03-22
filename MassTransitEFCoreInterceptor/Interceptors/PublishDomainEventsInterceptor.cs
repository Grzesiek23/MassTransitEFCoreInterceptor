using MassTransit;
using MassTransitEFCoreInterceptor.Events;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MassTransitEFCoreInterceptor.Interceptors;

internal sealed class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PublishDomainEventsInterceptor(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        // Collect domain events

        // Publish domain events
        // For testing purposes, we are publishing a dummy event
        _publishEndpoint.Publish(new PersonalDataUpdatedDomainEvent(Guid.NewGuid()), cancellationToken);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}