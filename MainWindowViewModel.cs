using ServoGapApp.OpenGL;

namespace ServoGapApp
{
    public class MainWindowViewModel : EasyNotifyPropertyChanged
    {
        private bool _showOpenGlControl = true;

        public bool ShowOpenGlControl
        {
            get => _showOpenGlControl;
            set
            {
                _showOpenGlControl = value;
                OnPropertyChanged();
            }
        }
    }
}