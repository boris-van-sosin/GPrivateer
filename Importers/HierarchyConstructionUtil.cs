using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullBroadside
{
    public static class HierarchyConstructionUtil
    {
        public static List<Node3D> ConstructHierarchy(HierarchyNode root, ObjectLoader loader)
        {
            uint layer = 0;
            return ConstructHierarchy(root, loader, layer, layer, layer);
        }

        public static List<Node3D> ConstructHierarchy(HierarchyNode root, ObjectLoader loader, uint generalLayer, uint meshLayer, uint particleSysLayer)
        {
            List<Node3D> res = ConstructHierarchyRecursive(root, true, loader, true,
                                                           null, Transform3D.Identity, Transform3D.Identity,
                                                           generalLayer, meshLayer, particleSysLayer);
            //SetOpenCloseAnims(res, root);
            return res;
        }

        private static List<Node3D> ConstructHierarchyRecursive(HierarchyNode obj, bool setName, ObjectLoader loader,
                                                                bool isMeshTopLvl, List<ObjTriangleMesh> meshCollector,
                                                                Transform3D trFromTopMesh, Transform3D trFromLastValidNode,
                                                                uint generalLayer, uint meshLayer, uint particleSysLayer)
        {
            Node3D resObj = null;
            List<ObjTriangleMesh> lclMeshesToCombine = meshCollector, nextMeshesToCombine = meshCollector;
            bool nextIsMeshTopLvl = isMeshTopLvl;
            Transform3D nextTrFromTopMesh = trFromTopMesh;
            bool setMesh = false;
            if (obj.NodeMesh != null)
            {
                if (!obj.NodeMesh.DoCombine || isMeshTopLvl)
                {
                    MeshInstance3D resMeshNode = new MeshInstance3D();
                    resObj = resMeshNode;

                    if (isMeshTopLvl && obj.NodeMesh.DoCombine)
                        nextMeshesToCombine = lclMeshesToCombine = new List<ObjTriangleMesh>();
                    else if (!obj.NodeMesh.DoCombine)
                        lclMeshesToCombine = new List<ObjTriangleMesh>();

                    if (isMeshTopLvl)
                    {
                        AppendMesh(lclMeshesToCombine, GlobalMeshLibrary.MeshLibrary.GetMesh(obj.NodeMesh.MeshPath, obj.NodeMesh.SubObjectPath));
                        nextTrFromTopMesh = obj.ToTransform();
                    }
                    else
                    {
                        AppendMesh(lclMeshesToCombine, GlobalMeshLibrary.MeshLibrary.GetMesh(obj.NodeMesh.MeshPath, obj.NodeMesh.SubObjectPath), trFromTopMesh);
                        nextTrFromTopMesh *= obj.ToTransform();
                    }
                    resMeshNode.Layers = meshLayer;
                    setMesh = true;
                }
                else
                {

                }
            }
            else if (obj.NodeParticleSystem != null)
            {
                //TODO: impelement
                resObj = new Node3D();
                nextTrFromTopMesh *= obj.ToTransform();
            }
            else
            {
                resObj = new Node3D(); //really?
                nextTrFromTopMesh *= obj.ToTransform();
            }

            if (setName && resObj != null)
            {
                resObj.Name = obj.Name;
            }

            if (resObj != null)
            {
                resObj.Transform = trFromLastValidNode * obj.ToTransform();
                for (int i = 0; i < obj.SubNodes.Length; ++i)
                {
                    List<Node3D> resSubNodes = ConstructHierarchyRecursive(obj.SubNodes[i], true, loader,
                                                                           nextIsMeshTopLvl, nextMeshesToCombine,
                                                                           nextTrFromTopMesh, Transform3D.Identity,
                                                                           generalLayer, meshLayer, particleSysLayer);
                    for (int j = 0; j < resSubNodes.Count; ++j)
                        resObj.AddChild(resSubNodes[j]);
                }
                if (setMesh)
                {
                    ObjectLoader.SetMeshToNode(resObj as MeshInstance3D, lclMeshesToCombine);
                }
                return new List<Node3D>() { resObj };
            }
            else
            {
                List<Node3D> nodesToUp = new List<Node3D>();
                Transform3D nextTrFromLastValidNode = trFromLastValidNode * obj.ToTransform();
                for (int i = 0; i < obj.SubNodes.Length; ++i)
                {
                    nodesToUp.AddRange(ConstructHierarchyRecursive(obj.SubNodes[i], true, loader,
                                                                   nextIsMeshTopLvl, nextMeshesToCombine,
                                                                   nextTrFromTopMesh, nextTrFromLastValidNode,
                                                                   generalLayer, meshLayer, particleSysLayer));
                }
                return nodesToUp;
            }
        }

        /*
        private static void SetOpenCloseAnims(Transform currObj, HierarchyNode currData)
        {
            if (currData.OpenCloseData != null)
            {
                GenericOpenCloseAnim openClose = currObj.gameObject.AddComponent<GenericOpenCloseAnim>();
                currData.OpenCloseData.SetOpenCloseAnim(openClose);
                openClose.AnimComponents = currData.OpenCloseData.AnimComponentPaths.Select(p => currObj.Find(p)).ToArray();
            }
            if (currData.SubNodes == null || currObj.childCount != currData.SubNodes.Length)
            {
                return;
            }
            for (int i = 0; i < currObj.childCount; ++i)
            {
                SetOpenCloseAnims(currObj.GetChild(i), currData.SubNodes[i]);
            }
        }
        */

        public static void AppendMesh(List<ObjTriangleMesh> meshlist, ObjTriangleMesh mesh, Transform3D tr)
        {
            ObjTriangleMesh transformedCopy = new ObjTriangleMesh()
            {
                Name = mesh.Name,
                Vertices = new List<Godot.Vector3>(mesh.Vertices.Count),
                Normals = new List<Godot.Vector3>(mesh.Normals.Count),
                UVs = new List<Godot.Vector2>(mesh.UVs),
                Faces = new List<TriangleInfo>(mesh.Faces)
            };

            for (int i = 0; i < mesh.Vertices.Count; ++i)
            {
                if (tr.IsEqualApprox(Transform3D.Identity))
                    transformedCopy.Vertices.Add(mesh.Vertices[i]);
                else
                    transformedCopy.Vertices.Add(tr * mesh.Vertices[i]);
            }

            for (int i = 0; i < mesh.Normals.Count; ++i)
            {
                if (tr.IsEqualApprox(Transform3D.Identity))
                    transformedCopy.Normals.Add(mesh.Vertices[i]);
                else
                    transformedCopy.Normals.Add(tr.Basis.GetRotationQuaternion() * mesh.Vertices[i]);
            }

            meshlist.Add(transformedCopy);
        }

        public static void AppendMesh(List<ObjTriangleMesh> meshlist, ObjTriangleMesh mesh)
        {
            AppendMesh(meshlist, mesh, Transform3D.Identity);
        }
    }
}
