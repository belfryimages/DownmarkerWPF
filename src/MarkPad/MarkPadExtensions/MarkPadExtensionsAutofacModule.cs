using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using NuGet;
using System.IO;
using Autofac.Integration.Mef;
using MarkPad.Extensions;

namespace MarkPad.MarkPadExtensions
{
	public class MarkPadExtensionsAutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<MarkPadExtensionViewModel>();

			// Register the nuget repositories (the Code52 one and the local dev repo):
			//var code52PackageSource = "http://code52.org/markpad??/nuget";
			var localPackageSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarkPad", "Packages");
			builder.Register<IPackageRepository>(c => new AggregateRepository(new[] {
				//PackageRepositoryFactory.Default.CreateRepository(code52PackageSource),
				PackageRepositoryFactory.Default.CreateRepository(localPackageSource)
			}));

			// Register the package manager (where MarkPad's extensions are installed by NuGet):
            var extensionsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarkPad", "Extensions");
			builder.Register<IPackageManager>(c => new PackageManager(c.Resolve<IPackageRepository>(), extensionsFolder)).SingleInstance();

			builder.RegisterType<MarkPadExtensionsManager>().As<IMarkPadExtensionsManager>().SingleInstance();
		}
	}
}
