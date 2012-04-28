using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Extensions;
using System.ComponentModel;
using MarkPad.Contracts;
using System.ComponentModel.Composition;

namespace Extension.Example
{
	public class ExampleExtension : 
		ICanCreateNewPage
	{
		[Import]
		IExtensionSettingsProvider _settingsProvider;

		public string Name { get { return "Example extension"; } }
		ExampleExtensionSettings _settings;
		public IExtensionSettings Settings { get { return _settings; } }
		public string CreateNewPageLabel { get { return "New example extension page"; } }

		public ExampleExtension()
		{
			_settings = _settingsProvider.GetSettings<ExampleExtensionSettings>();
		}

		public string CreateNewPage()
		{
			return "# Hello from the `Example` extension";
		}
	}

	public class ExampleExtensionSettings : IExtensionSettings
	{
		[DefaultValue(true)]
		public bool IsEnabled { get; set; }
	}
}
