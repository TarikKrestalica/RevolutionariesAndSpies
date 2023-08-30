using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines how my graph will be generated using these set of rules
[System.Serializable]
public class GraphRepresentation
{
    public List<int> hostSet = new List<int>(); 
    public List<Func<int, int>> assignmentFunction = new List<Func<int, int>>();
    public int conflictRule = 1;
}

// The idea behind the code: https://digitalcommons.uri.edu/cgi/viewcontent.cgi?article=1016&context=oa_diss
public class Graph : ScriptableObject
{
    public Vertex vertex;
    public GameObject edge;
    public string structure;
    [SerializeField] private char shortHand;
    [SerializeField] private int order;

    protected List<Vector3> positions = new List<Vector3>();
    protected List<GameObject> V = new List<GameObject>();
    protected List<HashSet<int>> E = new List<HashSet<int>>();
    public GraphRepresentation r;
    private Vector3 scaleFactor;
    protected Controller controller;

    public void SetController(Controller c)
    {
        controller = c;
    }

    public void PrepareDrawing()
    {
        switch (structure)
        {
            case "Path":
                FillHostSet(order + 1);
                SetAssignmentFunction(new List<Func<int, int>>() { (num) => num, (num) => num + 1 });
                GenerateAndAssignSubsets(order);
                break;
            case "Cycle":
                FillHostSet(order);
                SetAssignmentFunction(new List<Func<int, int>>() { (num) => num, (num) => (num + 1) % order });
                GenerateAndAssignSubsets(order);
                break;
            case "Complete Graph":
                FillHostSet(order);
                SetAssignmentFunction(new List<Func<int, int>>() { (num) => 0 });
                GenerateAndAssignSubsets(order);
                break;
            case "Star":
                FillHostSet(order);
                SetAssignmentFunction(new List<Func<int, int>>() { (num) => num });
                GenerateAndAssignSubsets(order);
                break;
            default:
                break;
        }
    }

    public void FillHostSet(int limit)
    {
        for (int i = 0; i < limit; i++)
        {
            r.hostSet.Add(i);
        }
    }

    public void SetAssignmentFunction(List<Func<int, int>> funcs)
    {
        foreach (Func<int, int> f in funcs)
            r.assignmentFunction.Add(f);
    }

    // Create and assign subsets of the host set for each vertex
    public void GenerateAndAssignSubsets(int constraint)
    {
        for (int i = 0; i < constraint; i++)
        {
            if (i == 0 && structure == "Star")  // For stars, the center vertex's set will be the entire host set
            {
                V[i].GetComponent<Vertex>().SetSubset(new HashSet<int>(r.hostSet));
                continue;
            }

            HashSet<int> result = CreateSubset(i);
            if (!result.IsSubsetOf(r.hostSet))
                continue;

            if(V[i] != null)
                V[i].GetComponent<Vertex>().SetSubset(result);
        }
    }

    protected HashSet<int> CreateSubset(int value)
    {
        HashSet<int> sub = new HashSet<int>();
        foreach (Func<int, int> f in r.assignmentFunction)
            sub.Add(f(value));

        return sub;
    }

    public void CreateGraph()
    {
        for (int i = 0; i < order; i++)
        {
            for (int j = 0; j < order; j++)
            {
                if (i == j) continue;  // Same vertex?
                if (IsThereEdge(new HashSet<int>() { i, j }))  // Does the edge already exist?
                    continue;

                if (V[j] == null)
                    continue;

                HashSet<int> set = new HashSet<int>(V[j].GetComponent<Vertex>().GetSubset());  // Avoid overwriting the vertex's member subset
                if (V[i] == null)
                    continue;

                set.IntersectWith(V[i].GetComponent<Vertex>().GetSubset());
                if (set.Count < r.conflictRule)
                    continue;

                GameObject first = V[i];
                GameObject second = V[j];
                PlaceEdge(ref first, ref second, Controller.e, Controller.edges);
                E.Add(new HashSet<int>() { i, j });
            }
        }
    }

