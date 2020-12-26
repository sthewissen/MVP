using System.Collections.Generic;
using System.ComponentModel;

namespace MVP.Validation
{
    /// <summary>
    /// Exposes validation related properties.
    /// </summary>
    public interface IValidatable<T> : INotifyPropertyChanged
    {
        List<IValidationRule<T>> Validations { get; }

        List<string> Errors { get; set; }

        bool Validate();

        bool IsValid { get; set; }
    }
}
