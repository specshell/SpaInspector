using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Charts.Streaming;
using Microsoft.AspNetCore.Components;

using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.LegendLib;
using Plotly.Blazor.LayoutLib.XAxisLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterGlLib;
using SpaInspectorReader;
using TicksEnum = Plotly.Blazor.LayoutLib.YAxisLib.TicksEnum;

namespace SpaInspector.Components
{
    public partial class View3
    {
        LineChart<object> lineChart = new LineChart<object>();
        object Options = new { ResponsiveAnimationDuration = 0 };


        [Parameter] public IList<Spa> SpaList { get; set; }

        List<string> backgroundColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(54, 162, 235, 0.2f), ChartColor.FromRgba(255, 206, 86, 0.2f), ChartColor.FromRgba(75, 192, 192, 0.2f), ChartColor.FromRgba(153, 102, 255, 0.2f), ChartColor.FromRgba(255, 159, 64, 0.2f) };
        List<string> borderColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 1f), ChartColor.FromRgba(54, 162, 235, 1f), ChartColor.FromRgba(255, 206, 86, 1f), ChartColor.FromRgba(75, 192, 192, 1f), ChartColor.FromRgba(153, 102, 255, 1f), ChartColor.FromRgba(255, 159, 64, 1f) };


        public List<MySlider> Sliders { get; private set; } = new List<MySlider>();




        public Layout Layout;

        // ReSharper disable once NotAccessedField.Local
        public IList<ITrace> Data;

        public List<List<object>> ChartData = new List<List<object>>();  // Y Axix Values
        public List<string> Labels;     // X Axis labels
        List<object> Waves;


        private bool FirstRender = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ReadChartData();
                await HandleChartRedraw(); //new way
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await HandleChartRedraw(); //new way
            }


        }

        async Task HandleChartRedraw()
        {

            await lineChart.Clear();
            await lineChart.AddLabelsDatasetsAndUpdate(Waves, GetLineChartDatasets());

            //
        }

        async Task HandleChartUpdate(int index)
        {

            //var newDataSet = GetLineChartDataset(index);

            //lineChart.RemoveDataSet(index);
            //lineChart.a
            await lineChart.Clear();
            await lineChart.AddLabelsDatasetsAndUpdate(Waves, GetLineChartDatasets());


            // await lineChart.Update();
            // StateHasChanged();
            // await InvokeAsync(StateHasChanged);
        }


        //gets all chart datasets
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

        //gets a single chart dataset
        LineChartDataset<object> GetLineChartDataset(int index)
        {

            var retval = new LineChartDataset<object>()
            {
                Label = "test",
                Data = ChartData[index],
                BackgroundColor = backgroundColors[index],
                BorderColor = borderColors[index],
                Fill = false,
                PointRadius = 0.5F,
                BorderDash = new List<int> { },
            };

            return retval;
        }






        void ReadChartData()
        {
            Data = new List<ITrace>();
            var counter = 0;

            foreach (var spa in SpaList)
            {
                //setup slider class
                if (FirstRender == true)
                {
                    var s = new MySlider() { Min = -2, Max = 2, Step = 0.1, Title = spa.Title, Index = counter };
                    s.SLIDER_CHANGED += SliderValueChanged;
                    Sliders.Add(s);
                }

                //transpose Y values depending on slider value for the series-------------------------------------------//
                for (var u = 0; u < spa.UnitIntensities.Length; u++)
                {
                    spa.UnitIntensities[u] += (float)Sliders[counter].Value;
                }
                //------------------------------------------------------------------------------------------------------//
                counter++;


                var unitIntensities = spa.UnitIntensities;
                if (unitIntensities.Length == 0) return;

                Waves = new List<object>();

                for (var i = unitIntensities.Length - 1; i >= 0; i--)
                {
                    Waves.Add(i * spa.Headers.DataSpacing + spa.Headers.LastX);
                }


                ChartData.Add(unitIntensities.Reverse().Cast<object>().ToList());

                Data.Add(new ScatterGl
                {
                    Name = spa.Title, // something from header
                    Mode = ModeFlag.Lines,
                    Y = unitIntensities.Reverse().Cast<object>().ToArray(),
                    X = Waves,
                    Line = new Line
                    {
                        Width = 1.5m,
                    }
                });
            }
        }

        void SliderValueChanged(MySlider slider)
        {
            UpdateChartDataOffsetValues(slider.Index);
        }

        void UpdateChartDataOffsetValues(int index)
        {

            var d = Data;
            var item = (ScatterGl)Data[index];
            for (var t = 0; t < item.Y.Count; t++)
            {
                ChartData[index][t] = ((float)item.Y[t] + (float)Sliders[index].Value);
            }

            var dt = lineChart?.Data?.Datasets;

        }



        public class MySlider
        {

            //public event SliderChangedEventHandler SLIDER_CHANGED;
            //public delegate void SliderChangedEventHandler(int index);

            public event SliderChangedEventHandler SLIDER_CHANGED;
            public delegate void SliderChangedEventHandler(MySlider slider);


            double _value { get; set; }
            public double Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        SLIDER_CHANGED(this);
                    }
                }
            }


            public double Max { get; set; } = 0;
            public double Min { get; set; } = 0;
            public double Step { get; set; } = 0;
            public string Title { get; set; } = "";
            public int Index { get; set; }
        }

        //protected override void OnInitialized_BACKUP_01SEP21()
        //{
        //    Sliders = new List<MySlider>();

        //    //transpose Y values -------------------------------------------//
        //    float offSet = 0;
        //    var uiLen = SpaList[0].UnitIntensities.Length - 1;
        //    for (var i = 0; i < SpaList.Count; i++)
        //    {
        //        if (i > 0)
        //        {
        //            offSet = (SpaList[i].UnitIntensities[uiLen]) - (SpaList[i - 1].UnitIntensities[uiLen]); //calc offset value
        //            offSet = (offSet < 0) ? (offSet * -1) : offSet; // all this does it turn a negative to a positive

        //            //apply offset to y values
        //            for (var u = 0; u <= uiLen; u++)
        //            {
        //                SpaList[i].UnitIntensities[u] += offSet;
        //            }
        //        }
        //    }
        //    //--------------------------------------------------------------//

        //    Data = new List<ITrace>();
        //    var counter = 0;

        //    foreach (var spa in SpaList)
        //    {


        //        var s = new MySlider() { Min = -2, Max = 2, Step = 0.01, Title = spa.Title, Index = counter };
        //        s.SLIDER_CHANGED += SliderValueChanged;
        //        counter++;

        //        Sliders.Add(s);

        //        var unitIntensities = spa.UnitIntensities;
        //        if (unitIntensities.Length == 0) return;

        //        var waves = new List<object>();

        //        for (var i = unitIntensities.Length - 1; i >= 0; i--)
        //        {
        //            waves.Add(i * spa.Headers.DataSpacing + spa.Headers.LastX);
        //        }

        //        Data.Add(new ScatterGl
        //        {

        //            Name = spa.Title, // <-- not sure what title you want but you could grab something from one of the headers?
        //            Mode = ModeFlag.Lines,
        //            Y = unitIntensities.Reverse().Cast<object>().ToArray(),
        //            X = waves,
        //            Line = new Line
        //            {
        //                Width = 1.5m,
        //            }


        //        });



        //        Layout = new Layout()
        //        {
        //            XAxis = new List<XAxis>
        //            {
        //                new XAxis()
        //                {
        //                    AutoRange = AutoRangeEnum.False,
        //                    Range = new List<object> { spa.Headers.FirstX, spa.Headers.LastX},
        //                    ShowGrid = false,
        //                },
        //            },
        //            YAxis = new List<YAxis>
        //            {
        //                new YAxis()
        //                {
        //                    AutoMargin = false,
        //                    Ticks = TicksEnum.Empty,
        //                    Tick0 = 0,
        //                    DTick = unitIntensities.Max() < 1.0f ? 0.1 : 1,
        //                    ZeroLine = false,
        //                    ShowGrid = false,
        //                },
        //            },
        //            AutoSize = false,
        //            Height = 750,
        //            Width = 1500,
        //        };

        //    }




        //}




        //protected override void OnInitialized_BACKUP_21AUG()
        //{
        //    Data = new List<ITrace>();

        //    foreach (var spa in SpaList)
        //    {
        //        var unitIntensities = spa.UnitIntensities;
        //        if (unitIntensities.Length == 0) return;

        //        var waves = new List<object>();

        //        for (var i = unitIntensities.Length - 1; i >= 0; i--)
        //        {
        //            waves.Add(i * spa.Headers.DataSpacing + spa.Headers.LastX);
        //        }

        //        Data.Add(new ScatterGl
        //        {

        //            Name = spa.Title, // <-- not sure what title you want but you could grab something from one of the headers?
        //            Mode = ModeFlag.Lines,
        //            Y = unitIntensities.Reverse().Cast<object>().ToArray(),
        //            X = waves,
        //            Line = new Line
        //            {
        //                Width = 1.5m,
        //            }


        //        });



        //        Layout = new Layout()
        //        {
        //            XAxis = new List<XAxis>
        //            {
        //                new XAxis()
        //                {
        //                    AutoRange = AutoRangeEnum.False,
        //                    Range = new List<object> { spa.Headers.FirstX, spa.Headers.LastX},
        //                    ShowGrid = false,
        //                },
        //            },
        //            YAxis = new List<YAxis>
        //            {
        //                new YAxis()
        //                {
        //                    AutoMargin = false,
        //                    Ticks = TicksEnum.Empty,
        //                    Tick0 = 0,
        //                    DTick = unitIntensities.Max() < 1.0f ? 0.1 : 1,
        //                    ZeroLine = false,
        //                    ShowGrid = false,
        //                },
        //            },
        //            AutoSize = false,
        //            Height = 750,
        //            Width = 1500,
        //        };

        //    }
        //}






    }
}
