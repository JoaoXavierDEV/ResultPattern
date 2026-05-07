using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ResultPattern.Validation;

internal record ErrorMensage(string Field, Error Message, string? Language = null);

[DebuggerDisplay("IsSuccess = {IsSuccess}, Possui {ValidationErrors.Count} Erros = {ToString()}")]
public record Result
{
    #region Construtores
    protected Result() { }

    internal Result(ErrorMensage error)
    {
        AddValidationError(error.Field, error.Message.GetMessage(error.Language));
    }
    #endregion

    #region Propriedades
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
    #endregion

    internal Result AddValidationError(string field, string error)
    {
        if (_validationErrors.TryGetValue(field, out var existing))
            _validationErrors[field] = [.. existing, error];
        else
            _validationErrors[field] = [error];
        return this;
    }

    public static Result<T> Ok<T>(T value) where T : notnull => new(value);

    public static Result<T> Create<T>(T value) where T : notnull => Ok(value);

    public static Result<T> Fail<T>(Expression<Func<T, object?>> property, Error error, T value, string? language = null) where T : notnull
        => new(value, new ErrorMensage(Result<T>.GetPropertyName(property), error, language));

}

public record Result<T> : Result where T : notnull
{
    [NotNull]
    public T Value { get => field! ?? throw new InvalidOperationException("Result has no value"); init; }

    protected internal Result(T value) : base()
    {
        Value = value;
    }

    internal Result(T value, ErrorMensage error) : base(error)
    {
        Value = value;
    }
    public static implicit operator Result<T>(T value) => Ok<T>(value);


    public static Result<T> Validate(Func<T, bool> predicate, Expression<Func<T, object?>> property, Error error, T value, string? language = null)
    {
        return predicate(value) ? Fail(property, error, value, language) : Ok(value);
    }

    /// <summary>
    /// Adiciona erro com Expression e Error (multi-idioma).
    /// Ex: result.AddMessageError(u => u.Email, Errors.InvalidEmail)
    /// </summary>
    public Result<T> AddMessageError(Expression<Func<T, object?>> property, Error error, string? language = null)
    {
        AddValidationError(GetPropertyName(property), error.GetMessage(language));
        return this;
    }

    /// <summary>
    /// Adiciona erro com Expression e mensagem string.
    /// Ex: result.AddMessageError(u => u.Email, "E-mail inválido")
    /// </summary>
    public Result<T> AddMessageError(Expression<Func<T, object?>> property, string message)
    {
        AddValidationError(GetPropertyName(property), message);
        return this;
    }

    internal static string GetPropertyName(Expression<Func<T, object?>> expression)
    {
        var parts = new Stack<string>();
        Expression? current = expression.Body is UnaryExpression unary ? unary.Operand : expression.Body;

        while (current is MemberExpression member)
        {
            parts.Push(member.Member.Name);
            current = member.Expression;
        }

        if (parts.Count == 0)
            throw new ArgumentException("A expressão deve referenciar uma propriedade.", nameof(expression));

        return $"{typeof(T).Name}.{string.Join(".", parts)}";
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

    public static void Match<T>(
        [NotNull] this Result<T> result,
        Action<T> onSuccess,
        Action<Dictionary<string, string[]>, T> onFailure) where T : notnull
    {
        if (result.IsSuccess)
            onSuccess(result.Value);
        else
            onFailure(result.ValidationErrors, result.Value);
    }
}

