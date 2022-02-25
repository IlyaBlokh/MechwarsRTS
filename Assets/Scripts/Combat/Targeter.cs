using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable target;

        public Targetable Target { get => target; }

        #region Server
        [Command]
        public void CmdSetTarget(Targetable target)
        {
            this.target = target;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }
        #endregion
    }
}