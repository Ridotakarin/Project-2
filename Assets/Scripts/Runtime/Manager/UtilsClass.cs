using System;
using UnityEngine;

public static class UtilsClass
{
    public static Vector3 GetRandomDir()
    {
        float angle = UnityEngine.Random.Range(0f, 360f);
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f);
    }
    private static System.Random random = new System.Random();

    public static T PickOneByRatio<T>(T[] items, float[] ratios)
    {
        if (items == null || ratios == null)
            throw new ArgumentNullException("Items and ratios cannot be null.");

        if (items.Length != ratios.Length)
            throw new ArgumentException("Items and ratios must be the same length.");

        float total = 0f;
        foreach (float ratio in ratios)
        {
            if (ratio < 0)
                throw new ArgumentException("Ratios must be non-negative.");
            total += ratio;
        }

        if (total == 0f)
            throw new ArgumentException("Total ratio cannot be zero.");

        float randomValue = (float)(random.NextDouble() * total);
        float cumulative = 0f;

        for (int i = 0; i < ratios.Length; i++)
        {
            cumulative += ratios[i];
            if (randomValue < cumulative)
                return items[i];
        }

        // Fallback (should not happen if the ratios are valid)
        return items[items.Length - 1];
    }

    public static void AdjustRatioByFertilizerLevel(ref float[] ratioArray, int fertilizerLevel)
    {
        int levelCount = ratioArray.Length;

        // Nothing to do if no fertilizer
        if (fertilizerLevel <= 0 || levelCount < 2)
            return;

        // Define fertilizer shift intensity per level (tweakable)
        float totalShift = 20f * fertilizerLevel; // Max 60% shift at level 3

        // Reduce from level 1
        ratioArray[0] -= totalShift;

        // Distribute the shifted amount across higher levels
        float perLevelGain = totalShift / (levelCount - 1);

        for (int i = 1; i < levelCount; i++)
            ratioArray[i] += perLevelGain;
    }
}
