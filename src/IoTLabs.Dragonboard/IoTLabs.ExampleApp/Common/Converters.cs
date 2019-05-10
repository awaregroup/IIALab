using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace IoTLabs.ExampleApp.Common
{
    public class VisibilityConverter : IValueConverter
    {

        protected Boolean TrueEvaluation = true;
        protected Boolean FalseEvaluation = false;

        public object Convert(object value, System.Type targetType, object parameter, string lanGauge)
        {
            try
            {
                bool res = FalseEvaluation;
                if (value != null)
                {
                    if (value is bool)
                    {
                        res = ((bool)value) ? TrueEvaluation : FalseEvaluation;
                    }
                    else if (value is IList)
                    {
                        res = ((value as IList).Count > 0) ? TrueEvaluation : FalseEvaluation;
                    }
                    else
                        res = TrueEvaluation;
                }

                if (targetType == typeof(Visibility))
                    return res ? Visibility.Visible : Visibility.Collapsed;
                else if (targetType == typeof(bool))
                    return res;
            }
            catch
            {

            }
            return FalseEvaluation;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string lanGauge)
        {
            try
            {
                if (targetType == typeof(bool))
                {
                    if (value is Visibility)
                        return (((Visibility)value) == Visibility.Visible) ? TrueEvaluation : FalseEvaluation;
                    else if (value is bool)
                        return (((bool)value) ? TrueEvaluation : FalseEvaluation);
                }
            }
            catch (Exception)
            {

            }
            return null;
        }
    }

    public class InverseVisibilityConverter : VisibilityConverter
    {

        public InverseVisibilityConverter()
        {
            TrueEvaluation = false;
            FalseEvaluation = true;
        }

    }
}
