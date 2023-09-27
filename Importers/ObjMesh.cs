using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace FullBroadside
{
    public struct FaceVertexInfo
    {
        public FaceVertexInfo()
        {
            VertexIdx = -1;
            UVIdx = -1;
            NormalIdx = -1;
            ShadeSmooth = false;
        }

        public int VertexIdx, UVIdx, NormalIdx;
        public bool ShadeSmooth;
    }

    public struct TriangleInfo
    {
        public static TriangleInfo From(FaceVertexInfo v0, FaceVertexInfo v1, FaceVertexInfo v2)
        {
            return new TriangleInfo() { _trig = (v0, v1, v2) };
        }

        public static TriangleInfo From((FaceVertexInfo, FaceVertexInfo, FaceVertexInfo) t)
        {
            return new TriangleInfo() { _trig = t };
        }

        public static TriangleInfo From(IList<FaceVertexInfo> lst)
        {
            if (lst.Count != 3)
                throw new IndexOutOfRangeException(string.Format("Triangle index out of range. List of length {0}", lst.Count));
            return new TriangleInfo() { _trig = (lst[0], lst[1], lst[2]) };
        }

        public FaceVertexInfo this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return _trig.Item1;
                    case 1: return _trig.Item2;
                    case 2: return _trig.Item3;
                    default: throw new IndexOutOfRangeException(string.Format("Triangle index out of range: {0}", index));
                }
            }
            /*
            set
            {
                switch (index)
                {
                    case 0:
                        _trig.Item1 = value;
                        break;
                    case 1:
                        _trig.Item2 = value;
                        break;
                    case 2:
                        _trig.Item3 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Triangle index out of range: {0}", index));
                }
            }
            */
        }
        private (FaceVertexInfo, FaceVertexInfo, FaceVertexInfo) _trig;
    }

    public abstract class ObjMeshBase
    {
        public ObjMeshBase()
        {
            Name = string.Empty;
            Vertices = new List<Vector3>();
            UVs = new List<Vector2>();
            Normals = new List<Vector3>();
        }
        public string Name { get; set; }
        public List<Vector3> Vertices { get; set; }
        public List<Vector2> UVs { get; set; }
        public List<Vector3> Normals { get; set; }
        public bool IsEmpty() { return Vertices.Count == 0; }
    }
    public class ObjMesh : ObjMeshBase
    {
        public ObjMesh()
        {
            Faces = new List<List<FaceVertexInfo>>();
        }
        public List<List<FaceVertexInfo>> Faces { get; set; }
    }

    public class ObjTriangleMesh : ObjMeshBase
    {
        public ObjTriangleMesh()
        {
            Faces = new List<TriangleInfo>();
        }
        public List<TriangleInfo> Faces { get; set; }
    }
}
