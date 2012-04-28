using System;
using System.Collections.Generic;
using System.Linq;
using MarkPad.Extensions;
using MarkPad.Contracts;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace SpellCheck
{
	public class SpellCheckExtension : IDocumentViewExtension
    {
		[Import]
		IExtensionSettingsProvider _settingsProvider;
		[Import]
		ISpellingService _spellingService;
		[Import]
		ISpellCheckProviderFactory _spellCheckProviderFactory;
        
		public string Name { get { return "Spell check"; } }
		SpellCheckExtensionSettings _settings;
		public IExtensionSettings Settings { get { return _settings; } }
		IList<ISpellCheckProvider> providers = new List<ISpellCheckProvider>();

		public SpellCheckExtension()
		{
			_settings = _settingsProvider.GetSettings<SpellCheckExtensionSettings>();
		}

        public void ConnectToDocumentView(IDocumentView view)
        {
			if (providers.Any(p => p.View == view))
			{
				throw new ArgumentException("View already has a spell check provider connected", "view");
			}

			var provider = _spellCheckProviderFactory.GetProvider(_spellingService, view);
            providers.Add(provider);
        }

        public void DisconnectFromDocumentView(IDocumentView view)
        {
            var provider = providers.FirstOrDefault(p => p.View == view);
            if (provider == null) return;

            provider.Disconnect();
            providers.Remove(provider);
        }
    }

	public class SpellCheckExtensionSettings : ExtensionSettings
	{
	}
}
