using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace MarkPad.Extensions
{
	// MEF has a convention model (http://blogs.msdn.com/b/hammett/archive/2011/03/08/mef-s-convention-model.aspx)
	// which should mean the dependency on MEF could be dropped from MarkPad.Extensions.dll, but that isn't
	// in the MEF in .NET 4, so for now this attribute is required.
	[InheritedExport]
	public interface IMarkPadExtension
	{
		string Name { get; }
	}
}
