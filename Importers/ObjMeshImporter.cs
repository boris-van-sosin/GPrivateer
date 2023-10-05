using Godot;
using System;
using System.Collections.Generic;

namespace FullBroadside.Importers
{
    public static class ObjMeshImporter
    {
        public static List<ObjMesh> ImportObj(System.IO.TextReader reader)
        {
            List<ObjMesh > res = new List<ObjMesh>();
            string line;
            string[] lineElems;
            ObjMesh currMesh = new ObjMesh() { Name = string.Empty };
            int lineCount = -1;
            int vertrxBase = 0, uvBase = 0, normalBase = 0;
            int smoothState = -1;
            while (null != (line = reader.ReadLine()))
            {
                ++lineCount;
                try
                {
                    line = line.Trim(' ', '\t');
                    if (line.StartsWith('#')) continue;
                    lineElems = line.Split(' ', '\t');
                    if (lineElems.Length == 0) continue;

                    if (lineElems[0].Equals("o", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        string subMeshName = lineElems.Length >= 2 ? lineElems[1] : string.Empty;
                        if (currMesh.IsEmpty())
                        {
                            currMesh.Name = subMeshName;
                            continue;
                        }
                        else
                        {
                            res.Add(currMesh);
                            vertrxBase += currMesh.Vertices.Count;
                            uvBase += currMesh.UVs.Count;
                            normalBase += currMesh.Normals.Count;
                            currMesh = new ObjMesh() { Name = subMeshName };
                            smoothState = -1;
                        }
                    }
                    else if (lineElems[0].Equals("v", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currMesh.Vertices.Add(new Godot.Vector3() { X = float.Parse(lineElems[1]),
                                                                    Y = float.Parse(lineElems[2]),
                                                                    Z = float.Parse(lineElems[3])});
                    }
                    else if (lineElems[0].Equals("vt", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currMesh.UVs.Add(new Godot.Vector2() { X = float.Parse(lineElems[1]),
                                                               Y = float.Parse(lineElems[2])});
                    }
                    else if (lineElems[0].Equals("vn", StringComparison.InvariantCultureIgnoreCase))
                    {
                        currMesh.Normals.Add(new Godot.Vector3() { X = float.Parse(lineElems[1]),
                                                                   Y = float.Parse(lineElems[2]),
                                                                   Z = float.Parse(lineElems[3])});
                    }
                    else if (lineElems[0].Equals("s", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (lineElems[1].Equals("off", StringComparison.InvariantCultureIgnoreCase))
                            smoothState = -1;
                        else
                            smoothState = int.Parse(lineElems[1]);
                    }
                    else if (lineElems[0].Equals("f", StringComparison.InvariantCultureIgnoreCase))
                    {
                        List<FaceVertexInfo> faceInfo = new List<FaceVertexInfo>();
                        for (int i = 1; i < lineElems.Length; ++i)
                        {
                            faceInfo.Add(FaceInfo(lineElems[i], vertrxBase, uvBase, normalBase));
                        }
                        currMesh.Faces.Add((faceInfo, smoothState));
                    }
                }
                catch (System.Exception exc)
                {
                    throw new System.FormatException(string.Format("Parsing error at line {0} Line: {1}", lineCount, line), exc);
                }
            }

            if (!currMesh.IsEmpty())
            {
                res.Add(currMesh);
            }

            return res;
        }

        private static FaceVertexInfo FaceInfo(string f, int vertexBase = 0, int uvBase = 0, int normalBase = 0)
        {
            string[] faceElems = f.Split('/');
            if (faceElems.Length == 1)
            {
                return new FaceVertexInfo() { VertexIdx = int.Parse(faceElems[0]) - 1 - vertexBase,
                                              UVIdx = -1,
                                              NormalIdx = -1, };
            }
            else if (faceElems.Length == 2)
            {
                return new FaceVertexInfo() { VertexIdx = int.Parse(faceElems[0]) - 1 - vertexBase,
                                              UVIdx = int.Parse(faceElems[1]) - 1 - uvBase,
                                              NormalIdx = -1 };
            }
            else if (faceElems.Length == 3)
            {
                if (faceElems[1] != string.Empty)
                {
                    return new FaceVertexInfo() { VertexIdx = int.Parse(faceElems[0]) - 1 - vertexBase,
                                                  UVIdx = int.Parse(faceElems[1]) - 1 - uvBase,
                                                  NormalIdx = int.Parse(faceElems[2]) - 1 - normalBase };
                }
                else
                {
                    return new FaceVertexInfo() { VertexIdx = int.Parse(faceElems[0]) - 1 - vertexBase,
                                                  UVIdx = - 1,
                                                  NormalIdx = int.Parse(faceElems[2]) - 1 - normalBase };
                }
            }
            else
            {
                throw new System.FormatException("Face info format invalid: " + f);
            }
        }

        public static ObjTriangleMesh TriangulateMesh(ObjMesh mesh)
        {
            ObjTriangleMesh res = new ObjTriangleMesh() { Name = mesh.Name };
            res.Vertices.AddRange(mesh.Vertices);
            res.UVs.AddRange(mesh.UVs);
            res.Normals.AddRange(mesh.Normals);
            
            res.Faces.EnsureCapacity(mesh.Faces.Count);
            for (int i = 0; i < mesh.Faces.Count; ++i)
            {
                TriangulatePolygonTo(mesh.Faces[i].Item1, mesh.Faces[i].Item2, mesh, res.Faces, (lst, trig) => { lst.Add(trig); });
            }
            return res;
        }

        private static List<TriangleInfo> TriangulatePolygon(List<FaceVertexInfo> poly, int smoothGroup, ObjMeshBase refMesh)
        {
            List<TriangleInfo> res = new List<TriangleInfo>();
            TriangulatePolygonTo(poly, smoothGroup, refMesh, res, (lst, trig) => { lst.Add(trig); });
            return res;
        }

        private static void TriangulatePolygonTo<T>(List<FaceVertexInfo> poly, int smoothGroup, ObjMeshBase refMesh, T target, Action<T, TriangleInfo> addAction)
        {
            
            if (poly.Count == 3)
            {
                addAction(target, TriangleInfo.From(poly[0], poly[1], poly[2], smoothGroup));
                return;
            }
            else if (poly.Count < 3)
            {
                throw new Exception("Face has fewer than 3 vertices");
            }
            List<FaceVertexInfo> polyCopy = new List<FaceVertexInfo>(poly);

            // Find some extreme vertex:
            int extrIdx = -1;
            for (int i = 0; i < polyCopy.Count; ++i)
            {
                if (extrIdx == -1 || refMesh.Vertices[polyCopy[i].VertexIdx].X < refMesh.Vertices[polyCopy[extrIdx].VertexIdx].X - Mathf.Epsilon)
                {
                    extrIdx = i;
                }
            }
            if (extrIdx == -1)
            {
                for (int i = 0; i < polyCopy.Count; ++i)
                {
                    if (extrIdx == -1 || refMesh.Vertices[polyCopy[i].VertexIdx].Y < refMesh.Vertices[polyCopy[extrIdx].VertexIdx].Y - Mathf.Epsilon)
                    {
                        extrIdx = i;
                    }
                }
            }
            // If we haven't found an extreme point on either X or Y axes, then the polygon is degenerate (a straight line parallel to the Z axis). Skip it.
            if (extrIdx == -1)
            {
                return;
            }

            Vector3 polyPlnN = (refMesh.Vertices[polyCopy[(extrIdx + polyCopy.Count - 1) % polyCopy.Count].VertexIdx] - refMesh.Vertices[polyCopy[extrIdx].VertexIdx]).Cross
                                (refMesh.Vertices[polyCopy[(extrIdx + 1) % polyCopy.Count].VertexIdx] - refMesh.Vertices[polyCopy[extrIdx].VertexIdx]);

            while (polyCopy.Count > 3)
            {
                bool reduced = false;
                for (int i = 0; i < polyCopy.Count; ++i)
                {
                    Vector3 a = refMesh.Vertices[polyCopy[i].VertexIdx];
                    Vector3 b = refMesh.Vertices[polyCopy[(i + 1) % polyCopy.Count].VertexIdx];
                    Vector3 c = refMesh.Vertices[polyCopy[(i + 2) % polyCopy.Count].VertexIdx];
                    Vector3 trigNormal = (a - b).Cross(c - b).Normalized();

                    // Skip degenerate triangles and concave points:
                    if (trigNormal.LengthSquared() < Mathf.Epsilon)
                    {
                        polyCopy.RemoveAt((i + 1) % polyCopy.Count);
                        reduced = true;
                        break;
                    }
                    else if (trigNormal.Dot(polyPlnN) < Mathf.Epsilon)
                    {
                        continue;
                    }

                    a = a.ProjectOnPlane(trigNormal);
                    b = b.ProjectOnPlane(trigNormal);
                    c = c.ProjectOnPlane(trigNormal);

                    bool isEar = true;
                    // Make sure there are no other vertices inside the triangle:
                    for (int j = 3; j < polyCopy.Count; ++j)
                    {
                        Vector3 other = refMesh.Vertices[polyCopy[(i + j) % polyCopy.Count].VertexIdx];
                        Vector3 otherProj = other.ProjectOnPlane(trigNormal);
                        float signedArea = (otherProj - a).Cross(otherProj - b).Dot(trigNormal) +
                                           (otherProj - b).Cross(otherProj - c).Dot(trigNormal) +
                                           (otherProj - c).Cross(otherProj - a).Dot(trigNormal);
                        // If the signed area is positive, then the point is internal
                        if (signedArea > Mathf.Epsilon)
                        {
                            isEar = false;
                            break;
                        }
                    }

                    // Output the ear and remove it from the polygon:
                    if (isEar)
                    {
                        addAction(target, TriangleInfo.From(polyCopy[i], polyCopy[(i + 1) % polyCopy.Count], polyCopy[(i + 2) % polyCopy.Count], smoothGroup));
                        polyCopy.RemoveAt((i + 1) % polyCopy.Count);
                        reduced = true;
                        break;
                    }
                }
                if (!reduced)
                {
                    throw new Exception("Failed to triangulate polygon");
                }
            }

            // Output the final triangle:
            addAction(target, TriangleInfo.From(polyCopy[0], polyCopy[1], polyCopy[2], smoothGroup));
        }
    }
}
