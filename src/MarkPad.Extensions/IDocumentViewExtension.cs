using MarkPad.Extensions;
using System.ComponentModel.Composition;

namespace MarkPad.Extensions
{
	[InheritedExport]
	public interface IDocumentViewExtension : IMarkPadExtension
	{
		void ConnectToDocumentView(IDocumentView view);
		void DisconnectFromDocumentView(IDocumentView view);
	}
}
