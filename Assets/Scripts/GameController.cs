using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private float m_spawnInterval = 1.0f;
    [SerializeField] private List<PathNode> m_spawners;
    [SerializeField] private List<PathNode> m_receivers;
    [SerializeField] private List<GameObject> m_gifts;
    [SerializeField] private Text m_scoreText;
    [SerializeField] private GameObject m_railwayTile;

    private float m_timeSinceStart;

    public int m_score { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        m_timeSinceStart = 0.0f;
        m_score = 0;

        InstantiateRailway();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpawners();
        UpdateScoreUI();
    }

    private void UpdateSpawners()
    {
        if (m_spawners.Count == 0 || m_gifts.Count == 0)
        {
            return;
        }

        m_timeSinceStart += Time.deltaTime;

        int spawned = (int)((m_timeSinceStart - Time.deltaTime) / m_spawnInterval);
        int toBeSpawned = (int)((m_timeSinceStart) / m_spawnInterval);

        for (int i = 0; i < toBeSpawned - spawned; ++i)
        {
            int selectedSpawner = Random.Range(0, m_spawners.Count);
            int selectedGift = Random.Range(0, m_gifts.Count);

            Vector3 giftPosition = new Vector3(m_spawners[selectedSpawner].transform.position.x,
                m_spawners[selectedSpawner].transform.position.y,
                m_gifts[selectedGift].transform.position.z);



            GameObject giftObject = Instantiate(m_gifts[selectedGift], giftPosition, Quaternion.identity);
            Gift gift = giftObject.GetComponent<Gift>();
            if (!gift)
            {
                continue;
            }

            if (!gift.IsPlaceFree(gift.transform.position, true))
            {
                Destroy(gift.gameObject);
                break;
            }

            gift.m_prevNode = m_spawners[selectedSpawner];

            List<PathNode> links = m_spawners[selectedSpawner].getLinks();
            if (links.Count == 0)
            {
                continue;
            }

            gift.m_nextNode = links[0];

            gift.m_event = new GiftReceivedEvent();
            gift.m_event.AddListener(OnGiftReceived);
        }
    }

    public void OnGiftReceived(PathNode receiver, Sprite sprite)
    {
        PathNode result = m_receivers.Find(item => item == receiver);
        if (!result)
        {
            return;
        }

        result.IncreaseCounter(sprite);

        m_score += 10;
    }

    private void UpdateScoreUI()
    {
        if (!m_scoreText)
        {
            return;
        }

        m_scoreText.text = string.Format("{0}: {1}", "Score", m_score);
    }

    private void InstantiateRailway()
    {
        if (m_spawners.Count == 0 || !m_railwayTile)
        {
            return;
        }

        List<PathNode> s = new List<PathNode>();
        foreach (PathNode node in m_spawners)
        {
            TraverseNode(s, node);
        }
    }

    private void TraverseNode(List<PathNode> s, PathNode node)
    {
        if (s.Find(item => item == node))
        {
            return;
        }

        var links = node.getLinks();
        if (links.Count == 0)
        {
            return;
        }

        Sprite sp = m_railwayTile.GetComponent<SpriteRenderer>().sprite;
        if (!sp)
        {
            return;
        }

        foreach (PathNode otherNode in links)
        {
            var from = (Vector2)node.transform.position;
            var to = (Vector2)otherNode.transform.position;

            Vector2 dir = (to - from).normalized;
            float distance = (to - from).magnitude;

            float spWidth = sp.bounds.size.x;

            int tilesCount = (int)(distance / spWidth) + 1;

            for (int i = 0; i < tilesCount; ++i)
            {
                float angle = Vector2.SignedAngle(new Vector2(1, 0), dir);

                Vector2 pos = (Vector2)node.transform.position + (dir * (i * spWidth + spWidth / 2.0f));
                Instantiate(m_railwayTile, (Vector3)pos + m_railwayTile.transform.position, Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)));
            }
        }

        s.Add(node);

        foreach (PathNode otherNode in links)
        {
            TraverseNode(s, otherNode);
        }
    }
}
