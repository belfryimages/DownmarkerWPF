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
using System.IO;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

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

		[ImportMany]
		public IEnumerable<IMarkPadExtension> Extensions { get; private set; }

		AggregateCatalog _catalog;
		CompositionContainer _container;

		public MarkPadExtensionsManager(
			IPackageManager packageManager,
			Func<IPackage, MarkPadExtensionViewModel> extensionViewModelCreator)
		{
			_packageManager = packageManager;
			_extensionViewModelCreator = extensionViewModelCreator;

			InitialiseCatalog();

			_packageManager.PackageInstalled += PackageInstalled;
			_packageManager.PackageUninstalled += PackageUninstalled;
		}

		private void InitialiseCatalog()
		{
			_catalog = new AggregateCatalog(
				new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			/*
			Extensions = new ObservableCollection<IMarkPadExtension>(new[] {
				IoC.Get<SpellCheck.SpellCheckExtension>()
			});
			*/

			foreach (var package in _packageManager.LocalRepository.GetPackages())
			{
				var packagePath = Path.Combine(
					_packageManager.PathResolver.GetInstallPath(package),
					"lib");
				if (Directory.Exists(packagePath)) _catalog.Catalogs.Add(new DirectoryCatalog(packagePath));
			}

			_container = new CompositionContainer(_catalog);
			_container.ComposeParts(this);
			//Extensions = _container.GetExportedValues<IMarkPadExtension>();
		}

		void PackageInstalled(object sender, PackageOperationEventArgs e)
		{
			// Ahem. Do something with MEF here?

			if (Directory.Exists(GetLibDir(e))) _catalog.Catalogs.Add(new DirectoryCatalog(GetLibDir(e)));

			//IoC.Get<IEventAggregator>().Publish(new SettingsChangedEvent());

			IoC.Get<IEventAggregator>().Publish(new SettingsChangedEvent());
		}

		void PackageUninstalled(object sender, PackageOperationEventArgs e)
		{
			var cleanPath = new Func<string, string>(path => path.TrimEnd(Path.DirectorySeparatorChar).ToLower());
			var shouldRemove = new Func<dynamic, bool>(c =>
				c is DirectoryCatalog &&
				cleanPath((c as DirectoryCatalog).FullPath) == cleanPath(GetLibDir(e)));
			_catalog.Catalogs.RemoveAll(c => shouldRemove(c));

			//IoC.Get<IEventAggregator>().Publish(new SettingsChangedEvent());
		}

		static string GetLibDir(PackageOperationEventArgs e)
		{
			return Path.Combine(e.InstallPath, "lib");
		}
		static bool PathsMatch(IFileSystem fs, string path1, string path2)
		{
			return String.Equals(path1.TrimEnd(Path.DirectorySeparatorChar), path2.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);
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
