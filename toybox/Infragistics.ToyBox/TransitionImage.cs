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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infragistics.ToyBox
{
	[TemplatePart(Name = TransitionImage.CONTROL_ROOT_NAME, Type = typeof(Panel))]
	[TemplatePart(Name = TransitionImage.BG_IMAGE_NAME, Type = typeof(Image))]
    [TemplatePart(Name = TransitionImage.FG_IMAGE_NAME, Type = typeof(Image))]
	public sealed class TransitionImage : System.Windows.Controls.Control
    {
        #region Constructors
        static TransitionImage()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionImage), new FrameworkPropertyMetadata(typeof(TransitionImage)));
        }

        public TransitionImage()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                this.Loaded += delegate { this.FinalizeImage(); };
        }
        #endregion

        #region Template Name Constants
        const string CONTROL_ROOT_NAME = "PART_ControlRoot";
        const string BG_IMAGE_NAME = "PART_BackgroundImage";
        const string FG_IMAGE_NAME = "PART_ForegroundImage";
        const string IMG_NOT_FOUND_NAME = "PART_ImgNotFoundSite";
        #endregion

        #region Fields & Properties
        bool _templateApplied = false;
        private double _transitionDuration = 15;
        private double _currentTime = 0;
        private int _pollTimeout = 0;
        bool _Transitioning = false;
        object _TransitionSyncLock = new object();

        private double _destinationWidth, _destinationHeight, _bgImgOriginalWidth, _bgImgOriginalHeight;
        private ScaleTransform _rtBG = new ScaleTransform();
        private ScaleTransform _rtFG = new ScaleTransform();

        #region Controls/Template Parts
        private Image _BackgroundImage = null;
        /// <summary>
        /// Gets the background image.
        /// </summary>
        public Image BackgroundImage
        {
            get
            {
                if (_BackgroundImage == null)
                    this.ApplyTemplate();
                return _BackgroundImage;
            }
        }

		private Image _ForegroundImage = null;
        /// <summary>
        /// Gets the foreground image.
        /// </summary>
        public Image ForegroundImage
        {
            get
            {
                if (_ForegroundImage == null)
                    this.ApplyTemplate();
                return _ForegroundImage;
            }
        }

        private Panel _ControlRoot;
        /// <summary>
        /// Gets the control template root.
        /// </summary>
        public Panel ControlRoot
        {
            get
            {
                if (_ControlRoot == null)
                    this.ApplyTemplate();
                return _ControlRoot;
            }
        }
        
		private Panel _ImageNotFound;
        /// <summary>
        /// Gets the image not found panel.
        /// </summary>
        public Panel ImageNotFound
        {
            get
            {
                if (_ImageNotFound == null)
                    this.ApplyTemplate();
                return _ImageNotFound;
            }
        }
        #endregion

        #region DependencyProperties
        #region Source
        /// <summary>
        /// The source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(TransitionImage), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged), new CoerceValueCallback(OnCoerceSource)));

        static object OnCoerceSource(DependencyObject o, object value)
        {
            // No coersion.
            return value;
        }

        private static void OnSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TransitionImage transitionImage = o as TransitionImage;
            if (transitionImage != null)
                transitionImage.OnSourceChanged((ImageSource)e.OldValue, (ImageSource)e.NewValue);
        }

        void OnSourceChanged(ImageSource oldValue, ImageSource newValue)
        {
            if ((newValue == null) || (newValue.ToString() == String.Empty))
            {
                this.ShowImageNotFound();
            }
            else
            {
                // Only transition if new value is different from existing
                if ((oldValue == null) || (newValue.ToString() != oldValue.ToString()))
                {
                    if (_templateApplied)
                    {
                        this.StartTransition();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }
        #endregion Source
        #endregion DependencyProperties
        #endregion

		#region Methods
        #region Framework Overrides
        public override void OnApplyTemplate()
        {
            if (_templateApplied)
                return;

            _templateApplied = true;

            base.OnApplyTemplate();

            _ForegroundImage = base.GetTemplateChild(FG_IMAGE_NAME) as Image;
            if (_ForegroundImage == null)
                _ForegroundImage = new Image();

            _BackgroundImage = base.GetTemplateChild(BG_IMAGE_NAME) as Image;
            if (_BackgroundImage == null)
                _BackgroundImage = new Image();

            _ControlRoot = base.GetTemplateChild(CONTROL_ROOT_NAME) as Panel;
            if (_ControlRoot == null)
                _ControlRoot = new Grid();

            _ImageNotFound = base.GetTemplateChild(IMG_NOT_FOUND_NAME) as Panel;
            if (_ImageNotFound == null)
                _ImageNotFound = new Grid();
        }

        #endregion

        #region Transition Methods
        void StartTransition()
		{
            this.BackgroundImage.Visibility = Visibility.Visible;
            this.ForegroundImage.Visibility = Visibility.Collapsed;
            this.ImageNotFound.Visibility = Visibility.Collapsed;
            this.ForegroundImage.Opacity = 0;
            this.ForegroundImage.Source = this.Source;

            _pollTimeout = 0; // reset poll timeout
            CompositionTarget.Rendering += PollForSize;
		}

		void PollForSize(object sender, EventArgs e)
		{
            if (this.ForegroundImage.Source == null || _pollTimeout > 60)
            {
                CompositionTarget.Rendering -= PollForSize;
                this.ShowImageNotFound();
                return;
            }
            _pollTimeout++;

            if (this.ForegroundImage.Source.Height > 1)
			{
                _destinationHeight = this.ForegroundImage.Source.Height;
                _destinationWidth = this.ForegroundImage.Source.Width;

				// Normalize Destination values for actual Size of available area
				double aspectRatio = _destinationWidth / _destinationHeight;

				// TODO: Take into account non-square controlRoot aspectRatios
				if (aspectRatio <= 1)
				{
					// Size to Vertical Axis
					_destinationHeight = _ControlRoot.MaxHeight;						
					_destinationWidth = Math.Round(_destinationHeight * aspectRatio);
				}
				else
				{
					_destinationWidth = _ControlRoot.MaxWidth;
					_destinationHeight = Math.Round(_destinationWidth / aspectRatio);
				}

				CompositionTarget.Rendering -= PollForSize;
                if (this.IsVisible) // do transitions
                {
                    if (!_Transitioning)
                    {
                        lock (_TransitionSyncLock)
                        {
                            if (!_Transitioning)
                            {
                                _Transitioning = true;
                                CompositionTarget.Rendering += FadeOutCurrent;
                            }
                        }
                    }
                }
                else // just set the image
                {
                    this.FinalizeImage();
                }
			}
		}

		void FadeOutCurrent(object sender, EventArgs e)
		{
			if (_currentTime < _transitionDuration)
			{				
				this.BackgroundImage.Opacity = ((_transitionDuration - _currentTime) / _transitionDuration);
				_currentTime++;
			}
			else
			{
                this.BackgroundImage.Opacity = 0;
				_currentTime = 0;

				// Get current width
                //this.BackgroundImage.Width = this.BackgroundImage.ActualWidth;
                //this.BackgroundImage.Height = this.BackgroundImage.ActualHeight;

				// Change animation 
				CompositionTarget.Rendering -= FadeOutCurrent;
				CompositionTarget.Rendering += SetNewWidth;
			}
		}

		void SetNewWidth(object sender, EventArgs e)
		{
			if (_currentTime <= _transitionDuration)
			{
                this.BackgroundImage.Width = WPFpenner.easeInQuad(_currentTime, _bgImgOriginalWidth, (_destinationWidth - _bgImgOriginalWidth), _transitionDuration);
                this.BackgroundImage.Height = WPFpenner.easeInQuad(_currentTime, _bgImgOriginalHeight, (_destinationHeight - _bgImgOriginalHeight), _transitionDuration);
				_currentTime++;
			}
			else
			{
				_currentTime = 0;
				CompositionTarget.Rendering -= SetNewWidth;
				CompositionTarget.Rendering += FadeInImage;

                this.ForegroundImage.Opacity = 0;
                this.ForegroundImage.Visibility = Visibility.Visible;
                this.ForegroundImage.Width = this.BackgroundImage.Width;
                this.ForegroundImage.Height = this.BackgroundImage.Height;
			}			
		}

		void FadeInImage(object sender, EventArgs e)
		{
			if (_currentTime < _transitionDuration)
			{
                this.ForegroundImage.Opacity = (_currentTime / _transitionDuration);
				_currentTime++;				
			}
			else
			{
                this.FinalizeImage();
				CompositionTarget.Rendering -= FadeInImage;
                lock(_TransitionSyncLock)
                    _Transitioning = false;
			}

		}

        void FinalizeImage()
        {
            this.BackgroundImage.Source = this.ForegroundImage.Source;
            this.BackgroundImage.Width = _destinationWidth;
            this.BackgroundImage.Height = _destinationHeight;
            _bgImgOriginalWidth = _destinationWidth;
            _bgImgOriginalHeight = _destinationHeight;
            this.BackgroundImage.Opacity = 1;
            this.BackgroundImage.Visibility = Visibility.Visible;

            this.ForegroundImage.Opacity = 0;
            this.ForegroundImage.Visibility = Visibility.Visible;
            this.ForegroundImage.Width = _destinationWidth;
            this.ForegroundImage.Height = _destinationHeight;

            _currentTime = 0;
        }

        void ShowImageNotFound()
        {
            this.BackgroundImage.Visibility = Visibility.Collapsed;
            this.ForegroundImage.Visibility = Visibility.Collapsed;
            this.ImageNotFound.Visibility = Visibility.Visible;
            _rtFG.ScaleX = _rtFG.ScaleY = 0;
        }
        #endregion
        #endregion Events
	}
}
