using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace TerrainSystem
{
    public class TreeGenerator : MonoBehaviour
    {
        [SerializeField] private int treeCount = 500;
        [SerializeField] private string seed = "hello world";
        [SerializeField] private float spawnSafeRadius = 20f;
        [SerializeField] private Vector3 spawnPoint;

        private Terrain _terrain;

        private void Start()
        {
            _terrain = GetComponent<Terrain>();

            List<TreeInstance> treeInstances = new List<TreeInstance>();
            TerrainData terrainData = _terrain.terrainData;
            Vector2 spawn2D = new Vector2(spawnPoint.x, spawnPoint.z);

            List<Vector2> points =
                PoissonDiskSampling.GeneratePoints(
                    radius: 6f,
                    regionSize: new Vector2(
                        terrainData.size.x,
                        terrainData.size.z
                    ),
                    seed: seed
                );
            
            Debug.Log($"terrainData.size: {terrainData.size}");

            foreach (Vector2 p in points)
            {
                Vector2 worldP = new Vector2(p.x + transform.position.x, p.y + transform.position.z);
                
                if (Vector2.Distance(worldP, spawn2D) <= spawnSafeRadius) continue;
                
                float nx = p.x / terrainData.size.x;
                float nz = p.y / terrainData.size.z;

                TreeInstance treeInstance = new TreeInstance
                {
                    position = new Vector3(
                        nx,
                        terrainData.GetInterpolatedHeight(nx, nz) / terrainData.size.y,
                        nz),
                    prototypeIndex = Random.Range(0, terrainData.treePrototypes.Length),
                    widthScale = Random.Range(0.8f, 1.2f),
                    heightScale = Random.Range(0.8f, 1.2f),
                    color = Color.white,
                    lightmapColor = Color.white,
                };

                treeInstances.Add(treeInstance);
            }

            terrainData.treeInstances = treeInstances.ToArray();
        }
    }
}