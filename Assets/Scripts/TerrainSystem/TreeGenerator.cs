using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace TerrainSystem
{
    public class TreeGenerator : MonoBehaviour
    {
        [SerializeField] private string seed = "hello world";
        [SerializeField] private float spawnSafeRadius = 20f;
        [SerializeField] private Vector3 spawnPoint;
        [SerializeField] private Tree treePrefab;

        private Terrain _terrain;
        private List<Vector2> _treePoints;
        private Transform _treeContainer;

        private void Start()
        {
            _terrain = GetComponent<Terrain>();

            GenerateTreeContainer();
            GeneratePoints();
            SpawnTrees();
        }

        private void GenerateTreeContainer()
        {
            if (_treeContainer != null)
            {
                Destroy(_treeContainer);
            }
            GameObject treeContainer = new GameObject("TreeContainer");
            treeContainer.transform.SetParent(transform);
            _treeContainer = treeContainer.transform;
        }
        
        private void GeneratePoints()
        {
            TerrainData terrainData = _terrain.terrainData;

            _treePoints = PoissonDiskSampling.GeneratePoints(
                radius: 6f,
                regionSize: new Vector2(
                    terrainData.size.x,
                    terrainData.size.z
                ),
                seed: seed
            );
        }

        private void SpawnTrees()
        {
            Vector2 spawn2D = new Vector2(spawnPoint.x, spawnPoint.z);

            float safeRadiusSquared = spawnSafeRadius * spawnSafeRadius;

            foreach (Vector2 p in _treePoints)
            {
                float wx = p.x + transform.position.x;
                float wz = p.y + transform.position.z;
                Vector2 diff = new Vector2(wx, wz) - spawn2D;

                if (diff.sqrMagnitude <= safeRadiusSquared) continue;
                
                float y = _terrain.SampleHeight(new Vector3(wx, 0, wz));

                Instantiate(
                    treePrefab,
                    new Vector3(wx, y, wz),
                    Quaternion.Euler(0, Random.Range(0, 360), 0),
                    _treeContainer
                );
            }
        }
    }
}