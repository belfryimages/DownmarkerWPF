using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace MarkPad.Extensions
{
	[InheritedExport]
	public interface ICanCreateNewPage : IMarkPadExtension
	{
		string CreateNewPageLabel { get; }
		string CreateNewPage();
	}
}
