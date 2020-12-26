using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class InputView : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(InputView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(string), typeof(InputView), string.Empty, defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(nameof(Description), typeof(string), typeof(InputView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(InputView), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty IsRequiredProperty =
            BindableProperty.Create(nameof(IsRequired), typeof(bool), typeof(InputView), false, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty IsMultiLineProperty =
            BindableProperty.Create(nameof(IsMultiLine), typeof(bool), typeof(InputView), false, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty KeyboardProperty =
            BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(InputView), Keyboard.Default, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(InputView), true, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ErrorsProperty =
             BindableProperty.Create(nameof(Errors), typeof(string), typeof(InputView), null, defaultBindingMode: BindingMode.OneWay);

        public string Errors
        {
            get => (string)GetValue(ErrorsProperty);
            set => SetValue(ErrorsProperty, value);
        }

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public bool IsRequired
        {
            get => (bool)GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }

        public bool IsMultiLine
        {
            get => (bool)GetValue(IsMultiLineProperty);
            set => SetValue(IsMultiLineProperty, value);
        }

        public Keyboard Keyboard
        {
            get => (Keyboard)GetValue(KeyboardProperty);
            set => SetValue(KeyboardProperty, value);
        }

        public InputView()
            => InitializeComponent();

        void Entry_Focused(object sender, FocusEventArgs e)
        {
            frameEntry.SetAppThemeColor(Frame.BorderColorProperty,
                (Color)Application.Current.Resources["primary"],
                (Color)Application.Current.Resources["light_primary"]);

            frameEditor.SetAppThemeColor(Frame.BorderColorProperty,
                (Color)Application.Current.Resources["primary"],
                (Color)Application.Current.Resources["light_primary"]);
        }

        void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            frameEntry.SetAppThemeColor(Frame.BorderColorProperty,
                (Color)Application.Current.Resources["neutral_1"],
                (Color)Application.Current.Resources["neutral_3"]);

            frameEditor.SetAppThemeColor(Frame.BorderColorProperty,
                (Color)Application.Current.Resources["neutral_1"],
                (Color)Application.Current.Resources["neutral_3"]);
        }
    }
}
