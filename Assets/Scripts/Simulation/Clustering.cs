using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class Clustering
{
    public static List<List<int>> DetectClusters(ParticleStruct[] particles, float clusterDistance, int minClusterSize)
    {
        int n = particles.Length;
        bool[] visited = new bool[n];
        List<List<int>> clusters = new List<List<int>>();

        for (int i = 0; i < n; i++)
        {
            if (visited[i]) continue;

            List<int> cluster = new List<int>();
            Queue<int> queue = new Queue<int>();

            queue.Enqueue(i);
            visited[i] = true;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                cluster.Add(current);

                for (int j = 0; j < n; j++)
                {
                    if (visited[j]) continue;

                    float dist = Vector2.Distance(particles[current].position, particles[j].position);

                    if (dist < clusterDistance)
                    {
                        visited[j] = true;
                        queue.Enqueue(j);
                    }
                }
            }

            clusters.Add(cluster);
        }

        List<List<int>> clusters_filtered = clusters.Where(c => c.Count >= minClusterSize).ToList();

        Debug.Log($"Found {clusters.Count} clusters");
        Debug.Log($"After filtering {clusters_filtered.Count} clusters");
        for (int c = 0; c < clusters_filtered.Count; c++)
        {
            Debug.Log($"Cluster {c} size = {clusters_filtered[c].Count}");
        }

        return clusters_filtered;
    }

}