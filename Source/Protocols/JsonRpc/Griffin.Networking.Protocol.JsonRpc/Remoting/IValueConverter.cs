using System;

namespace Griffin.Networking.JsonRpc.Remoting
{
    /// <summary>
    /// Converts values in .NET
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Try to convert a value
        /// </summary>
        /// <param name="sourceValue">Value to convert</param>
        /// <param name="targetType">Type to convert to</param>
        /// <param name="convertedValue">Converted value</param>
        /// <returns>true if successful; otherwise false.</returns>
        bool TryConvert(object sourceValue, Type targetType, out object convertedValue);
    }
}