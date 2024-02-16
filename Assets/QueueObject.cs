using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using TMPro;

public class QueueObject : MonoBehaviour
{
    public GameObject toQueue;
    private Vector3 initPosition = new Vector3(-1.8f, 6f, 0f);

    private float interval = 1.5f;
    private float nextFrameTime = 0;

    private Queue<GameObject> queuedObjects = new Queue<GameObject>();

    private int minLength = 10;
    private int currentStep = 0;
    private int currentBall = 1;
    private int currentPeak = 0;
    private int currentCount = 0;
    private string currentString = "";


    public TMP_InputField minLengthField;
    public TMP_InputField parenField;
    public TMP_Text statsField;

    // Start is called before the first frame update
    void Start()
    {
        /*
         * int myMax = 0;
        for (int i = 0; i < 10; i++)
        {
            string newParen = genParentheses();
            if (newParen.Length > myMax)
            {
                myMax = newParen.Length;
                UnityEngine.Debug.Log("Length: " + newParen.Length + "\nParentheses: " + newParen);
            }
        }
        UnityEngine.Debug.Log("Max: " + myMax);
        */
    }
        // Update is called once per frame
        void Update()
    {
        if (Time.time >= nextFrameTime && currentString.Length > 0)
        {
            nextFrameTime = Time.time + interval;
            //UnityEngine.Debug.Log("Updating time!");
            if (currentString[currentStep] == ')')
            {
                removeFromQueue();
                currentCount -= 1;
            }
            else
            {
                addToQueue();
                currentCount += 1;
            }
            statsField.text = "Length: " + currentString.Length + "\nPeak: " + currentPeak + "\nCount: " + currentCount + "\nStep: " + (currentStep+1);
            currentStep = (currentStep + 1) % currentString.Length;
        }

    }

    void addToQueue()
    {
        // Place toStack object above the scene
        GameObject newCircle = Instantiate(toQueue, initPosition, Quaternion.identity);
        newCircle.GetComponentInChildren<Canvas>().GetComponentInChildren<TMP_Text>().text = (currentBall % (currentString.Length / 2 + 1)).ToString();
        currentBall++;
        // Add it to the queue
        queuedObjects.Enqueue(newCircle);
    }

    void removeFromQueue()
    {
        // Destroy game object.
        GameObject toDestroy = queuedObjects.Dequeue();
        Destroy(toDestroy);
    }

    private string genParentheses()
    {
        // Using BOLTZMANN GENERATION!
        // For these objects, the generating function is D = (1 - sqrt(1 - 4z))/(2z).
        // The dominant singularity occurs at z = 0.25, so that's where we evaluate it.
        // Then D(0.25) = 1/(0.5) = 2.
        // We know the grammar: D = epsilon + "(" * D * ")"
        // So: This is a combinatorial sum.
        Stack<char> charStack = new Stack<char>();
        charStack.Push('D');
        string toReturn = "";
        while (charStack.Count > 0)
        {
            char currentChar = charStack.Pop();
            if (currentChar == 'D')
            {
                float u = UnityEngine.Random.value;
                if (u >= 0.5) // Because 1 / D(0.25) = 0.5
                {
                    charStack.Push('D');
                    charStack.Push(')');
                    charStack.Push('D');
                    toReturn += '(';
                }
            }
            else
            {
                toReturn += currentChar;
            }
        }
        return toReturn;

        /*
        float u = UnityEngine.Random.value;
        if (u < 0.5) // Because 1 / D(0.25) = 0.5
        {
            return "";
        }
        else
        {
            return "(" + genParentheses() + ")" + genParentheses();
        }
        */
    }

    private string genParentheses(int minLength)
    {
        string toReturn = genParentheses();
        while (toReturn.Length < minLength)
        {
            toReturn = genParentheses();
        }
        return toReturn;
    }

    public void pressGenerateButton()
    {
        // Reset everything
        // Get the text from the minLength field.
        string minLengthInput = minLengthField.text;
        int newVal = 0;
        bool isValidInput = int.TryParse(minLengthInput, out newVal);
        if (isValidInput)
        {
            resetQueue();
            minLength = newVal;
            // Generate parentheses.
            currentString = genParentheses(minLength);
            currentPeak = findPeak(currentString);
            // Display parentheses in text field
            parenField.text = currentString;
            // Start dropping circles
        }
    }

    private void resetQueue()
    {
        while (queuedObjects.Count > 0)
        {
            GameObject toDestoy = queuedObjects.Dequeue();
            Destroy(toDestoy);
        }
        currentStep = 0;
        currentBall = 1;
    }

    private int findPeak(string parenString)
    {
        int currentMax = 0;
        int currentHeight = 0;
        for (int i = 0; i < parenString.Length; i++)
        {
            if (parenString[i] == '(')
            {
                currentHeight += 1;
                if (currentHeight > currentMax)
                {
                    currentMax = currentHeight;
                }
            }
            else
            {
                currentHeight -= 1;
            }
        }
        return currentMax;
    }
}
