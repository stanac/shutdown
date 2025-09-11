using System;

namespace ShutDown
{
    public class ObservableProperty<T>
    {
        private readonly string _propertyName;
        private readonly ObservableObject _parentObject;
        private readonly Action<T> _onValueChanged;
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (_value == null && value == null)
                {
                    return;
                }

                if (_value != null && _value.Equals(value))
                {
                    return;
                }

                if (value != null && value.Equals(_value))
                {
                    return;
                }

                _value = value;
                _parentObject.RaisePropertyChanged(_propertyName);
                _onValueChanged?.Invoke(value);
            }
        }

        public ObservableProperty(string propertyName, ObservableObject parentObject, T defaultValue, Action<T> onValueChanged = null)
        {
            _propertyName = propertyName;
            _parentObject = parentObject;
            _onValueChanged = onValueChanged;
            Value = defaultValue;
        }
    }
}