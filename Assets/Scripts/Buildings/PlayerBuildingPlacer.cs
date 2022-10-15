using System;
using Mirror;
using System.Collections.Generic;
using Data.Config;
using GameResources;
using UnityEngine;
using UnityEngine.AI;

namespace Buildings
{
    public class PlayerBuildingPlacer : NetworkBehaviour
    {
        [SerializeField] BuildingsConfig buildingsConfig;
        [SerializeField] PlayerResources playerResources;

        private List<Building> buildings = new();
        public List<Building> Buildings => buildings;
        
        public bool IsPlacingAllowed(BoxCollider buildingCollider, Vector3 locationPoint) =>
            !OverlapsWithLayers(buildingCollider, locationPoint) 
            && IsNearToOwnBuildings(locationPoint);

        private bool IsNearToOwnBuildings(Vector3 locationPoint)
        {
            foreach (Building building in buildings)
                if ((building.transform.position - locationPoint).sqrMagnitude 
                    < buildingsConfig.BuildRange * buildingsConfig.BuildRange) 
                    return true;
            return false;
        }

        private bool OverlapsWithLayers(BoxCollider buildingCollider, Vector3 locationPoint)
        {
            return Physics.CheckBox(locationPoint,
                buildingCollider.size / 2,
                buildingCollider.transform.rotation,
                buildingsConfig.BuildingLockedLayers);
        }

        #region Server
        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 location)
        {
            Building buildingToPlace = buildingsConfig.Buildings.Find(b => b.Id == buildingId);
            if (buildingToPlace == null) return;

            if (!buildingToPlace.TryGetComponent(out BoxCollider buildingCollider))
            {
                Debug.LogError($"{buildingToPlace} missing box collider");
                return;
            }

            if (IsPlacingAllowed(buildingCollider, location))
            {
                //Check for resources
                 if (!playerResources.TrySubstractCredits(buildingToPlace.Price)) { return; }

                GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, location, buildingToPlace.transform.rotation);
                NetworkServer.Spawn(buildingInstance, connectionToClient);
            }
        }
        #endregion
    }
}