using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    private float m_DefaultLength = 5.0f;
    public GameObject m_Dot;
    public VRInputModule m_InputModule;

    private LineRenderer m_LineRenderer = null;

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        m_LineRenderer.enabled = m_InputModule.IsWorldStarted;
        m_Dot.SetActive(m_LineRenderer.enabled);

        if (!m_LineRenderer.enabled) return;
         
        // Use default or distance
        PointerEventData data = m_InputModule.GetData(); // EventSystem.Data

        float targetLength = data.pointerCurrentRaycast.distance;
        if (targetLength <= 0)
        {
            targetLength = m_DefaultLength;
        }

        // Raycast
        RaycastHit hit = CreateRaycast(targetLength);

        // Deafult
        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        // Or Based on hit
        if (hit.collider != null)
        {
            endPosition = hit.point;
        }

        // Set position of the dot
        m_Dot.transform.position = endPosition;

        // Set LineRenderer
        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, endPosition);
    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, m_DefaultLength);

        return hit;
    }
}
