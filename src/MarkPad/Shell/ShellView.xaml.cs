using System.Windows;
using System.Windows.Input;
using MarkPad.Framework.Events;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using MarkPad.Extensions;
using MarkPad.MarkPadExtensions;
using System.Windows.Controls;
using MarkPad.Framework;

namespace MarkPad.Shell
{
    public partial class ShellView : IHandle<ExtensionsChangedEvent>
    {
		readonly IMarkPadExtensionsManager markPadExtensionsManager;

        private bool ignoreNextMouseMove;
		[ImportMany(AllowRecomposition = true)]
		private IEnumerable<ICanCreateNewPage> canCreateNewPageExtensions;
		[ImportMany(AllowRecomposition = true)]
		private IEnumerable<ICanSavePage> canSavePageExtensions;

		public ShellView(IMarkPadExtensionsManager markPadExtensionsManager)
		{
			this.markPadExtensionsManager = markPadExtensionsManager;

			InitializeComponent();

			this.markPadExtensionsManager.Container.ComposeParts(this);

			Handle(new ExtensionsChangedEvent());
		}

        private void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (WindowState == WindowState.Maximized && e.ClickCount != 2) return;

            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                ignoreNextMouseMove = true;
                return;
            }

            DragMove();
        }

        private void MouseMoveWindow(object sender, MouseEventArgs e)
        {
            if (ignoreNextMouseMove)
            {
                ignoreNextMouseMove = false;
                return;
            }

            if (WindowState != WindowState.Maximized) return;

            if (e.MiddleButton == MouseButtonState.Pressed) return;
            if (e.RightButton == MouseButtonState.Pressed) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!header.IsMouseOver) return;

            // Calculate correct left coordinate for multi-screen system
            var mouseX = PointToScreen(Mouse.GetPosition(this)).X;
            var width = RestoreBounds.Width;
            var left = mouseX - width / 2;
            if (left < 0) left = 0;

            // Align left edge to fit the screen
            var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
            if (left + width > virtualScreenWidth) left = virtualScreenWidth - width;

            Top = 0;
            Left = left;

            WindowState = WindowState.Normal;

            DragMove();
        }

        private void ButtonMinimiseOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ButtonMaxRestoreOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        protected override void OnStateChanged(System.EventArgs e)
        {
            RefreshMaximiseIconState();
            base.OnStateChanged(e);
        }

        private void RefreshMaximiseIconState()
        {
            if (WindowState == WindowState.Normal)
            {
                maxRestore.Content = "1";
                maxRestore.SetResourceReference(ToolTipProperty, "WindowCommandsMaximiseToolTip");
            }
            else
            {
                maxRestore.Content = "2";
                maxRestore.SetResourceReference(ToolTipProperty, "WindowCommandsRestoreToolTip");
            }
        }

        private void WindowDragOver(object sender, DragEventArgs e)
        {
            var isFileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);
            e.Effects = isFileDrop ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }

		public void Handle(ExtensionsChangedEvent e)
		{
			this.markPadExtensionsManager.Container.ComposeParts(this);

			newPageHook.Children.Clear();
			foreach (var extension in canCreateNewPageExtensions)
			{
				HookExtension(extension, extension.CreateNewPageLabel, newPageHookButtonClick, newPageHook);
			}
			foreach (var extension in canSavePageExtensions)
			{
				HookExtension(extension, extension.SavePageLabel, savePageHookButtonClick, savePageHook);
			}
		}

		void HookExtension(IMarkPadExtension extension, string label, RoutedEventHandler handler, WrapPanel wrapPanel)
		{
			var button = new Button
			{
				Content = label.ToUpper(),
				Tag = extension
			};
			button.Click += new RoutedEventHandler(handler);
			wrapPanel.Children.Add(button);
		}

		void newPageHookButtonClick(object sender, RoutedEventArgs e)
		{
			var button = (Button)e.Source;
			var extension = (ICanCreateNewPage)button.Tag;
			var viewModel = DataContext as ShellViewModel;

			var text = extension.CreateNewPage();

			viewModel.ExecuteSafely(vm => vm.NewDocument(text));
		}

		void savePageHookButtonClick(object sender, RoutedEventArgs e)
		{
			var button = (Button)e.Source;
			var extension = (ICanSavePage)button.Tag;
			var viewModel = DataContext as ShellViewModel;

			extension.SavePage(viewModel.ActiveDocumentViewModel);
		}
	}
}
