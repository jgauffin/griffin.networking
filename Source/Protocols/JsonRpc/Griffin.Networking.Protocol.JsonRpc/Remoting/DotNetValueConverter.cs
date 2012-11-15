using System;
using System.ComponentModel;

namespace Griffin.Networking.JsonRpc.Remoting
{
    /// <summary>
    /// Uses built in features of .NET to convert between types
    /// </summary>
    public class DotNetValueConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Try to convert a value
        /// </summary>
        /// <param name="sourceValue">Value to convert</param>
        /// <param name="targetType">Type to convert to</param>
        /// <param name="convertedValue">Converted value</param>
        /// <returns>true if successful; otherwise false.</returns>
        public bool TryConvert(object sourceValue, Type targetType, out object convertedValue)
        {
            var tc = TypeDescriptor.GetConverter(targetType);
            if (!tc.CanConvertFrom(sourceValue.GetType()))
            {
                try
                {
                    convertedValue = Convert.ChangeType(sourceValue, targetType);
                    return true;
                }
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                }
                convertedValue = null;
                return false;
            }

            convertedValue = tc.ConvertFrom(sourceValue);
            return true;
        }

        #endregion
    }
}