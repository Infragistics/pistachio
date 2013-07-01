using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Markup;

namespace Pistachio
{
	//This code is just a prototype of some helpers
	//to make XamlReading more convenient...let me know
	//what you think.
	public static class XamlReader2<T> where T : class
	{
		public static T Load(Stream stream)
		{
			object o = System.Windows.Markup.XamlReader.Load(stream);
			return CheckRootType(o);
		}

		public static T Load(XmlReader xmlReader)
		{
			object o = XamlReader.Load(xmlReader);
			return (CheckRootType(o));
		}

		private static T CheckRootType(object o)
		{
			T root = o as T;
			if (root != null)
				return root;
			else
				throw new Exception("Expected <" + typeof(T).Name + "> as root tag.");
		}

		public static T Load(FileInfo fileInfo)
		{
			if (!fileInfo.Exists)
				throw new FileNotFoundException(fileInfo.FullName + " doesn't exist.");
			StreamReader sr = new StreamReader(fileInfo.FullName);
			return XamlReader2<T>.Load(sr.BaseStream);
		}

		public static T Load(string xamlContainingString)
		{
			StringReader stringReader = new StringReader(xamlContainingString);
			XmlReader xmlReader = XmlTextReader.Create(stringReader, new XmlReaderSettings());
			return Load(xmlReader);
		}
	}
}
