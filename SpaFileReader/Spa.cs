using System;
using System.Collections.Generic;

namespace SpaFileReader
{
    public class Spa
    {
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public Headers Headers { get; set; }
        public string History { get; set; }
        public float[] UnitIntensities { get; set; } = Array.Empty<float>();
        public float[] UnitInterferogram { get; set; } = Array.Empty<float>();
        public float[] BackgroundInterferogram { get; set; } = Array.Empty<float>();
    }
}
