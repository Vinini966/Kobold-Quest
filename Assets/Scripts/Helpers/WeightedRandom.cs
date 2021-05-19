using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedRandom : MonoBehaviour
{
    //I couldn't remember the way to do overloads without copypasting the function sorry for messy-ness.

    public static int Random(GameObject[] objectArray, float weight)
    {
        float[] weightedArray = new float[objectArray.Length]; //Make an array that will hold weighted values for our weighted randomness (must be same size as the array given
        float weightedNumber = weight;

        for (int i = 0; i <= weightedArray.Length - 1; i++)
        {
            weightedArray[i] = weightedNumber; //Assign each index the current weighted number value
            weightedNumber /= 2.5f; //after each assignment, divide that number by 2.5
        }

        float weightedRandom = UnityEngine.Random.Range(0f, weight); //choose a random number based on the max weight value

        for (int j = weightedArray.Length - 1; j >= 0; j--) //loop through the weighted array
        {
            if (weightedRandom <= weightedArray[j]) //stop if the random number is below one of the weighted numbers
            {
                return j; //This should be much more likely to return as the 1st value in the array
            }
        }

        return objectArray.Length - 1;
    }

    public static int Random(Sprite[] objectArray, float weight)
    {

        float[] weightedArray = new float[objectArray.Length]; //Make an array that will hold weighted values for our weighted randomness (must be same size as the array given
        float weightedNumber = weight;

        for (int i = 0; i <= weightedArray.Length - 1; i++)
        {
            weightedArray[i] = weightedNumber; //Assign each index the current weighted number value
            weightedNumber /= 2.5f; //after each assignment, divide that number by 2.5
        }

        float weightedRandom = UnityEngine.Random.Range(0f, weight); //choose a random number based on the max weight value

        for (int j = weightedArray.Length - 1; j >= 0; j--) //loop through the weighted array
        {
            if (weightedRandom <= weightedArray[j]) //stop if the random number is below one of the weighted numbers
            {
                return j; //This should be much more likely to return as the 1st value in the array
            }
        }

        return objectArray.Length - 1;
    }

    public static int Random(int[] objectArray, float weight)
    {

        float[] weightedArray = new float[objectArray.Length]; //Make an array that will hold weighted values for our weighted randomness (must be same size as the array given
        float weightedNumber = weight;

        for (int i = 0; i <= weightedArray.Length - 1; i++)
        {
            weightedArray[i] = weightedNumber; //Assign each index the current weighted number value
            weightedNumber /= 2.5f; //after each assignment, divide that number by 2.5
        }

        float weightedRandom = UnityEngine.Random.Range(0f, weight); //choose a random number based on the max weight value

        for (int j = weightedArray.Length - 1; j >= 0; j--) //loop through the weighted array
        {
            if (weightedRandom <= weightedArray[j]) //stop if the random number is below one of the weighted numbers
            {
                return j; //This should be much more likely to return as the 1st value in the array
            }
        }

        return objectArray.Length - 1;
    }
}
