using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ResultPattern.Validation;

internal record ErrorMensage(string Field, Error Message, string? Language = null);

[DebuggerDisplay("IsSuccess = {IsSuccess}, Erros = {ToString()}")]
public record Result
{
    protected Result() { }

    internal Result(ErrorMensage error)
    {
        AddValidationError(error.Field, error.Message.GetMessage(error.Language));
    }

    public bool IsSuccess => !_validationErrors.Any();
    public bool IsFailure => !IsSuccess;
    public int ErrorCount => _validationErrors.Values.Sum(v => v.Length);

    private readonly Dictionary<string, string[]> _validationErrors = [];

    /// <summary>
    /// Compatível com ValidationProblemDetails (campo → mensagens[]).
    /// </summary>
    public Dictionary<string, string[]> ValidationErrors
        => _validationErrors.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);

    internal Result AddValidationError(string field, string error)
    {
        if (_validationErrors.TryGetValue(field, out var existing))
            _validationErrors[field] = [.. existing, error];
        else
            _validationErrors[field] = [error];
        return this;
    }

    internal void MergeErrors(Dictionary<string, string[]> errors)
    {
        foreach (var (field, messages) in errors)
            foreach (var msg in messages)
                AddValidationError(field, msg);
    }

    public static Result<T> Ok<T>(T value) where T : notnull => new(value);

    public static Result<T> Create<T>(T value) where T : notnull => Ok(value);

    /// <summary>
    /// Falha com valor de T disponível (ex: validar uma entidade já existente e retorná-la com erros).
    /// </summary>
    public static Result<T> Fail<T>(
        Expression<Func<T, object?>> property,
        Error error,
        T value,
        string? language = null) where T : notnull
        => new(value, new ErrorMensage(Result<T>.GetPropertyName(property), error, language));

    /// <summary>
    /// Falha sem precisar de uma instância de T — use em factories/construtores onde não há
    /// um objeto válido para retornar (ex: value objects com construtor privado).
    /// </summary>
    public static Result<T> Fail<T>(
        Expression<Func<T, object?>> property,
        Error error,
        string? language = null) where T : notnull
    {
        var result = new Result<T>();
        result.AddValidationError(Result<T>.GetPropertyName(property), error.GetMessage(language));
        return result;
    }

    /// <summary>
    /// Cria um Result falho de outro tipo, copiando os erros de validação acumulados.
    /// Útil para converter Result&lt;TInput&gt; em Result&lt;TOutput&gt; quando a validação falha.
    /// </summary>
    public static Result<TResult> FailFrom<TResult>(Result source)
    {
        var result = new Result<TResult>();
        result.MergeErrors(source.ValidationErrors);
        return result;
    }

    public override string ToString()
        => string.Join(", ", ValidationErrors.SelectMany(kv =>
            string.IsNullOrEmpty(kv.Key)
                ? kv.Value
                : kv.Value.Select(v => $"{kv.Key}: {v}")));
}

public record Result<T> : Result
{
    [NotNull]
    public T Value
    {
        get => field is not null
            ? field
            : throw new InvalidOperationException("Result é uma falha — Value não está disponível.");
        init;
    }

    protected internal Result(T value) : base() { Value = value; }

    internal Result(T value, ErrorMensage error) : base(error) { Value = value; }

    // Construtor para FailFrom — sem valor
    internal Result() : base() { }

    private string? _defaultLanguage;

    public static implicit operator Result<T>(T value) => Ok<T>(value);

    /// <summary>
    /// Define o idioma padrão para todas as chamadas de AddMessageError nesta instância.
    /// Elimina a necessidade de passar o idioma em cada erro individualmente.
    /// </summary>
    public Result<T> WithLanguage(string language)
    {
        _defaultLanguage = language;
        return this;
    }


    /// <summary>
    /// Adiciona erro usando o idioma configurado via WithLanguage (ou CultureInfo como fallback).
    /// </summary>
    public Result<T> AddMessageError(Expression<Func<T, object?>> property, Error error)
    {
        AddValidationError(GetPropertyName(property), error.GetMessage(_defaultLanguage));
        return this;
    }


    /// <summary>
    /// Adiciona erro com mensagem literal.
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
