using Mirror;
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