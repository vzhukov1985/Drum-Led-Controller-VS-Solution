using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.IO;

namespace Drum_Led_Patch_Editor.Converters
{
    class FullPathNameToFileNameConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is string str)
                return Path.GetFileName(str);
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // perform the same conversion in both directions
            if (value is string vals)
            {
                if (vals == "Play")
                    return true;
                return false;
            }
            return Convert(value, targetType, parameter, culture);
        }
    }
}
