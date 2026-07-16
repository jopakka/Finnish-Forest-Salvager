using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace TerrainSystem
{
    public class TreeGenerator : MonoBehaviour
    {
        [SerializeField] private string seed = "hello world";
        [SerializeField] private float spawnSafeRadius = 20f;
        [SerializeField] private Vector3 spawnPoint;
        [SerializeField] private Tree treePrefab;
        [SerializeField] private Transform player;
        [SerializeField] private int chunkSize = 50;
        [SerializeField] private int chunkSpawnRadius = 1;

        private Terrain _terrain;
        private List<Vector2> _treePoints;
        private Transform _treeContainer;
        private readonly Dictionary<string, Tree> _trees = new();
        private const float TreeGenerateTick = 0.1f;
        private Chunk _playerChunk = Chunk.Empty;

        private void Start()
        {
            _terrain = GetComponent<Terrain>();

            GenerateTreeContainer();
            GeneratePoints();
            HandleTrees();
            StartCoroutine(RegenerateTrees());
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

        private IEnumerator RegenerateTrees()
        {
            while (true)
            {
                yield return new WaitForSeconds(TreeGenerateTick);
                HandleTrees();
            }
        }

        private void HandleTrees()
        {
            int playerChunkX = Mathf.FloorToInt(player.position.x / chunkSize);
            int playerChunkZ = Mathf.FloorToInt(player.position.z / chunkSize);
            Chunk playerChunk = new Chunk(playerChunkX, playerChunkZ);
            if (playerChunk.Equals(_playerChunk)) return;
            _playerChunk = playerChunk;

            Vector2 spawn2D = new Vector2(spawnPoint.x, spawnPoint.z);
            float safeRadiusSquared = spawnSafeRadius * spawnSafeRadius;

            SpawnTrees(playerChunkX, playerChunkZ, safeRadiusSquared, spawn2D);
            RemoveTreesTooFar(playerChunkX, playerChunkZ);
        }

        private void SpawnTrees(
            int playerChunkX,
            int playerChunkZ,
            float safeRadiusSquared,
            Vector2 spawn2D
        )
        {
            foreach (Vector2 p in _treePoints)
            {
                SpawnTree(p, playerChunkX, playerChunkZ, safeRadiusSquared, spawn2D);
            }
        }

        private void SpawnTree(
            Vector2 treePoint,
            int playerChunkX,
            int playerChunkZ,
            float safeRadiusSquared,
            Vector2 spawn2D
        )
        {
            string id = $"{treePoint.x}_{treePoint.y}";
            if (_trees.ContainsKey(id)) return;

            float wx = treePoint.x + transform.position.x;
            float wz = treePoint.y + transform.position.z;

            // Spawn trees on chunk where player is
            int treeChunkX = Mathf.FloorToInt(wx / chunkSize);
            int treeChunkZ = Mathf.FloorToInt(wz / chunkSize);

            if (Mathf.Abs(treeChunkX - playerChunkX) > chunkSpawnRadius ||
                Mathf.Abs(treeChunkZ - playerChunkZ) > chunkSpawnRadius)
            {
                return;
            }

            // Ignore spawn
            Vector2 diff = new Vector2(wx, wz) - spawn2D;
            if (diff.sqrMagnitude <= safeRadiusSquared) return;

            float y = _terrain.SampleHeight(new Vector3(wx, 0, wz));

            Tree tree = Instantiate(
                treePrefab,
                new Vector3(wx, y, wz),
                Quaternion.Euler(0, Random.Range(0, 360), 0),
                _treeContainer
            );

            _trees.Add(id, tree);
        }

        private void RemoveTreesTooFar(
            int playerChunkX,
            int playerChunkZ
        )
        {
            List<string> treesToRemove = new();

            foreach (var pair in _trees)
            {
                Tree tree = pair.Value;

                int chunkX = Mathf.FloorToInt(tree.transform.position.x / chunkSize);
                int chunkZ = Mathf.FloorToInt(tree.transform.position.z / chunkSize);

                if (Mathf.Abs(chunkX - playerChunkX) > chunkSpawnRadius ||
                    Mathf.Abs(chunkZ - playerChunkZ) > chunkSpawnRadius)
                {
                    Destroy(tree.gameObject);
                    treesToRemove.Add(pair.Key);
                }
            }

            foreach (string key in treesToRemove)
            {
                _trees.Remove(key);
            }
        }
        
        private struct Chunk : IEquatable<Chunk>
        {
            private int _x;
            private int _z;
            private bool _isEmpty;
            
            public Chunk(int x, int z)
            {
                _x = x;
                _z = z;
                _isEmpty = false;
            }
            
            public static readonly Chunk Empty = new()
            {
                _isEmpty = true,
            };

            public override string ToString()
            {
                return $"Chunk(x={_x}, z={_z}, isEmpty={_isEmpty})";
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                
                Chunk other = (Chunk)obj;
                return Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_x, _z, _isEmpty);
            }

            public bool Equals(Chunk other)
            {
                return _isEmpty == other._isEmpty && _x == other._x && _z == other._z;
            }
        }
    }
}