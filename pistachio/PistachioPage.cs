using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Pistachio
{
	class PistachioPage : INotifyPropertyChanged
	{
		#region Properties

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private ObservableCollection<PistachioKey> _keys;
		public ObservableCollection<PistachioKey> Keys
		{
			get { return _keys; }
			set { _keys = value; }
		}

		private int _keyCount;
		public int KeyCount
		{
			get { return _keys.Count; }
		}

		public string KeyUsageInfo
		{
			get {
				int usedKeyCount = 0;
				foreach (PistachioKey key in this.Keys)
				{
					if (key.IsUsed)
					{
						usedKeyCount++;
					}
				}
				return this.Keys.Count.ToString() + "/" + usedKeyCount.ToString(); 
			}
		}

		#endregion Properties

		#region Constructor

		public PistachioPage(string name)
		{
			Name = name;
			Keys = new ObservableCollection<PistachioKey>();
		}
		
		public PistachioPage(string name, ObservableCollection<PistachioKey> keys)
		{
			Name = name;
			Keys = keys;
		}

		public PistachioPage(string name, PistachioKey key)
		{
			Name = name;
			ObservableCollection<PistachioKey> collection = new ObservableCollection<PistachioKey>();
			collection.Add(key);
			Keys = collection;
		}

		#endregion Constructor

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		#endregion
	}
}
