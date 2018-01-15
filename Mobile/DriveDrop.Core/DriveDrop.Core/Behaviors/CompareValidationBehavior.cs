
using DriveDrop.Core.Behaviors.Base;
using Xamarin.Forms;

namespace DriveDrop.Core.Behaviors
{
    public class CompareValidationBehavior : BindableBehavior<Entry>
    {

        //public static BindableProperty TextProperty = BindableProperty.Create<CompareValidationBehavior, string>(tc => tc.Text, string.Empty, BindingMode.TwoWay);
        public static BindableProperty TextProperty = BindableProperty.Create("CompareValidationBehavior", typeof(string), typeof(EventToCommandBehavior), string.Empty, BindingMode.TwoWay);

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }


        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsValid = false;
            IsValid = e.NewTextValue == Text;

            ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
}
