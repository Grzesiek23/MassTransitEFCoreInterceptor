﻿namespace MassTransitEFCoreInterceptor.Domain;

public sealed class Employee
{
    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
}