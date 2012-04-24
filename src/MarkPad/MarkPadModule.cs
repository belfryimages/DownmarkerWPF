using System;
using Autofac;
using NuGet;
using System.IO;
using MarkPad.MarkPadExtensions;
using MarkPad.Settings;

namespace MarkPad
{
	public class MarkPadModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			// Register local and Code52 (not yet) NuGet repositories
			//var code52PackageSource = "http://code52.org/markpad??/nuget";
			var localPackageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarkPad", "Packages");
			builder.Register<IPackageRepository>(c => new AggregateRepository(new[] {
				//PackageRepositoryFactory.Default.CreateRepository(code52PackageSource),
				PackageRepositoryFactory.Default.CreateRepository(localPackageSource)
			}));

			// Register the package manager (where MarkPad's extensions are installed by NuGet):
            var extensionsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarkPad", "Extensions");
			builder.Register<IPackageManager>(c => new PackageManager(c.Resolve<IPackageRepository>(), extensionsFolder)).SingleInstance();

			builder.RegisterType<MarkPadExtensionViewModel>();
			builder.RegisterType<MarkPadExtensionsManager>().As<IMarkPadExtensionsManager>().SingleInstance();
		}
	}
}
