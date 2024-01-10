using System.Linq.Expressions;
using System.Reflection;
using Commands;

namespace Cli.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="MethodInfo" /> class.
/// </summary>
public static class MethodInfoExtensions
{
    /// <summary>
    ///     Creates a delegate of the specified delegate type for the given method and command.
    /// </summary>
    /// <param name="method">The method to create the delegate for.</param>
    /// <param name="command">The command to bind the delegate to.</param>
    /// <returns>A delegate of the specified delegate type.</returns>
    public static Delegate CreateCommandDelegate(this MethodInfo method, CliCommand command)
    {
        var delegateType = BuildDelegateType(method);
        return Delegate.CreateDelegate(delegateType, command, method);
    }

    /// <summary>
    ///     Builds a delegate type based on the provided MethodInfo.
    /// </summary>
    /// <param name="method">The MethodInfo object representing the method.</param>
    /// <returns>The Type object representing the delegate type.</returns>
    /// /
    private static Type BuildDelegateType(MethodInfo method)
    {
        var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var returnType = method.ReturnType;
        return Expression.GetDelegateType(parameterTypes.Concat(new[] { returnType }).ToArray());
    }
}