using Mirror;
using Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        [SerializeField]
        private List<Gun> guns = new List<Gun>();
        [SerializeField]
        private float rotationSpeed = 20.0f;

        private Targetable target;
        private Quaternion targetRotation;

        public Targetable Target { get => target; }

        #region Server

        public override void OnStartServer()
        {
            GameLoopController.OnServerGameOver += HandleServerGameOver;
        }

        public override void OnStopServer()
        {
            GameLoopController.OnServerGameOver -= HandleServerGameOver;
        }

        [Server]
        private void HandleServerGameOver()
        {
            ClearTarget();
        }

        [ServerCallback]
        private void Update()
        {
            if (target)
            {
                targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        [Command]
        public void CmdSetTarget(Targetable target)
        {
            this.target = target;
            guns.ForEach(gun => gun.Target = target);
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
            guns.ForEach(gun => gun.Target = null);
        }
        #endregion
    }
}