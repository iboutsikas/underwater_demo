using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelpers
{
    public static float GaussianRandom01(float mean, float sigma/*, int min, int max*/)
    {
        float rand1 = Random.Range(0.0f, 1.0f);
        float rand2 = Random.Range(0.0f, 1.0f);

        float n = Mathf.Sqrt(-2.0f * Mathf.Log(rand1)) * Mathf.Cos((2.0f * Mathf.PI) * rand2);

        int generatedNumber = Mathf.FloorToInt(mean + sigma * n);


        return generatedNumber;
    }

    public static float GaussianRandom(float mean, float sigma, int min, int max)
    {
        float number = GaussianRandom01(mean, sigma);
        number = Mathf.Clamp(number, min, max);
        return number;
    }
}
