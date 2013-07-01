using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Pistachio
{
	class PistachioKeyManager : INotifyPropertyChanged
	{
		#region Properties

		private ObservableCollection<PistachioPage> _pages;
		public ObservableCollection<PistachioPage> Pages
		{
			get { return _pages; }
			set { _pages = value; }
		}

		private ObservableCollection<PistachioPage> _activePage;

		public ObservableCollection<PistachioPage> ActivePage
		{
			get { return _activePage; }
			set { _activePage = value; }
		}

		public ObservableCollection<PistachioKey> ActivePageKeys
		{
			get {
				if (ActivePage.Count == 1)
				{
					return ActivePage[0].Keys;
				}
				else
				{
					return new ObservableCollection<PistachioKey>();
				}				
			}			
		}


		#endregion Properties

		public void SetActivePage(PistachioPage activePage)
		{
			_activePage.Clear();
			ActivePage.Add(activePage);
		}

		#region Constructor

		public PistachioKeyManager()
		{
			Pages = new ObservableCollection<PistachioPage>();
			ActivePage = new ObservableCollection<PistachioPage>();
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
