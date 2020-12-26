using System.Windows.Input;
using Xamarin.Forms;

namespace MVP.Controls
{
    public partial class EmptyState : Grid
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(EmptyState), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(EmptyState), default, defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(nameof(Description), typeof(string), typeof(EmptyState), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty CommandTextProperty =
            BindableProperty.Create(nameof(CommandText), typeof(string), typeof(EmptyState), string.Empty, defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EmptyState), default, defaultBindingMode: BindingMode.OneWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string CommandText
        {
            get => (string)GetValue(CommandTextProperty);
            set => SetValue(CommandTextProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public EmptyState()
            => InitializeComponent();
    }
}
