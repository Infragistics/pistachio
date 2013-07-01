using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Media;
using System.Xml;

namespace Pistachio
{
	public class PistachioKey
	{
		#region Properties

		private string _keyName;
		public string KeyName
		{
			get { return _keyName; }
			set { _keyName = value; }
		}

		private List<PistachioReferenceItem> _pagesLocatedIn;
		public List<PistachioReferenceItem> PagesLocatedIn
		{
			get { return _pagesLocatedIn; }
			set { _pagesLocatedIn = value; }
		}

		private string _resourceType;
		public string ResourceType
		{
			get { return _resourceType; }
			set { _resourceType = value; }
		}

		private string _targetType;

		public string TargetType
		{
			get { return _targetType; }
			set { _targetType = value; }
		}
		
		private bool _isUsed;
		public bool IsUsed
		{
			get {
				if (PagesLocatedIn.Count > 0)
				{
					_isUsed = true;
				}
				else
				{
					_isUsed = false;
				}
				return _isUsed; 			
			}
			// This originally did not exist //
			// set { _isUsed = value; }
			///////////////////////////////////
		}

		private string _previewBrushNode;

		public string PreviewBrushNode
		{
			get { return _previewBrushNode; }
			set { _previewBrushNode = value; }
		}
				
		private string _resourceDictionary;

		public string ResourceDictionary
		{
			get { return _resourceDictionary; }
			set { _resourceDictionary = value; }
		}


		#endregion Properties

		#region Constructor

		public PistachioKey(string key, string resourceType, string resourceDictionary)
		{
			KeyName = key;
			ResourceType = resourceType;
			ResourceDictionary = resourceDictionary;
			PagesLocatedIn = new List<PistachioReferenceItem>();
			PreviewBrushNode = String.Empty;
			// PreviewBrush = new SolidColorBrush(Colors.Transparent);
		}

		public PistachioKey(string key, string targetType, string resourceType, string resourceDictionary)
		{
			KeyName = key;
			TargetType = targetType;
			ResourceType = resourceType;
			ResourceDictionary = resourceDictionary;
			PagesLocatedIn = new List<PistachioReferenceItem>();
			PreviewBrushNode = String.Empty;
			// PreviewBrush = new SolidColorBrush(Colors.Transparent);
		}

		public PistachioKey(string key, string targetType, string resourceType, string resourceDictionary, string previewBrushNode)
		{
			KeyName = key;
			TargetType = targetType;
			ResourceType = resourceType;
			ResourceDictionary = resourceDictionary;
			PagesLocatedIn = new List<PistachioReferenceItem>();
			PreviewBrushNode = previewBrushNode;
		}

		#endregion Constructor
	}
}
