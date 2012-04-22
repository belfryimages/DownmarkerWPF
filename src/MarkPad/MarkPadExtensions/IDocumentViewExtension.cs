using MarkPad.Document;
using MarkPad.Extensions;

namespace MarkPad.MarkPadExtensions
{
	public interface IDocumentViewExtension : IMarkPadExtension
	{
		void ConnectToDocumentView(DocumentView view);
		void DisconnectFromDocumentView(DocumentView view);
	}
}
