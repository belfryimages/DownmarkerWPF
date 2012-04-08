using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using MarkPad.Extensions;
using NuGet;
using System.ComponentModel.Composition;
using MarkPad.Framework.Events;

namespace MarkPad.MarkPadExtensions
{
	public interface IMarkPadExtensionsManager
	{
		ObservableCollection<IMarkPadExtension> Extensions { get; }
		IEnumerable<MarkPadExtensionViewModel> GetAvailableExtensions();
	}

	public class MarkPadExtensionsManager : IMarkPadExtensionsManager
	{
		readonly IPackageManager _packageManager;
		readonly Func<IPackage, MarkPadExtensionViewModel> _extensionViewModelCreator;

		[ImportMany(AllowRecomposition=true)]
		public ObservableCollection<IMarkPadExtension> Extensions { get; private set; }

		public MarkPadExtensionsManager(
			IPackageManager packageManager,
			Func<IPackage, MarkPadExtensionViewModel> extensionViewModelCreator)
		{
			_packageManager = packageManager;
			_extensionViewModelCreator = extensionViewModelCreator;

			Extensions = new ObservableCollection<IMarkPadExtension>(new[] {
				IoC.Get<SpellCheck.SpellCheckExtension>()
			});

			_packageManager.PackageInstalled += PackageInstalled;
			_packageManager.PackageUninstalled += PackageUninstalled;
		}

		void PackageInstalled(object sender, PackageOperationEventArgs e)
		{
			// Ahem. Do something with MEF here?

			// The extensions need to be loaded from the package (e.Package).			

			//var extensions = _packageManager.PathResolver.
			//Extensions.AddRange(

			IoC.Get<IEventAggregator>().Publish(new SettingsChangedEvent());

		}

		void PackageUninstalled(object sender, PackageOperationEventArgs e)
		{
		}

		public IEnumerable<MarkPadExtensionViewModel> GetAvailableExtensions()
		{
			return _packageManager.SourceRepository
				.GetPackages()
				.AsEnumerable()
				.Select(p => _extensionViewModelCreator(p));
		}
	}
}
