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

        [SyncVar(hook = nameof(HandleHealthUpdate))]
        private float currentHealth;

        public event Action OnServerDied;
        public event Action<float, float> OnClientHealthUpdated;

        #region Server
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
        #endregion
        #region Client
        private void HandleHealthUpdate(float oldValue, float newValue)
        {
            OnClientHealthUpdated?.Invoke(newValue, maxHealth);
        }
        #endregion
    }
}