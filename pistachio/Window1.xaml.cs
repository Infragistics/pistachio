using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Markup;

namespace Pistachio
{
	public partial class Window1
	{
		#region Fields and Properties

		private string _currentProject;
		private string _currentPage;
		private double _totalProgress;
		private string _output = null;
		Regex regexExp;
		MatchCollection matchObj;
		OpenFileDialog openProjectDialog = new OpenFileDialog();
		StreamReader myFile;
		List<string> filesInCSProj = new List<string>();
		List<string> nonReferencedKeys = new List<string>();
		List<string> referencedKeys = new List<string>();
		List<PistachioReferenceItem> listOfFiles = new List<PistachioReferenceItem>();
		Dictionary<string, string> allFilesCache = new Dictionary<string, string>();
		Dictionary<string, PistachioKey> allTheKeys = new Dictionary<string, PistachioKey>();
		ObservableCollection<PistachioKey> dataSourceKeys;
		PistachioKeyManager keyManager = new PistachioKeyManager();


		#endregion Fields and Properties

		#region Constructor

		public Window1()
		{
			this.InitializeComponent();
			CurrentProject = "Select Project (.csproj)";
		}

		#endregion Constructor

		#region Methods

		#region Get File Name In Main Directory

		public static string GetFileNameInMainDirectory(string currentDirectory, string fileName)
		{
			string returnVal;
			int start = fileName.IndexOf(currentDirectory);
			int mainLength = currentDirectory.Length + 1;
			int end = fileName.Length;
			returnVal = fileName.Substring(start + mainLength, (end - (start + mainLength)));
			return returnVal;
		}

		#endregion Get File Name In Main Directory

		#region GetNumFilesInCsProj
		private int GetNumFilesInCsProj(string csProjFile)
		{
			int files = 0;
			XmlDocument doc = new XmlDocument();

			// Load File
			doc.Load(csProjFile);

			// Add Namespaces
			XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
			mgr.AddNamespace("default", "http://schemas.microsoft.com/developer/msbuild/2003");

			// Get all xaml and .cs files
			XmlNodeList allFiles = doc.SelectNodes("//*[@Include[contains(.,'.xaml') or contains(.,'.cs')]]", mgr); //doc.SelectNodes("//*[@Include[contains(., '.xaml')]]", mgr);

			string currentFile;
			string appDir = System.IO.Path.GetDirectoryName(csProjFile);
			foreach (XmlNode node in allFiles)
			{
				currentFile = node.Attributes["Include"].Value;
				if (currentFile.EndsWith(".xaml") || currentFile.EndsWith(".cs"))
				{
					files++;//files.Add(System.IO.Path.Combine(appDir, currentFile));
				}
			}
			return files;
		}
		#endregion GetNumFilesInCsProj

		#region SelectProjectFile

		private void SelectProjectFile()
		{
			openProjectDialog.Filter = "C# Project Files (*.csproj)|*.csproj";
			openProjectDialog.ShowDialog();
			if (openProjectDialog.FileName == null || openProjectDialog.FileName == "")
			{
				SelectProjectFile();
			}
			else
			{
				CurrentProject = openProjectDialog.FileName + "   (" + GetNumFilesInCsProj(openProjectDialog.FileName) + ")";
				_currentProject = openProjectDialog.FileName;
			}
		}

		#endregion SelectProjectFile

		#region Find Resource Key In List Of Files

		private int FindResourceReferences(PistachioKey key, Dictionary<string, string> Files)
		{
			int timesFound = 0;
			char[] characters = { '\n' };	

			foreach (KeyValuePair<string, string> pair in Files)
			{
				if (pair.Key.EndsWith(".cs"))
				{
					string[] lines = pair.Value.Split(characters);
					int lineNum = 0;
					foreach (string line in lines)
					{
						lineNum++;
						if (line.IndexOf(key.KeyName) != -1)
						{
							key.PagesLocatedIn.Add(new PistachioReferenceItem(pair.Key, lineNum));
							timesFound++;
						}
					}
				}
				else
				{
					string[] lines = pair.Value.Split(characters);
					int lineNum = 0;
					foreach (string line in lines)
					{
						lineNum++;
						if (line.IndexOf("Resource " + key.KeyName) != -1)
						{
							key.PagesLocatedIn.Add(new PistachioReferenceItem(pair.Key, lineNum));
							timesFound++;
						}
					}
				}				
				//regexExp = new Regex("{([a-zA-z]+)Resource " + key.KeyName + "");
				//if (regexExp.IsMatch(pair.Value))
				//{
				//     matchObj = regexExp.Matches(pair.Value);
				//     foreach (Match match in matchObj)
				//     {
				//          key.PagesLocatedIn.Add(new PistachioReferenceItem(pair.Key, 0));
				//          timesFound++;
				//     }
				//}
			}
			return timesFound;
		}

