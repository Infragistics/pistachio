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
using System.Xml;
using System.Windows.Media;

namespace Pistachio
{
	/// <summary>
	/// Represents the converter that converts <see cref="System.Int32">Integers</see> into formatted strings given the inputted formatted string.
	/// </summary>
	[ValueConversion(typeof(XmlNode), typeof(Brush)), Localizability(LocalizationCategory.NeverLocalize)]
	class XamlNodeToBrushConverter : IValueConverter
	{
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Enforce Method Contract
			if (value == null)
				return new SolidColorBrush(Colors.Transparent);

			try 
			{
				string brushXaml = value.ToString();
				string brushType = brushXaml.Substring(1, brushXaml.IndexOf(" ") - 1);

				switch (brushType)
				{
					case "SolidColorBrush" :
						return XamlReader2<SolidColorBrush>.Load(brushXaml);
						break;

					case "LinearGradientBrush" :
						return XamlReader2<LinearGradientBrush>.Load(brushXaml);
						break;

					case "RadialGradientBrush" :
						return XamlReader2<RadialGradientBrush>.Load(brushXaml);
						break;

					default :
						return new SolidColorBrush(Colors.Transparent);
					//case "Color" :
					//     return new SolidColorBrush(XamlReader2<Color>.Load(value.ToString()));
					//     break;
				}
				//LinearGradientBrush previewBrush = XamlReader2<LinearGradientBrush>.Load(value.ToString());
				//return previewBrush;
			}
			catch (Exception e)
			{
				return new SolidColorBrush(Colors.Transparent);
			}			
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

		private Brush GetBrush(XmlNode node)
		{
			Brush returnBrush;
			if (node.Attributes["x:Key"] != null)
			{
				XmlAttribute key = node.Attributes["x:Key"];
				node.Attributes.Remove(key);
			}

			try
			{
				switch (node.Name)
				{
					case "SolidColorBrush":
						returnBrush = XamlReader2<SolidColorBrush>.Load(node.OuterXml);
						break;

					case "LinearGradientBrush":
						returnBrush = XamlReader2<LinearGradientBrush>.Load(node.OuterXml);
						break;

					case "RadialGradientBrush":
						returnBrush = XamlReader2<RadialGradientBrush>.Load(node.OuterXml);
						break;

					default:
						returnBrush = new SolidColorBrush(Colors.Transparent);
						break;
				}
			}
			catch (Exception ex)
			{
				returnBrush = new SolidColorBrush(Colors.Transparent);
			}
			finally
			{

			}

			return returnBrush;
		}
	}
}
