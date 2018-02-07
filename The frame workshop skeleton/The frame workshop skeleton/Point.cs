using System;

namespace Theframeworkshopskeleton
{
    public class Point
    {

        private float x, y, z;
        private float rotX, rotY, rotZ;
        private readonly string name;
        private bool fixedPoint;
        private float weight;
        private int treePos;


        public Point(string name, int x, int y, int z)
        {
            this.name = name;
            this.fixedPoint = false;

            this.x = x;
            this.y = y;
            this.z = z;

            rotX = 0;
            rotY = 0;
            rotZ = 0;

            treePos = -1;
            weight = 1;
        }




        public string Name
        {
            get { return name; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }
        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public float RotX
        {
            get { return rotX; }
            set { rotX = value; }
        }
        public float RotY
        {
            get { return rotY; }
            set { rotY = value; }
        }
        public float RotZ
        {
            get { return rotZ; }
            set { rotZ = value; }
        }

        public bool FixedPoint
        {
            get { return fixedPoint; }
            set { fixedPoint = value; }
        }

        public float Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public int TreePos
        {
            get { return treePos; }
            set { treePos = value; }
        }

        public void AddValuePos(int x, int y, int z)
        {
            this.x += x;
            this.y += y;
            this.z += z;
        }

        public void AddValueRotPos(int rotX, int rotY, int rotZ)
        {
            this.rotX += rotX;
            this.rotY += rotY;
            this.rotZ += rotZ;
        }

    }
}

