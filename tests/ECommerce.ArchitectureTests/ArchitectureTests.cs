using System.Reflection;
using ECommerce.Domain.Entities;
using NetArchTest.Rules;
using Shouldly;

namespace ECommerce.ArchitectureTests;

public class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(UseCases.DependencyInjection).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;
    private static readonly Assembly ApiAssembly = typeof(API.DependencyInjection).Assembly;

    [Fact]
    public void Domain_Should_Not_Depend_On_Any_Other_Project()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "ECommerce.UseCases",
                "ECommerce.Infrastructure",
                "ECommerce.API")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailingTypes(result));
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "ECommerce.Infrastructure",
                "ECommerce.API")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailingTypes(result));
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("ECommerce.API")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailingTypes(result));
    }

    [Fact]
    public void Api_Should_Not_Reference_Infrastructure_Internals_Except_DependencyInjection()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .DoNotHaveName("DependencyInjection")
            .And()
            .DoNotHaveName("Program")
            .ShouldNot()
            .HaveDependencyOnAny(
                "ECommerce.Infrastructure.Data",
                "ECommerce.Infrastructure.Migrations")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailingTypes(result));
    }

    [Fact]
    public void Controllers_Should_Not_Use_DbContext()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "ECommerce.Infrastructure.Data")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FormatFailingTypes(result));
    }

    [Fact]
    public void Domain_Entities_Should_Not_Have_Public_Setters()
    {
        var entityTypes = DomainAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => typeof(BaseEntity).IsAssignableFrom(t));

        var violations = entityTypes
            .SelectMany(type => type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.SetMethod?.IsPublic == true)
                .Select(property => $"{type.Name}.{property.Name}"))
            .ToList();

        violations.ShouldBeEmpty(
            $"Domain entities must not expose public setters. Violations: {string.Join(", ", violations)}");
    }

    [Fact]
    public void Commands_And_Queries_Should_Be_Sealed()
    {
        var violations = ApplicationAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.Name.EndsWith("Command", StringComparison.Ordinal) ||
                        t.Name.EndsWith("Query", StringComparison.Ordinal))
            .Where(t => !t.IsSealed)
            .Select(t => t.FullName)
            .ToList();

        violations.ShouldBeEmpty(
            $"Commands and queries must be sealed. Violations: {string.Join(", ", violations)}");
    }

    [Fact]
    public void Handlers_Should_End_With_Handler()
    {
        var violations = ApplicationAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(IsHandler)
            .Where(t => !t.Name.EndsWith("Handler", StringComparison.Ordinal))
            .Select(t => t.FullName)
            .ToList();

        violations.ShouldBeEmpty(
            $"Handlers must end with 'Handler'. Violations: {string.Join(", ", violations)}");
    }

    [Fact]
    public void Validators_Should_End_With_Validator()
    {
        var violations = ApplicationAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(IsValidator)
            .Where(t => !t.Name.EndsWith("Validator", StringComparison.Ordinal))
            .Select(t => t.FullName)
            .ToList();

        violations.ShouldBeEmpty(
            $"Validators must end with 'Validator'. Violations: {string.Join(", ", violations)}");
    }

    [Fact]
    public void Repositories_Should_Only_Be_Implemented_In_Infrastructure()
    {
        var violations = new[] { DomainAssembly, ApplicationAssembly, InfrastructureAssembly, ApiAssembly }
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.Name.EndsWith("Repository", StringComparison.Ordinal))
            .Where(t => t.Assembly != InfrastructureAssembly)
            .Select(t => t.FullName)
            .ToList();

        violations.ShouldBeEmpty(
            $"Repository implementations must live in Infrastructure. Violations: {string.Join(", ", violations)}");
    }

    private static bool IsHandler(Type type) =>
        type.Namespace?.EndsWith(".Handlers", StringComparison.Ordinal) == true ||
        type.GetInterfaces().Any(IsRequestHandlerInterface);

    private static bool IsValidator(Type type) =>
        type.Namespace?.EndsWith(".Validators", StringComparison.Ordinal) == true ||
        type.GetInterfaces().Any(interfaceType =>
            interfaceType.IsGenericType &&
            interfaceType.GetGenericTypeDefinition().FullName?.StartsWith(
                "FluentValidation.IValidator`1",
                StringComparison.Ordinal) == true);

    private static bool IsRequestHandlerInterface(Type interfaceType) =>
        interfaceType.IsGenericType &&
        interfaceType.GetGenericTypeDefinition().FullName?.StartsWith(
            "MediatR.IRequestHandler`",
            StringComparison.Ordinal) == true;

    private static string FormatFailingTypes(TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : $"Violating types: {string.Join(", ", result.FailingTypes.Select(type => type.FullName))}";
}
