using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

[ExcludeFromCodeCoverage]
public class Result : IResult
{
    public double Duration { protected set; get; }
    public bool IsCanceled { get; protected set; }

    public void SetDuration(double duration) => Duration = duration;

    public static Result Default() => new();
}

[ExcludeFromCodeCoverage]
public class Result<T> : Result
{
    public T? Data { get; set; }

    protected Result(T? data = default)
    {
        Data = data;
    }

    public static Result<T> From(T? data = default) => new(data);

    public static Result<T> Canceled() => new(default) { IsCanceled = true };
}
