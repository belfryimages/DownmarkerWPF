using Caliburn.Micro;

namespace MarkPad.Settings
{
    public class FileAssociationViewModel : PropertyChangedBase
    {
        public FileAssociationViewModel(string extension, bool enabled)
        {
            Extension = extension;
            Enabled = enabled;
        }

        public string Extension { get; private set; }
        public bool Enabled { get; set; }
    }
}