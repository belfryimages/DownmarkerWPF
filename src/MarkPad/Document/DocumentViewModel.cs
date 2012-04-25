using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Threading;
using Caliburn.Micro;
using CookComputing.XmlRpc;
using ICSharpCode.AvalonEdit.Document;
using MarkPad.HyperlinkEditor;
using MarkPad.Services.Implementation;
using MarkPad.Services.Interfaces;
using MarkPad.Services.Metaweblog;
using MarkPad.Services.Settings;
using Ookii.Dialogs.Wpf;
using MarkPad.Contracts;

namespace MarkPad.Document
{
	internal class DocumentViewModel : Screen, IDocumentViewModel
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(DocumentViewModel));

        private readonly IDialogService dialogService;
        private readonly IWindowManager windowManager;
        private readonly ISiteContextGenerator siteContextGenerator;
        private readonly Func<string, IMetaWeblogService> getMetaWeblog;
		private readonly IDocumentParser documentParser;

        private readonly TimeSpan delay = TimeSpan.FromSeconds(0.5);
        private readonly DispatcherTimer timer;

		public ISiteContext SiteContext { get; private set; }
		public string FileName { get; private set; }
		public string Title { get; set; }
		public TextDocument Document { get; set; }
		public string Original { get; set; }
		public string Render { get; private set; }
		public string MarkdownContent { get { return Document.Text; } }
		
		public override string DisplayName
		{
			get { return Title; }
		}

        public DocumentViewModel(
			IDialogService dialogService, 
			IWindowManager windowManager, 
			ISiteContextGenerator siteContextGenerator, 
			Func<string, IMetaWeblogService> getMetaWeblog,
			IDocumentParser documentParser)
        {
            this.dialogService = dialogService;
            this.windowManager = windowManager;
            this.siteContextGenerator = siteContextGenerator;
            this.getMetaWeblog = getMetaWeblog;
			this.documentParser = documentParser;

            Title = "New Document";
            Original = "";
            Document = new TextDocument();
            Post = new Post();
            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = delay;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            timer.Stop();

			Task.Factory.StartNew(text => documentParser.Parse(text.ToString()), Document.Text)
            .ContinueWith(s =>
            {
                if (s.IsFaulted)
                {
                    Log.Error(s.Exception);
                    return;
                }

                var result = s.Result;
                if (SiteContext != null)
                {
                    result = SiteContext.ConvertToAbsolutePaths(result);
                }

                Render = result;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Open(string path)
        {
            FileName = path;
            Title = new FileInfo(path).Name;

            var text = File.ReadAllText(path);
            Document.Text = text;
            Original = text;

            Update();
            EvaluateContext();
        }

        public void OpenFromWeb(Post post)
        {
            Post = post;

            Title = post.permalink ?? string.Empty; // TODO: no title is displayed now
            Document.Text = post.description ?? string.Empty;
            Original = post.description ?? string.Empty;

            Update();
            EvaluateContext();
        }

        public Post Post { get; private set; }

        public void Update()
        {
            timer.Stop();
            timer.Start();
            NotifyOfPropertyChange(() => HasChanges);
            NotifyOfPropertyChange(() => DisplayName);
        }

        public bool SaveAs()
        {
            var path = dialogService.GetFileSavePath("Choose a location to save the document.", "*.md", Constants.FileAssociationExtensionFilter + "|All Files (*.*)|*.*");

            if (string.IsNullOrEmpty(path))
                return false;

            FileName = path;
            Title = new FileInfo(FileName).Name;
            NotifyOfPropertyChange(() => DisplayName);
            EvaluateContext();

            return Save();
        }

        public bool Save()
        {
            if (!HasChanges)
                return true;

            if (string.IsNullOrEmpty(FileName))
            {
                var path = dialogService.GetFileSavePath("Choose a location to save the document.", "*.md", Constants.FileAssociationExtensionFilter + "|All Files (*.*)|*.*");

                if (string.IsNullOrEmpty(path))
                    return false;

                FileName = path;
                Title = new FileInfo(FileName).Name;
                NotifyOfPropertyChange(() => DisplayName);
                EvaluateContext();
            }

            try
            {
                File.WriteAllText(FileName, Document.Text);
                Original = Document.Text;
            }
            catch (Exception)
            {
                var saveResult = dialogService.ShowConfirmation("MarkPad", "Cannot save file",
                                                String.Format("Do you want to save changes for {0} to a different file?", Title),
                                                new ButtonExtras(ButtonType.Yes, "Save", "Save the file at a different location."),
                                                new ButtonExtras(ButtonType.No, "Do not save", "The file will be considered a New Document.  The next save will prompt for a file location."));

                string prevFileName = FileName;
                string prevTitle = Title;

                Title = "New Document";
                FileName = "";
                if (saveResult)
                {
                    saveResult = Save();
                    if (!saveResult)  //We decide not to save, keep existing title and filename 
                    {
                        Title = prevTitle;
                        FileName = prevFileName;
                    }
                }

                NotifyOfPropertyChange(() => DisplayName);
                return saveResult;
            }

            return true;
        }

        private void EvaluateContext()
        {
            SiteContext = siteContextGenerator.GetContext(FileName);
        }

        public bool HasChanges
        {
            get { return Original != Document.Text; }
        }

        public override void CanClose(Action<bool> callback)
        {
            var view = GetView() as DocumentView;

            if (!HasChanges)
            {
                CheckAndCloseView(view);
                callback(true);
                return;
            }

            var saveResult = dialogService.ShowConfirmationWithCancel("MarkPad", "Save modifications.", "Do you want to save your changes to '" + Title + "'?",
                new ButtonExtras(ButtonType.Yes, "Save",
                    string.IsNullOrEmpty(FileName) ? "The file has not been saved yet" : "The file will be saved to " + Path.GetFullPath(FileName)),
                new ButtonExtras(ButtonType.No, "Don't Save", "Close the document without saving the modifications"),
                new ButtonExtras(ButtonType.Cancel, "Cancel", "Don't close the document")
            );
            var result = false;

            // true = Yes
            switch (saveResult)
            {
                case true:
                    result = Save();
                    break;
                case false:
                    result = true;
                    break;
            }

            // Close browser if tab is being closed
            if (result)
            {
                CheckAndCloseView(view);
            }

            callback(result);
        }

        private static void CheckAndCloseView(DocumentView view)
        {
            if (view != null
                && view.wb != null)
            {
                view.wb.Close();
            }
        }

        public void Print()
        {
            var view = GetView() as DocumentView;
            if (view != null)
            {
                view.wb.Print();
            }
        }

        public bool DistractionFree { get; set; }

        public void Publish(string postid, string postTitle, string[] categories, BlogSetting blog)
        {
            if (categories == null) categories = new string[0];

            var proxy = getMetaWeblog(blog.WebAPI);

            var newpost = new Post();
            try
            {
                var renderBody = DocumentParser.GetBodyContents(Document.Text);

                if (string.IsNullOrWhiteSpace(postid))
                {
                    var permalink = DisplayName.Split('.')[0] == "New Document"
                                ? postTitle
                                : DisplayName.Split('.')[0];

                    newpost = new Post
                               {
                                   permalink = permalink,
                                   title = postTitle,
                                   dateCreated = DateTime.Now,
                                   description = blog.Language == "HTML" ? renderBody : Document.Text,
                                   categories = categories,
                                   format = blog.Language
                               };
                    newpost.postid = proxy.NewPost(blog, newpost, true);
                }
                else
                {
                    newpost = proxy.GetPost(postid, blog);
                    newpost.title = postTitle;
                    newpost.description = blog.Language == "HTML" ? renderBody : Document.Text;
                    newpost.categories = categories;
                    newpost.format = blog.Language;

                    proxy.EditPost(postid, blog, newpost, true);
                }
            }
            catch (WebException ex)
            {
                dialogService.ShowError("Error Publishing", ex.Message, "");
            }
            catch (XmlRpcException ex)
            {
                dialogService.ShowError("Error Publishing", ex.Message, "");
            }
            catch (XmlRpcFaultException ex)
            {
                dialogService.ShowError("Error Publishing", ex.Message, "");
            }

            Post = newpost;
            Original = Document.Text;
            Title = postTitle;
            NotifyOfPropertyChange(() => DisplayName);
        }

        public MarkPadHyperlink GetHyperlink(MarkPadHyperlink hyperlink)
        {
            var viewModel = new HyperlinkEditorViewModel(hyperlink.Text, hyperlink.Url)
                                {
                                    IsUrlFocussed = !String.IsNullOrWhiteSpace(hyperlink.Text)
                                };
            windowManager.ShowDialog(viewModel);
            if (!viewModel.WasCancelled)
            {
                hyperlink.Set(viewModel.Text, viewModel.Url);
                return hyperlink;
            }
            return null;
        }
    }
}
