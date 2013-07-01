using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace ToyBoxTester
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{

		public Window1()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (transImage.Source.ToString() == "pack://application:,,,/images/mainMenu.png")
			{
				transImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/images/AppIcon48.png"));
			}
			else
			{
				transImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/images/mainMenu.png"));
			}
		}

	}
}