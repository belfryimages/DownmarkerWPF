using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkPad.Contracts
{
	public interface IExtensionSettingsProvider
	{
		T GetSettings<T>() where T : new();
		void SaveSettings<T>(T settings);
	}
}
