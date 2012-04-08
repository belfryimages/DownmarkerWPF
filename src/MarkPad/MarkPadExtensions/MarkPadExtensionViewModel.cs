using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using Caliburn.Micro;

namespace MarkPad.MarkPadExtensions
{
	public class MarkPadExtensionViewModel : PropertyChangedBase
	{
		readonly IPackageManager _packageManager;

		IPackage _package;

		public MarkPadExtensionViewModel(
			IPackageManager packageManager,
			IPackage package)
		{
			_packageManager = packageManager;

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
		public string Authors { get { return string.Join(", ", _package.Authors); } }
		public string Description
		{
			get
			{
				if (!string.IsNullOrEmpty(_package.Description)) return _package.Description;
				return "";
			}
		}
		bool IsInstalled
		{
			get
			{
				return _packageManager.LocalRepository.Exists(_package.Id);
			}
		}

		public bool CanInstall { get { return !IsInstalled; } }
		public void Install()
		{
			if (IsInstalled)
			{
				Uninstall();
				return;
			}

			System.Windows.MessageBox.Show(this.Title);

			_packageManager.InstallPackage(_package, false, false);
			NotifyChangedInstallationStatus();
		}

		public bool CanUpdate { get { return false; } }
		public void Update()
		{
			throw new NotImplementedException();
		}

		public bool CanUninstall { get { return IsInstalled; } }
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
		}
	}
}
