namespace MVP.Validation
{
    public class IsNotNullRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
            => value != null;
    }
}
