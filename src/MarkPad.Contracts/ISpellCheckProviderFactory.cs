﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkPad.Contracts
{
	public interface ISpellCheckProviderFactory
	{
		ISpellCheckProvider GetProvider(ISpellingService spellingService, IDocumentView view);
	}
}
