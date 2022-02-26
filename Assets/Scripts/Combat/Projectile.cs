using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : NetworkBehaviour
    {
        [SerializeField]
        private float velocity;
        [SerializeField]
        private float lifetimeSeconds;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Start()
        {
            rb.velocity = transform.forward * velocity;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroyProjectile), lifetimeSeconds);
        }

        [Server]
        private void DestroyProjectile()
        {
            NetworkServer.Destroy(gameObject);
        }

    }
}