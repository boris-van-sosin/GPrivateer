using FullBroadside.Importers;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullBroadside
{
    public static class GlobalMeshLibrary
    {
        public static readonly ImportedMeshLibrary MeshLibrary = new ImportedMeshLibrary();
    }

    public class ImportedMeshLibrary
    {
        public ObjTriangleMesh GetMesh(string filePath, string meshName)
        {
            ObjTriangleMesh resMesh = null;
            if (_meshLibrary.TryGetValue((filePath, meshName), out resMesh))
            {
                return resMesh;
            }

            // The mesh was not found in the library. Try loading it from the data file
            // (along with all other meshes in the same file)
            string globFilePath = ProjectSettings.GlobalizePath(filePath);
            using (StreamReader sr = new StreamReader(globFilePath))
            {
                List<ObjMesh> meshes = ObjMeshImporter.ImportObj(sr);
                for (int i = 0; i < meshes.Count; ++i)
                {
                    if (!_meshLibrary.ContainsKey((filePath, meshes[i].Name)))
                    {
                        ObjTriangleMesh currTrigMesh = ObjMeshImporter.TriangulateMesh(meshes[i]);
                        _meshLibrary.Add((filePath, currTrigMesh.Name), currTrigMesh);
                        if (currTrigMesh.Name == meshName)
                            resMesh = currTrigMesh;
                    }
                }
            }
            return resMesh;
        }

        private Dictionary<(string, string), ObjTriangleMesh> _meshLibrary = new Dictionary<(string, string), ObjTriangleMesh>();
    }
}
