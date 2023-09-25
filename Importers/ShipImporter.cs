using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullBroadside.Importers
{
    public static class ShipImporter
    {
        public static MeshInstance3D SpawnMonitor(Node3D rootNode)
        {
            ObjTriangleMesh hull = ImportedMeshLibrary.GetMesh("res://LoadedAssets/GeomAssets/terran - sloop monitor.obj", "MonitorHull");
            ObjTriangleMesh teamColor = ImportedMeshLibrary.GetMesh("res://LoadedAssets/GeomAssets/terran - sloop monitor.obj", "MonitorTeamColor");
            MeshInstance3D shipMesh = new MeshInstance3D();

            SurfaceTool st = new SurfaceTool();
            st.Begin(Mesh.PrimitiveType.Triangles);
            for (int i = 0; i < hull.Faces.Count; ++i)
            {
                if (hull.Faces[i].Item3.UVIdx >= 0)
                    st.SetUV(hull.UVs[hull.Faces[i].Item3.UVIdx]);
                if (hull.Faces[i].Item3.NormalIdx >= 0)
                    st.SetNormal(hull.Normals[hull.Faces[i].Item3.NormalIdx]);
                st.AddVertex(hull.Vertices[hull.Faces[i].Item3.VertexIdx]);

                if (hull.Faces[i].Item2.UVIdx >= 0)
                    st.SetUV(hull.UVs[hull.Faces[i].Item2.UVIdx]);
                if (hull.Faces[i].Item2.NormalIdx >= 0)
                    st.SetNormal(hull.Normals[hull.Faces[i].Item2.NormalIdx]);
                st.AddVertex(hull.Vertices[hull.Faces[i].Item2.VertexIdx]);


                if (hull.Faces[i].Item1.UVIdx >= 0)
                    st.SetUV(hull.UVs[hull.Faces[i].Item1.UVIdx]);
                if (hull.Faces[i].Item1.NormalIdx >= 0)
                    st.SetNormal(hull.Normals[hull.Faces[i].Item1.NormalIdx]);
                st.AddVertex(hull.Vertices[hull.Faces[i].Item1.VertexIdx]);
            }
            for (int i = 0; i < teamColor.Faces.Count; ++i)
            {
                if (teamColor.Faces[i].Item3.UVIdx >= 0)
                    st.SetUV(teamColor.UVs[teamColor.Faces[i].Item3.UVIdx]);
                if (teamColor.Faces[i].Item3.NormalIdx >= 0)
                    st.SetNormal(teamColor.Normals[teamColor.Faces[i].Item3.NormalIdx]);
                st.AddVertex(teamColor.Vertices[teamColor.Faces[i].Item3.VertexIdx]);

                if (teamColor.Faces[i].Item2.UVIdx >= 0)
                    st.SetUV(teamColor.UVs[teamColor.Faces[i].Item2.UVIdx]);
                if (teamColor.Faces[i].Item2.NormalIdx >= 0)
                    st.SetNormal(teamColor.Normals[teamColor.Faces[i].Item2.NormalIdx]);
                st.AddVertex(teamColor.Vertices[teamColor.Faces[i].Item2.VertexIdx]);

                if (teamColor.Faces[i].Item1.UVIdx >= 0)
                    st.SetUV(teamColor.UVs[teamColor.Faces[i].Item1.UVIdx]);
                if (teamColor.Faces[i].Item1.NormalIdx >= 0)
                    st.SetNormal(teamColor.Normals[teamColor.Faces[i].Item1.NormalIdx]);
                st.AddVertex(teamColor.Vertices[teamColor.Faces[i].Item1.VertexIdx]);
            }

            st.Index();
            shipMesh.Mesh = st.Commit();
            rootNode.AddChild(shipMesh);
            shipMesh.Transform = Transform3D.Identity;

            return shipMesh;
        }
    }
}
