using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace FullBroadside
{
    public enum WeaponBehaviorType { Unknown, Gun, Beam, ContinuousBeam, Torpedo, BomberTorpedo, Special };

    [Serializable]
    public class TurretDefinition
    {
        public HierarchyNode Geometry { get; set; }
        public string TurretType { get; set; }
        public string WeaponNum { get; set; }
        public string WeaponSize { get; set; }
        public string WeaponType { get; set; }
        public WeaponBehaviorType BehaviorType { get; set; }

        /*
        public static TurretDefinition FromTurret(TurretBase t)
        {
            return FromTurret(t, "", "", "", "");
        }

        public static TurretDefinition FromTurret(TurretBase t, string meshABPath, string meshAssetPath, string partSysABPath, string partSysAssetPath)
        {
            ObjectFactory.WeaponBehaviorType behaviorType = t.BehaviorType;

            return new TurretDefinition()
            {
                Geometry = t.transform.ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath),
                TurretType = t.TurretType,
                WeaponNum = "1",
                WeaponSize = t.TurretWeaponSize,
                WeaponType = t.TurretWeaponType,
                BehaviorType = behaviorType
            };
        }
        */
    }
}
