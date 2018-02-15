using System.Collections.Generic;

namespace Theframeworkshopskeleton
{
    public class Tree
    {

        private readonly string label;
        private List<Tree> subtrees;
        private Point root;


        public Tree(string name, Point root)
        {
            label = name;
            this.root = root;
            subtrees = new List<Tree>();
        }

        public void AddPoint(Point point)
        {
            Tree subTree = new Tree(" / ", point);
            subtrees.Add(subTree);
        }

        public void AddTree(Tree tree)
        {
            subtrees.Add(tree);
        }

        public void RemoveTree(Tree tree)
        {
            subtrees.Remove(tree);
        }

        public int Size()
        {
            int count = 1;
            
            for (int i = 0; i < subtrees.Count; i++)
            {
                count += subtrees[i].Size();
            }
            
            return count;
        }
        

        public string Label
        {
            get => label;
            
        }

        public List<Tree> Subtrees
        {
            get => subtrees;
            set => subtrees = value;
        }

        public Point Root
        {
            get => root;
        }

        public Point GetPoint(int PosList)
        {
            return subtrees[PosList].root;
        }
    }
}