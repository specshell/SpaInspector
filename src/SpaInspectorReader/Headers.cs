namespace SpaInspectorReader;

public record Headers
{
    public uint UnitSize { get; init; }
    public string XUnits { get; init; }
    public string XTitle { get; init; }
    public string YUnit { get; init; }
    public string YUnitTitle { get; init; }
    public float FirstX { get; init; }
    public float LastX { get; init; }
    public uint NumberOfScan { get; init; }
    public uint NumberOfBackgroundScan { get; init; }
    public float DataSpacing => Math.Abs((LastX - FirstX) / (UnitSize - 1));
    public float SignalStrength { get; set; }
    public float Gain { get; set; }
    public float OpticalVelocity { get; set; }
}