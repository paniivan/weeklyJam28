using UnityEngine;

public class RouteFollow : MonoBehaviour
{
    [SerializeField]
    public GameObject m_routeFrom;
    public GameObject m_routeTo;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        const float speed = 1.0f;

        if (!m_routeFrom || !m_routeTo)
        {
            return;
        }

        Vector3 dir = (m_routeTo.transform.position - m_routeFrom.transform.position).normalized;
        Vector3 velocity = speed * dir;

        transform.position = transform.position + velocity * Time.deltaTime;
    }
}
