using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[System.Serializable]
public class GiftReceivedEvent : UnityEvent<PathNode>
{
}

public class Gift : MonoBehaviour
{
    [SerializeField] private Sprite[] m_gifts;
    private int selectedGiftIndex = 0;

    public GiftReceivedEvent m_event;

    public PathNode m_prevNode;
    public PathNode m_nextNode;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        UpdateNodes();
        UpdatePosition();
        UpdateScore();
    }

    private void Initialize()
    {
        SpriteRenderer[] renderes = GetComponentsInChildren<SpriteRenderer>();
        Assert.IsTrue(renderes.Length >= 2);

        selectedGiftIndex = Random.Range(0, m_gifts.Length);
        renderes[1].sprite = m_gifts[selectedGiftIndex];
    }

    private void UpdateNodes()
    {
        if (!m_nextNode)
        {
            return;
        }

        const float distanceThreshold = 0.01f;
        if (Vector2.Distance(transform.position, m_nextNode.transform.position) > distanceThreshold)
        {
            return;
        }

        m_prevNode = m_nextNode;
        transform.position = (Vector3)((Vector2)m_prevNode.transform.position) + new Vector3(0, 0, transform.position.z);
        m_nextNode = m_prevNode.getNextNode();
    }

    private void UpdatePosition()
    {
        if (!m_nextNode)
        {
            return;
        }

        Vector2 dir = ((Vector2)m_nextNode.transform.position - (Vector2)transform.position).normalized;

        const float speed = 1.25f;

        Vector2 newPos = (Vector2)(transform.position) + dir * speed * Time.deltaTime;

        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //if (!rb)
        //{
        //    return;
        //}
        // rb.velocity = dir * speed;

        if (IsPlaceFree(newPos))
        {
            transform.position = (Vector3)(newPos) + new Vector3(0, 0, transform.position.z);
        }
    }

    public bool IsPlaceFree(Vector2 pos, bool absolute = false)
    {
        Collider2D c2d = GetComponent<Collider2D>();
        if (!c2d)
        {
            return false;
        }

        bool isPlaceFree = true;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, c2d.bounds.extents.magnitude);
        foreach (Collider2D collider in colliders)
        {
            Gift otherGift = collider.GetComponent<Gift>();
            if (!otherGift)
            {
                continue;
            }
            if (this == otherGift)
            {
                continue;
            }

            if (absolute)
            {
                isPlaceFree = false;
                break;
            }

            if (m_prevNode == otherGift.m_prevNode && m_nextNode == otherGift.m_nextNode)
            {
                isPlaceFree = false;
                break;
            }

            if (m_nextNode == otherGift.m_prevNode)
            {
                isPlaceFree = false;
                break;
            }

            if (m_nextNode == otherGift.m_nextNode)
            {
                float distanceToNode = Vector2.Distance(transform.position, m_nextNode.transform.position);
                float otherDistanceToNode = Vector2.Distance(otherGift.transform.position, m_nextNode.transform.position);
                if (distanceToNode > otherDistanceToNode)
                {
                    isPlaceFree = false;
                    break;
                }
            }
        }

        return isPlaceFree;
    }

    private void UpdateScore()
    {
        if (m_nextNode)
        {
            return;
        }

        if (m_event == null)
        {
            return;
        }

        m_event.Invoke(m_prevNode);

        Destroy(gameObject);
    }
}
