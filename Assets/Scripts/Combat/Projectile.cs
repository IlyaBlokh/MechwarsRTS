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
        [SerializeField]
        private float damageToDeal;

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

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out NetworkIdentity networkIdentity)) 
                return;

            if (networkIdentity.connectionToClient == connectionToClient)
                return;

            if (!other.TryGetComponent(out Damageable damageable))
                return;

            damageable.ApplyDamage(damageToDeal);
            DestroyProjectile();
        }

        [Server]
        private void DestroyProjectile()
        {
            NetworkServer.Destroy(gameObject);
        }

    }
}