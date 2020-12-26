namespace MVP.Validation
{
    /// <summary>
    /// Exposes the necessary parts for validation rules to function.
    /// </summary>
    public interface IValidationRule<T>
    {
        string ValidationMessage { get; set; }
        bool Check(T value);
    }
}
