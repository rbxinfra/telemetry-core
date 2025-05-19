namespace Roblox.ApplicationTelemetry;

using System;
using System.Diagnostics;
using System.Threading.Tasks;


/// <summary>
/// A class for including telemetry for calls on methods.
/// </summary>
public interface ITelemetry
{
    /// <summary>
    /// Wrap a call to a method in a telemetry call.
    /// </summary>
    /// <typeparam name="T">The return type of the method.</typeparam>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="method">The method to wrap.</param>
    /// <returns>The return value of the method.</returns>
    Task<T> Wrap<T>(string caller, string methodName, Func<Task<T>> method);

    /// <summary>
    /// Wrap a call to a method in a telemetry call specifying the telemetry type.
    /// </summary>
    /// <typeparam name="T">The return type of the method.</typeparam>
    /// <param name="telemetryType">The type of telemetry to use.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="method">The method to wrap.</param>
    /// <returns>The return value of the method.</returns>
    Task<T> Wrap<T>(TelemetryType telemetryType, string caller, string methodName, Func<Task<T>> method);

    /// <summary>
    /// Wrap a call to a method in a telemetry call without a return value.
    /// </summary>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="action">The method to wrap.</param>
    /// <returns>An awaitable task.</returns>
    Task Wrap(string caller, string methodName, Func<Task> action);

    /// <summary>
    /// Wrap a call to a method in a telemetry call specifying the telemetry type without a return value.
    /// </summary>
    /// <param name="telemetryType">The type of telemetry to use.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="action">The method to wrap.</param>
    /// <returns>An awaitable task.</returns>
    Task Wrap(TelemetryType telemetryType, string caller, string methodName, Func<Task> action);

    /// <summary>
    /// Wrap a call to a method in a telemetry call without a return value in synchronous mode.
    /// </summary>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="action">The method to wrap.</param>
    void WrapSync(string caller, string methodName, Action action);

    /// <summary>
    /// Wrap a call to a method in a telemetry call specifying the telemetry type without a return value in synchronous mode.
    /// </summary>
    /// <param name="telemetryType">The type of telemetry to use.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="action">The method to wrap.</param>
    void WrapSync(TelemetryType telemetryType, string caller, string methodName, Action action);

    /// <summary>
    /// Wrap a call to a method in a telemetry call without a return value in synchronous mode.
    /// </summary>
    /// <typeparam name="T">The return type of the method.</typeparam>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="action">The method to wrap.</param>
    /// <returns>The return value of the method.</returns>
    T WrapSync<T>(string caller, string methodName, Func<T> action);

    /// <summary>
    /// Wrap a call to a method in a telemetry call specifying the telemetry type without a return value in synchronous mode.
    /// </summary>
    /// <typeparam name="T">The return type of the method.</typeparam>
    /// <param name="telemetryType">The type of telemetry to use.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="action">The method to wrap.</param>
    /// <returns>The return value of the method.</returns>
    T WrapSync<T>(TelemetryType telemetryType, string caller, string methodName, Func<T> action);

    /// <summary>
    /// Called before the method is called.
    /// </summary>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>A stopwatch that can be used to measure the duration of the method.</returns>
    Stopwatch PreInvoke(string caller, string methodName);

    /// <summary>
    /// Called after an invocation succeeds.
    /// </summary>
    /// <param name="watch">A stopwatch that was started when the method was called.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    void PostInvokeSuccess(Stopwatch watch, string caller, string methodName);

    /// <summary>
    /// Called after an invocation fails.
    /// </summary>
    /// <param name="watch">A stopwatch that was started when the method was called.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="ex">The exception that was thrown.</param>
    void PostInvokeFailure(Stopwatch watch, string caller, string methodName, Exception ex);

    /// <summary>
    /// Called before the method is called with telemetry type.
    /// </summary>
    /// <param name="telemetryType">The type of telemetry to use.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>A stopwatch that can be used to measure the duration of the method.</returns>
    Stopwatch PreInvoke(TelemetryType telemetryType, string caller, string methodName);

    /// <summary>
    /// Called after an invocation succeeds with telemetry type.
    /// </summary>
    /// <param name="telemetryType">The type of telemetry to use.</param>
    /// <param name="watch">A stopwatch that was started when the method was called.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    void PostInvokeSuccess(TelemetryType telemetryType, Stopwatch watch, string caller, string methodName);

    /// <summary>
    /// Called after an invocation fails with telemetry type.
    /// </summary>
    /// <param name="telemetryType">The type of telemetry to use.</param>
    /// <param name="watch">A stopwatch that was started when the method was called.</param>
    /// <param name="caller">The caller of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="ex">The exception that was thrown.</param>
    void PostInvokeFailure(TelemetryType telemetryType, Stopwatch watch, string caller, string methodName, Exception ex);
}
