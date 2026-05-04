namespace ResultPattern.Exceptions;

public class DomainExceptionValidation : Exception
{
    public IDictionary<string, string[]> Dictionary { get; private set; } = new Dictionary<string, string[]>();

    public DomainExceptionValidation(string error, string field) : base(error)
    {
        Dictionary.Add(field, [error]);
    }

    public DomainExceptionValidation(IDictionary<string, string[]> dictionary) : base()
    {
        if (dictionary is not null)
        {
            Dictionary = dictionary;
        }
    }

    public static void When(bool hasError, string field, string error)
    {
        var dictionary = new Dictionary<string, string[]>() { { field, [error] } };

        if (hasError)
            throw new DomainExceptionValidation(dictionary);
    }

    public static void When(bool hasError, IDictionary<string, string[]> dictionary)
    {
        if (hasError)
            throw new DomainExceptionValidation(dictionary);
    }


}
