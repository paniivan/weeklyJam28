﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PathNode : MonoBehaviour
{
    [SerializeField] private List<PathNode> m_links;

    private int m_selectedNode = 0;

    public List<PathNode> getLinks()
    {
        return m_links;
    }

    public PathNode getNextNode()
    {
        if (m_links.Count == 0)
        {
            return null;
        }

        Assert.IsTrue(0 <= m_selectedNode && m_selectedNode <= m_links.Count - 1);
        return m_links[m_selectedNode];
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDrawGizmos()
    {
        DrawLines();
    }

    private void DrawLines()
    {
        Vector3 begin = transform.position;

        foreach (PathNode node in m_links)
        {
            if (!node)
            {
                return;
            }

            Vector3 end = node.GetComponent<Transform>().position;
            Gizmos.DrawLine(begin, end);
        }
    }

    void OnMouseDown()
    {
        if (m_links.Count == 0)
        {
            return;
        }

        m_selectedNode = (m_selectedNode + 1) % m_links.Count;
    }
}