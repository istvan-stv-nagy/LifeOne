using UnityEngine;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    [SerializeField]
    private ParticleRenderer particleRenderer;

    public ComputeShader computeShader;
    public int particleCount = 100;
    public const int defaultParticleCount = 4000;

    public float worldSize = 10.0f;

    public float interactionRadius = 2f;
    public float attractionStrength = 5f;
    public float maxSpeed = 5f;
    public SimulationParameters simulationParameters;

    ComputeBuffer particleBuffer;
    ComputeBuffer rulesBuffer;

    ParticleStruct[] particles;

    public const int maxNumTypes = 10;

    public int currentUsedTypes = maxNumTypes;

    private float[] rules = new float[maxNumTypes * maxNumTypes];

    int kernel;

    private float simulationSpeed = 1f;

    void Start()
    {
        particles = new ParticleStruct[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            particles[i].position = Random.insideUnitCircle * worldSize;
            particles[i].velocity = Vector2.zero;
            particles[i].type = Random.Range(0, maxNumTypes);
            particles[i].clusterId = -1;
        }

        particleBuffer = new ComputeBuffer(particleCount, sizeof(float) * 4 + 2 * sizeof(int));

        particleBuffer.SetData(particles);

        kernel = computeShader.FindKernel("CSMain");

        computeShader.SetBuffer(kernel, "particles", particleBuffer);
        computeShader.SetInt("particleCount", particleCount);
        computeShader.SetInt("numTypes", maxNumTypes);

        RandomizeRules();
        rulesBuffer = new ComputeBuffer(maxNumTypes * maxNumTypes, sizeof(float));
        rulesBuffer.SetData(rules);
        computeShader.SetBuffer(kernel, "Rules", rulesBuffer);

        particleRenderer.SetParticleComputeBuffer(particleBuffer);
        particleRenderer.SetParticleCount(particleCount);
    }

    void Update()
    {
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("interactionRadius", interactionRadius);
        computeShader.SetFloat("attractionStrength", attractionStrength);
        computeShader.SetFloat("maxSpeed", maxSpeed);
        computeShader.SetFloat("simulationSpeed", simulationSpeed);

        computeShader.Dispatch(kernel, particleCount / 256 + 1, 1, 1);
    }

    public void RestartSimulation(int numTypesOfCells = maxNumTypes, int particlesToGenerate = defaultParticleCount)
    {
        Debug.Log("Restarting simulation...");
        currentUsedTypes = numTypesOfCells;
        particles = new ParticleStruct[particlesToGenerate];
        for (int i = 0; i < particlesToGenerate; i++)
        {
            particles[i].position = Random.insideUnitCircle * worldSize;
            particles[i].velocity = Vector2.zero;
            particles[i].type = Random.Range(0, numTypesOfCells);
            particles[i].clusterId = -1;
        }

        particleBuffer?.Release();
        particleBuffer = new ComputeBuffer(particlesToGenerate, sizeof(float) * 4 + 2 * sizeof(int));
        particleBuffer.SetData(particles);
        computeShader.SetBuffer(kernel, "particles", particleBuffer);

        particleRenderer.SetParticleComputeBuffer(particleBuffer);
        particleRenderer.SetParticleCount(particlesToGenerate);

        rulesBuffer.SetData(rules);
    }

    public void RandomizeRules()
    {
        for (int i = 0; i < maxNumTypes * maxNumTypes; i++)
        {
            rules[i] = Random.Range(-1.0f, 1.0f);
        }
        Debug.Log(string.Join(" ", rules));
    }

    public void SetPreset(SimulationParameters preset)
    {
        int numTypes = maxNumTypes;
        SetRules(preset.rules, out numTypes);
        Debug.Log($"This are the rules:{string.Join(" ", rules)}");
        interactionRadius = preset.interactionRadius;
        attractionStrength = preset.attractionStrength;
        maxSpeed = preset.maxSpeed;
        RestartSimulation(preset.usedTypes, preset.numParticles);
    }

    public void SetRules(float[] presetRules, out int numTypes)
    {
        if (presetRules.Length == maxNumTypes * maxNumTypes)
        {
            for (int i = 0; i < maxNumTypes * maxNumTypes; i++)
                rules[i] = presetRules[i];
            numTypes = maxNumTypes;
        }
        else
        {
            int presetDim = (int)Mathf.Sqrt(presetRules.Length);
            numTypes = presetDim;
            int sourceIdx = 0;
            int destIdx = 0;
            for (int r = 0; r < maxNumTypes; r++)
            {
                for (int c = 0; c < maxNumTypes; c++)
                {
                    if (c >= presetDim || r >= presetDim)
                    {
                        rules[destIdx] = 0.0f;
                    }
                    else
                    {
                        rules[destIdx] = presetRules[sourceIdx];
                        sourceIdx++;
                    }
                    destIdx++;
                }
            }
        }
    }

    public void LoadPreset(SimulationParameters preset)
    {
        SetPreset(preset);
    }

    public void SelectRandomized(int numTypes = maxNumTypes, int numParticles = 4000, float attraction = 5f, float interactionR = 2f)
    {
        RandomizeRules();
        attractionStrength = attraction;
        interactionRadius = interactionR;
        RestartSimulation(numTypes, numParticles);
    }

    public void SetSimulationSpeed(float selectedSpeed)
    {
        simulationSpeed = selectedSpeed;
    }

    public void Cluster()
    {
        particleBuffer.GetData(particles);
        List<List<int>> clusters = Clustering.DetectClusters(particles, interactionRadius, 50);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].clusterId = -1;
        }
        for (int i = 0; i < clusters.Count; i++)
        {
            for (int j = 0; j < clusters[i].Count; j++)
            {
                particles[clusters[i][j]].clusterId = i;
            }
        }

        particleBuffer.SetData(particles);
    }

    public void VisualizeParticleTypes()
    {
        particleBuffer.GetData(particles);
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].clusterId = -1;
        }
        particleBuffer.SetData(particles);
    }

    public float[] GetRulesZeroed()
    {
        float[] rules_zeroed = new float[maxNumTypes * maxNumTypes];
        int sourceIdx = 0;
        int destIdx = 0;
        for (int r = 0; r < maxNumTypes; r++)
        {
            for (int c = 0; c < maxNumTypes; c++)
            {
                if (c >= currentUsedTypes || r >= currentUsedTypes)
                {
                    rules_zeroed[destIdx] = 0.0f;
                }
                else
                {
                    rules_zeroed[destIdx] = rules[sourceIdx];
                    sourceIdx++;
                }
                destIdx++;
            }
        }
        return rules_zeroed;
    }

    public void SavePreset(string presetName)
    {
        float[] rules_zeroed = GetRulesZeroed();
        SimulationParameters newPreset = new SimulationParameters(
            currentUsedTypes,
            rules_zeroed,
            particleCount,
            interactionRadius,
            attractionStrength,
            maxSpeed
        );
        PresetDataLoader.SavePreset(newPreset, presetName);
    }

    void OnDestroy()
    {
        particleBuffer.Release();
        rulesBuffer.Release();
    }
}