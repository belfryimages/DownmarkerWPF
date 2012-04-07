using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace MarkPad.MarkPadExtensions
{
	public class MarkPadExtensionViewModel
	{
		IPackage _package;

		public MarkPadExtensionViewModel(IPackage package)
		{
			_package = package;
		}

		public string Title
		{
			get
			{
				if (!string.IsNullOrEmpty(_package.Title)) return _package.Title;
				return _package.Id;
			}
		}
		public string Authors { get { return string.Join(", ", _package.Authors); } }
		public string Description
		{
			get
			{
				if (!string.IsNullOrEmpty(_package.Description)) return _package.Description;
				return "";
			}
		}
	}
}
