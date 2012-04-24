using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Extensions;

namespace Extension.Example
{
	public class ExampleExtension : 
		ICanCreateNewPage
	{
		public string Name { get { return "Example extension"; } }

		public string CreateNewPageLabel { get { return "New example extension page"; } }

		public string CreateNewPage()
		{
			return "# Hello from the `Example` extension";
		}
	}
}
