using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using MarkPad.Extensions;
using NuGet;
using System.ComponentModel.Composition;

namespace MarkPad.MarkPadExtensions
{
	public class MarkPadExtensionsManager
	{
		readonly IPackageManager _packageManager;

		[ImportMany(AllowRecomposition=true)]
		public ObservableCollection<IMarkPadExtension> Extensions { get; private set; }

		public MarkPadExtensionsManager(IPackageManager packageManager)
		{
			_packageManager = packageManager;

			Extensions = new ObservableCollection<IMarkPadExtension>(new[] {
				IoC.Get<SpellCheck.SpellCheckExtension>()
			});

			_packageManager.PackageInstalled += PackageInstalled;
			_packageManager.PackageUninstalled += PackageUninstalled;
		}

		void PackageInstalled(object sender, PackageOperationEventArgs e)
		{
			// Ahem. Do something with MEF here.

			// The extensions need to be loaded from the package (e.Package).			

			//var extensions = _packageManager.PathResolver.
			//Extensions.AddRange(
		}

		void PackageUninstalled(object sender, PackageOperationEventArgs e)
		{
		}

		public IEnumerable<IPackage> GetAvailableExtensions()
		{
			return _packageManager.SourceRepository.GetPackages();
		}
	}
}
