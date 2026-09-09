using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors = new();

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public bool HasErrors => _errors.Values.Any(list => list.Count > 0);

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.SelectMany(kv => kv.Value);

            return _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
        }

        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void SetError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                OnErrorsChanged(propertyName);
            }
        }

        public virtual bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            return true;
        }

        public void ClearAllErrors()
        {
            var keys = _errors.Keys.ToList();
            _errors.Clear();
            foreach (var key in keys)
            {
                OnErrorsChanged(key);
            }
        }
    }
}
