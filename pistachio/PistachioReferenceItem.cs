using System;
using System.Collections.Generic;
using System.Text;

namespace Pistachio
{
	public class PistachioReferenceItem
	{
		private string _fileName;
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		private int _lineNum;
		public int LineNum
		{
			get { return _lineNum; }
			set { _lineNum = value; }
		}

		public PistachioReferenceItem(string fileName, int lineNum)
		{
			FileName = fileName;
			LineNum = lineNum;
		}

	}
}
