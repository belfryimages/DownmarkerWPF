using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkPad.Extensions
{
	public interface ICanCreateNewPage : IMarkPadExtension
	{
		string CreateNewPageLabel { get; }
		string CreateNewPage();
	}
}
