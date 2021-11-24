﻿namespace Basket.API.IntegrationEvents.Events;

// Integration Events notes:
// An Event is "something that has happened in the past", therefore its name has to be
// An Integration Event is an event that can cause side effects to other microservices, Bounded-Contexts or external systems.
public record OrderStartedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; init; }

    public OrderStartedIntegrationEvent(string userId)
        => UserId = userId;
}
