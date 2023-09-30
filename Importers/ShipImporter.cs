using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullBroadside.Importers
{
    public static class ShipImporter
    {
        public static List<Node3D> SpawnMonitor(Node3D rootNode)
        {
            string monitorDefPath = ProjectSettings.GlobalizePath("res://LoadedAssets/TextAssets/ShipHulls/Terran Sloop Monitor.yml");
            ShipHullDefinition monitorDef = HierarchySerializer.LoadHierarchy<ShipHullDefinition>(new StreamReader(monitorDefPath));
            List<Node3D> res = new List<Node3D>();
            res.AddRange(HierarchyConstructionUtil.ConstructHierarchy(monitorDef.Geometry, null, 1, 2, 3));
            res.AddRange(HierarchyConstructionUtil.ConstructHierarchy(monitorDef.TeamColorComponents, null, 1, 2, 3));
            for (int i = 0; i < res.Count; ++i)
                rootNode.AddChild(res[i]);
            return res;
        }

        public static MeshInstance3D SpawnMonitorOld(Node3D rootNode)
        {
            ObjTriangleMesh hull = GlobalMeshLibrary.MeshLibrary.GetMesh("res://LoadedAssets/GeomAssets/terran - sloop monitor.obj", "MonitorHull");
            ObjTriangleMesh teamColor = GlobalMeshLibrary.MeshLibrary.GetMesh("res://LoadedAssets/GeomAssets/terran - sloop monitor.obj", "MonitorTeamColor");
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
