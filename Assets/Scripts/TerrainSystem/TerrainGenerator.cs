using UnityEngine;

namespace TerrainSystem
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private string seed = "hello world";

        [SerializeField] private float heightScale = 0.08f;

        [SerializeField] private float continentScale = 0.0015f;
        [SerializeField] private float hillScale = 0.005f;
        [SerializeField] private float detailScale = 0.02f;

        [SerializeField] private float continentWeight = 0.75f;
        [SerializeField] private float hillWeight = 0.20f;
        [SerializeField] private float detailWeight = 0.05f;

        [SerializeField] private float flatRadius = 40f; // Completely flat
        [SerializeField] private float blendRadius = 70f; // Smooth transition
        [SerializeField] private float spawnHeight = 0.02f;

        private Terrain _terrain;

        private void Start()
        {
            _terrain = GetComponent<Terrain>();
            GenerateTerrain();
        }

        private void GenerateTerrain()
        {
            int seedHash = seed.GetHashCode();
            System.Random random = new System.Random(seedHash);
            float offsetX = random.Next(-100000, 100000);
            float offsetZ = random.Next(-100000, 100000);

            int resolution = _terrain.terrainData.heightmapResolution;
            int center = resolution / 2;

            float[,] heights = new float[resolution, resolution];

            float minHeight = float.MaxValue;

            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float height = GenerateHeight(x + offsetX, z + offsetZ, offsetX, offsetZ, center);
                    heights[z, x] = Mathf.Clamp01(height * heightScale);

                    if (height < minHeight)
                    {
                        minHeight = height;
                    }
                }
            }

            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    heights[z, x] -= minHeight;
                }
            }

            _terrain.terrainData.SetHeights(0, 0, heights);
        }

        private float GenerateHeight(float x, float z, float offsetX, float offsetZ, float center)
        {
            float wx = x + offsetX;
            float wz = z + offsetZ;

            // Warp the coordinates slightly to break up obvious Perlin patterns                
            float warpX = Mathf.PerlinNoise(wx * 0.0008f, wz * 0.0008f) * 100f;
            float warpZ = Mathf.PerlinNoise((wx + 5000) * 0.0008f, (wz + 5000) * 0.0008f) * 100f;

            wx += warpX;
            wz += warpZ;

            float continent = Fbm(wx * continentScale, wz * continentScale, 4, 0.5f, 2f);
            float hills = Fbm(wx * hillScale, wz * hillScale, 3, 0.5f, 2f);
            float detail = Fbm(wx * detailScale, wz * detailScale, 2, 0.5f, 2f);

            // Shape continents so most land is flat with occasional mountains
            continent = Mathf.Pow(continent, 2.5f);

            float height =
                continent * 0.70f +
                hills * 0.25f +
                detail * 0.05f;

            height *= heightScale;

            // Flat spawn area
            float dx = x - center - offsetX;
            float dz = z - center - offsetZ;
            float distance = Mathf.Sqrt(dx * dx + dz * dz);

            if (distance < blendRadius)
            {
                float t = Mathf.InverseLerp(flatRadius, blendRadius, distance);

                // Smooth interpolation
                t = t * t * (3f - 2f * t);

                float meadowNoise = Mathf.PerlinNoise(wx * 0.03f, wz * 0.03f) * 0.003f;
                float flatHeight = spawnHeight + meadowNoise;

                height = Mathf.Lerp(flatHeight, height, t);
            }

            return height;
        }

        private static float Fbm(float x, float y, int octaves, float persistence, float lacunarity)
        {
            float value = 0f;
            float amplitude = 1f;
            float frequency = 1f;
            float maxValue = 0f;

            for (int i = 0; i < octaves; i++)
            {
                value += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;

                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return value / maxValue;
        }
    }
}