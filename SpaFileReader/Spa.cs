using System;
using System.Collections.Generic;

namespace SpaFileReader
{
    public class Spa
    {
        public string FileTitle { get; set; }
        public DateTime FileDateTime { get; set; }

        public List<Spectrum> Spectrums { get; set; } = new();

    }

    public class Spectrum
    {
        public Headers Headers { get; set; }
        public float[] UnitIntensities { get; set; } = Array.Empty<float>();
        public float[] UnitInterferogram { get; set; } = Array.Empty<float>();
        public float[] BackgroundInterferogram { get; set; } = Array.Empty<float>();
        public string History { get; set; }
    }
}
