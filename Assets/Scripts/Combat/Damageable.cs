using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Damageable : NetworkBehaviour
    {
        [SerializeField]
        private float maxHealth;

        private float currentHealth;

        public Action OnServerDied;

        public override void OnStartServer()
        {
            currentHealth = maxHealth;
        }

        [Server]
        public void ApplyDamage(float damage)
        {
            if (currentHealth == 0) return;
            currentHealth = Mathf.Max(0, currentHealth - damage);
            if (currentHealth == 0)
            {
                OnServerDied?.Invoke();
                Debug.Log($"{gameObject.name} died");
            }
        }
    }
}