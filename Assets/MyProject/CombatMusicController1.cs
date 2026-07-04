using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;

public class CombatMusicController1 : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private GameObject[] enemies;

    private List<GameObject> aggroedEnemies =
        new List<GameObject>();

    private bool battleMusicIsPlaying;

    private void Update()
    {
        RememberAggroedEnemies();
        RemoveDeadEnemies();

        if (aggroedEnemies.Count > 0 && !battleMusicIsPlaying)
        {
            musicSource.clip = battleMusic;
            musicSource.Play();
            battleMusicIsPlaying = true;
        }
        else if (aggroedEnemies.Count == 0 && battleMusicIsPlaying)
        {
            musicSource.clip = gameplayMusic;
            musicSource.Play();
            battleMusicIsPlaying = false;
        }
    }

    private void RememberAggroedEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null &&
                IsAggroed(enemy) &&
                !aggroedEnemies.Contains(enemy))
            {
                aggroedEnemies.Add(enemy);
            }
        }
    }

    private bool IsAggroed(GameObject enemy)
    {
        ChomperBehavior chomper =
            enemy.GetComponentInChildren<ChomperBehavior>();

        if (chomper != null)
            return chomper.target != null;

        SpitterBehaviour spitter =
            enemy.GetComponentInChildren<SpitterBehaviour>();

        if (spitter != null)
            return spitter.target != null;

        GrenadierBehaviour grenadier =
            enemy.GetComponentInChildren<GrenadierBehaviour>();

        if (grenadier != null)
            return grenadier.target != null;

        return false;
    }

    private void RemoveDeadEnemies()
    {
        for (int i = aggroedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = aggroedEnemies[i];

            if (enemy == null)
            {
                aggroedEnemies.RemoveAt(i);
                continue;
            }

            Damageable health =
                enemy.GetComponentInChildren<Damageable>();

            if (health == null || health.currentHitPoints <= 0)
                aggroedEnemies.RemoveAt(i);
        }
    }
}