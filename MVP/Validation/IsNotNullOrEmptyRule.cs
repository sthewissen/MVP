namespace MVP.Validation
{
    /// <summary>
    /// Checks whether an incoming value is not null or empty.
    /// </summary>
    public class IsNotNullOrEmptyRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
                return false;

            var str = value as string;
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