		#endregion Find Resource Key In List Of Files

		#region Build Page Collection

		private void buildPageCollection()
		{
			filesInCSProj = GetFilesFromCsProjFile(_currentProject);
			string appDir = System.IO.Path.GetDirectoryName(_currentProject);
			foreach (string file in filesInCSProj)
			{
				FindAllResources(file);
			}

			// Now find usage //
			int currentPage = 0;
			int totalPages = keyManager.Pages.Count;

			int currentKey = 0;
			int totalKeys = 0;

			// Find total keys //
			foreach (PistachioPage page in keyManager.Pages)
			{
				foreach (PistachioKey key in page.Keys)
				{
					totalKeys++;
				}
			}

			// Now Find out if they are actually USED... //	
			foreach (PistachioPage page in keyManager.Pages)
			{
				currentPage++;
				foreach (PistachioKey key in page.Keys)
				{
					currentKey++;
					_totalProgress = (currentKey / totalKeys);
					Debug.WriteLine(_totalProgress);
					FindResourceReferences(key, allFilesCache);
					LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send,
								new OnReportProgressInvoke(OnReportProgress),
								new ProgressEventArgs(0, 0, totalKeys, currentKey,
															0, 0, "", "page " + currentPage + " of " + totalPages, 0, 0));
				}
			}

		}

		#endregion Build Page Collection

		#region GetFilesFromCsProjFile
		private List<string> GetFilesFromCsProjFile(string csProjFile)
		{
			List<string> files = new List<string>();
			XmlDocument doc = new XmlDocument();

			// Load File
			doc.Load(csProjFile);

			// Add Namespaces
			XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
			mgr.AddNamespace("default", "http://schemas.microsoft.com/developer/msbuild/2003");

			// Get all xaml files
			XmlNodeList xamlFiles = doc.SelectNodes("//*[@Include[contains(.,'.xaml') or contains(.,'.cs')]]", mgr);

			string currentFile;
			string appDir = System.IO.Path.GetDirectoryName(csProjFile);
			foreach (XmlNode node in xamlFiles)
			{
				currentFile = node.Attributes["Include"].Value;
				if (currentFile.EndsWith(".xaml") || currentFile.EndsWith(".cs") )
				{
					files.Add(System.IO.Path.Combine(appDir, currentFile));
				}
			}
			return files;
		}
		#endregion GetFilesFromCsProjFile

