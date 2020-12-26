using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.CommunityToolkit.ObjectModel;

namespace MVP.Validation
{
    /// <summary>
    /// Defines an object that can be validated.
    /// </summary>
    public class ValidatableObject<T> : IValidatable<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<IValidationRule<T>> Validations { get; } = new List<IValidationRule<T>>();

        public List<string> Errors { get; set; } = new List<string>();

        public bool CleanOnChange { get; set; } = true;

        T value;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;

                if (CleanOnChange)
                    IsValid = true;
            }
        }

        public bool IsValid { get; set; } = true;

        public virtual bool Validate()
        {
            Errors.Clear();

            var errors = Validations
                .Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage);

            Errors = errors.ToList();
            IsValid = !Errors.Any();

            return IsValid;
        }

        public override string ToString()
            => $"{Value}";
    }
}
