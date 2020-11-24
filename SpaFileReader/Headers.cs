using System;

namespace SpaFileReader
{
    public record Headers()
    {
        public uint UnitSize { get; init; }
        public string XUnits { get; init; }
        public string XTitle { get; init; }
        public string Unit { get; init; }
        public string UnitTitle { get; init; }
        public float FirstX { get; init; }
        public float LastX { get; init; }
        public uint NumberOfScan { get; init; }
        public uint NumberOfBackgroundScan { get; init; }
        public float Resolution => MathF.Abs((LastX - FirstX) / UnitSize);
    }
}
