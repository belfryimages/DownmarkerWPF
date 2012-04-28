using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Extensions;
using System.ComponentModel.Composition;
using MarkPad.Contracts;

namespace MarkPad.Services.Settings
{
	/// <summary>
	/// This is a wrapper for SettingsProvider, which can be injected into an extension using MEF
	/// </summary>
	[Export(typeof(IExtensionSettingsProvider))]
	public class ExtensionSettingsProvider : IExtensionSettingsProvider
	{
		readonly ISettingsProvider _settingsProvider;

		public ExtensionSettingsProvider(ISettingsProvider settingsProvider)
		{
			_settingsProvider = settingsProvider;
		}

		public T GetSettings<T>() where T : new()
		{
			return _settingsProvider.GetSettings<T>();
		}

		public void SaveSettings<T>(T settings)
		{
			_settingsProvider.SaveSettings(settings);
		}
	}
}
