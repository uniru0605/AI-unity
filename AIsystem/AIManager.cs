using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;

    [Header("AI Objects")]
    public GameObject AI;
    public GameObject AIparent;

    [Header("Necessary arguments")]
    public UnityEvent initializeGame;

    [Header("GA Parameter")]
    public int player_num;
    public int numofinputs;
    public int numofoutputs;
    public int numofneurons;
    public int AI_passpercentage;
    public float mutation_percentage;
    public float firstmutation_percentage;
    public float weightrange;
    public float target_achievement;
    public bool Generate_from_AIinfo;


    [HideInInspector]
    public List<GameObject> AIs;

    [HideInInspector]
    public List<int> best_weights;

    [HideInInspector]
    private bool start_simulation;
    private bool finish;

    [Header("Info")]
    public int Generation;
    public List<float> best_weightsinfo;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    void Start()
    {
        AI.SetActive(false);
    }
    // Start is called before the first frame update
    public void Startsimulation()
    {
        AI.SetActive(false);
        start_simulation = true;
        finish = false;
        AIs.Clear();
        best_weights.Clear();
        Generation = 0;
        int i = 0;
        while (i < player_num)
        {
            GameObject target = Instantiate(AI, AIparent.transform);
            target.SetActive(true);
            AIs.Add(target);
            i += 1;
        }
        int i2 = 0;
        while (i2 < AIs.Count)
        {

            AIs[i2].GetComponent<AI>().weights.Clear();
            AIs[i2].GetComponent<AI>().initialize();
            i = 0;
            while (i < (numofneurons + 1) * numofoutputs + (numofinputs + 1) * numofneurons)
            {
                if (Generate_from_AIinfo && i < best_weightsinfo.Count)
                {
                    if (i2 == 0)
                    {
                        AIs[i2].GetComponent<AI>().weights.Add(best_weightsinfo[i]);
                    }
                    else
                    {
                        if (Random.Range(1.00000f, 100.00000f) <= firstmutation_percentage)
                        {
                            AIs[i2].GetComponent<AI>().weights.Add(Random.Range(-weightrange, weightrange));
                        }
                        else
                        {
                            AIs[i2].GetComponent<AI>().weights.Add(best_weightsinfo[i]);
                        }
                    }
                }
                else
                {
                    AIs[i2].GetComponent<AI>().weights.Add(Random.Range(-weightrange, weightrange));
                }
                i += 1;
            }
            i2 += 1;
        }
        initializeGame.Invoke();
    }
    public void Finish()
    {
        finish = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (start_simulation)
        {
            bool everyone_destroyed = true;
            int i2 = 0;
            while (i2 < AIs.Count)
            {
                if (!AIs[i2].GetComponent<AI>().destroyed && target_achievement > AIs[i2].GetComponent<AI>().score)
                {
                    everyone_destroyed = false;
                }
                i2 += 1;
            }
            if (everyone_destroyed)
            {
                Generation += 1;
                initializeGame.Invoke();
                best_weights.Clear();
                i2 = 0;
                while (i2 < AIs.Count)
                {
                    if (best_weights.Count < AIs.Count * AI_passpercentage / 100)
                    {
                        best_weights.Add(i2);
                    }
                    else
                    {
                        int i = 0;
                        float num = 0;
                        int finished = 0;
                        bool first = true;
                        while (i < best_weights.Count)
                        {
                            if (first)
                            {
                                num = AIs[best_weights[i]].GetComponent<AI>().score;
                                finished = i;
                                first = false;
                            }
                            if (num > AIs[best_weights[i]].GetComponent<AI>().score)
                            {
                                num = AIs[best_weights[i]].GetComponent<AI>().score;
                                finished = i;
                            }
                            i += 1;
                        }
                        if (AIs[i2].GetComponent<AI>().score > AIs[best_weights[finished]].GetComponent<AI>().score)
                        {
                            best_weights.RemoveAt(finished);
                            best_weights.Add(i2);
                        }
                    }
                    i2 += 1;
                }
                i2 = 0;
                while (i2 < AIs.Count)
                {
                    AIs[i2].GetComponent<AI>().initialize();
                    int i = 0;
                    bool goodspiecies = false;
                    while (i < best_weights.Count)
                    {
                        if (best_weights[i] == i2)
                        {
                            goodspiecies = true;
                        }
                        i += 1;
                    }
                    if (!goodspiecies)
                    {
                        AIs[i2].GetComponent<AI>().weights.Clear();
                        i = 0;
                        while (i < (numofneurons + 1) * numofoutputs + (numofinputs + 1) * numofneurons)
                        {
                            if (Random.Range(1.00000f, 100.00000f) <= mutation_percentage)
                            {
                                AIs[i2].GetComponent<AI>().weights.Add(Random.Range(-weightrange, weightrange));
                            }
                            else
                            {
                                AIs[i2].GetComponent<AI>().weights.Add(AIs[best_weights[Random.Range(0, best_weights.Count - 1)]].GetComponent<AI>().weights[i]);
                            }
                            i += 1;
                        }
                    }
                    i2 += 1;
                }
                if (finish)
                {
                    int i = 0;
                    float num = 0;
                    int finished = 0;
                    while (i < best_weights.Count)
                    {
                        if (num < AIs[best_weights[i]].GetComponent<AI>().score)
                        {
                            num = AIs[best_weights[i]].GetComponent<AI>().score;
                            finished = i;
                        }
                        i += 1;
                    }
                    AI.GetComponent<AI>().weights = AIs[best_weights[finished]].GetComponent<AI>().weights;
                    best_weightsinfo = AIs[best_weights[finished]].GetComponent<AI>().weights;
                    i = 0;
                    while (i < AIs.Count)
                    {
                        AIs[i].SetActive(false);
                        i += 1;
                    }
                    AI.SetActive(true);
                    AI.GetComponent<AI>().initialize();
                    initializeGame.Invoke();
                }
            }
        }
    }
}
