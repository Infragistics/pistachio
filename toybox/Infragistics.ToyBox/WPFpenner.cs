using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Infragistics.ToyBox
{
	/// <summary>
	/// This class contains all of the different types of equations
	/// </summary>
	public static class WPFpenner
	{

		/*
		Easing Equations (c) 2003 Robert Penner, all rights reserved.
		This work is subject to the terms in http://www.robertpenner.com/easing_terms_of_use.html.
		*/


		/// <summary>
		/// This performs the Ease Out Elastic Equation (decelerating to the newVal with elastic like movement)
		/// </summary>
		/// <param name="time">The current time</param>
		/// <param name="startVal">The beginning value</param>
		/// <param name="newVal">The change in value</param>
		/// <param name="duration">The duration of time</param>
		/// <returns></returns>
		public static double easeOutElastic(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration) == 1) return startVal + newVal;
			double p = duration * .3;
			double s = p / 4;
			return (newVal * Math.Pow(2, -10 * time) * Math.Sin((time * duration - s) * (2 * Math.PI) / p) + newVal + startVal);
		}

		public static double easeInElastic(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration) == 1) return startVal + newVal;
			double p = duration * .3;
			double s = p / 4;
			return -(newVal * Math.Pow(2, 10 * (time -= 1)) * Math.Sin((time * duration - s) * (2 * Math.PI) / p)) + startVal;
		}

		public static double easeInOutElastic(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration / 2) == 2) return startVal + newVal;
			double p = duration * (.3 * 1.5);
			double s = p / 4;
			if (time < 1) return -.5 * (newVal * Math.Pow(2, 10 * (time -= 1)) * Math.Sin((time * duration - s) * (2 * Math.PI) / p)) + startVal;
			return newVal * Math.Pow(2, -10 * (time -= 1)) * Math.Sin((time * duration - s) * (2 * Math.PI) / p) * .5 + newVal + startVal;
		}

		public static double easeOutBounce(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration) < (1 / 2.75)) return newVal * (7.5625 * time * time) + startVal;
			else if (time < (2 / 2.75)) return newVal * (7.5625 * (time -= (1.5 / 2.75)) * time + .75) + startVal;
			else if (time < (2.5 / 2.75)) return newVal * (7.5625 * (time -= (2.25 / 2.75)) * time + .9375) + startVal;
			else return newVal * (7.5625 * (time -= (2.625 / 2.75)) * time + .984375) + startVal;
		}

		public static double easeInBounce(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration) < (1 / 2.75)) return newVal * (7.5625 * time * time) + startVal;
			else if (time < (2 / 2.75)) return newVal * (7.5625 * (time -= (1.5 / 2.75)) * time + .75) + startVal;
			else if (time < (2.5 / 2.75)) return newVal * (7.5625 * (time -= (2.25 / 2.75)) * time + .9375) + startVal;
			else return newVal * (7.5625 * (time -= (2.625 / 2.75)) * time + .984375) + startVal;
			// return newVal - easeOutBounce(duration - time, 0, newVal, duration) + startVal;
		}

		public static double easeInOutBounce(double time, double startVal, double newVal, double duration)
		{
			if (time < duration / 2) return easeInBounce(time * 2, 0, newVal, duration) * .5 + startVal;
			else return easeOutBounce(time * 2 - duration, 0, newVal, duration) * .5 + newVal * .5 + startVal;
		}

		public static double easeOutExpo(double time, double startVal, double newVal, double duration)
		{
			return (time == duration) ? startVal + newVal : newVal * (-Math.Pow(2, -10 * time / duration) + 1) + startVal;
		}

		public static double easeInExpo(double time, double startVal, double newVal, double duration)
		{
			return (time == 0) ? startVal : newVal * Math.Pow(2, 10 * (time / duration - 1)) + startVal;
		}

		public static double easeInOutExpo(double time, double startVal, double newVal, double duration)
		{
			if (time == 0) return startVal;
			if (time == duration) return startVal + newVal;
			if ((time /= duration / 2) < 1) return newVal / 2 * Math.Pow(2, 10 * (time - 1)) + startVal;
			return newVal / 2 * (-Math.Pow(2, -10 * --time) + 2) + startVal;
		}

		public static double easeOutQuad(double time, double startVal, double newVal, double duration)
		{
			return -newVal * (time /= duration) * (time - 2) + startVal;
		}

		public static double easeInQuad(double time, double startVal, double newVal, double duration)
		{
			return newVal * (time /= duration) * time + startVal;
		}

		public static double easeInOutQuad(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration / 2) < 1) return newVal / 2 * time * time + startVal;
			return -newVal / 2 * ((--time) * (time - 2) - 1) + startVal;
		}

		public static double easeOutSine(double time, double startVal, double newVal, double duration)
		{
			return newVal * Math.Sin(time / duration * (Math.PI / 2)) + startVal;
		}

		public static double easeInSine(double time, double startVal, double newVal, double duration)
		{
			return -newVal * Math.Cos(time / duration * (Math.PI / 2)) + newVal + startVal;
		}

		public static double easeInOutSine(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration / 2) < 1) return newVal / 2 * (Math.Sin(Math.PI * time / 2)) + startVal;
			return -newVal / 2 * (Math.Cos(Math.PI * --time / 2) - 2) + startVal;
		}

		public static double easeOutCirc(double time, double startVal, double newVal, double duration)
		{
			return newVal * Math.Sqrt(1 - (time = time / duration - 1) * time) + startVal;
		}

		public static double easeInCirc(double time, double startVal, double newVal, double duration)
		{
			return -newVal * (Math.Sqrt(1 - (time /= duration) * time) - 1) + startVal;
		}

		public static double easeInOutCirc(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration / 2) < 1) return -newVal / 2 * (Math.Sqrt(1 - time * time) - 1) + startVal;
			return newVal / 2 * (Math.Sqrt(1 - (time -= 2) * time) + 1) + startVal;
		}

		public static double easeOutCubic(double time, double startVal, double newVal, double duration)
		{
			return newVal * ((time = time / duration - 1) * time * time + 1) + startVal;
		}

		public static double easeInCubic(double time, double startVal, double newVal, double duration)
		{
			return newVal * (time /= duration) * time * time + startVal;
		}

		public static double easeInOutCubic(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration / 2) < 1) return newVal / 2 * time * time * time + startVal;
			return newVal / 2 * ((time -= 2) * time * time + 2) + startVal;
		}

		public static double easeOutQuint(double time, double startVal, double newVal, double duration)
		{
			return newVal * ((time = time / duration - 1) * time * time * time * time + 1) + startVal;
		}

		public static double easeInQuint(double time, double startVal, double newVal, double duration)
		{
			return newVal * (time /= duration) * time * time * time * time + startVal;
		}

		public static double easeInOutQuint(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration / 2) < 1) return newVal / 2 * time * time * time * time * time + startVal;
			return newVal / 2 * ((time -= 2) * time * time * time * time + 2) + startVal;
		}

		public static double easeOutBack(double time, double startVal, double newVal, double duration)
		{
			return newVal * ((time = time / duration - 1) * time * ((1.70158 + 1) * time + 1.70158) + 1) + startVal;
		}

		public static double easeInBack(double time, double startVal, double newVal, double duration)
		{
			return newVal * (time /= duration) * time * ((1.70158 + 1) * time - 1.70158) + startVal;
		}

		public static double easeInOutBack(double time, double startVal, double newVal, double duration)
		{
			double s = 1.70158;
			if ((time /= duration / 2) < 1) return newVal / 2 * (time * time * (((s *= (1.525)) + 1) * time - s)) + startVal;
			return newVal / 2 * ((time -= 2) * time * (((s *= (1.525)) + 1) * time + s) + 2) + startVal;
		}

		public static double easeOutQuart(double time, double startVal, double newVal, double duration)
		{
			return -newVal * ((time = time / duration - 1) * time * time * time - 1) + startVal;
		}

		public static double easeInQuart(double time, double startVal, double newVal, double duration)
		{
			return newVal * (time /= duration) * time * time * time + startVal;
		}

		public static double easeInOutQuart(double time, double startVal, double newVal, double duration)
		{
			if ((time /= duration / 2) < 1) return newVal / 2 * time * time * time * time + startVal;
			return -newVal / 2 * ((time -= 2) * time * time * time - 2) + startVal;
		}

		public static double linear(double time, double startVal, double newVal, double duration)
		{
			return newVal * time / duration + startVal;
		}

	}

}