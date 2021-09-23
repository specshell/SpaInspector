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
using Blazorise.Charts;
using TicksEnum = Plotly.Blazor.LayoutLib.YAxisLib.TicksEnum;

namespace SpaInspector.Components
{
    public partial class View4
    {
        // Using the list of Spa, rather than Spa object
        // [Parameter] public Spa Spa { get; set; }
        LineChart<object> lineChart = new LineChart<object>();
        [Parameter] public IList<Spa> SpaList { get; set; }

        List<string> backgroundColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(54, 162, 235, 0.2f), ChartColor.FromRgba(255, 206, 86, 0.2f), ChartColor.FromRgba(75, 192, 192, 0.2f), ChartColor.FromRgba(153, 102, 255, 0.2f), ChartColor.FromRgba(255, 159, 64, 0.2f) };
        List<string> borderColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 1f), ChartColor.FromRgba(54, 162, 235, 1f), ChartColor.FromRgba(255, 206, 86, 1f), ChartColor.FromRgba(75, 192, 192, 1f), ChartColor.FromRgba(153, 102, 255, 1f), ChartColor.FromRgba(255, 159, 64, 1f) };

        public PlotlyChart Chart;
        public Config Config = new Config();

        public Layout Layout;

        //Rename the first chart
        private async Task Restyle()
        {
            var updateScatterChart = new Scatter
            {
                Name = "First chart"
            };
            await Chart.Restyle(updateScatterChart, 0);
        }
        //Delete the first chart
        private async Task DeleteScatter()
        {
            await Chart.DeleteTrace(0);
        }
        // ReSharper disable once NotAccessedField.Local
        public IList<ITrace> Data;

        //Adding new chart for substract
        public List<List<object>> ChartData = new List<List<object>>();  // Y Axix Values
        public List<string> Labels;     // X Axis labels
        List<object> Waves;

        private bool FirstRender = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await HandleChartRedraw(); //new way

            }
        }

        async Task HandleChartRedraw()
        {

            await lineChart.Clear();
            await lineChart.AddLabelsDatasetsAndUpdate(Waves, GetLineChartDatasets());


        }

        //Read the data and make substract
        void ReadChartData()
        {

        public IList<ITrace> Data;
        public List<List<object>> ChartData = new List<List<object>>();  // Y Axix Values
        public List<string> Labels;     // X Axis labels
        List<object> Waves;
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

                    Name = "Spa file", // Figure it out title
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
        LineChartDataset<object>[] GetLineChartDatasets()
        {
            var retval = new LineChartDataset<object>[ChartData.Count];

            for (var i = 0; i < ChartData.Count; i++)
            {
                retval[i] = new LineChartDataset<object>
                {
                    Label = "test",
                    Data = ChartData[i],
                    BackgroundColor = backgroundColors,
                    BorderColor = borderColors[i],
                    Fill = false,
                    PointRadius = 0.5F,
                    BorderDash = new List<int> { },

                };
            }
            return retval;
        }
    }
}
