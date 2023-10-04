using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace FullBroadside
{
    [Serializable]
    public class HierarchyNode
    {
        public string Name { get; set; }
        public SerializableVector3 Position { get; set; }
        public SerializableVector3 Rotation { get; set; }
        public SerializableVector3 Scale { get; set; }
        public MeshData NodeMesh { get; set; }
        public ParticleSystemData NodeParticleSystem { get; set; }
        public OpenCloseComponentData OpenCloseData { get; set; }
        public HierarchyNode[] SubNodes { get; set; }
        public Transform3D ToTransform()
        {
            return new Transform3D(new Basis(Rotation.ToQuaternion()) * Basis.FromScale(Scale.ToVector3()), Position.ToVector3());
        }
    }

    [Serializable]
    public struct SerializableVector2
    {
        public float x { get; set; }
        public float y { get; set; }

        static public SerializableVector2 FromSerializable(Vector2 v) => new SerializableVector2() { x = v.X, y = v.Y };

        public Vector2 ToVector2() => new Vector2(x, y);
    }

    [Serializable]
    public struct SerializableVector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        static public SerializableVector3 FromSerializable(Vector3 v) => new SerializableVector3() { x = v.X, y = v.Y, z = v.Z };

        static public SerializableVector3 FromSerializable(Quaternion q) => FromSerializable(q.GetEuler(EulerOrder.Yxz));

        public Vector3 ToVector3() => new Vector3(x, y, z);

        public Quaternion ToQuaternion() => Quaternion.FromEuler(new Vector3(Mathf.DegToRad(x), Mathf.DegToRad(y), Mathf.DegToRad(z)));
    }

    [Serializable]
    public class MeshData
    {
        public string MeshPath { get; set; }
        public string SubObjectPath { get; set; }
        public bool DoCombine { get; set; }
    }

    [Serializable]
    public class ParticleSystemData
    {
        public string ResourcePath { get; set; }
        public SerializableVector2 Scale { get; set; }
        public float LifetimeFactor { get; set; }
        public float VelocityFactor { get; set; }
    }

    [Serializable]
    public class OpenCloseComponentData
    {
        public string[] AnimComponentPaths { get; set; }
        public SerializableAnimState ClosedState { get; set; }
        public SerializableAnimState OpenState { get; set; }
        public SerializableAnimState[] AnimWaypoints { get; set; }

        /*
        public static OpenCloseComponentData FromOpenCloseAnim(GenericOpenCloseAnim openClose)
        {
            return new OpenCloseComponentData()
            {
                ClosedState = SerializableAnimState.FromAnimState(openClose.ClosedState),
                OpenState = SerializableAnimState.FromAnimState(openClose.OpenState),
                AnimWaypoints = openClose.AnimWaypoints.Select(w => SerializableAnimState.FromAnimState(w)).ToArray(),
                AnimComponentPaths = openClose.AnimComponents.Select(c => HierarchySerializer.TransformPath(c, openClose.transform)).ToArray()
            };
        }

        public void SetOpenCloseAnim(GenericOpenCloseAnim openClose)
        {
            openClose.ClosedState = ClosedState.ToAnimState();
            openClose.OpenState = OpenState.ToAnimState();
            openClose.AnimWaypoints = AnimWaypoints.Select(a => a.ToAnimState()).ToArray();
        }
        */
    }

    [Serializable]
    public class SerializableAnimState
    {
        public SerializableVector3[] Positions { get; set; }
        public SerializableVector3[] Rotations { get; set; }
        public float Duration { get; set; }

        /*
        public static SerializableAnimState FromAnimState(GenericOpenCloseAnim.AnimState orig)
        {
            SerializableAnimState res = new SerializableAnimState()
            {
                Positions = orig.Positions.Select(p => SerializableVector3.FromSerializable(p)).ToArray(),
                Rotations = orig.Rotations.Select(r => SerializableVector3.FromSerializable(r)).ToArray(),
                Duration = orig.Duration
            };

            return res;
        }

        public GenericOpenCloseAnim.AnimState ToAnimState()
        {
            return new GenericOpenCloseAnim.AnimState()
            {
                Positions = Positions.Select(p => p.ToVector3()).ToArray(),
                Rotations = Rotations.Select(r => r.ToVector3()).ToArray(),
                Duration = Duration
            };
        }
        */
    }

    public enum TurretAIHint
    {
        Main,
        Secondary,
        CloseIn,
        HitandRun
    };

    public enum ShipSection { Fore, Aft, Left, Right, Center, Hidden };

    public enum TacMapEntityType { Torpedo, StrikeCraft, Sloop, Frigate, Destroyer, Cruiser, CapitalShip, StaticDefence };
}
