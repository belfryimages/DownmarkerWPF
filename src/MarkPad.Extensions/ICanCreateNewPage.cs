using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkPad.Extensions
{
	public interface ICanCreateNewPage
	{
		string CreateNewPageLabel { get; }
		string CreateNewPage();
	}
}
