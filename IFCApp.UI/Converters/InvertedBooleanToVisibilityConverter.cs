using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IFCApp.UI.Converters
{
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a Boolean value to a Visibility value.
        /// If the Boolean is True, returns Visibility.Collapsed.
        /// If the Boolean is False, returns Visibility.Visible.
        /// </summary>
        /// <param name="value">The Boolean value to convert.</param>
        /// <param name="targetType">The target type (should be Visibility).</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>A Visibility value based on the Boolean input.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;

            if (value is bool)
                flag = (bool)value;

            return flag ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts back from Visibility to Boolean.
        /// If Visibility is Collapsed or Hidden, returns True.
        /// If Visibility is Visible, returns False.
        /// </summary>
        /// <param name="value">The Visibility value to convert back.</param>
        /// <param name="targetType">The target type (should be Boolean).</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>A Boolean value based on the Visibility input.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (Visibility)value != Visibility.Visible;

            return false;
        }
    }
}
