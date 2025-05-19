namespace Roblox.ApplicationTelemetry;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Prometheus;

/// <inheritdoc cref="ITelemetry"/>
public class Telemetry : ITelemetry
{
    private static Telemetry _Instance;

    private readonly ConcurrentDictionary<string, Counter> _AttemptCounters;
    private readonly ConcurrentDictionary<string, Histogram> _DurationHistograms;
    private readonly ConcurrentDictionary<string, Counter> _FailCounters;
    private readonly ConcurrentDictionary<string, Counter> _SuccessCounters;
    private readonly ConcurrentDictionary<string, Gauge> _Gauges;

    /// <summary>
    /// Creates a new instance of the <see cref="Telemetry"/> class.
    /// </summary>
    public Telemetry()
    {
        _DurationHistograms = new();
        _AttemptCounters = new();
        _SuccessCounters = new();
        _FailCounters = new();
        _Gauges = new();
    }

    /// <inheritdoc cref="ITelemetry.WrapSync{T}(string, string, Func{T})"/>
    public T WrapSync<T>(string caller, string methodName, Func<T> method)
    {
        var watch = PreInvoke(caller, methodName);

        try
        {
            var data = method();

            PostInvokeSuccess(watch, caller, methodName);

            return data;
        }
        catch (Exception ex)
        {
            PostInvokeFailure(watch, caller, methodName, ex);

            throw;
        }
    }

    /// <inheritdoc cref="ITelemetry.WrapSync{T}(TelemetryType, string, string, Func{T})"/>
    public T WrapSync<T>(TelemetryType telemetryType, string caller, string methodName, Func<T> method)
    {
        var watch = PreInvoke(telemetryType, caller, methodName);

        try
        {
            var data = method();

            PostInvokeSuccess(telemetryType, watch, caller, methodName);

            return data;
        }
        catch (Exception ex)
        {
            PostInvokeFailure(telemetryType, watch, caller, methodName, ex);

            throw;
        }
    }

    /// <inheritdoc cref="ITelemetry.WrapSync(string, string, Action)"/>
    public void WrapSync(string caller, string methodName, Action action)
    {
        var watch = PreInvoke(caller, methodName);

        try
        {
            action();

            PostInvokeSuccess(watch, caller, methodName);
        }
        catch (Exception ex)
        {
            PostInvokeFailure(watch, caller, methodName, ex);

            throw;
        }
    }

    /// <inheritdoc	cref="ITelemetry.WrapSync(TelemetryType, string, string, Action)"/>
    public void WrapSync(TelemetryType telemetryType, string caller, string methodName, Action action)
    {
        var watch = PreInvoke(telemetryType, caller, methodName);

        try
        {
            action();

            PostInvokeSuccess(telemetryType, watch, caller, methodName);
        }
        catch (Exception ex)
        {
            PostInvokeFailure(telemetryType, watch, caller, methodName, ex);

            throw;
        }
    }

    /// <inheritdoc cref="ITelemetry.Wrap(string, string, Func{Task})"/>
    public async Task Wrap(string caller, string methodName, Func<Task> action)
    {
        var latency = PreInvoke(caller, methodName);

        try
        {
            await action();

            PostInvokeSuccess(latency, caller, methodName);
        }
        catch (Exception ex)
        {
            PostInvokeFailure(latency, caller, methodName, ex);

            throw;
        }
    }

    /// <inheritdoc cref="ITelemetry.Wrap(TelemetryType, string, string, Func{Task})"/>
    public async Task Wrap(TelemetryType telemetryType, string caller, string methodName, Func<Task> action)
    {
        var latency = PreInvoke(telemetryType, caller, methodName);

        try
        {
            await action();

            PostInvokeSuccess(telemetryType, latency, caller, methodName);
        }
        catch (Exception ex)
        {
            PostInvokeFailure(telemetryType, latency, caller, methodName, ex);

            throw;
        }
    }

    /// <inheritdoc cref="ITelemetry.Wrap{T}(string, string, Func{Task{T}})"/>
    public async Task<T> Wrap<T>(string caller, string methodName, Func<Task<T>> method)
    {
        var latency = PreInvoke(caller, methodName);

        try
        {
            var data = await method();

            PostInvokeSuccess(latency, caller, methodName);

            return data;
        }
        catch (Exception ex)
        {
            PostInvokeFailure(latency, caller, methodName, ex);

            throw;
        }
    }

    /// <inheritdoc cref="ITelemetry.Wrap{T}(TelemetryType, string, string, Func{Task{T}})"/>
    public async Task<T> Wrap<T>(TelemetryType telemetryType, string caller, string methodName, Func<Task<T>> method)
    {
        var latency = PreInvoke(telemetryType, caller, methodName);

        try
        {
            var data = await method();

            PostInvokeSuccess(telemetryType, latency, caller, methodName);

            return data;
        }
        catch (Exception ex)
        {
            PostInvokeFailure(telemetryType, latency, caller, methodName, ex);

            throw;
        }
    }

