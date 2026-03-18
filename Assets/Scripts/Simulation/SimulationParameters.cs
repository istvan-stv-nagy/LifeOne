using UnityEngine;

[System.Serializable]
public class SimulationParameters
{
    public int numTypes;
    public int usedTypes;
    public float[] rules;
    public int numParticles;
    public float interactionRadius;
    public float attractionStrength;
    public float maxSpeed;
    public float worldSize;

    public SimulationParameters(int _usedTypes, float[] _rules, int _numParticles, float _interactionRadius, float _attractionStrength, float _maxSpeed, float _worldSize)
    {
        usedTypes = _usedTypes;
        numTypes = (int)Mathf.Sqrt(_rules.Length);
        rules = new float[numTypes * numTypes];
        for (int i = 0; i < numTypes * numTypes; i++) rules[i] = _rules[i];
        numParticles = _numParticles;
        interactionRadius = _interactionRadius;
        attractionStrength = _attractionStrength;
        maxSpeed = _maxSpeed;
        worldSize = _worldSize;
    }
}