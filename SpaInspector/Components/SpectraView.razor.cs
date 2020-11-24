using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.XAxisLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;
using SpaFileReader;
using TicksEnum = Plotly.Blazor.LayoutLib.YAxisLib.TicksEnum;

namespace SpaInspector.Components
{
    public partial class SpectraView
    {
        [Parameter] public Spa Spa { get; set; }

        public PlotlyChart Chart;
        public Config Config = new();

        public Layout Layout;

        // ReSharper disable once NotAccessedField.Local
        public IList<ITrace> Data;

        protected override void OnInitialized()
        {
            var unitIntensities = Spa.UnitIntensities;
            if (unitIntensities.Length == 0) return;

            var waves = new List<object>();

            for (var i = unitIntensities.Length - 1; i >= 0; i--)
            {
                waves.Add(i * Constants.SpectraResolution + 700);
            }

            Data = new List<ITrace>
            {
                new Scatter
                {
                    Name = "ScatterTrace",
                    Mode = ModeFlag.Lines | ModeFlag.Markers,
                    Y = unitIntensities.Cast<object>().ToArray(),
                    X = waves,
                    Marker = new Marker
                    {
                        Size = 2
                    },
                }
            };

            Layout = new Layout()
            {
                XAxis = new List<XAxis>
                {
                    new()
                    {
                        AutoRange = AutoRangeEnum.Reversed,
                        Range = new List<object> {700, 4000},
                    }
                },
                YAxis = new List<YAxis>
                {
                    new()
                    {
                        AutoMargin = false,
                        Ticks = TicksEnum.Outside,
                        Tick0 = 0,
                        DTick = unitIntensities.Max() < 1.0f ? 0.1 : 1,
                    }
                },
                AutoSize = false,
                Height = 750,
                Width = 1500,
            };
        }
    }
}
