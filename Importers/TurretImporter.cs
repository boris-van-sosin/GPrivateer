using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullBroadside.Importers
{
    public static class TurretImporter
    {
        public static List<Node3D> SpawnMonitorForeGun(Node3D monitorRoot, Transform3D tr)
        {
            string turretDefPath = ProjectSettings.GlobalizePath("res://LoadedAssets/TextAssets/Turrets/SmallFixedSmallHowitzer.yml");
            TurretDefinition turretDef = HierarchySerializer.LoadHierarchy<TurretDefinition>(new StreamReader(turretDefPath));
            List<(Node3D, Transform3D)> res = HierarchyConstructionUtil.ConstructHierarchy(turretDef.Geometry, null, 1, 2, 3);
            List<Node3D> resNodes = new List<Node3D>(res.Count);
            for (int i = 0; i < res.Count; ++i)
            {
                monitorRoot.AddChild(res[i].Item1);
                res[i].Item1.Transform = tr * res[i].Item2;
                resNodes.Add(res[i].Item1);
            }
            return resNodes;
        }
    }
}
