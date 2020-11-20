using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.XAxisLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;
using TicksEnum = Plotly.Blazor.LayoutLib.YAxisLib.TicksEnum;

namespace SpaInspector.Components
{
    public partial class SpectraView
    {
        [Parameter] public Spectra Spectra { get; set; }

        public PlotlyChart Chart;
        public Config Config = new();

        public Layout Layout;

        // ReSharper disable once NotAccessedField.Local
        public IList<ITrace> Data;

        protected override void OnInitialized()
        {
            var absorbanceList = Spectra.AbsorbanceList;

            var waves = new List<object>();

            for (var i = absorbanceList.Count - 1; i >= 0; i--)
            {
                waves.Add(i * Constants.SpectraResolution + 700);
            }

            Data = new List<ITrace>
            {
                new Scatter
                {
                    Name = "ScatterTrace",
                    Mode = ModeFlag.Lines | ModeFlag.Markers,
                    Y = absorbanceList.Cast<object>().ToArray(),
                    X = waves,
                    Marker = new Marker
                    {
                        Size = 2
                    },
                }
            };

            Layout = new()
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
                        DTick = absorbanceList.Max() < 1.0f ? 0.1 : 1,
                    }
                },
                AutoSize = false,
                Height = 750,
                Width = 1500,
            };
        }
    }
}
