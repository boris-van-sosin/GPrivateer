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
                for (int v = 2; v >= 0; --v)
                {
                    if (hull.Faces[i][v].UVIdx >= 0)
                        st.SetUV(hull.UVs[hull.Faces[i][v].UVIdx]);
                    if (hull.Faces[i][v].NormalIdx >= 0)
                        st.SetNormal(hull.Normals[hull.Faces[i][v].NormalIdx]);
                    st.AddVertex(hull.Vertices[hull.Faces[i][v].VertexIdx]);
                }
            }
            for (int i = 0; i < teamColor.Faces.Count; ++i)
            {
                for (int v = 2; v >= 0; --v)
                {
                    if (teamColor.Faces[i][v].UVIdx >= 0)
                        st.SetUV(teamColor.UVs[teamColor.Faces[i][v].UVIdx]);
                    if (teamColor.Faces[i][v].NormalIdx >= 0)
                        st.SetNormal(teamColor.Normals[teamColor.Faces[i][v].NormalIdx]);
                    st.AddVertex(teamColor.Vertices[teamColor.Faces[i][v].VertexIdx]);
                }
            }

            st.Index();
            shipMesh.Mesh = st.Commit();
            rootNode.AddChild(shipMesh);
            shipMesh.Transform = Transform3D.Identity;

            return shipMesh;
        }
    }
}
