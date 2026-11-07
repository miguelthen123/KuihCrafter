using UnityEngine;
using UnityEngine.InputSystem;

public class MindMapManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public Node rootNode; 
    
    // Increased distance to spread the nodes out more
    public float semanticDistance = 1.5f; 
    public float springStiffness = 15f;
    public float springDamper = 3f;

    public void GenerateChildNode(Node parentNode, string newIdea)
    {
        if (parentNode.childNodes.Count >= parentNode.maxAllowedChildren)
        {
            Debug.Log($"Limit reached for {parentNode.gameObject.name}.");
            return; 
        }

        Vector3 spawnDir = Vector3.zero;
        int currentChildCount = parentNode.childNodes.Count;

        // LAYER 1: Perfect Circle around the Root
        if (parentNode == rootNode) 
        {
            // Spaced evenly at 45°, 135°, 225°, and 315° angles
            float angle = (currentChildCount * 90f) + 45f; 
            float radians = angle * Mathf.Deg2Rad;
            
            // Convert angle to a 3D direction vector
            spawnDir = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians)).normalized; 
        }
        // LAYER 2: Radial fan-out from the parent's outward angle
        else 
        {
            // Find the exact direction the parent is sitting relative to the root
            Vector3 outwardFromRoot = (parentNode.transform.position - rootNode.transform.position).normalized;
            outwardFromRoot.y = 0; 
            
            // Convert that vector back into an angle (0 to 360)
            float parentAngle = Mathf.Atan2(outwardFromRoot.x, outwardFromRoot.z) * Mathf.Rad2Deg;
            
            // Fan the two children outward by +/- 45 degrees from the parent's angle
            float fanAngle = (currentChildCount == 0) ? 45f : -45f;
            float finalAngle = parentAngle + fanAngle;
            
            float radians = finalAngle * Mathf.Deg2Rad;
            spawnDir = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians)).normalized;
        }

        Vector3 spawnPos = parentNode.transform.position + (spawnDir * semanticDistance);

        // Instantiate
        GameObject newObj = Instantiate(nodePrefab, spawnPos, Quaternion.identity);
        int newLayer = parentNode.nodeLayer + 1;
        newObj.name = $"Node_L{newLayer}_{currentChildCount}"; 
        
        Node newNode = newObj.GetComponent<Node>();
        int newMaxChildren = (newLayer == 1) ? 2 : 0; 

        // Link and Initialize
        newNode.Initialize(newIdea, newLayer, newMaxChildren, parentNode);
        parentNode.childNodes.Add(newNode);

        // Physics limits updated to accommodate the wider spread
        SpringJoint joint = newObj.AddComponent<SpringJoint>();
        joint.connectedBody = parentNode.GetComponent<Rigidbody>();
        joint.spring = springStiffness;
        joint.damper = springDamper;
        joint.maxDistance = semanticDistance;
        joint.minDistance = semanticDistance * 0.7f; 

        newObj.GetComponent<Rigidbody>().AddForce(spawnDir * 4f, ForceMode.Impulse);
    }

    void Update()
    {
        // Interactive Raycast Testing via New Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Node clickedNode = hit.collider.GetComponent<Node>();
                if (clickedNode != null)
                {
                    GenerateChildNode(clickedNode, $"L{clickedNode.nodeLayer + 1} Idea");
                }
            }
        }
    }
}