using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PathNode : MonoBehaviour
{
    [SerializeField] private List<PathNode> m_links;

    private int m_selectedNode = 0;

    private Dictionary<Sprite, int> m_counter = new Dictionary<Sprite, int>();

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
        UpdateRotation();
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

    private void UpdateRotation()
    {
        if (m_links.Count == 0)
        {
            return;
        }

        Vector2 dir = ((Vector2)getNextNode().transform.position - (Vector2)transform.position).normalized;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (!sr)
        {
            return;
        }


        float angle = Vector2.SignedAngle(new Vector2(1, 0), dir);
        // Debug.Log(angle);
        // Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + 10 * dir, Color.red);
        sr.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
    }

    public void IncreaseCounter(Sprite sprite)
    {
        if (m_counter.ContainsKey(sprite))
        {
            m_counter[sprite]++;
        }
        else
        {
            m_counter[sprite] = 1;
        }
    }

    public string CounterToString()
    {
        string str = "";

        foreach (Sprite sprite in m_counter.Keys)
        {
            str += string.Format("{0}: {1}\n", sprite.name, m_counter[sprite]);
        }

        return str;
    }
}
