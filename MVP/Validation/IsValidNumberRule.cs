namespace MVP.Validation
{
    public class IsValidNumberRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (!int.TryParse(value.ToString(), out var numb))
                return false;

            return numb >= 0;
        }
    }
}
