using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using Boo.Lang;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.WSA.Persistence;

namespace Extremities
{
    internal class Program
    {
        public static List<Vector3> getExtremities(Mesh mesh, float density)
        // returns the List of extremities of the mesh
        {
            Vector3[] dimmensions = __getDimensions(mesh);
            
            List<Vector3> extremities = new List<Vector3>();
            
            // radius is the average length in the three dimmensions of the object, divided by the density
            float radius =
            ((dimmensions[1].x - dimmensions[0].x) + (dimmensions[1].y - dimmensions[0].y) +
             (dimmensions[1].z - dimmensions[0].z)) / (3 * density);
            for (float i = dimmensions[0].z; i < dimmensions[1].z; i += radius / 2)
            {
                for (float j = dimmensions[0].y; j < dimmensions[1].y; j += radius / 2)
                {
                    List<Vector3> intersects = Line_meshIntersect.Program.line_meshIntersect(mesh, new Vector3(dimmensions[0].x - 0.1f, j, i), new Vector3(dimmensions[1].x - 0.1f, j, i));

                    float pos;
                    
                    for (int k = 0; k < intersects.Count; k += 2)
                    {
                        pos = (intersects[k + 1].x - intersects[k].x) % (radius / 2);

                        while (pos < intersects[k + 1].x)
                        {
                            if (__isExtremity(mesh, radius, new Vector3(pos, j, i)))
                            {
                                extremities.Add(new Vector3(pos, j, i));
                            }
                            
                            pos += radius / 2;
                        }
                    }
                }
            }

            return extremities;
        }

        private static bool __isExtremity(Mesh mesh, float radius, Vector3 point)
        // returns true if point is an extremity, else returns false
        {
            int count = 0;
            
            float ro2 = ((float) Math.Sqrt(2) / 2) * radius;
            float[] sphere = new float[]
            {
                0, 0, -radius,
                
                0, ro2, -ro2,
                radius/2, radius/2, -ro2,
                ro2, 0, -ro2,
                radius/2, -radius/2, -ro2,
                0, -ro2, -ro2,
                -radius/2, -radius/2, -ro2,
                -ro2, 0, -ro2,
                -radius/2, radius/2, -ro2,
                
                0, radius, 0,
                ro2, ro2, 0,
                radius, 0, 0,
                ro2, -ro2, 0,
                0, -radius, 0,
                -ro2, -ro2, 0,
                -radius, 0, 0,
                -ro2, ro2, 0,
                
                0, ro2, ro2,
                radius/2, radius/2, ro2,
                ro2, 0, ro2,
                radius/2, -radius/2, ro2,
                0, -ro2, ro2,
                -radius/2, -radius/2, ro2,
                -ro2, 0, ro2,
                -radius/2, radius/2, ro2,
                
                0, 0, radius
            };

            for (int i = 0; i < sphere.Length; i += 3)
            {
                if (Line_meshIntersect.Program.line_meshIntersect(mesh, point, new Vector3(point.x + sphere[i], point.y + sphere[i + 1], point.z + sphere[i + 2])).Count == 0)
                {
                    count++;
                }
            }


            return count < 4;
        }

        private static Vector3[] __getDimensions(Mesh mesh)
        // returns xMin, yMin, zMin, xMax, yMax and zMax in the form of an array of Vector3
        //     these give the dimensions of the mesh
        {
            Vector3[] dimensions = new[] {new Vector3(mesh.vertices[0].x, mesh.vertices[0].y, mesh.vertices[0].z), new Vector3(mesh.vertices[0].x, mesh.vertices[0].y, mesh.vertices[0].z)};

            for (int i = 1; i < mesh.vertices.Length; i++)
            {
                if (mesh.vertices[i].x < dimensions[0].x)
                {
                    dimensions[0].x = mesh.vertices[i].x;
                }
                else if (mesh.vertices[i].x > dimensions[1].x)
                {
                    dimensions[1].x = mesh.vertices[i].x;
                }
                
                if (mesh.vertices[i].y < dimensions[0].y)
                {
                    dimensions[0].y = mesh.vertices[i].y;
                }
                else if (mesh.vertices[i].y > dimensions[1].y)
                {
                    dimensions[1].y = mesh.vertices[i].y;
                }
                
                if (mesh.vertices[i].z < dimensions[0].z)
                {
                    dimensions[0].z = mesh.vertices[i].z;
                }
                else if (mesh.vertices[i].z > dimensions[1].z)
                {
                    dimensions[1].z = mesh.vertices[i].z;
                }
            }

            return dimensions;
        }
    }
}