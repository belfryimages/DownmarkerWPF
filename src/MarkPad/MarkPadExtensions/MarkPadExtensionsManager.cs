using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkPad.Services.MarkPadExtensions;
using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace MarkPad.MarkPadExtensions
{
	public class MarkPadExtensionsManager
	{
		public ObservableCollection<IMarkPadExtension> Extensions { get; private set; }

		public MarkPadExtensionsManager()
		{
			Extensions = new ObservableCollection<IMarkPadExtension>(new[] {
				IoC.Get<SpellCheck.SpellCheckExtension>()
			});
		}
	}
}
