using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ResultPattern.Validation;

[DebuggerDisplay("IsSuccess = {IsSuccess}, Possui {ValidationErrors.Count} Erros = {ToString()}")]
public record Result
{
    protected Result() { }

    protected Result(Error? error = null, string? language = null)
    {
        if (error is not null)
            AddValidationError("", error.GetMessage(language));
    }

    protected Result(string? error = null, string? language = null)
    {
        if (error is not null)
            AddValidationError("", error);
    }

    public bool IsSuccess => !_validationErrors.Any();
    private readonly Dictionary<string, string[]> _validationErrors = [];
    public int ErrorCount => _validationErrors.Values.Sum(v => v.Length);

    /// <summary>
    /// Erros com campo associado, compatível com ValidationProblemDetails (campo -> mensagens).
    /// </summary>
    public Dictionary<string, string[]> ValidationErrors
        => _validationErrors.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);

    public override string ToString()
    {
        return string.Join(", ", ValidationErrors.SelectMany(kv =>
            string.IsNullOrEmpty(kv.Key)
                ? kv.Value
                : kv.Value.Select(v => $"{kv.Key}: {v}")));
    }

    public bool IsFailure => !IsSuccess;

    protected Result AddValidationError(string field, string error)
    {
        if (_validationErrors.TryGetValue(field, out var existing))
            _validationErrors[field] = [.. existing, error];
        else
            _validationErrors[field] = [error];
        return this;
    }

    public static Result Ok() => new();
    public static Result Fail(Error error) => new(error);
    public static Result Fail(string error) => new(error);
    public static Result Fail(string field, string error) => new Result().AddValidationError(field, error);


    public static Result<T> Create<T>() => Ok<T>();

    public static Result<T> Create<T>(T value) => Ok<T>(value);

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

public record Result<T> : Result where T : notnull
{
    public static implicit operator Result<T>(T value) => Create<T>(value);

    private readonly T _value;

    [NotNull]
    public T Value => _value! ?? throw new InvalidOperationException("Result has no value");

    protected internal Result(T value) : base()
    {
        _value = value;
    }

    protected internal Result(T value, Error? error = null) : base(error)
    {
        _value = value;
    }

    protected internal Result(T value, string? error = null) : base(error)
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
        AddValidationError("", error.GetMessage(language));
        return this;
    }

    public Result<T> AddMessageError(string message)
    {
        AddValidationError("", message);
        return this;
    }

    /// <summary>
    /// Adiciona erro com campo nomeado, compatível com ValidationProblemDetails.
    /// </summary>
    [Obsolete("Use AddMessageError com Expression para garantir que o campo é uma propriedade válida do tipo T.")]
    public Result<T> AddMessageError(string field, string message)
    {
        AddValidationError($"{typeof(T).Name}.{field}", message);
        return this;
    }

    /// <summary>
    /// Adiciona erro usando Expression para garantir que o campo é uma propriedade válida do tipo T.
    /// Ex: result.AddMessageError(u => u.Email, "Email é obrigatório")
    /// </summary>
    public Result<T> AddMessageError([NotNull] Expression<Func<T, object>> property, string message) 
    {
        var field = GetPropertyName(property)!;
        AddValidationError(field, message);
        return this;
    }

    /// <summary>
    /// Adiciona erro usando Expression com suporte a multi-idiomas.
    /// </summary>
    public Result<T> AddMessageError([NotNull] Expression<Func<T, object>> property, Error error, string? language = null)
    {
        var field = GetPropertyName(property);
        AddValidationError(field, error.GetMessage(language));
        return this;
    }

    private static string GetPropertyName([NotNull] Expression<Func<T, object>> expression)
    {
        if (expression == null)
        {
            throw new ArgumentException();
        }

        var member = expression.Body switch
        {
            MemberExpression m => m,
            UnaryExpression { Operand: MemberExpression m } => m,
            _ => throw new ArgumentException("A expressão deve referenciar uma propriedade.", nameof(expression))
        };
        return $"{typeof(T).Name}.{member!.Member.Name}";
    }

    /// <summary>
    /// Adiciona erro com campo nomeado a partir de um <see cref="Error"/>.
    /// </summary>
    public Result<T> AddMessageError(string field, Error error, string? language = null)
    {
        AddValidationError($"{typeof(T).Name}.{field}", error.GetMessage(language));
        return this;
    }

    }

