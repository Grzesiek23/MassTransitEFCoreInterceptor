namespace MassTransitEFCoreInterceptor.Events;

public sealed record PersonalDataUpdatedDomainEvent(Guid Id) : IDomainEvent;