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
    public partial class View2
    {
        // Using the list of Spa, rather than Spa object
        // [Parameter] public Spa Spa { get; set; }

        [Parameter] public IList<Spa> SpaList { get; set; }
        public PlotlyChart Chart;
        public Config Config = new Config();

        public Layout Layout;

        // ReSharper disable once NotAccessedField.Local
        public IList<ITrace> Data;

        protected override void OnInitialized()
        {
            Data = new List<ITrace>();

            foreach (var spa in SpaList)
            {
                var unitIntensities = spa.UnitIntensities;
                if (unitIntensities.Length == 0) return;

                var waves = new List<object>();

                for (var i = unitIntensities.Length - 1; i >= 0; i--)
                {
                    waves.Add(i * spa.Headers.DataSpacing + spa.Headers.LastX);
                }

                Data.Add(new ScatterGl
                {

                    Name = spa.Title, // Figure it out title
                    Mode = ModeFlag.Lines,
                    Y = unitIntensities.Reverse().Cast<object>().ToArray(),
                    X = waves,
                    Line = new Line
                    {
                        Width = 1.5m,
                    }


                });



                Layout = new Layout()
                {
                    XAxis = new List<XAxis>
                    {
                        new XAxis()
                        {
                            AutoRange = AutoRangeEnum.False,
                            Range = new List<object> { spa.Headers.FirstX, spa.Headers.LastX},
                            ShowGrid = false,
                        },
                    },
                    YAxis = new List<YAxis>
                    {
                        new YAxis()
                        {
                            AutoMargin = false,
                            Ticks = TicksEnum.Empty,
                            Tick0 = 0,
                            DTick = unitIntensities.Max() < 1.0f ? 0.1 : 1,
                            ZeroLine = false,
                            ShowGrid = false,
                        },
                    },
                    AutoSize = false,
                    Height = 750,
                    Width = 1500,
                };

            }




        }
    }
}
