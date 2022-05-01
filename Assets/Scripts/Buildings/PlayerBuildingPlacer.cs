using Config;
using Mirror;
using Resources;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class PlayerBuildingPlacer : NetworkBehaviour
    {
        [SerializeField] BuildingsConfig buildingsConfig;
        [SerializeField] PlayerResources playerResources;

        private List<Building> buildings = new List<Building>();
        public List<Building> Buildings { get => buildings; }

        public bool IsPlacingAllowed(BoxCollider buildingCollider, Vector3 locationPoint)
        {
            //Check for overlapping
            if (Physics.CheckBox(locationPoint,
                buildingCollider.size / 2,
                buildingCollider.transform.rotation,
                buildingsConfig.BuildingLockedLayers))
            {
                return false;
            }

            //Check for range
            foreach (Building building in buildings)
            {
                if ((building.transform.position - locationPoint).sqrMagnitude
                    < buildingsConfig.BuildRange * buildingsConfig.BuildRange)
                {
                    return true;
                }
            }
            return false;
        }

        #region Server
        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 location)
        {
            var buildingToPlace = buildingsConfig.Buildings.Find(b => b.Id == buildingId);
            if (buildingToPlace == null) return;

            BoxCollider buildingCollider;
            if (!buildingToPlace.TryGetComponent(out buildingCollider))
            {
                Debug.LogError($"{buildingToPlace} missing box collider");
                return;
            }

            if (IsPlacingAllowed(buildingCollider, location))
            {
                //Check for resources
                 if (!playerResources.TrySubstractCredits(buildingToPlace.Price)) { return; }

                var buildingInstance = Instantiate(buildingToPlace.gameObject, location, buildingToPlace.transform.rotation);
                NetworkServer.Spawn(buildingInstance, connectionToClient);
            }
        }
        #endregion
    }
}