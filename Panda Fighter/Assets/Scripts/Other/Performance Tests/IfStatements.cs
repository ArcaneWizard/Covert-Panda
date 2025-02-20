using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using UnityEngine;

public class IfStatements : MonoBehaviour
{
    const int NUM_ENTITIES_SPAWNED = 30;
    const int NUM_ASPECTS_CREATED = 15;
    const int MULTIPLIER_OF_ADDITIONAL_CASES_POSSIBLE = 4;

    int iterations; // total number of searches performed on the case search specified
    int numManualCases; // number of if statements to go through (worst case) for a single search when not using dictionaries
    int numDictionariesUsed; // number of dictionaries used to speed up performance
    int numDictionaryManualCases; // number of if statements to go through (worst case) for a single search after using dictionaries
    int memorySize;  // total memory used (total number of cases)

    int[] caseSearches = new int[] { 25, 25, 5, 6 };
    int numberOfVarsToOptimize = 2;

    void Awake()
    {
        iterations = NUM_ENTITIES_SPAWNED * NUM_ASPECTS_CREATED * MULTIPLIER_OF_ADDITIONAL_CASES_POSSIBLE;

        memorySize = 1;
        numDictionariesUsed = 1;
        numDictionaryManualCases = 0;
        numManualCases = 0;

        for (int i = 0; i < caseSearches.Length; i++) {
            memorySize *= caseSearches[i];
            numManualCases += caseSearches[i];
        }

        for (int i = 0; i < numberOfVarsToOptimize; i++)
            numDictionariesUsed *= caseSearches[i];

        for (int i = numberOfVarsToOptimize; i < caseSearches.Length; i++)
            numDictionaryManualCases += caseSearches[i];

        var printCases = new StringBuilder("Case searches of:");
        for (int i = 0; i < caseSearches.Length; i++)
            printCases.Append($" {caseSearches[i]}");
        UnityEngine.Debug.Log(printCases.ToString());

        UnityEngine.Debug.Log($"memorySize: {memorySize}, DictionaryCount: {numDictionariesUsed}, IfStatement cases: {numManualCases} " +
            $" \nIf statements ot manually check: {numDictionaryManualCases}");

        performTimeTest();
    }

    private void performTimeTest()
    {
        Stopwatch swBeforeInitialization = Stopwatch.StartNew();
        var data = new List<Dictionary<int, int>>();
        for (int i = 0; i < numDictionariesUsed; i++) {
            var dict = new Dictionary<int, int>();
            dict[i + 1] = i * 2 - 1;
            dict[i - 1] = i;
            dict[i - 2] = i + 2;
            dict[i - 2] = i - 2912;
            data.Add(dict);
        }

        Stopwatch swAfterInitialization = Stopwatch.StartNew();
        int[] arr = new int[] { 40, 123, 1232, 12332, 11, 3, 12, 4, 13 };
        BigInteger bigInteger = 0;

        for (int i = 0; i < iterations; i++) {
            for (int w = 0; w < data.Count / 4; w++)
                data[Random.Range(0, data.Count)][numberOfVarsToOptimize] = Random.Range(4, memorySize);

            for (int h = 0; h < numDictionaryManualCases; h++) {
                int c = Random.Range(1, memorySize);
                if (c == arr[0])
                    bigInteger += 2;
                else if (c == arr[1])
                    bigInteger += 14;
                else if (c == arr[2])
                    bigInteger += 23;
                else if (c == arr[4])
                    bigInteger += 9;
                else if (c == arr[7])
                    bigInteger += (h - 2);
            }

            if (bigInteger == Random.Range(1, memorySize) || bigInteger % Random.Range(1, memorySize) == Random.Range(1, memorySize))
                UnityEngine.Debug.LogError("Mission failed");

            for (int j = 0; j < numberOfVarsToOptimize; j++)
                if (data[Random.Range(0, data.Count)].ContainsKey(Random.Range(1, memorySize)))
                    UnityEngine.Debug.LogError("Mission failed");
        }

        swBeforeInitialization.Stop();
        swAfterInitialization.Stop();
        UnityEngine.Debug.Log($"Dictionaries Initialization Time: {swBeforeInitialization.ElapsedMilliseconds - swAfterInitialization.ElapsedMilliseconds}");
        UnityEngine.Debug.Log($"Dictionaries Iteration Time: {swAfterInitialization.ElapsedMilliseconds}");

        Stopwatch sw = Stopwatch.StartNew();
        BigInteger a = 0;
        for (int i = 0; i < iterations; i++) {
            for (int j = 0; j < numManualCases + data.Count / 4; j++) {
                int c = Random.Range(1, 12300);
                if (c == arr[0])
                    a += 2;
                else if (c == arr[1])
                    a += 14;
                else if (c == arr[2])
                    a += 23;
                else if (c == arr[4])
                    a += 9;
                else if (c == arr[7])
                    a += (j - 2);
            }

            if (a == Random.Range(1, memorySize) || a % Random.Range(1, memorySize) == Random.Range(1, memorySize))
                UnityEngine.Debug.LogError("Mission failed");
        }

        sw.Stop();
        UnityEngine.Debug.Log($"Foreach loop: {sw.ElapsedMilliseconds}");
    }
}
