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

        transform.position = (Vector3)((Vector2)(transform.position) + dir * speed * Time.deltaTime) + new Vector3(0, 0, transform.position.z);
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
