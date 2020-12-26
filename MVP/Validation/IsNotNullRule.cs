namespace MVP.Validation
{
    /// <summary>
    /// Checks whether an incoming value is not null.
    /// </summary>
    public class IsNotNullRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
            => value != null;
    }
}
