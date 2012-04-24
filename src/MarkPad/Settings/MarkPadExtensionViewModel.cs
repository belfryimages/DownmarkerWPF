using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using Caliburn.Micro;
using MarkPad.Framework.Events;

namespace MarkPad.Settings
{
	public class MarkPadExtensionViewModel : PropertyChangedBase
	{
		readonly IPackageManager _packageManager;
		readonly IEventAggregator _eventAggregator;

		IPackage _package;

		public MarkPadExtensionViewModel(
			IPackageManager packageManager,
			IEventAggregator eventAggregator,
			IPackage package)
		{
			_packageManager = packageManager;
			_eventAggregator = eventAggregator;

			_package = package;
		}

		public string Title
		{
			get
			{
				if (!string.IsNullOrEmpty(_package.Title)) return _package.Title;
				return _package.Id;
			}
		}
		public string Version
		{
			get
			{
				if (GetInstalledPackage() != null) return GetInstalledPackage().Version.ToString();
				return _package.Version.ToString();
			}
		}
		public string Authors { get { return string.Join(", ", _package.Authors); } }
		public string Description
		{
			get
			{
				if (!string.IsNullOrEmpty(_package.Description)) return _package.Description;
				return "";
			}
		}

		public bool CanInstall { get { return GetInstalledPackage() == null; } }
		public void Install()
		{
			_packageManager.InstallPackage(_package, false, false);
			NotifyChangedInstallationStatus();
		}

		public bool CanUpdate
		{
			get
			{
				if (GetInstalledPackage() == null) return false;
 				return GetInstalledPackage().Version < _package.Version;
			}
		}
		public string UpdateText
		{
			get
			{
				if (!CanUpdate) return "No updates";
				return string.Format(
					"Update to {0}",
					_package.Version.ToString());
			}
		}
		public void Update()
		{
			_packageManager.UpdatePackage(_package, true, false);
			_eventAggregator.Publish(new ExtensionsChangedEvent());
			NotifyChangedInstallationStatus();
		}

		IPackage GetInstalledPackage()
		{
			return _packageManager.LocalRepository.FindPackage(_package.Id);
		}

		public bool CanUninstall { get { return GetInstalledPackage() != null; } }
		public void Uninstall()
		{
			_packageManager.UninstallPackage(_package, false, true);
			NotifyChangedInstallationStatus();
		}

		void NotifyChangedInstallationStatus()
		{
			this.NotifyOfPropertyChange(() => CanInstall);
			this.NotifyOfPropertyChange(() => CanUpdate);
			this.NotifyOfPropertyChange(() => CanUninstall);
			this.NotifyOfPropertyChange(() => Version);
			this.NotifyOfPropertyChange(() => UpdateText);
		}
	}
}
