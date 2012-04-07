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

		public string Title { get { return _package.Title; } }
	}
}
