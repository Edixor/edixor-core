using System;

[AttributeUsage(AttributeTargets.Class)]
public class OrderAttributeFactory : Attribute
{
    public int Order { get; }
    public OrderAttributeFactory(int order) => Order = order;
}
