using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drum_Led_Patch_Editor.Models;
using System.Globalization;
using System.Windows.Data;

namespace Drum_Led_Patch_Editor.Converters
{
    class TriggersLedsColorBehaviourToBoolConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is TriggerLedsColorBehaviourType tlcb)
            {
                if (tlcb == TriggerLedsColorBehaviourType.Constant)
                    return true;
                return false;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // perform the same conversion in both directions
            return Convert(value, targetType, parameter, culture);
        }
    }
}
