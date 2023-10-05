using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace FullBroadside
{
    public static class HierarchyConstructionUtil
    {
        public static List<(Node3D, Transform3D)> ConstructHierarchy(HierarchyNode root, ObjectLoader loader)
        {
            uint layer = 0;
            return ConstructHierarchy(root, loader, layer, layer, layer);
        }

        public static List<(Node3D, Transform3D)> ConstructHierarchy(HierarchyNode root, ObjectLoader loader, uint generalLayer, uint meshLayer, uint particleSysLayer)
        {
            List<Node3D> resNodes = ConstructHierarchyRecursive(root, true, loader, true,
                                                           null, Transform3D.Identity, Transform3D.Identity,
                                                           generalLayer, meshLayer, particleSysLayer);
            //SetOpenCloseAnims(res, root);
            List<(Node3D, Transform3D)> res = new List<(Node3D, Transform3D)>(resNodes.Count);
            Transform3D rootTr = root.ToTransform();
            Quaternion q = rootTr.Basis.GetRotationQuaternion();
            Vector3 axis = q.GetAxis();
            float t = q.GetAngle();
            GD.Print("Forward", axis, t);
            for (int i = 0; i < resNodes.Count; ++i)
            {
                res.Add((resNodes[i], rootTr));
            }
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
                        nextIsMeshTopLvl = !obj.NodeMesh.DoCombine;
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
                    nextTrFromTopMesh *= obj.ToTransform();
                    AppendMesh(lclMeshesToCombine, GlobalMeshLibrary.MeshLibrary.GetMesh(obj.NodeMesh.MeshPath, obj.NodeMesh.SubObjectPath), nextTrFromTopMesh);
                }
            }
            else if (obj.NodeParticleSystem != null)
            {
                //TODO: impelement
                resObj = ConfigureParticleSystem(obj.NodeParticleSystem, resObj);
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
                for (int i = 0; i < obj.SubNodes.Length; ++i)
                {
                    List<Node3D> resSubNodes = ConstructHierarchyRecursive(obj.SubNodes[i], true, loader,
                                                                           nextIsMeshTopLvl, nextMeshesToCombine,
                                                                           nextTrFromTopMesh, Transform3D.Identity,
                                                                           generalLayer, meshLayer, particleSysLayer);
                    for (int j = 0; j < resSubNodes.Count; ++j)
                    {
                        resObj.AddChild(resSubNodes[j]);
                        resSubNodes[j].Transform = trFromLastValidNode * obj.SubNodes[i].ToTransform();
                    }
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

        private static Node3D ConfigureParticleSystem(ParticleSystemData partSysData, Node3D resObj)
        {
            if (partSysData.ResourcePath != null &&
                partSysData.ResourcePath != string.Empty)
            {
                Node loadedPartcSys = ResourceLoader.Load<PackedScene>(partSysData.ResourcePath).Instantiate();
                if (loadedPartcSys is Node3D)
                {
                    if (loadedPartcSys is GpuParticles3D gpuPartSys)
                    {
                        SerializableVector2 sc = partSysData.Scale;
                        if (!sc.ToVector2().IsEqualApprox(Vector2.One))
                        {
                            Aabb bbox = gpuPartSys.VisibilityAabb;

                            Vector3 bboxPos = bbox.Position;
                            bboxPos.X *= sc.x;
                            bboxPos.Z *= sc.x;
                            bboxPos.Y *= sc.y;

                            Vector3 bboxSz = bbox.Size;
                            bboxSz.X *= sc.x;
                            bboxSz.Z *= sc.x;
                            bboxSz.Y *= sc.y;

                            bbox.Position = bboxPos;
                            bbox.Size = bboxSz;
                            gpuPartSys.VisibilityAabb = bbox;

                            if (gpuPartSys.ProcessMaterial is ParticleProcessMaterial procMat)
                            {
                                if (procMat.EmissionShape == ParticleProcessMaterial.EmissionShapeEnum.Sphere ||
                                    procMat.EmissionShape == ParticleProcessMaterial.EmissionShapeEnum.SphereSurface)
                                {
                                    procMat.EmissionSphereRadius *= sc.x;
                                }
                                else if (procMat.EmissionShape == ParticleProcessMaterial.EmissionShapeEnum.Ring)
                                {
                                    procMat.EmissionRingRadius *= sc.x;
                                    procMat.EmissionRingInnerRadius *= sc.x;
                                    procMat.EmissionRingHeight *= sc.y;
                                }
                                else if (procMat.EmissionShape == ParticleProcessMaterial.EmissionShapeEnum.Box)
                                {
                                    Vector3 emissionBox = procMat.EmissionBoxExtents;
                                    emissionBox.X *= sc.x;
                                    emissionBox.Z *= sc.x;
                                    emissionBox.Y *= sc.y;
                                    procMat.EmissionBoxExtents = emissionBox;
                                }
                            }
                        }

                        if (gpuPartSys.ProcessMaterial is ParticleProcessMaterial procMatV &&
                            Mathf.Abs(partSysData.VelocityFactor - 1.0f) > Mathf.Epsilon)
                        {
                            procMatV.InitialVelocityMin *= partSysData.VelocityFactor;
                            procMatV.InitialVelocityMax *= partSysData.VelocityFactor;
                        }
                        if (Mathf.Abs(partSysData.LifetimeFactor - 1.0f) > Mathf.Epsilon)
                        {
                            gpuPartSys.Lifetime *= partSysData.LifetimeFactor;
                        }

                        Variant meshSz = gpuPartSys.DrawPass1.Get("size");
                        Variant meshCntrOffs = gpuPartSys.DrawPass1.Get("center_offset");
                        if (meshSz.VariantType == Variant.Type.Vector2)
                        {
                            Vector2 meshSzVec = meshSz.AsVector2();
                            meshSzVec.X *= sc.x;
                            meshSzVec.Y *= sc.y;
                            gpuPartSys.DrawPass1.Set("size", Variant.CreateFrom(meshSzVec));
                        }
                        if (meshCntrOffs.VariantType == Variant.Type.Vector3)
                        {
                            Vector3 meshOffsVec = meshCntrOffs.AsVector3();
                            meshOffsVec.X *= sc.x;
                            meshOffsVec.Z *= sc.x;
                            meshOffsVec.Y *= sc.y;
                            gpuPartSys.DrawPass1.Set("center_offset", Variant.CreateFrom(meshOffsVec));
                        }
                    }
                    resObj = (Node3D)loadedPartcSys;
                }
                else
                {
                    if (loadedPartcSys != null)
                        loadedPartcSys.Free();

                    resObj = null;
                }
            }

            return resObj;
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