public static class ResultExtensions
{
    public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<Dictionary<string, string[]>, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.ValidationErrors);
    }

    public static void Match2<T>(
        [NotNull] this Result<T> result,
        Action<T> onSuccess,
        Action<Dictionary<string, string[]>, T> onFailure)
    {
        if (result.IsSuccess)
            onSuccess(result.Value);
        else
            onFailure(result.ValidationErrors, result.Value);
    }

    /*
    #region Métodos JS
    /// <summary>
    /// Executa uma ação após o resultado, independente de sucesso ou falha (similar ao Finally do TypeScript).
    /// </summary>
    public static Result Finally(
        this Result result,
        Action action)
    {
        action();
        return result;
    }

    /// <summary>
    /// Executa uma ação se o resultado for bem-sucedido.
    /// </summary>
    public static Result Tap(
        this Result result,
        Action action)
    {
        if (result.IsSuccess)
            action();
        return result;
    }

    /// <summary>
    /// Executa uma ação se o resultado falhar.
    /// </summary>
    public static Result TapError(
        this Result result,
        Action<List<string>, Dictionary<string, string[]>> action)
    {
        if (result.IsFailure)
            action(result.Errors, result.ValidationErrors);
        return result;
    }

    /// <summary>
    /// Encadeia operações que retornam Result (flatMap/bind - similar ao Then do TypeScript).
    /// </summary>
    public static Result Then(
        this Result result,
        Func<Result> func)
    {
        return result.IsSuccess ? func() : result;
    }

    /// <summary>
    /// Trata apenas erros e retorna novo Result (similar ao Catch do TypeScript).
    /// </summary>
    public static Result Catch(
        this Result result,
        Func<List<string>, Dictionary<string, string[]>, Result> handler)
    {
        return result.IsFailure ? handler(result.Errors, result.ValidationErrors) : result;
    }

    /// <summary>
    /// Encadeia operações que retornam Result<TOut> com transformação (flatMap/bind para Result<T>).
    /// </summary>
    public static Result<TOut> Then<T, TOut>(
        this Result<T> result,
        Func<T, Result<TOut>> func)
    {
        return result.IsSuccess ? func(result.Value) : Result.Fail<TOut>(string.Join(", ", result.ValidationErrors.SelectMany(kv => kv.Value)));
    }

    /// <summary>
    /// Executa uma ação se o resultado for bem-sucedido (Tap com acesso ao valor).
    /// </summary>
    public static Result<T> Tap<T>(
        this Result<T> result,
        Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);
        return result;
    }

    /// <summary>
    /// Executa uma ação sem argumentos se o resultado for bem-sucedido.
    /// </summary>
    public static Result<T> Tap<T>(
        this Result<T> result,
        Action action)
    {
        if (result.IsSuccess)
            action();
        return result;
    }

    /// <summary>
    /// Executa uma ação se o resultado falhar.
    /// </summary>
    public static Result<T> TapError<T>(
        this Result<T> result,
        Action<List<string>, Dictionary<string, string[]>> action)
    {
        if (result.IsFailure)
            action(result.Errors, result.ValidationErrors);
        return result;
    }

    /// <summary>
    /// Executa uma ação após o resultado, independente de sucesso ou falha.
    /// </summary>
    public static Result<T> Finally<T>(
        this Result<T> result,
        Action action)
    {
        action();
        return result;
    }

    /// <summary>
    /// Trata apenas erros e retorna novo Result<T> (Catch com acesso ao valor anterior).
    /// </summary>
    public static Result<T> Catch<T>(
        this Result<T> result,
        Func<List<string>, Dictionary<string, string[]>, T, Result<T>> handler)
    {
        return result.IsFailure ? handler(result.Errors, result.ValidationErrors, result.Value) : result;
    } 
    #endregion
    */

}




