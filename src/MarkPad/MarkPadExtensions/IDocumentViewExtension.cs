using MarkPad.Document;
using MarkPad.Extensions;
using System.ComponentModel.Composition;

namespace MarkPad.MarkPadExtensions
{
	[InheritedExport]
	public interface IDocumentViewExtension : IMarkPadExtension
	{
		void ConnectToDocumentView(DocumentView view);
		void DisconnectFromDocumentView(DocumentView view);
	}
}
