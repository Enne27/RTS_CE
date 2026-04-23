using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Box
{
    public Vector2 Min;
    public Vector2 Max;

    public static Box Empty => new Box
    {
        Min = new Vector2(float.PositiveInfinity, float.PositiveInfinity),
        Max = new Vector2(float.NegativeInfinity, float.NegativeInfinity)
    };

    public void Include(Vector3 p)
    {
        Min = new Vector2(MathF.Min(Min.x, p.x), MathF.Min(Min.y, p.z));

        Max = new Vector2(MathF.Max(Max.x, p.x), MathF.Max(Max.y, p.z));
    }

    public void MakeSquare()
    {
        float sizeX = Max.x - Min.x;
        float sizeY = Max.y - Min.y;

        float maxSize = Mathf.Max(sizeX, sizeY);

        Vector2 center = (Min + Max) * 0.5f;
        Vector2 halfSize = new Vector2(maxSize, maxSize) * 0.5f;

        Min = center - halfSize;
        Max = center + halfSize;
    }

    public static Box FromPoints<T>(IReadOnlyList<T> items, Func<T, Vector3> getPos)
    {
        var b = Empty;
        for (int i = 0; i < items.Count; i++)
            b.Include(getPos(items[i]));

        b.MakeSquare();
        return b;
    }

    public Vector2 Center => new Vector2((Min.x + Max.x) * 0.5f, (Min.y + Max.y) * 0.5f);
}

public struct Node
{
    public int[] Children;
    public Box Bounds;

    public Node(Box bounds)
    {
        Bounds = bounds;
        Children = new[] { -1, -1, -1, -1 };
    }
}

public sealed class Quadtree<T>
{
    public Box Bounds;
    public readonly List<T> Items = new();
    public int Root;
    public readonly List<Node> Nodes = new();

    const int Null = -1;
    const int MaxDepth = 4;

    readonly Func<T, Vector3> _getPos;

    public Quadtree(Func<T, Vector3> getPos)
    {
        _getPos = getPos;
    }

    public static Quadtree<T> Build(IEnumerable<T> items, Func<T, Vector3> getPos)
    {
        var tree = new Quadtree<T>(getPos);
        tree.Items.AddRange(items);

        if (tree.Items.Count == 0)
        {
            tree.Bounds = Box.Empty;
            tree.Root = Null;
            return tree;
        }

        tree.Bounds = Box.FromPoints(tree.Items, getPos);
        tree.Root = tree.BuildImpl(0, tree.Items.Count, tree.Bounds, MaxDepth);
        return tree;
    }

    int BuildImpl(int begin, int end, Box bbox, int depthLimit)
    {
        int id = Nodes.Count;
        Nodes.Add(new Node(bbox));

        if (depthLimit == 0)
            return id;
        
        var center = bbox.Center;

        int splitY = Partition(begin, end, item => _getPos(item).z < center.y);
        int splitXLower = Partition(begin, splitY, item => _getPos(item).x < center.x);
        int splitXUpper = Partition(splitY, end, item => _getPos(item).x < center.x);

        Box box00 = new Box { Min = bbox.Min, Max = center };
        if (Items.Any(i => PositionInsideBox(_getPos(i), box00)))
            SetChild(id, 0, 0, BuildImpl(begin, splitXLower, box00, depthLimit - 1));

        Box box01 = new Box { Min = new Vector2(center.x, bbox.Min.y), Max = new Vector2(bbox.Max.x, center.y) };
        if (Items.Any(i => PositionInsideBox(_getPos(i), box01)))
            SetChild(id, 0, 1, BuildImpl(splitXLower, splitY, box01, depthLimit - 1));

        Box box10 = new Box { Min = new Vector2(bbox.Min.x, center.y), Max = new Vector2(center.x, bbox.Max.y) };
        if (Items.Any(i => PositionInsideBox(_getPos(i), box10)))
            SetChild(id, 1, 0, BuildImpl(splitY, splitXUpper, box10, depthLimit - 1));

        Box box11 = new Box { Min = center, Max = bbox.Max };
        if (Items.Any(i => PositionInsideBox(_getPos(i), box11)))
            SetChild(id, 1, 1, BuildImpl(splitXUpper, end, box11, depthLimit - 1));

        return id;
    }

    void SetChild(int nodeId, int y, int x, int child)
    {
        var children = Nodes[nodeId].Children;
        children[y * 2 + x] = child;
        var n = Nodes[nodeId];
        n.Children = children;
        Nodes[nodeId] = n;
    }

    int Partition(int begin, int end, Func<T, bool> pred)
    {
        int i = begin, j = end - 1;
        while (i <= j)
        {
            while (i <= j && pred(Items[i])) i++;
            while (i <= j && !pred(Items[j])) j--;
            if (i < j)
            {
                (Items[i], Items[j]) = (Items[j], Items[i]);
                i++;
                j--;
            }
        }
        return i;
    }

    bool PositionInsideBox(Vector3 position, Box box)
    {
        Vector2 min = box.Min;
        Vector2 max = box.Max;

        return position.x >= min.x && position.x <= max.x &&
               //position.y >= min.y && position.y <= max.y &&
               position.z >= min.y && position.z <= max.y;
    }
}