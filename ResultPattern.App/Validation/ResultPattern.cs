using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ResultPattern.Validation;

namespace ResultPattern;

[DebuggerDisplay("IsSuccess = {IsSuccess}, Possui {Errors.Count} Erros = {ToString()}")]
public class Result
{
    protected Result(bool isSucess)
    {
        IsSuccess = isSucess;
    }

    protected Result() { }

    protected Result(bool isSuccess, Error? error = null, string? language = null)
    {
        IsSuccess = isSuccess;

        if (!isSuccess)
        {
            if (error is not null)
                Errors.Add(error.GetMessage(language));
        }
    }

    protected Result(bool isSuccess, string? error = null, string? language = null)
    {
        IsSuccess = isSuccess;

        if (!isSuccess)
        {
            if (error is not null)
                Errors.Add(error);
        }
    }

    public bool IsSuccess { get; protected set; }
    public List<string> Errors { get; protected set; } = new List<string>();

    public override string ToString()
    {
        return string.Join(", ", Errors);
    }

    public bool IsFailure => !IsSuccess;

    public static Result Ok() => new(true);
    public static Result Fail(Error error) => new(false, error);
    public static Result Fail(string error) => new(false, error);


    public static Result<T> Create<T>(T? value) =>
        value is not null
            ? Ok<T>(value)
            : Fail<T>(Error.NullValue);


    public static Result<T> Ok<T>(T data)
        => new(data, true);

    public static Result<T> Ok<T>()
        => new(default, true);

    // TODO testar versoes <T> e default
    public static Result<T> Fail<T>(Error error)
        => new(default, false, error);
    //=> new(Activator.CreateInstance<T>(), false, error);

    public static Result<T> Fail<T>(Error error, T data)
        => new(data, false, error);

    public static Result<T> Fail<T>(string error, T data)
        => new(data, false, error);

}

public class Result<T> : Result where T : notnull
{
    public static implicit operator Result<T>(T? value) => Create(value);

    private readonly T? _value;

    [NotNull]
    public T Value => _value! ?? throw new InvalidOperationException("Result has no value");

    protected internal Result(T? value, bool isSuccess) : base(isSuccess)
    {
        _value = value;
    }

    protected internal Result(T? value, bool isSuccess, Error? error = null) : base(isSuccess, error)
    {
        _value = value;
    }

    protected internal Result(T? value, bool isSuccess, string? error = null) : base(isSuccess, error)
    {
        _value = value;
    }

    private Result()
    {
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



    public Result AddMessageError(Error error, string? language = null)
    {
        var msg = error.GetMessage(language);
        return AddMessageErrorInternal(msg);
    }

    public Result AddMessageError(string message)
    {
        return AddMessageErrorInternal(message);
    }

    private Result AddMessageErrorInternal(string message)
    {
        if (IsSuccess)
            IsSuccess = false;

        if (!Errors.Contains(message))
            Errors.Add(message);

        return this;
    }
}


