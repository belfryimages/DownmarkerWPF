using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Extensions.Host;

namespace MarkPad.Extensions
{
	public interface ISpellCheckExtension :
		IDocumentViewExtension
	{
		ISpellingService SpellingService { set; }
		Func<ISpellingService, IDocumentView, ISpellCheckProvider> SpellCheckProviderFactory { set; }
	}
}