    /// <inheritdoc cref="ITelemetry.PreInvoke(string, string)"/>
    public Stopwatch PreInvoke(string caller, string methodName) => PreInvoke(TelemetryType.all, caller, methodName);

    /// <inheritdoc cref="ITelemetry.PreInvoke(TelemetryType, string, string)"/>
    public Stopwatch PreInvoke(TelemetryType telemetryType, string caller, string methodName)
    {
        AssertParams(caller, methodName);

        GetAttemptCounter(telemetryType).WithLabels(caller, methodName).Inc();
        GetConcurrentExecutionsGauge(telemetryType).WithLabels(caller, methodName).Inc();

        return Stopwatch.StartNew();
    }

    /// <inheritdoc cref="ITelemetry.PostInvokeSuccess(Stopwatch, string, string)"/>
    public void PostInvokeSuccess(Stopwatch watch, string caller, string methodName)
        => PostInvokeSuccess(TelemetryType.all, watch, caller, methodName);

    /// <inheritdoc cref="ITelemetry.PostInvokeSuccess(TelemetryType, Stopwatch, string, string)"/>
    public void PostInvokeSuccess(TelemetryType telemetryType, Stopwatch watch, string caller, string methodName)
    {
        GetConcurrentExecutionsGauge(telemetryType).WithLabels(caller, methodName).Dec();
        GetSuccessCounter(telemetryType).WithLabels(caller, methodName).Inc();
        GetDurationHistogram(telemetryType).WithLabels(caller, methodName).Observe(watch.Elapsed.TotalSeconds);
    }

    /// <inheritdoc cref="ITelemetry.PostInvokeFailure(Stopwatch, string, string, Exception)"/>
    public void PostInvokeFailure(Stopwatch watch, string caller, string methodName, Exception ex)
        => PostInvokeFailure(TelemetryType.all, watch, caller, methodName, ex);

    /// <inheritdoc cref="ITelemetry.PostInvokeFailure(TelemetryType, Stopwatch, string, string, Exception)"/>
    public void PostInvokeFailure(TelemetryType telemetryType, Stopwatch watch, string caller, string methodName, Exception ex)
    {
        GetConcurrentExecutionsGauge(telemetryType).WithLabels(caller, methodName).Dec();
        GetFailureCounter(telemetryType).WithLabels(caller, methodName).Inc();
    }

    /// <summary>
    /// Gets the default instance of the <see cref="Telemetry"/> class.
    /// </summary>
    /// <returns>The default instance of the <see cref="Telemetry"/> class.</returns>
    public static Telemetry Default()
    {
        if (_Instance != null) return _Instance;
        _Instance = new Telemetry();
        return _Instance;
    }

    private static void AssertParams(string caller, string methodName)
    {
        if (string.IsNullOrWhiteSpace(caller))
            throw new ArgumentException("caller is not specified");
        if (string.IsNullOrWhiteSpace(methodName))
            throw new ArgumentException("methodName is not specified");
    }

    private Histogram GetDurationHistogram(TelemetryType telemetryType)
    {
        var type = telemetryType.ToString().ToLower() ?? "";
        if (_DurationHistograms.ContainsKey(type))
            return _DurationHistograms[type];

        var histogram = Metrics.CreateHistogram(
            $"{type}_duration_seconds",
            $"Duration historgram for the {telemetryType}",
            "Caller",
            "MethodName"
        );

        _DurationHistograms.GetOrAdd(type, histogram);

        return histogram;
    }

    private static Counter GetCounter(TelemetryType telemetryType, string status, ConcurrentDictionary<string, Counter> counters)
    {
        var type = $"{telemetryType.ToString().ToLower()}_{status}";
        if (counters.ContainsKey(type))
            return counters[type];

        var counter = Metrics.CreateCounter(
            $"{type}_total",
            $"Total number of times on {type} happened.",
            "Caller",
            "MethodName"
        );

        counters.GetOrAdd(type, counter);

        return counter;
    }
    private Counter GetAttemptCounter(TelemetryType telemetryType)
        => GetCounter(telemetryType, "attempt", _AttemptCounters);

    private Counter GetSuccessCounter(TelemetryType telemetryType)
        => GetCounter(telemetryType, "success", _SuccessCounters);

    private Counter GetFailureCounter(TelemetryType telemetryType)
        => GetCounter(telemetryType, "fail", _FailCounters);

    private Gauge GetConcurrentExecutionsGauge(TelemetryType telemetryType)
    {
        var type = telemetryType.ToString().ToLower() ?? "";
        if (_Gauges.ContainsKey(type))
            return _Gauges[type];

        var gauge = Metrics.CreateGauge(
            $"{type}_concurrent_executions",
            $"Concurrent executions of {type}.",
            "Caller",
            "MethodName"
        );

        _Gauges.GetOrAdd(type, gauge);

        return gauge;
    }
}
