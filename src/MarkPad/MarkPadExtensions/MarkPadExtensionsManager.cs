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
		IEnumerable<IMarkPadExtension> Extensions { get; }
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

			_catalog = new AggregateCatalog(
				new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			foreach (var package in _packageManager.LocalRepository.GetPackages())
			{
				IncludePackage(package);
			}

			_container = new CompositionContainer(_catalog);
			_container.ComposeParts(this);

			_packageManager.PackageInstalled += PackageInstalled;
			_packageManager.PackageUninstalled += PackageUninstalled;
		}

		private string GetPackagePath(IPackage package)
		{
			var packagePath = Path.Combine(
				_packageManager.PathResolver.GetInstallPath(package),
				"lib");
			return packagePath;
		}

		private void IncludePackage(IPackage package)
		{
			if (!Directory.Exists(GetPackagePath(package))) return;
			
			_catalog.Catalogs.Add(new DirectoryCatalog(GetPackagePath(package)));

				foreach (var subpath in Directory.EnumerateDirectories(GetPackagePath(package)))
				{
					_catalog.Catalogs.Add(new DirectoryCatalog(subpath));
				}
		}

		void ExcludePackage(IPackage package)
		{
			if (!Directory.Exists(GetPackagePath(package))) return;

			_catalog.Catalogs.Remove(new DirectoryCatalog(GetPackagePath(package)));

			foreach (var subpath in Directory.EnumerateDirectories(GetPackagePath(package)))
			{
				_catalog.Catalogs.Remove(new DirectoryCatalog(subpath));
			}
		}

		void PackageInstalled(object sender, PackageOperationEventArgs e)
		{
			IncludePackage(e.Package);
			
			//IoC.Get<IEventAggregator>().Publish(new SettingsChangedEvent());

		}

		void PackageUninstalled(object sender, PackageOperationEventArgs e)
		{
			ExcludePackage(e.Package);

			//IoC.Get<IEventAggregator>().Publish(new SettingsChangedEvent());
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
