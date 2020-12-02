using System;

namespace SpaFileReader
{
    public class SpectrumBuilder
    {
        private uint _unitSize;
        private string _xUnits;
        private string _xTitle;
        private string _unit;
        private string _unitTitle;
        private float _firstX;
        private float _lastX;
        private uint _numberOfScan;
        private uint _numberOfBackgroundScan;
        private string _history;
        private float[] _unitIntensities = Array.Empty<float>();
        private float[] _unitInterferogram = Array.Empty<float>();
        private float[] _backgroundInterferogram = Array.Empty<float>();
        private float _signalStrength;
        private float _gain;
        private float _opticalVelocity;
        private string _name;
        private DateTime _dateTime;

        public SpectrumBuilder UnitSize(uint unitSize)
        {
            _unitSize = unitSize;
            return this;
        }

        public SpectrumBuilder XUnits(string xUnits)
        {
            _xUnits = xUnits;
            return this;
        }

        public SpectrumBuilder XTitle(string xTitle)
        {
            _xTitle = xTitle;
            return this;
        }

        public SpectrumBuilder Unit(string unit)
        {
            _unit = unit;
            return this;
        }

        public SpectrumBuilder UnitTitle(string unitTitle)
        {
            _unitTitle = unitTitle;
            return this;
        }

        public SpectrumBuilder FirstX(float firstX)
        {
            _firstX = firstX;
            return this;
        }

        public SpectrumBuilder LastX(float lastX)
        {
            _lastX = lastX;
            return this;
        }

        public SpectrumBuilder NumberOfScan(uint numberOfScan)
        {
            _numberOfScan = numberOfScan;
            return this;
        }

        public SpectrumBuilder NumberOfBackgroundScan(uint numberOfBackgroundScan)
        {
            _numberOfBackgroundScan = numberOfBackgroundScan;
            return this;
        }

        public SpectrumBuilder History(string history)
        {
            _history = history;
            return this;
        }

        public SpectrumBuilder UnitIntensities(float[] unitIntensities)
        {
            _unitIntensities = unitIntensities;
            return this;
        }

        public SpectrumBuilder UnitInterferogram(float[] unitInterferogram)
        {
            _unitInterferogram = unitInterferogram;
            return this;
        }

        public SpectrumBuilder BackgroundInterferogram(float[] backgroundInterferogram)
        {
            _backgroundInterferogram = backgroundInterferogram;
            return this;
        }

        public SpectrumBuilder SignalStrength(float signalStrength)
        {
            _signalStrength = signalStrength;
            return this;
        }

        public SpectrumBuilder Gain(in float gain)
        {
            _gain = gain;
            return this;
        }

        public SpectrumBuilder OpticalVelocity(in float opticalVelocity)
        {
            _opticalVelocity = opticalVelocity;
            return this;
        }

        public SpectrumBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public Spectrum Build()
        {
            return new()
            {
                History = _history,
                UnitIntensities = _unitIntensities,
                UnitInterferogram = _unitInterferogram,
                BackgroundInterferogram = _backgroundInterferogram,
                Headers = new Headers
                {
                    Name = _name,
                    UnitSize = _unitSize,
                    XUnits = _xUnits,
                    XTitle = _xTitle,
                    Unit = _unit,
                    UnitTitle = _unitTitle,
                    FirstX = _firstX,
                    LastX = _lastX,
                    NumberOfScan = _numberOfScan,
                    NumberOfBackgroundScan = _numberOfBackgroundScan,
                    SignalStrength = _signalStrength,
                    Gain = _gain,
                    OpticalVelocity = _opticalVelocity,
                    DateTime = _dateTime,
                },
            };
        }

        public SpectrumBuilder DateTime(in DateTime spectraDateTime)
        {
            _dateTime = spectraDateTime;
            return this;
        }
    }
}
