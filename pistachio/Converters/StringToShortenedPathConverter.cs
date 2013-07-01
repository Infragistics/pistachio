using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.IO;

namespace Pistachio
{
	public class StringToShortenedPathConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				string filePath = value.ToString();
				int lengthToDisplay = Int32.Parse(parameter.ToString());

				if (filePath.Length > 0 && filePath.Length > lengthToDisplay)
				{
					string fileName = "\\" + Path.GetFileName(filePath);
					lengthToDisplay -= fileName.Length;
					if (lengthToDisplay <= 3)
						return fileName;
					else
					{
						string front = filePath.Remove(filePath.Length - fileName.Length, fileName.Length);
						front = front.Substring(0, lengthToDisplay - 3);
						return front + "..." + fileName;
					}
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
