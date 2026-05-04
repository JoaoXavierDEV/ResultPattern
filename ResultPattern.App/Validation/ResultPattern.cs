using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ResultPattern.Validation;

namespace ResultPattern;

[DebuggerDisplay("IsSuccess = {IsSuccess}, Possui {Errors.Count} Erros = {ToString()}")]
public record Result
{
    protected Result() { }

    protected Result(Error? error = null, string? language = null)
    {
        if (error is not null)
            Errors.Add(error.GetMessage(language));
    }

    protected Result(string? error = null, string? language = null)
    {
        if (error is not null)
            Errors.Add(error);
    }

    public bool IsSuccess => !Errors.Any();

    public List<string> Errors { get; protected set; } = new List<string>();

    public override string ToString()
    {
        return string.Join(", ", Errors);
    }

    public bool IsFailure => !IsSuccess;

    public static Result Ok() => new();
    public static Result Fail(Error error) => new(error);
    public static Result Fail(string error) => new(error);


    public static Result<T> Create<T>() => Ok<T>();

    public static Result<T?> Create<T>(T? value) => Ok<T?>(value);

    public static Result<T> Ok<T>(T data)
        => new(data);

    public static Result<T> Ok<T>()
        => new(Activator.CreateInstance<T>());

    public static Result<T> Fail<T>(Error error)
        => new(default, error);

    public static Result<T> Fail<T>(Error error, T? data)
        => new(data, error);

    public static Result<T> Fail<T>(string error, T? data)
        => new(data, error);

    public static Result<T> Fail<T>(string error)
        => new(default, error);

    public static implicit operator Result(Error error) => Fail(error);

}

public record Result<T> : Result //where T : notnull
{
    public static implicit operator Result<T?>(T? value) => Create<T>(value);

    private readonly T? _value;

    [NotNull]
    public T Value => _value! ?? throw new InvalidOperationException("Result has no value");

    protected internal Result(T? value) : base()
    {
        _value = value;
    }

    protected internal Result(T? value, Error? error = null) : base(error)
    {
        _value = value;
    }

    protected internal Result(T? value, string? error = null) : base(error)
    {
        _value = value;
    }

    public static Result<T> Validate(Func<T, bool> func, Error message, T data)
    {
        var resultTest = func(data);

        if (resultTest)
            return Fail(message, data);
        else
            return Ok(data);
    }

    public static Result<T> Validate(Func<T, bool> func, string message, T data)
    {
        var resultTest = func(data);

        if (resultTest)
            return Fail(message, data);
        else
            return Ok(data);
    }



    public Result<T> AddMessageError(Error error, string? language = null)
    {
        var msg = error.GetMessage(language);
        return AddMessageErrorInternal(msg);
    }

    public Result<T> AddMessageError(string message)
    {
        return AddMessageErrorInternal(message);
    }

    private Result<T> AddMessageErrorInternal(string message)
    {
        if (!Errors.Contains(message))
            Errors.Add(message);

        return this;
    }
}

public static class ResultExtensions
{
    public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<List<string>, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Errors);
    }

    public static void Match2<T>(
        [NotNull] this Result<T> result,
        Action<T> onSuccess,
        Action<List<string>, T> onFailure)
    {
        if (result.IsSuccess)
        {
            onSuccess(result.Value);
        }
        else
        {
            onFailure(result.Errors, result.Value);
        }
    }
}



