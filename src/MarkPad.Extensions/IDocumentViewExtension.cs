using System.ComponentModel.Composition;
using MarkPad.Contracts;

namespace MarkPad.Extensions
{
	[InheritedExport]
	public interface IDocumentViewExtension : IMarkPadExtension
	{
		void ConnectToDocumentView(IDocumentView view);
		void DisconnectFromDocumentView(IDocumentView view);
	}
}