		#region FindAllResources
		private void FindAllResources(string fileName)
		{
			string appDir = System.IO.Path.GetDirectoryName(_currentProject);

			if (fileName.EndsWith(".cs"))
			{
				// Load C# File //
				myFile = new StreamReader(fileName);
				string info = myFile.ReadToEnd();
				myFile.Close();
				_output = info;

				allFilesCache.Add(GetFileNameInMainDirectory(appDir, fileName), info);

			}
			else
			{
				XmlDocument doc = new XmlDocument();

				// Load File
				myFile = new StreamReader(fileName);
				string info = myFile.ReadToEnd();
				myFile.Close();
				_output = info;

				allFilesCache.Add(GetFileNameInMainDirectory(appDir, fileName), info);

				dataSourceKeys = new ObservableCollection<PistachioKey>();

				// Load for XPath
				doc.Load(fileName);

				// Add Namespaces
				XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
				mgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
				mgr.AddNamespace("presentation", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
				mgr.AddNamespace("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
				mgr.AddNamespace("d", "http://schemas.microsoft.com/expression/interactivedesigner/2006");
				mgr.AddNamespace("igWindows", "http://infragistics.com/Windows");
				mgr.AddNamespace("editors", "http://infragistics.com/Editors");
				mgr.AddNamespace("igDP", "http://infragistics.com/DataPresenter");

				XmlNodeList keyedNodes = doc.SelectNodes("//*[@x:Key]", mgr);
				// XmlNodeList unkeyedNodes = doc.SelectNodes("//Style", mgr);
				//XmlNodeList unkeyedNodes = doc.SelectNodes("//*[@TargetType][not(@x:Key)]", mgr);
				XmlNodeList unkeyedNodes = doc.SelectNodes("//*/presentation:Style[not(@x:Key)][@TargetType]", mgr);
					//XmlNodeList staticResourceReferences;
					//XmlNodeList dynamicResourceReferences;
				Debug.WriteLine("Number of Keyed Items: " + keyedNodes.Count);

				foreach (XmlNode node in keyedNodes)
				{
					string keyName = node.Attributes["x:Key"].Value;
					string typeName = node.Name;
					string targetType = null;
					
					// string val = "Name: " + node.Attributes["x:Key"].Value + ", ResourceType: " + node.Name;
					if (node.Attributes["TargetType"] != null)
					{
						// val += ", TargetType: " + node.Attributes["TargetType"].Value;
						targetType = node.Attributes["TargetType"].Value;
						/////keyName += node.Attributes["TargetType"].Value;						
					}

					if (node.Attributes["x:Key"] != null)
					{
						XmlAttribute key = node.Attributes["x:Key"];
						node.Attributes.Remove(key);
					}

					if (targetType != null)
					{
						// PistachioKey key = new PistachioKey(keyName, targetType, typeName, GetFileNameInMainDirectory(appDir, fileName), node);
						// LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new SetBrushPreviewInvoke(SetBrushPreview), key, node);
						dataSourceKeys.Add(new PistachioKey(keyName, targetType, typeName, GetFileNameInMainDirectory(appDir, fileName), node.OuterXml));
					}
					else
					{
						// PistachioKey key = new PistachioKey(keyName, "", typeName, GetFileNameInMainDirectory(appDir, fileName), node);
						// LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new SetBrushPreviewInvoke(SetBrushPreview), key, node);
						dataSourceKeys.Add(new PistachioKey(keyName, "", typeName, GetFileNameInMainDirectory(appDir, fileName), node.OuterXml));
					}
					// LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new SetBrushPreviewInvoke(SetBrushPreview), key, node);
					// LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new AddBrushPreviewInvoke(AddBrushPreview), node);
					// LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new OnStartInvoke(OnProcessingStart));
					
					
					#region UnUsed Old Stuff

					/////KeyedResources.Add(val);

							//staticResourceReferences = doc.SelectNodes("//*[@*[contains(., 'StaticResource " + node.Attributes["x:Key"].Value + "')]]", mgr);
							//dynamicResourceReferences = doc.SelectNodes("//*[@*[contains(., 'DynamicResource " + node.Attributes["x:Key"].Value + "')]]", mgr);

					//foreach (XmlNode resref in staticResourceReferences)
					//{
					//     Console.WriteLine(resref.Name + " references " + node.Attributes["x:Key"].Value);						
					//     keyName = node.Attributes["x:Key"].Value;
					//     typeName = node.Name;
					//     dataSourceKeys.Add(new PistachioKey(keyName, typeName, GetFileNameInMainDirectory(appDir, fileName)));
					//}

					//foreach (XmlNode resref in dynamicResourceReferences)
					//{
					//     Console.WriteLine(resref.Name + " references " + node.Attributes["x:Key"].Value);
					//     keyName = node.Attributes["x:Key"].Value;
					//     typeName = node.Name;
					//     dataSourceKeys.Add(new PistachioKey(keyName, typeName, GetFileNameInMainDirectory(appDir, fileName)));
					//}

					#endregion UnUsed Old Stuff
				}

				foreach (XmlNode node in unkeyedNodes)
				{
					string val = node.Name + ": Style.TargetType: " + node.Attributes["TargetType"].Value;
					if (node.Name == "Style")
					{
						/////Styles.Add(val);

						string keyName = node.Attributes["TargetType"].Value;
						string typeName = node.Name;

						dataSourceKeys.Add(new PistachioKey(keyName, keyName, typeName, GetFileNameInMainDirectory(appDir, fileName)));
					}
				}

				
				keyManager.Pages.Add(new PistachioPage(GetFileNameInMainDirectory(appDir, fileName), dataSourceKeys));			

			}
		}
		#endregion FindAllResources

		#region Asynchronous

		#region Processing

		public void startProcessing()
		{
			if (CurrentProject != null && CurrentProject != "")
			{
				// Start Asynchronous Synchronization
				DoProcessInvoke newSync = new DoProcessInvoke(this.DoProcess);
				newSync.BeginInvoke(null, null);
				Thread.Sleep(10);
			}
		}

		private delegate void DoProcessInvoke();
		private void DoProcess()
		{
			// Report that we've started
			LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new OnStartInvoke(OnProcessingStart));
			LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new OnReportProgressInvoke(OnReportProgress), new ProgressEventArgs(0, 0, 0, 0, 0, 0, " ", "", 0, 0));
			buildPageCollection();
			//fetchAllKeys();
			LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new OnReportProgressInvoke(OnReportProgress), new ProgressEventArgs(1, 1, 0, 0, 0, 0, " ", "", 0, 0));
			LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Send, new OnEndInvoke(OnProcessingEnd));
		}

		#endregion Processing

		#region Dispatcher Stuff

		#region OnReportProgress
		private delegate void OnReportProgressInvoke(ProgressEventArgs progress);
		private void OnReportProgress(ProgressEventArgs e)
		{
			if (e.TotalFileCount > 0)
			{
				CurrentProgress = (int)((double)e.TotalFilesComplete / (double)e.TotalFileCount * 100);
			}
			Debug.WriteLine("current progress : " + CurrentProgress);
			Debug.WriteLine("in case it doesn't update::: " + e.TotalFilesComplete + " " + e.TotalFileCount);
			CurrentPageProgress = e.NotesOnCurrent;
		}
		#endregion OnReportProgress

		#region AddBrushPreview
		private delegate void AddBrushPreviewInvoke(XmlNode node);
		private void AddBrushPreview(XmlNode node)
		{
			Rectangle rect = new Rectangle();
			rect.Width = 75;
			rect.Height = 30;
			rect.Margin = new Thickness(5, 5, 5, 5);

			if (node.Attributes["x:Key"] != null)
			{
				XmlAttribute key = node.Attributes["x:Key"];
				node.Attributes.Remove(key);
			}

			switch (node.Name)
			{
				case "SolidColorBrush":				
					SolidColorBrush brush = XamlReader2<SolidColorBrush>.Load(node.OuterXml);
					rect.Fill = brush;

					RectPreviews.Children.Add(rect);
					break;

				case "LinearGradientBrush":
					LinearGradientBrush lgb = XamlReader2<LinearGradientBrush>.Load(node.OuterXml);
					rect.Fill = lgb;

					RectPreviews.Children.Add(rect);
					break;

				case "RadialGradientBrush":
					RadialGradientBrush rgb = XamlReader2<RadialGradientBrush>.Load(node.OuterXml);
					rect.Fill = rgb;

					RectPreviews.Children.Add(rect);
					break;

				default:
					break;
			}
		}
		#endregion

		#region SetPreviewBrush
		private delegate void SetBrushPreviewInvoke(PistachioKey key, XmlNode node);
		//private void SetBrushPreview(PistachioKey pistachioKey, XmlNode node)
		//{
		//     if (node.Attributes["x:Key"] != null)
		//     {
		//          XmlAttribute key = node.Attributes["x:Key"];
		//          node.Attributes.Remove(key);
		//     }

		//     try
		//     {
		//          switch (node.Name)
		//          {
		//               case "SolidColorBrush":
		//                    pistachioKey.PreviewBrush = XamlReader2<SolidColorBrush>.Load(node.OuterXml);
		//                    break;

		//               case "LinearGradientBrush":
		//                    pistachioKey.PreviewBrush = XamlReader2<LinearGradientBrush>.Load(node.OuterXml);
		//                    break;

		//               case "RadialGradientBrush":
		//                    pistachioKey.PreviewBrush = XamlReader2<RadialGradientBrush>.Load(node.OuterXml);
		//                    break;

		//               default:
		//                    pistachioKey.PreviewBrush = new SolidColorBrush(Colors.Transparent);
		//                    break;
		//          }
		//     }
		//     catch (Exception ex)
		//     {
		//          pistachioKey.PreviewBrush = new SolidColorBrush(Colors.Transparent);
		//     }
		//     finally
		//     {

		//     }
		//}
		#endregion

		#endregion Dispatcher Stuff

		#endregion Asynchronous

		#endregion Methods

		#region Event Handlers

		#region Buttons

		public void btnChooseProject_Click(object sender, RoutedEventArgs e)
		{
			keyManager = new PistachioKeyManager();

			try
			{
				nonReferencedKeys.Clear();
				referencedKeys.Clear();
				listOfFiles.Clear();
				allFilesCache.Clear();
				allTheKeys.Clear();
				dataSourceKeys.Clear();
			}
			catch (NullReferenceException ex)
			{

			}

			SelectProjectFile();
			Cursor = System.Windows.Input.Cursors.Wait;
			if (_currentProject != null || _currentProject != "")
			{
				startProcessing();
				Cursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		public void btnFetchKeys_Click(object sender, RoutedEventArgs e)
		{
			startProcessing();
		}


		#endregion Buttons

		#region CarouselListBox
		private void PagesCarousel_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PistachioPage activePage = PagesCarousel.SelectedItem as PistachioPage;
			keyManager.SetActivePage(activePage);
			_currentPage = "details: " + activePage.Name;
			CurrentPage = _currentPage;
			xdgVisualizer.Records[0].IsExpanded = true;
}
		#endregion CarouselListBox

		#endregion Event Handlers		

		#region DependencyProperties

		public static readonly DependencyProperty CurrentProjectProperty = DependencyProperty.Register("CurrentProject", typeof(string), typeof(Window1), new FrameworkPropertyMetadata(null));
		/// <summary>
		/// Contains an informational string of the current viewing state.
		/// </summary>
		public string CurrentProject
		{
			get
			{
				return (string)GetValue(CurrentProjectProperty);
			}
			set
			{
				SetValue(CurrentProjectProperty, value);
			}
		}

		public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(string), typeof(Window1), new FrameworkPropertyMetadata(null));
		/// <summary>
		/// Contains an informational string of the current viewing state.
		/// </summary>
		public string CurrentPage
		{
			get
			{
				return (string)GetValue(CurrentPageProperty);
			}
			set
			{
				SetValue(CurrentPageProperty, value);
			}
		}

		public static readonly DependencyProperty CurrentPageProgressProperty = DependencyProperty.Register("CurrentPageProgress", typeof(string), typeof(Window1), new FrameworkPropertyMetadata(null));
		/// <summary>
		/// Contains an informational string of the current viewing state.
		/// </summary>
		public string CurrentPageProgress
		{
			get
			{
				return (string)GetValue(CurrentPageProgressProperty);
			}
			set
			{
				SetValue(CurrentPageProgressProperty, value);
			}
		}

		public static readonly DependencyProperty CurrentProgressProperty = DependencyProperty.Register("CurrentProgress", typeof(int), typeof(Window1), new FrameworkPropertyMetadata(null));
		/// <summary>
		/// Contains an informational string of the current viewing state.
		/// </summary>
		public int CurrentProgress
		{
			get
			{
				return (int)GetValue(CurrentProgressProperty);
			}
			set
			{
				SetValue(CurrentProgressProperty, value);
			}
		}

		#endregion DependencyProperties
		
		#region Routed Events

			#region ProcessingStart
		/// <summary>
		/// Event ID for the 'ProcessingStart' routed event
		/// </summary>
		public static readonly RoutedEvent ProcessingStartEvent = EventManager.RegisterRoutedEvent("ProcessingStart", RoutingStrategy.Direct, typeof(EventHandler<RoutedEventArgs>), typeof(Window1));

		/// <summary>
		/// Occurs when the search begins
		/// </summary>

		private delegate void OnStartInvoke();
		protected virtual void OnProcessingStart()
		{
			RoutedEventArgs args = new RoutedEventArgs();
			args.RoutedEvent = Window1.ProcessingStartEvent;
			args.Source = this;
			this.RaiseEvent(args);
		}

		/// <summary>
		/// Occurs when the search begins
		/// </summary>
		[Description("Occurs when the asyncronous process begins")]
		[Category("Behavior")]
		public event EventHandler<RoutedEventArgs> ProcessingStart
		{
			add
			{
				base.AddHandler(Window1.ProcessingStartEvent, value);
			}
			remove
			{
				base.RemoveHandler(Window1.ProcessingStartEvent, value);
			}
		}
		#endregion ProcessingStart
	
			#region ProcessingEnd
		/// <summary>
		/// Event ID for the 'ProcessingEnd' routed event
		/// </summary>
		public static readonly RoutedEvent ProcessingEndEvent = EventManager.RegisterRoutedEvent("ProcessingEnd", RoutingStrategy.Direct, typeof(EventHandler<RoutedEventArgs>), typeof(Window1));

		/// <summary>
		/// Occurs when the search begins
		/// </summary>

		private delegate void OnEndInvoke();
		protected virtual void OnProcessingEnd()
		{
			RoutedEventArgs args = new RoutedEventArgs();
			args.RoutedEvent = Window1.ProcessingEndEvent;
			args.Source = this;
			this.RaiseEvent(args);

			xdgVisualizer.DataSource = keyManager.ActivePage; //  Keys;
			PagesCarousel.ItemsSource = keyManager.Pages;
		}

		/// <summary>
		/// Occurs when the search begins
		/// </summary>
		[Description("Occurs when the asyncronous process completes")]
		[Category("Behavior")]
		public event EventHandler<RoutedEventArgs> ProcessingEnd
		{
			add
			{
				base.AddHandler(Window1.ProcessingEndEvent, value);
			}
			remove
			{
				base.RemoveHandler(Window1.ProcessingEndEvent, value);
			}
		}
		#endregion ProcessingEnd

		#endregion Routed Events

	}
}