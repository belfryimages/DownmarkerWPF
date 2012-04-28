using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MarkPad.Extensions
{
	/// <summary>
	/// Implement this with your own set of settings
	/// </summary>
	public interface IExtensionSettings
	{
		bool IsEnabled { get; set; }
	}

	public class ExtensionSettings : IExtensionSettings
	{
		[DefaultValue(true)]
		public bool IsEnabled { get; set; }
	}
}
