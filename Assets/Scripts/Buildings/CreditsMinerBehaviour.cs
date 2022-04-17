using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class CreditsMinerBehaviour : NetworkBehaviour
    {
        [SerializeField] private int incomePerInterval;
        [SerializeField] private float intervalInSec;

        private float timer = .0f;
        private RTSPlayer ownerPlayer = null;

        public override void OnStartServer()
        {
            timer = intervalInSec;
            ownerPlayer = connectionToClient.identity.GetComponent<RTSPlayer>();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer += intervalInSec;
                if (ownerPlayer == null)
                {
                    Debug.LogError("Can't retrieve RTSPlayer");
                }
                else
                {
                    ownerPlayer.PlayerResources.AddCredits(incomePerInterval);
                }
            }
        }

        private void InitNetworkClient()
        {
            Debug.Log("init network client");
            NetworkClient.connection.identity.TryGetComponent(out ownerPlayer);
        }
    }
}