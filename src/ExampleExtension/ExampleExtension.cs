using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Extensions;

namespace ExampleExtension
{
	public class ExampleExtension
		: ICanCreateNewPage
	{
		public string CreateNewPageLabel
		{
			get { return "Example - New page"; }
		}

		public string CreateNewPage()
		{
			return "# Hello from ExampleExtension";
		}
	}
}
