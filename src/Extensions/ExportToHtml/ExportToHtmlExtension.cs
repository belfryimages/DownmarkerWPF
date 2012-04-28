using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Extensions;
using MarkPad.Contracts;
using Analects.DialogService;
using System.IO;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace ExportToHtml
{
	public class ExportToHtmlExtension : ICanSavePage
	{
		const string SAVE_TITLE = "Choose a location to save the HTML document";
		const string SAVE_DEFAULT_EXT = "*.html";
		const string SAVE_FILTER = "HTML Files (*.html,*.htm)|*.html;*.htm|All Files (*.*)|*.*";

		[Import]
		IExtensionSettingsProvider _settingsProvider;
		[Import]
		IDocumentParser documentParser;

		public string Name
		{
			get { return "Export to HTML"; }
		}

		ExportToHtmlExtensionSettings _settings;
		public IExtensionSettings Settings { get { return _settings; } }

		public string SavePageLabel
		{
			get { return "Save HTML"; }
		}

		public ExportToHtmlExtension()
		{
			_settings = _settingsProvider.GetSettings<ExportToHtmlExtensionSettings>();
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

	public class ExportToHtmlExtensionSettings : IExtensionSettings
	{
		[DefaultValue(true)]
		public bool IsEnabled { get; set; }
	}
}
