using System;
using System.Collections.Generic;
using System.Linq;
using MarkPad.Extensions;
using MarkPad.Extensions.Host;

namespace SpellCheck
{
	public class SpellCheckExtension : ISpellCheckExtension
    {
        public string Name { get { return "Spell check"; } }
		public ISpellingService SpellingService { get; set; }
		public Func<ISpellingService, IDocumentView, ISpellCheckProvider> SpellCheckProviderFactory { get; set; }
		
		IList<ISpellCheckProvider> providers = new List<ISpellCheckProvider>();

        public void ConnectToDocumentView(IDocumentView view)
        {
			if (providers.Any(p => p.View == view))
			{
				throw new ArgumentException("View already has a spell check provider connected", "view");
			}

			var provider = SpellCheckProviderFactory(SpellingService, view);
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
}
