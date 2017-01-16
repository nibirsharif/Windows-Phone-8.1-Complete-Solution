using MVVMEx.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MVVMEx.Models
{
    public class Auto : BaseModel
    {
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
    }
}
