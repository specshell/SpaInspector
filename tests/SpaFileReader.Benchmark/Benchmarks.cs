﻿using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using SpaInspectorReader;
using static SpaTestUtils.TestFixture;

namespace SpaFileReader.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Benchmarks
{
    private static readonly string TestFile = BenchmarkTestFile("test1.spa");
    private readonly byte[] _bytes;

    public Benchmarks()
    {
        _bytes = File.ReadAllBytes(TestFile);
    }

    [Benchmark]
    public Span<float> SpanReadFloats()
    {
        var absorbance = SpaFile.ReadYUnitAsSpanFloat(_bytes);
        return absorbance;
    }

    [Benchmark]
    public float[] SpanReadFloatArray()
    {
        var absorbance = SpaFile.ReadYUnitAsFloatArray(_bytes);
        return absorbance;
    }

    [Benchmark]
    public Span<byte> SpanReadBytes()
    {
        var absorbance = SpaFile.ReadYUnitAsSpanByte(_bytes);
        return absorbance;
    }

    [Benchmark]
    public Span<double> SpanReadDouble()
    {
        var absorbance = SpaFile.ReadYUnitAsSpanDouble(_bytes);
        return absorbance;
    }

    [Benchmark]
    public double[] SpanReadDoubleArray()
    {
        var absorbance = SpaFile.ReadYUnitAsDoubleArray(_bytes);
        return absorbance;
    }

    [Benchmark]
    public float[] BinaryReaderReadAbsorbance()
    {
        Stream stream = new MemoryStream(_bytes);
        var read = new Read(stream);
        var absorbance = read.ReadBinaryToSpa().UnitIntensities;
        return absorbance;
    }
}