    bool IsThereEdge(HashSet<int> vertexPair)
    {
        foreach (HashSet<int> edge in E)
        {
            if (vertexPair.SetEquals(edge))
                return true;
        }
        return false;
    }

    // Scale edges when needed
    void PlaceEdge(ref GameObject v1, ref GameObject v2, GameObject eb, List<GameObject> edges)  
    {
        // Apply Pythagoreas
        float width = v2.GetComponent<RectTransform>().localPosition.y - v1.GetComponent<RectTransform>().localPosition.y;
        float length = v2.GetComponent<RectTransform>().localPosition.x - v1.GetComponent<RectTransform>().localPosition.x;
        float hypotenuse = (float)(Math.Sqrt(Math.Pow(width, 2) + Math.Pow(length, 2)));

        // Apply object between the two vertices
        GameObject e = Controller.CreateObject(edge, eb, new Vector3(v2.GetComponent<RectTransform>().localPosition.x - (length / 2), v2.GetComponent<RectTransform>().localPosition.y - (width / 2)));
        e.GetComponent<RectTransform>().sizeDelta = new Vector3(hypotenuse - vertex.GetComponent<RectTransform>().sizeDelta.x, edge.GetComponent<RectTransform>().sizeDelta.y, 0);

        // Apply proper angle of rotation
        double angle = Math.Acos(length / hypotenuse) * (180 / Math.PI);
        if (width < 0)
            angle *= -1;

        e.transform.Rotate(0, 0, (float)angle, Space.Self);
        edges.Add(e);

    }

    public void FindScale()
    {
        if (order <= 10)
            scaleFactor = Vector3.zero;
        else if (order > 10 && order <= 12)
            scaleFactor = new Vector3(-0.1f, -0.1f, 0);
        else if (order > 12 && order <= 14)
            scaleFactor = new Vector3(-0.3f, -0.3f, 0);
    }

    public Vector3 GetScale()
    {
        return scaleFactor;
    }

    public void PopulateVertices()  // Add some scaling depending on order of graph
    {
        for (int i = 0; i < order; i++)
        {
            GameObject vert = Controller.CreateObject(vertex.node, Controller.v, positions[i]);
            vert.GetComponent<Vertex>().SetVertID(i + 1);
            vert.GetComponent<Transform>().localScale += GetScale();
            vert.GetComponent<Vertex>().UpdateScaleForWarning(GetScale() * -2);
            V.Add(vert);
        }
    }

    // Based on the graph structure the player wants to model, return an array of positions to apply as I create my vertices
    public void RetrievePositions()
    {
        if (structure == "Star")
        {
            FindPositions(ref positions, Vector3.zero, order - 1, order * 30);  // After central vertex, account for the order - 1 vertices remaining
        }
        else if (structure == "Cycle" || structure == "Complete Graph" || structure == "Path")
        {
            FindPositions(ref positions, new Vector3(order * 30, 0, 0), order, order * 30); 
        }
        else
        {
            return;
        }
    }

    // Any of the 4 structures as of right now
    public void FindPositions(ref List<Vector3> v3s, Vector3 startPos, int constraint, float radius)
    {
        for(int i = 0; i < constraint + 1; i++)
        {
            if(i == 0)  
            {
                v3s.Add(startPos);
                continue;
            }

            float angle = (float)((i * (360 / constraint)) * (Math.PI / 180));
            v3s.Add(new Vector3(radius * (float)Math.Cos(angle), radius * (float)Math.Sin(angle), 0));
        }
    }

    public void SetOrder(int num)
    {
        order = num;
    }

    public int GetOrder()
    {
        return order;
    }

    public List<GameObject> GetVertices()
    {
        return V;
    }

    public List<HashSet<int>> GetEdges()
    {
        return E;
    }

    public List<Vector3> GetPositions()
    {
        return positions;
    }

    public char GetShortHand()
    {
        return shortHand;
    }
    
}
