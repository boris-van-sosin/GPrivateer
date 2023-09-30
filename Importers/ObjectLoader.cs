using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullBroadside
{
    public class ObjectLoader
    {

        public static void SetMeshToNode(MeshInstance3D meshNode, List<ObjTriangleMesh> meshes)
        {
            SurfaceTool st = new SurfaceTool();
            st.Begin(Mesh.PrimitiveType.Triangles);
            for (int i = 0; i < meshes.Count; ++i)
            {
                for (int j = 0; j < meshes[i].Faces.Count; ++j)
                {
                    for (int v = 2; v >= 0; --v)
                    {
                        if (meshes[i].Faces[j][v].UVIdx >= 0)
                            st.SetUV(meshes[i].UVs[meshes[i].Faces[j][v].UVIdx]);
                        if (meshes[i].Faces[j][v].NormalIdx >= 0)
                            st.SetNormal(meshes[i].Normals[meshes[i].Faces[j][v].NormalIdx]);
                        st.AddVertex(meshes[i].Vertices[meshes[i].Faces[j][v].VertexIdx]);
                    }
                }
            }
            st.Index();
            meshNode.Mesh = st.Commit();
        }
    }
}
