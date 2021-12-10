﻿using System;

namespace SpaInspectorReader
{
    public class SpaBuilder
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
        private string _title;
        private DateTime _dateTime;
        private string _history;
        private float[] _unitIntensities = Array.Empty<float>();
        private float[] _unitInterferogram = Array.Empty<float>();
        private float[] _backgroundInterferogram = Array.Empty<float>();
        private float _signalStrength;
        private float _gain;
        private float _opticalVelocity;

        public SpaBuilder UnitSize(uint unitSize)
        {
            _unitSize = unitSize;
            return this;
        }

        public SpaBuilder XUnits(string xUnits)
        {
            _xUnits = xUnits;
            return this;
        }

        public SpaBuilder XTitle(string xTitle)
        {
            _xTitle = xTitle;
            return this;
        }

        public SpaBuilder Unit(string unit)
        {
            _unit = unit;
            return this;
        }

        public SpaBuilder UnitTitle(string unitTitle)
        {
            _unitTitle = unitTitle;
            return this;
        }

        public SpaBuilder FirstX(float firstX)
        {
            _firstX = firstX;
            return this;
        }

        public SpaBuilder LastX(float lastX)
        {
            _lastX = lastX;
            return this;
        }

        public SpaBuilder NumberOfScan(uint numberOfScan)
        {
            _numberOfScan = numberOfScan;
            return this;
        }

        public SpaBuilder NumberOfBackgroundScan(uint numberOfBackgroundScan)
        {
            _numberOfBackgroundScan = numberOfBackgroundScan;
            return this;
        }

        public SpaBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public SpaBuilder DateTime(DateTime dateTime)
        {
            _dateTime = dateTime;
            return this;
        }

        public SpaBuilder History(string history)
        {
            _history = history;
            return this;
        }

        public SpaBuilder UnitIntensities(Span<float> unitIntensities)
        {
            _unitIntensities = unitIntensities.ToArray();
            return this;
        }

        public SpaBuilder UnitInterferogram(Span<float> unitInterferogram)
        {
            _unitInterferogram = unitInterferogram.ToArray();
            return this;
        }

        public SpaBuilder BackgroundInterferogram(Span<float> backgroundInterferogram)
        {
            _backgroundInterferogram = backgroundInterferogram.ToArray();
            return this;
        }

        public SpaBuilder SignalStrength(float signalStrength)
        {
            _signalStrength = signalStrength;
            return this;
        }

        public SpaBuilder Gain(in float gain)
        {
            _gain = gain;
            return this;
        }

        public SpaBuilder OpticalVelocity(in float opticalVelocity)
        {
            _opticalVelocity = opticalVelocity;
            return this;
        }

        public Spa Build()
        {
            return new()
            {
                DateTime = _dateTime,
                History = _history,
                UnitIntensities = _unitIntensities,
                UnitInterferogram = _unitInterferogram,
                BackgroundInterferogram = _backgroundInterferogram,
                Title = _title,
                Headers = new Headers
                {
                    UnitSize = _unitSize,
                    XUnits = _xUnits,
                    XTitle = _xTitle,
                    YUnit = _unit,
                    YUnitTitle = _unitTitle,
                    FirstX = _firstX,
                    LastX = _lastX,
                    NumberOfScan = _numberOfScan,
                    NumberOfBackgroundScan = _numberOfBackgroundScan,
                    SignalStrength = _signalStrength,
                    Gain = _gain,
                    OpticalVelocity = _opticalVelocity,
                }
            };
        }
    }
}
