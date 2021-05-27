using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.XAxisLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterGlLib;
using SpaInspectorReader;
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
                waves.Add(i * Spa.Headers.Resolution + Spa.Headers.LastX);
            }

            Data = new List<ITrace>
            {
                new ScatterGl()
                {
                    Name = "ScatterTrace",
                    Mode = ModeFlag.Lines,
                    Y = unitIntensities.Cast<object>().ToArray(),
                    X = waves,
                    Line = new Line
                    {
                        Width = 1.5m
                    }
                }
            };

            Layout = new Layout()
            {
                XAxis = new List<XAxis>
                {
                    new()
                    {
                        AutoRange = AutoRangeEnum.False,
                        Range = new List<object> {Spa.Headers.FirstX, Spa.Headers.LastX},
                    }
                },
                YAxis = new List<YAxis>
                {
                    new()
                    {
                        AutoMargin = false,
                        Ticks = TicksEnum.Empty,
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
