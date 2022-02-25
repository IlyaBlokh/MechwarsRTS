using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(Collider))]
    public class Targetable : NetworkBehaviour
    {
        private Collider collider;

        private void Awake()
        {
            collider = GetComponent<Collider>();
        }
        public Vector3 GetAimPoint()
        {
            return collider.bounds.center;
        }
    }
}