using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Node : MonoBehaviour
{
    public TextMeshPro textComponent;
    public List<Node> childNodes = new List<Node>(); 
    public Node parentNode; 

    public int nodeLayer = 0; 
    public int maxAllowedChildren = 4;
    private LineRenderer lineRenderer;

    void Awake()
    {
        // BUG FIX: Force clear the list on spawn in case the Unity Prefab accidentally saved old references
        childNodes = new List<Node>(); 
        parentNode = null;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
    }

    public void Initialize(string idea, int layer, int maxChildren, Node parent)
    {
        if (textComponent != null) textComponent.text = idea;
        nodeLayer = layer;
        maxAllowedChildren = maxChildren;
        parentNode = parent;
    }

    void Update()
    {
        // Draw exactly one line back to the parent to prevent overlapping webs
        if (parentNode != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, parentNode.transform.position);
        }
        else
        {
            lineRenderer.positionCount = 0; 
        }
    }
}