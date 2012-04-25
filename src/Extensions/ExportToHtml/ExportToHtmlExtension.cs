using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Extensions;
using MarkPad.Contracts;
using Analects.DialogService;
using System.IO;
using System.ComponentModel.Composition;

namespace ExportToHtml
{
	public class ExportToHtmlExtension : ICanSavePage
	{
		const string SAVE_TITLE = "Choose a location to save the HTML document";
		const string SAVE_DEFAULT_EXT = "*.html";
		const string SAVE_FILTER = "HTML Files (*.html,*.htm)|*.html;*.htm|All Files (*.*)|*.*";

		[Import]
		IDocumentParser documentParser;

		public string Name
		{
			get { return "Export to HTML"; }
		}

		public string SavePageLabel
		{
			get { return "Save HTML"; }
		}

		public void SavePage(IDocumentViewModel documentViewModel)
		{
			var dialogService = new DialogService();
			var filename = dialogService.GetFileSavePath(SAVE_TITLE, SAVE_DEFAULT_EXT, SAVE_FILTER);
			if (string.IsNullOrEmpty(filename)) return;

			var markdown = documentViewModel.MarkdownContent;
			var html = documentParser.ParseClean(markdown);

			File.WriteAllText(filename, html);
		}
	}
}
