using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Windows.Media;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;

namespace Infragistics.ToyBox
{
	public class ImageToAvgColorBrushConverter : IValueConverter
	{
        private const int SAMPLE_PERCENTAGE = 10;
        private const int MAX_SAMPLES = 30;
		
		#region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
                Debug.WriteLine(value.ToString());
                if (false) // !(value is BitmapImage))
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
                else
                {
				 string imgPath = "";
				 if (value is BitmapImage)
				 {
					 BitmapImage bi = (BitmapImage)value;
					 imgPath = bi.UriSource.OriginalString;
				 }
				 else if (value is String)
				 {
					 imgPath = value as string;
					 if (String.IsNullOrEmpty(imgPath))
						 return new SolidColorBrush(Colors.Transparent);
				 }
                
                    //Uri fileUri = bi.UriSource;
                    //string fileURL = fileUri.AbsolutePath;
                    //fileURL.Replace("/", "\\\\");
				 try
				 {
					 System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
						System.Drawing.Image.FromFile(imgPath));
					 System.Drawing.Color pixel;

					 ArrayList arrPixels = new ArrayList();

					 int randX, randY;

					 int maxY = bmp.Height / SAMPLE_PERCENTAGE;
					 int maxX = bmp.Width / SAMPLE_PERCENTAGE;

					 if (maxY > MAX_SAMPLES)
						 maxY = MAX_SAMPLES;

					 if (maxX > MAX_SAMPLES)
						 maxX = MAX_SAMPLES;

					 for (int y = 0; y < maxY; y++)
					 {
						 for (int x = 0; x < maxX; x++)
						 {
							 randX = GetRandomNumber(0, bmp.Width);
							 randY = GetRandomNumber(0, bmp.Height);
							 pixel = bmp.GetPixel(randX, randY);

							 //Debug.WriteLine("randX: " + randX.ToString() + ", randY: " + randY.ToString() +
							 //                ", hue: " + pixel.GetHue().ToString());
							 //Debug.WriteLine("        >> r: " + pixel.R.ToString() + ", g: " + pixel.G.ToString() + ", b: " + pixel.B.ToString());

							 arrPixels.Add(pixel);//.GetHue());					
						 }
					 }

					 int rSum = 0;
					 int gSum = 0;
					 int bSum = 0;
					 foreach (System.Drawing.Color cpixel in arrPixels)
					 {
						 rSum += cpixel.R;
						 gSum += cpixel.G;
						 bSum += cpixel.B;
					 }

					 byte newR = (byte)Math.Floor((double)(rSum / ((bmp.Height / 10) * (bmp.Width / 10))));
					 byte newG = (byte)Math.Floor((double)(gSum / ((bmp.Height / 10) * (bmp.Width / 10))));
					 byte newB = (byte)Math.Floor((double)(bSum / ((bmp.Height / 10) * (bmp.Width / 10))));

					 return new SolidColorBrush(Color.FromRgb(newR, newG, newB));
				 }
				 catch (Exception ex)
				 {
					 return new SolidColorBrush(Colors.Transparent);
				 }
                    // return new SolidColorBrush(Color.FromRgb(newR, newG, newB));
                }
			}
			else
			{
                return new SolidColorBrush(Colors.Transparent);
                // return new SolidColorBrush(Color.FromRgb(0, 0, 0));
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

        #region Helper Methods
        private System.Random objRandom = new Random();
        private int GetRandomNumber(int Low, int High)
        {

            string guid = System.Guid.NewGuid().ToString("N").Replace("a", "").Replace("b", "").Replace("c", "").Replace("d", "").Replace("e", "").Replace("f", "");

            //Random very large number/string;    
            int seed = int.Parse(guid.Substring(0, 5));
            objRandom = new Random(seed);
            return objRandom.Next(Low, High);
        }
        #endregion Helper Methods
	}
}
