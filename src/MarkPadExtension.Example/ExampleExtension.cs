using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Extensions;

namespace MarkPadExtension.Example
{
	public class ExampleExtension : 
		ICanCreateNewPage
	{
		public string Name { get { return "Example extension"; } }

		public string CreateNewPageLabel { get { return "Example - New page"; } }

		public string CreateNewPage()
		{
			return "# Hello from ExampleExtension";
		}
	}
}
