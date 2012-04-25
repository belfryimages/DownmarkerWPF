﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MarkPad.Contracts;

namespace MarkPad.Extensions
{
	[InheritedExport]
	public interface ICanSavePage : IMarkPadExtension
	{
		string SavePageLabel { get; }
		void SavePage(IDocumentViewModel documentViewModel);
	}
}
