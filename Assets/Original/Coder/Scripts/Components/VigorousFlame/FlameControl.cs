using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class FlameControl : MonoBehaviour
{
    [SerializeField] List<GameObject> Flames;

    int maxHealth;
    [SerializeField] int currentHealth;

    void Awake()
    {
        maxHealth = Flames.Count-1;
        currentHealth = maxHealth;
    }
    void Start()
    {
        for (int i = 0; i != maxHealth; i++) {
            Flames[i].SetActive(false);
        }
        Player.Instance.Damagable.OnDamage
            .Subscribe(delta => SetHealth(currentHealth - delta))
            .AddTo(this);

    }

    public void SetHealth(int newHealth)
    {
        int h = Math.Clamp(newHealth, 0, maxHealth);
        if (currentHealth != h){
            Flames[currentHealth].SetActive(false);
            Flames[h].SetActive(true);
            currentHealth = h;
        }
    }
}
