using Microsoft.UI.Xaml.Data;
using System;

namespace RayMarchEditor
{
    public class BoolInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool b = (bool)value;

            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
