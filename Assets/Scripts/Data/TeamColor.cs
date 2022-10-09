using System;
using UnityEngine;

namespace Data
{
  [Serializable]
  public class TeamColor
  {
    public ColorId ColorId;
    public Material GameplayMaterial;
    public Material MinimapMaterial;
  }
}