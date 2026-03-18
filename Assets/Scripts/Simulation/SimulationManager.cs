using UnityEngine;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    [SerializeField]
    private ParticleRenderer particleRenderer;

    public ComputeShader computeShader;

    public float worldSize = 10.0f;

    public SimulationParameters simulationParameters;

    ComputeBuffer particleBuffer;
    ComputeBuffer rulesBuffer;

    ParticleStruct[] particles;

    public const int maxNumTypes = 10;

    int kernel;

    private float simulationSpeed = 1f;

    void Start()
    {
        float[] rules = RandomizeRules(maxNumTypes, maxNumTypes);
        simulationParameters = new SimulationParameters(
            maxNumTypes,
            rules,
            4000,
            2f,
            5f,
            5f
        );

        kernel = computeShader.FindKernel("CSMain");

        RestartSimulation();
    }

    void Update()
    {
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("interactionRadius", simulationParameters.interactionRadius);
        computeShader.SetFloat("attractionStrength", simulationParameters.attractionStrength);
        computeShader.SetFloat("maxSpeed", simulationParameters.maxSpeed);
        computeShader.SetFloat("simulationSpeed", simulationSpeed);

        computeShader.Dispatch(kernel, simulationParameters.numParticles / 256 + 1, 1, 1);
    }

    public void RestartSimulation()
    {
        Debug.Log("Restarting simulation...");
        particles = new ParticleStruct[simulationParameters.numParticles];
        for (int i = 0; i < simulationParameters.numParticles; i++)
        {
            particles[i].position = Random.insideUnitCircle * worldSize;
            particles[i].velocity = Vector2.zero;
            particles[i].type = Random.Range(0, simulationParameters.usedTypes);
            particles[i].clusterId = -1;
        }

        particleBuffer?.Release();
        particleBuffer = new ComputeBuffer(simulationParameters.numParticles, sizeof(float) * 4 + 2 * sizeof(int));
        particleBuffer.SetData(particles);
        computeShader.SetBuffer(kernel, "particles", particleBuffer);

        computeShader.SetInt("particleCount", simulationParameters.numParticles);
        computeShader.SetInt("numTypes", maxNumTypes);

        rulesBuffer = new ComputeBuffer(maxNumTypes * maxNumTypes, sizeof(float));
        rulesBuffer.SetData(simulationParameters.rules);
        computeShader.SetBuffer(kernel, "Rules", rulesBuffer);

        particleRenderer.SetParticleComputeBuffer(particleBuffer);
        particleRenderer.SetParticleCount(simulationParameters.numParticles);
    }

    public float[] RandomizeRules(int maxTypes, int usedTypes)
    {
        float[] newRules = new float[maxTypes * maxTypes];
        for (int i = 0; i < maxTypes * maxTypes; i++)
        {
            newRules[i] = Random.Range(-1.0f, 1.0f);
        }
        int idx = 0;
        for (int r = 0; r < maxTypes; r++)
        {
            for (int c = 0; c < maxTypes; c++)
            {
                if (c >= usedTypes || r >= usedTypes)
                {
                    newRules[idx] = 0.0f;
                }
                idx++;
            }
        }
        return newRules;
    }

    public void LoadPreset(SimulationParameters preset)
    {
        simulationParameters = preset;
        RestartSimulation();
    }

    public void SelectRandomized(int numTypes, int numParticles, float attraction, float interactionR)
    {
        float[] newRules = RandomizeRules(maxNumTypes, numTypes);
        simulationParameters = new SimulationParameters(numTypes, newRules, numParticles, interactionR, attraction, 5f);
        RestartSimulation();
    }

    public void SetSimulationSpeed(float selectedSpeed)
    {
        simulationSpeed = selectedSpeed;
    }

    public void Cluster()
    {
        particleBuffer.GetData(particles);
        List<List<int>> clusters = Clustering.DetectClusters(particles, simulationParameters.interactionRadius, 50);

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

    public void SavePreset(string presetName)
    {
        PresetDataLoader.SavePreset(simulationParameters, presetName);
    }

    void OnDestroy()
    {
        particleBuffer.Release();
        rulesBuffer.Release();
    }
}