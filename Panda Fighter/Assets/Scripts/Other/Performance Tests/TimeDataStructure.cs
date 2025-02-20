/*using System.Diagnostics;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Collections.Specialized;

public class TimeDataStructure : MonoBehaviour
{
    const int Size = 40;
    const int Iterations = 1500;
    const string a = "Adf";
    void Start()
    {
        //timeArrays();
        timeDictionary();
    }

    private void OnValidate()
    {
    }

    private void timeDictionary()
    {
        var data = new Dictionary<double, double>();
        for (int i = 0; i < Size; i++)
            data[i] = i * 1.243f;

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            double sum = 0;
            for (int j = 0; j < data.Count; j++)
                sum += data[j];

            if (Math.Abs(sum - 23) < 0.1)
                return;
        }
        sw.Stop();
        UnityEngine.Debug.Log($"For loop: {sw.ElapsedMilliseconds}");

         data = new Dictionary<double, double>();
        for (int i = 0; i < Size; i++)
            data[i] = i * 1.243f;

        sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            double sum = 0;
            foreach (int j in data.Keys)
                sum += data[j];

            if (Math.Abs(sum - 23) < 0.1)
                return;
        }
        sw.Stop();
        UnityEngine.Debug.Log($"Foreach loop: {sw.ElapsedMilliseconds}");

    }

    private void timeArrays()
    {
        double[] data = new double[Size];
        System.Random rng = new System.Random();
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = rng.NextDouble();
        }

        double correctSum = data.Sum();

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            double sum = 0;
            for (int j = 0; j < data.Length; j++)
            {
                sum += data[j];
            }
            if (Math.Abs(sum - correctSum) > 0.1)
            {
                UnityEngine.Debug.Log("Summation failed");
                return;
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log($"For loop: {sw.ElapsedMilliseconds}");

        sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            double sum = 0;
            foreach (double d in data)
            {
                sum += d;
            }
            if (Math.Abs(sum - correctSum) > 0.1)
            {
                UnityEngine.Debug.Log("Summation failed");
                return;
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log($"Foreach loop: {sw.ElapsedMilliseconds}");
    }

    private void timeListsWithKnownEndSize()
    {
        var data = new List<double>(10000);
        System.Random rng = new System.Random(2);
        for (int i = 0; i < Size; i++)
            data.Add(rng.NextDouble());

        double correctSum = data.Sum();

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            double sum = 0;
            for (int j = 0; j < data.Count; j++)
            {
                sum += data[j];
            }
            if (Math.Abs(sum - correctSum) > 0.1)
            {
                UnityEngine.Debug.Log("Summation failed");
                return;
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log($"For loop: {sw.ElapsedMilliseconds}");

        sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            double sum = 0;
            foreach (double d in data)
                sum += d;
            
            if (Math.Abs(sum - correctSum) > 0.1)
            {
                UnityEngine.Debug.Log("Summation failed");
                return;
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log($"Foreach loop: {sw.ElapsedMilliseconds}");
    }

    private void timeLists()
    {
        var data = new List<double>();
        System.Random rng = new System.Random(3);
        for (int i = 0; i < Size; i++)
            data.Add(rng.NextDouble());

        double correctSum = data.Sum();

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            double sum = 0;
            for (int j = 0; j < data.Count; j++)
                sum += data[j];

            if (Math.Abs(sum - correctSum) > 0.1)
            {
                UnityEngine.Debug.Log("Summation failed");
                return;
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log($"For loop: {sw.ElapsedMilliseconds}");

        sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            double sum = 0;
            foreach (double d in data)
            sum += d;

            if (Math.Abs(sum - correctSum) > 0.1)
            {
                UnityEngine.Debug.Log("Summation failed");
                return;
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log($"Foreach loop: {sw.ElapsedMilliseconds}");
    }


    private void timeListsWithGameObjects()
    {
        var data = new List<GameObject>();
        for (int i = 0; i < Size; i++)
            data.Add(new GameObject());

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            BigInteger a;
            for (int j = 0; j < data.Count; j++)
                a += data[j].GetInstanceID();

            if (a == 2)
                return;
        }
        sw.Stop();
        UnityEngine.Debug.Log($"For loop: {sw.ElapsedMilliseconds}");

        sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            BigInteger a;
            for (int j = 0; j < data.Count; j++)
                a += data[j].GetInstanceID();

            if (a == 2)
                return;
        }

        sw.Stop();
        UnityEngine.Debug.Log($"Foreach loop: {sw.ElapsedMilliseconds}");
    }
}*/