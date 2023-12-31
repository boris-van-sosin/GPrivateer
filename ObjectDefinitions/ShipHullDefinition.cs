﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullBroadside
{
    public enum ShipSize { Sloop = 0, Frigate = 1, Destroyer = 2, Cruiser = 3, CapitalShip = 4 };

    [Serializable]
    public class ShipHullDefinition
    {
        public string HullName { get; set; }
        public string ShipSize { get; set; }
        public string ShipType { get; set; }
        public HierarchyNode Geometry { get; set; }
        public MeshData CollisionMesh { get; set; }
        public ShipHullMovementData MovementData { get; set; }
        public HierarchyNode TeamColorComponents { get; set; }
        public ComponentSlotsData ComponentSlots { get; set; }
        public WeaponHardpointDefinition[] WeaponHardpoints { get; set; }
        public ShipHullProtectionData HullProtection { get; set; }
        public float Mass { get; set; }
        public int SkeletonCrew { get; set; }
        public int OperationalCrew { get; set; }
        public int MaxCrew { get; set; }
        public int MaxSpecialCharacters { get; set; }
        public HierarchyNode Shield { get; set; }
        public HierarchyNode MagneticField { get; set; }
        public HierarchyNode DamageSmoke { get; set; }
        public HierarchyNode EngineExhaustOn { get; set; }
        public HierarchyNode EngineExhaustIdle { get; set; }
        public HierarchyNode EngineExhaustBrake { get; set; }
        public HierarchyNode StatusRing { get; set; }
        public ShipCarrierModuleData CarrierModuleData { get; set; }

        /*
        public static ShipHullDefinition FromShip(Ship s)
        {
            return FromShip(s, "", "", "", "");
        }


        public static ShipHullDefinition FromShip(Ship s, string meshABPath, string meshAssetPath, string partSysABPath, string partSysAssetPath)
        {
            MeshCollider coll = s.GetComponent<MeshCollider>();
            MeshData collisionMeshData = new MeshData()
            {
                AssetBundlePath = meshABPath,
                AssetPath = "",
                MeshPath = coll.sharedMesh.name,
                DoCombine = false
            };

            ShipHullDefinition res = new ShipHullDefinition()
            {
                HullName = s.name,
                ShipSize = s.ShipSize.ToString(),
                Geometry = s.HullObject.ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath),
                CollisionMesh = collisionMeshData,
                MovementData = ShipHullMovementData.FromShip(s),
                TeamColorComponents = s.TeamColorComponents.Select(tc => tc.transform.ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath)).ToArray(),
                ComponentSlots = ComponentSlotsData.FromShip(s),
                WeaponHardpoints = s.GetComponentsInChildren<TurretHardpoint>().Select(hp => WeaponHardpointDefinition.FromHardpoint(hp)).ToArray(),
                HullProtection = ShipHullProtectionData.FromShip(s),
                Mass = s.Mass,
                SkeletonCrew = s.SkeletonCrew,
                OperationalCrew = s.OperationalCrew,
                MaxCrew = s.MaxCrew,
                MaxSpecialCharacters = s.MaxSpecialCharacters,
                Shield = s.transform.Find("Shield").ToSerializableHierarchy(),
                MagneticField = s.transform.Find("MagneticField").ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath),
                DamageSmoke = s.transform.Find("Damage smoke effect").ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath),
                EngineExhaustOn = s.transform.Find("Engine exhaust on").ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath),
                EngineExhaustIdle = s.transform.Find("Engine exhaust idle").ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath),
                EngineExhaustBrake = s.transform.Find("Engine exhaust brake").ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath),
                StatusRing = s.GetComponentInChildren<LineRenderer>().transform.ToSerializableHierarchy()
            };

            FixEngineExhausts(res.EngineExhaustIdle);
            FixEngineExhausts(res.EngineExhaustOn);
            FixEngineExhausts(res.EngineExhaustBrake);

            CarrierBehavior carrier;
            if ((carrier = s.GetComponent<CarrierBehavior>()) != null)
            {
                res.CarrierModuleData = ShipCarrierModuleData.FromCarrierModule(carrier, meshABPath, meshAssetPath, partSysABPath, partSysAssetPath);
            }

            res.Shield.NodeMesh = null;

            return res;
        }
        */

        private static void FixEngineExhausts(HierarchyNode node)
        {
            if (node.NodeParticleSystem != null)
            {
                node.SubNodes = new HierarchyNode[0];
            }
            else
            {
                foreach (HierarchyNode subNode in node.SubNodes)
                {
                    FixEngineExhausts(subNode);
                }
            }
        }
    }

    [Serializable]
    public class TurretHardpointDefinition
    {
        public string DisplayString { get; set; }
        public float MinRotation { get; set; }
        public float MaxRotation { get; set; }
        public string[] DeadZoneAngles { get; set; }
        public bool TreatAsFixed { get; set; }
        public string[] AllowedWeaponTypes { get; set; }
        public ShipSection LocationOnShip { get; set; }
        public TurretAIHint WeaponAIHint { get; set; }
        public int DefaultGroup { get; set; }

        /*
        public static TurretHardpointDefinition FromHardpoint(TurretHardpoint hp)
        {
            TurretHardpointDefinition res = new TurretHardpointDefinition()
            {
                DisplayString = hp.DisplayString,
                MinRotation = hp.MinRotation,
                MaxRotation = hp.MaxRotation,
                DeadZoneAngles = new string[hp.DeadZoneAngles.Length],
                TreatAsFixed = hp.TreatAsFixed,
                AllowedWeaponTypes = new string[hp.AllowedWeaponTypes.Length],
                LocationOnShip = hp.LocationOnShip,
                WeaponAIHint = hp.WeaponAIHint,
                DefaultGroup = hp.DefaultGroup
            };
            hp.DeadZoneAngles.CopyTo(res.DeadZoneAngles, 0);
            hp.AllowedWeaponTypes.CopyTo(res.AllowedWeaponTypes, 0);
            return res;
        }

        public void SetHardpointFields(TurretHardpoint hp)
        {
            hp.DisplayString = DisplayString;
            hp.MinRotation = MinRotation;
            hp.MaxRotation = MaxRotation;
            hp.DeadZoneAngles = new string[DeadZoneAngles.Length];
            hp.TreatAsFixed = TreatAsFixed;
            hp.AllowedWeaponTypes = new string[AllowedWeaponTypes.Length];
            hp.LocationOnShip = LocationOnShip;
            hp.WeaponAIHint = WeaponAIHint;
            hp.DefaultGroup = DefaultGroup;

            DeadZoneAngles.CopyTo(hp.DeadZoneAngles, 0);
            AllowedWeaponTypes.CopyTo(hp.AllowedWeaponTypes, 0);
        }
        */
    }

    [Serializable]
    public class TorpedoHardpointDefinition
    {
        public TurretHardpointDefinition HardpointBaseDefinition { get; set; }
        public SerializableVector3 LaunchVector { get; set; }

        /*
        public static TorpedoHardpointDefinition FromHardpoint(TorpedoHardpoint hp)
        {
            TurretHardpointDefinition hardpointBaseDef = new TurretHardpointDefinition()
            {
                DisplayString = hp.DisplayString,
                MinRotation = hp.MinRotation,
                MaxRotation = hp.MaxRotation,
                DeadZoneAngles = new string[hp.DeadZoneAngles.Length],
                TreatAsFixed = hp.TreatAsFixed,
                AllowedWeaponTypes = new string[hp.AllowedWeaponTypes.Length],
                LocationOnShip = hp.LocationOnShip,
                WeaponAIHint = hp.WeaponAIHint,
                DefaultGroup = hp.DefaultGroup,

            };
            hp.DeadZoneAngles.CopyTo(hardpointBaseDef.DeadZoneAngles, 0);
            hp.AllowedWeaponTypes.CopyTo(hardpointBaseDef.AllowedWeaponTypes, 0);
            return new TorpedoHardpointDefinition()
            {
                HardpointBaseDefinition = hardpointBaseDef,
                LaunchVector = SerializableVector3.FromSerializable(hp.LaunchVector)
            };
        }

        public void SetHardpointFields(TorpedoHardpoint hp)
        {
            HardpointBaseDefinition.SetHardpointFields(hp);
            hp.LaunchVector = LaunchVector.ToVector3();
        }
        */
    }

    [Serializable]
    public class WeaponHardpointDefinition
    {
        public HierarchyNode HardpointNode { get; set; }
        public TurretHardpointDefinition TurretHardpoint { get; set; }
        public TorpedoHardpointDefinition TorpedoHardpoint { get; set; }

        /*
        public static WeaponHardpointDefinition FromHardpoint(TurretHardpoint hp)
        {
            WeaponHardpointDefinition res = new WeaponHardpointDefinition();
            res.HardpointNode = hp.transform.ToSerializableHierarchy();
            TorpedoHardpoint torpHardponit;
            if ((torpHardponit = hp.GetComponent<TorpedoHardpoint>()) != null)
            {
                res.TorpedoHardpoint = TorpedoHardpointDefinition.FromHardpoint(torpHardponit);
                res.TurretHardpoint = null;
            }
            else
            {
                res.TorpedoHardpoint = null;
                res.TurretHardpoint = TurretHardpointDefinition.FromHardpoint(hp);
            }
            return res;
        }
        */
    }

    [Serializable]
    public struct ShipHullMovementData
    {
        public float BaseMaxSpeed { get; set; }
        public float BaseThrust { get; set; }
        public float BaseBraking { get; set; }
        public float BaseTurnRate { get; set; }

        /*
        public static ShipHullMovementData FromShip(ShipBase s)
        {
            return new ShipHullMovementData()
            {
                MaxSpeed = s.MaxSpeed,
                Thrust = s.Thrust,
                Braking = s.Braking,
                TurnRate = s.TurnRate
            };
        }

        public void FillShipData(ShipBase s)
        {
            s.MaxSpeed = MaxSpeed;
            s.Thrust = Thrust;
            s.Braking = Braking;
            s.TurnRate = TurnRate;
        }
        */
    }

    [Serializable]
    public struct ShipHullFourSidesValues
    {
        public int Fore;
        public int Aft;
        public int Left;
        public int Right;

        public int ForeValue { get { return Fore; } set { Fore = value; } }
        public int AftValue { get { return Aft; } set { Aft = value; } }
        public int LeftValue { get { return Left; } set { Left = value; } }
        public int RightValue { get { return Right; } set { Right = value; } }

        public Dictionary<ShipSection, int> ToDict()
        {
            Dictionary<ShipSection, int> res = new Dictionary<ShipSection, int>();
            res[ShipSection.Fore] = Fore;
            res[ShipSection.Aft] = Aft;
            res[ShipSection.Left] = Left;
            res[ShipSection.Right] = Right;
            return res;
        }
    }

    [Serializable]
    public struct ShipHullProtectionData
    {
        public int HullHitpoints { get; set; }
        public ShipHullFourSidesValues Armour { get; set; }
        public ShipHullFourSidesValues ReducedArmour { get; set; }
        public ShipHullFourSidesValues MitigationArmour { get; set; }
        public float ArmourMitigation { get; set; }

        /*
        public static ShipHullProtectionData FromShip(Ship s)
        {
            return new ShipHullProtectionData()
            {
                HullHitpoints = s.MaxHullHitPoints,
                Armour = s.Armour,
                ReducedArmour = s.ReducedArmour,
                MitigationArmour = s.DefaultMitigationArmour,
                ArmourMitigation = s.ArmourMitigation
            };
        }

        public void FillShipData(Ship s)
        {
            s.MaxHullHitPoints = HullHitpoints;
            s.Armour = Armour;
            s.ReducedArmour = ReducedArmour;
            s.DefaultMitigationArmour = MitigationArmour;
            s.ArmourMitigation = ArmourMitigation;
        }
        */
    }

    [Serializable]
    public class ComponentSlotsData
    {
        public string[] CenterComponentSlots { get; set; }
        public string[] ForeComponentSlots { get; set; }
        public string[] AftComponentSlots { get; set; }
        public string[] LeftComponentSlots { get; set; }
        public string[] RightComponentSlots { get; set; }

        /*
        public static ComponentSlotsData FromShip(Ship s)
        {
            return new ComponentSlotsData()
            {
                CenterComponentSlots = Array.ConvertAll(s.CenterComponentSlots, x => x),
                ForeComponentSlots = Array.ConvertAll(s.ForeComponentSlots, x => x),
                AftComponentSlots = Array.ConvertAll(s.AftComponentSlots, x => x),
                LeftComponentSlots = Array.ConvertAll(s.LeftComponentSlots, x => x),
                RightComponentSlots = Array.ConvertAll(s.RightComponentSlots, x => x)
            };
        }

        public void FillShipData(Ship s)
        {
            s.CenterComponentSlots = Array.ConvertAll(CenterComponentSlots, x => x);
            s.ForeComponentSlots = Array.ConvertAll(ForeComponentSlots, x => x);
            s.AftComponentSlots = Array.ConvertAll(AftComponentSlots, x => x);
            s.LeftComponentSlots = Array.ConvertAll(LeftComponentSlots, x => x);
            s.RightComponentSlots = Array.ConvertAll(RightComponentSlots, x => x);
        }
        */

        public Dictionary<ShipSection, string[]> ToDictionary()
        {
            Dictionary<ShipSection, string[]> res = new Dictionary<ShipSection, string[]>();
            res.Add(ShipSection.Center, CenterComponentSlots);
            res.Add(ShipSection.Fore, ForeComponentSlots);
            res.Add(ShipSection.Aft, AftComponentSlots);
            res.Add(ShipSection.Left, LeftComponentSlots);
            res.Add(ShipSection.Right, RightComponentSlots);
            return res;
        }
    }

    [Serializable]
    public class ShipCarrierModuleData
    {
        public int MaxFormations { get; set; }
        public HierarchyNode[] HangerRootNodes { get; set; }

        /*
        public static ShipCarrierModuleData FromCarrierModule(CarrierBehavior c)
        {
            return FromCarrierModule(c, "", "", "", "");
        }

        public static ShipCarrierModuleData FromCarrierModule(CarrierBehavior c, string meshABPath, string meshAssetPath, string partSysABPath, string partSysAssetPath)
        {
            ShipCarrierModuleData res = new ShipCarrierModuleData()
            {
                MaxFormations = c.MaxFormations,
                HangerRootNodes = c.CarrierHangerAnim.Select(openClose => openClose.transform.ToSerializableHierarchy(meshABPath, meshAssetPath, partSysABPath, partSysAssetPath)).ToArray()
            };
            return res;
        }
        */
    }

}
