using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infragistics.ToyBox
{
	/// <summary>
	/// ========================================
	/// .NET Framework 3.0 Custom Control
	/// ========================================
	///
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Infragistics.ToyBox"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Infragistics.ToyBox;assembly=Infragistics.ToyBox"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file. Note that Intellisense in the
	/// XML editor does not currently work on custom controls and its child elements.
	///
	///     <MyNamespace:Reflector/>
	///
	/// </summary>
	public class Reflector : System.Windows.Controls.Control
	{
		static Reflector()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Reflector), new FrameworkPropertyMetadata(typeof(Reflector)));			
		}

		/*
		protected override void OnRender(DrawingContext drawingContext)
		{
			this.Width = ReflectionTarget.ActualWidth;
			this.Height = ReflectionTarget.ActualHeight;
		}
		 * */

		void Draw(Object sender, EventArgs e)
		{

		}

		
		#region Public Properties
		#region Styling Properties

		#region ReflectionTarget

		/// <summary>
		/// Identifies the <see cref="ReflectionTarget"/> dependency property
		/// </summary>
		public static DependencyProperty ReflectionTargetProperty = DependencyProperty.Register("ReflectionTarget",
			typeof(FrameworkElement), typeof(Reflector), new FrameworkPropertyMetadata((object)null));

		/// <summary>
		/// Element to reflect.
		/// </summary>
		/// <seealso cref="ReflectionTargetProperty"/>
		[Description("The element to create a reflection of.")]
		[Category("Appearance")]
		public FrameworkElement ReflectionTarget
		{
			get
			{
				return (FrameworkElement)this.GetValue(Reflector.ReflectionTargetProperty);
			}
			set
			{
				this.SetValue(Reflector.ReflectionTargetProperty, value);
			}
		}

		#endregion ReflectionTarget

		#region BackgroundHover

		/// <summary>
		/// Identifies the <see cref="BackgroundHover"/> dependency property
		/// </summary>
		public static DependencyProperty BackgroundHoverProperty = DependencyProperty.Register("BackgroundHover",
			typeof(Brush), typeof(Reflector), new FrameworkPropertyMetadata((object)null));

		/// <summary>
		/// Brush applied when IsMouseOver = true.
		/// </summary>
		/// <seealso cref="BackgroundHoverProperty"/>
		[Description("Brush applied when IsMouseOver = true.")]
		[Category("Brushes")]
		public Brush BackgroundHover
		{
			get
			{
				return (Brush)this.GetValue(Reflector.BackgroundHoverProperty);
			}
			set
			{
				this.SetValue(Reflector.BackgroundHoverProperty, value);
			}
		}

		#endregion BackgroundHover		
				
		#region BorderHoverBrush

		/// <summary>
		/// Identifies the <see cref="BorderHoverBrush"/> dependency property
		/// </summary>
		public static DependencyProperty BorderHoverBrushProperty = DependencyProperty.Register("BorderHoverBrush",
			typeof(Brush), typeof(Reflector), new FrameworkPropertyMetadata((object)null));

		/// <summary>
		/// The border brush applied to the background area when IsMouseOver = true.
		/// </summary>
		/// <seealso cref="BorderHoverBrushProperty"/>
		[Description("The border brush applied to the background area when IsMouseOver = true.")]
		[Category("Brushes")]
		public Brush BorderHoverBrush
		{
			get
			{
				return (Brush)this.GetValue(Reflector.BorderHoverBrushProperty);
			}
			set
			{
				this.SetValue(Reflector.BorderHoverBrushProperty, value);
			}
		}

		#endregion BorderHoverBrush		

		#endregion Styling Properties
		#endregion // Public Properties
	}

}
