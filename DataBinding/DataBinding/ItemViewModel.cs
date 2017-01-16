using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataBinding
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        // Properties

        private string _manufacturer;
        private string _model;
        private string _color;
        private int _year;

        public string manufacturer
        {
            get { return _manufacturer; }

            set { this.SetProperty(ref this._manufacturer, value); }
        }

        public string model
        {
            get { return _model; }

            set { this.SetProperty(ref this._model, value); }
        }

        public string color
        {
            get { return _color; }

            set { this.SetProperty(ref this._color, value); }
        }

        public int year
        {
            get { return _year; }

            set { this.SetProperty(ref this._year, value); }
        }


        // Property Change Logic
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChaned(propertyName);
            return true;
        }

        private void OnPropertyChaned(string propertyName)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
