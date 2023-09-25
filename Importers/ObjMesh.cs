using Godot;
using System;
using System.Collections.Generic;
using System.Data;

namespace FullBroadside
{
    public struct FaceVertexInfo
    {
        public int VertexIdx, UVIdx, NormalIdx;
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
            Faces = new List<(FaceVertexInfo, FaceVertexInfo, FaceVertexInfo)>();
        }
        public List<(FaceVertexInfo, FaceVertexInfo, FaceVertexInfo)> Faces { get; set; }
    }
}
