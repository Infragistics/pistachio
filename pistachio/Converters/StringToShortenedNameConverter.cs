using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.IO;

namespace Pistachio
{
	class StringToShortenedNameConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				string filePath = value.ToString();
				int start = filePath.LastIndexOf("\\") + 1;
				if (start > 0)
				{
					int end = filePath.Length;
					filePath = filePath.Substring(start, (end - start));
				}
				return filePath;
			}
			else
			{
				return String.Empty;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
