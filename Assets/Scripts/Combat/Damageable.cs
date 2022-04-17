using Buildings;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    public class Damageable : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private float maxHealth;

        [SyncVar(hook = nameof(HandleHealthUpdate))]
        private float currentHealth;

        public event Action OnServerDestruct;
        public event Action<float, float> OnClientHealthUpdated;
        public event Action OnClientFocused;
        public event Action OnClientUnfocused;

        #region Server
        public override void OnStartServer()
        {
            currentHealth = maxHealth;
            UnitBase.OnServerPlayerLost += ServerHandlePlayerLost;
        }

        public override void OnStopServer()
        {
            UnitBase.OnServerPlayerLost -= ServerHandlePlayerLost;
        }

        [Server]
        private void ServerHandlePlayerLost(int playerID)
        {
            if (connectionToClient.connectionId != playerID) return;
            ApplyDamage(currentHealth);            
        }

        [Server]
        public void ApplyDamage(float damage)
        {
            if (currentHealth == 0) return;
            currentHealth = Mathf.Max(0, currentHealth - damage);
            if (currentHealth == 0)
            {
                OnServerDestruct?.Invoke();
                NetworkServer.Destroy(gameObject);
            }
        }
        #endregion
        #region Client
        private void HandleHealthUpdate(float oldValue, float newValue)
        {
            OnClientHealthUpdated?.Invoke(newValue, maxHealth);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnClientFocused?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnClientUnfocused?.Invoke();
        }
        #endregion
    }
}