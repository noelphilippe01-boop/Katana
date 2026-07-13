using System.Collections.Generic;
using Katana.Core;
using UnityEngine;

namespace Katana.Combat
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] int initialCount = 5;
        [SerializeField] float spawnRadius = 14f;
        [SerializeField] float respawnDelay = 4f;
        [SerializeField] int maxAlive = 8;
        [SerializeField] float minSpawnDistanceFromPlayer = 4f;

        readonly Queue<float> respawnQueue = new();
        Transform player;

        void Start()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;

            for (var i = 0; i < initialCount; i++)
                SpawnEnemy();
        }

        void OnEnable() => GameEventBus.EnemyKilled += OnEnemyKilled;
        void OnDisable() => GameEventBus.EnemyKilled -= OnEnemyKilled;

        void Update()
        {
            if (respawnQueue.Count == 0)
                return;

            if (Time.time < respawnQueue.Peek())
                return;

            if (CountAliveEnemies() >= maxAlive)
                return;

            respawnQueue.Dequeue();
            SpawnEnemy();
        }

        void OnEnemyKilled(GameObject enemy)
        {
            if (enemy == null)
                return;

            respawnQueue.Enqueue(Time.time + respawnDelay);
        }

        void SpawnEnemy()
        {
            var position = FindSpawnPosition();
            EnemyFactory.Create(position, $"Enemy_{CountAliveEnemies() + 1}");
        }

        Vector3 FindSpawnPosition()
        {
            for (var attempt = 0; attempt < 16; attempt++)
            {
                var angle = Random.Range(0f, Mathf.PI * 2f);
                var distance = Random.Range(minSpawnDistanceFromPlayer, spawnRadius);
                var position = new Vector3(Mathf.Cos(angle) * distance, 1f, Mathf.Sin(angle) * distance);

                if (player == null)
                    return position;

                var flatPlayer = new Vector3(player.position.x, 0f, player.position.z);
                var flatSpawn = new Vector3(position.x, 0f, position.z);
                if (Vector3.Distance(flatPlayer, flatSpawn) >= minSpawnDistanceFromPlayer)
                    return position;
            }

            return new Vector3(Random.Range(-spawnRadius, spawnRadius), 1f, Random.Range(-spawnRadius, spawnRadius));
        }

        static int CountAliveEnemies() => GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}
