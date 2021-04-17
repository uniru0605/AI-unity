using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AI : MonoBehaviour
{
    [Header("Necessary arguments")]
    public UnityEvent Move;
    public UnityEvent Initialize;
    public UnityEvent MakeInputs;

    [HideInInspector]
    public List<float> weights = new List<float>();

    [HideInInspector]
    public List<float> input = new List<float>();

    [HideInInspector]
    public bool destroyed;

    [Header("Info")]
    public float score;

    // Start is called before the first frame update
    public void initialize()
    {
        Initialize.Invoke();
        score = 0;
        destroyed = false;
        gameObject.SetActive(true);
    }
    public void Update()
    {

        if (destroyed)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            Move.Invoke();
        }
        MakeInputs.Invoke();
    }
    public float outputreturn(int numofoutput)
    {
        int i = 0;
        float addupvalue = 0;
        while (i < AIManager.instance.numofneurons)
        {
            int i2 = 0;
            float addupvalue2 = 0;
            while (i2 < input.Count)
            {
                addupvalue2 += input[i2] * (weights[i2 + i * (AIManager.instance.numofinputs + 1) + AIManager.instance.numofoutputs * (AIManager.instance.numofneurons + 1)]);
                i2 += 1;
            }

            double value2 = System.Math.Tanh(addupvalue2 + weights[AIManager.instance.numofinputs + AIManager.instance.numofoutputs * (AIManager.instance.numofneurons + 1)]);

            addupvalue += (float)value2 * (weights[i + numofoutput * (AIManager.instance.numofneurons + 1)]);
            i += 1;
        }

        double value = System.Math.Tanh(addupvalue + weights[AIManager.instance.numofneurons + numofoutput * (AIManager.instance.numofneurons + 1)]);
        return (float)value;
    }
}
