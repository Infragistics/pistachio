//-------------------------------------------------------------------------
// <copyright file="HeightToItemsPerPageConverter.cs" company="Infragistics">
//
// Copyright (c) 2007 Infragistics, Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// </copyright>
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using System.Collections;

namespace Pistachio
{
    /// <summary>
    /// Represents the converter that converts <see cref="System.Int32">Integers</see> into formatted strings given the inputted formatted string.
    /// </summary>
    [ValueConversion(typeof(double), typeof(int)), Localizability(LocalizationCategory.NeverLocalize)]
    class HeightToItemsPerPageConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts a <see cref="System.Double" /> that represents the actual height into an <see cref="System.Int32">Integer</see> that represents the number of items to display.
        /// </summary>
        /// <remarks>
        /// The conversion uses the converter parameter as the item's height.  The converter parameter is required.
        /// </remarks>
        /// <param name="value">The actual height of the container.  This value should be a standard <see cref="System.Double" /> and not null.</param>
        /// <param name="targetType">This parameter is not used.  The target type is always <see cref="System.Int32">Integer</see>.</param>
        /// <param name="parameter">The item height (including padding if needed.)  This value should be a standard <see cref="System.Int32">Integer</see> and not null.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>Number of items to display.</returns>
        /// <exception cref="System.ArgumentException">If value or parameter cannot be parsed into their respective target types.</exception>
        /// <exception cref="System.ArgumentNullException">If value or parameter is null.</exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Enforce Method Contract
            if (value == null)
                throw new ArgumentNullException("value", "The parameter cannot be null.");
            if (parameter == null)
                throw new ArgumentNullException("parameter", "The parameter cannot be null.");

            double actualHeight;
            int itemHeight;

            // Check both value and parameter to ensure they are integers
            if (value is double)
            {
                actualHeight = (double)value;
            }
            else
            {
                if (!double.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out actualHeight))
                    throw new ArgumentException("The parameter is not a valid double.", "value");
            }

            if (parameter is int)
            {
                itemHeight = (int)parameter;
            }
            else
            {
                if (!int.TryParse(parameter.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out itemHeight))
                    throw new ArgumentException("The parameter is not a valid integer.", "parameter");
            }

            // Calculate return value.  (Note:  Integer division used to calculate maximum number of items to be shown.)
            int itemsPerPage = (int)(actualHeight / itemHeight);

            return itemsPerPage;
        }

        /// <summary>
        /// This method has not been implemented and will throw an exception.
        /// </summary>
        /// <param name="value">This paremater is not used.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This paremeter is not used.</param>
        /// <exception cref="System.NotImplementedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion
    }
}
