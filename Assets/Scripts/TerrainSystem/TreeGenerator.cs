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
        [SerializeField] private Camera camera;

        private readonly Dictionary<Chunk, List<Vector2>> _chunkPoints = new();
        private Transform _treeContainer;
        private readonly Dictionary<string, Tree> _trees = new();
        private const float TreeGenerateTick = 0.1f;
        private Chunk _playerChunk = Chunk.Empty;

        public void Start()
        {
            GenerateTreeContainer();
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
            Chunk currentPlayerChunk = Chunk.FromPoint(player.position, chunkSize);
            if (currentPlayerChunk.Equals(_playerChunk)) return;
            _playerChunk = currentPlayerChunk;

            Vector2 spawn2D = new Vector2(spawnPoint.x, spawnPoint.z);
            float safeRadiusSquared = spawnSafeRadius * spawnSafeRadius;

            SpawnTrees(_playerChunk.X, _playerChunk.Z, safeRadiusSquared, spawn2D);
            RemoveTreesTooFar(_playerChunk.X, _playerChunk.Z);
        }

        private void SpawnTrees(
            int playerChunkX,
            int playerChunkZ,
            float safeRadiusSquared,
            Vector2 spawn2D
        )
        {
            for (int chunkX = playerChunkX - chunkSpawnRadius;
                 chunkX <= playerChunkX + chunkSpawnRadius;
                 chunkX++)
            {
                for (int chunkZ = playerChunkZ - chunkSpawnRadius;
                     chunkZ <= playerChunkZ + chunkSpawnRadius;
                     chunkZ++)
                {
                    List<Vector2> points = GetPointsForChunk(new Chunk(chunkX, chunkZ));

                    foreach (Vector2 localPoint in points)
                    {
                        SpawnTree(
                            localPoint,
                            chunkX,
                            chunkZ,
                            safeRadiusSquared,
                            spawn2D
                        );
                    }
                }
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

            float wx = playerChunkX * chunkSize + treePoint.x;
            float wz = playerChunkZ * chunkSize + treePoint.y;

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

            Tree tree = Instantiate(
                treePrefab,
                new Vector3(wx, 0f, wz),
                Quaternion.Euler(0, Random.Range(0, 360), 0),
                _treeContainer
            );

            tree.Initialize(player, camera);

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
        
        private List<Vector2> GetPointsForChunk(Chunk chunk)
        {
            if (_chunkPoints.TryGetValue(chunk, out var points))
                return points;

            points = PoissonDiskSampling.GeneratePoints(
                radius: 6f,
                regionSize: new Vector2(chunkSize, chunkSize),
                seed: $"{seed}_{chunk.X}_{chunk.Z}"
            );

            _chunkPoints.Add(chunk, points);
            return points;
        }

        private struct Chunk : IEquatable<Chunk>
        {
            public readonly int X;
            public readonly int Z;
            private bool _isEmpty;

            public Chunk(int x, int z)
            {
                X = x;
                Z = z;
                _isEmpty = false;
            }

            public static Chunk FromPoint(Vector3 point, int chunkSize)
            {
                Vector2 point2D = new Vector2(point.x, point.z);
                return FromPoint(point2D, chunkSize);
            }

            public static Chunk FromPoint(Vector2 point, int chunkSize)
            {
                int playerChunkX = Mathf.FloorToInt(point.x / chunkSize);
                int playerChunkZ = Mathf.FloorToInt(point.y / chunkSize);
                return new Chunk(playerChunkX, playerChunkZ);
            }

            public static readonly Chunk Empty = new()
            {
                _isEmpty = true,
            };

            public override string ToString()
            {
                return $"Chunk(x={X}, z={Z}, isEmpty={_isEmpty})";
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
                return HashCode.Combine(X, Z, _isEmpty);
            }

            public bool Equals(Chunk other)
            {
                return _isEmpty == other._isEmpty && X == other.X && Z == other.Z;
            }
        }
    }
}