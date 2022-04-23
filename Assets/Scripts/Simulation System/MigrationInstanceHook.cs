using System.Collections;
using UnityEngine;

public class MigrationInstanceHook : MonoBehaviour
{
    public float lifeTimer;
    public float yOffset;
    
    private Vector3 startPos;
    private Vector3 endPos;

    public GameObject agentVisual;
    private bool startTravel = false;
    

    private void Start()
    {
        StartCoroutine(LifeTime());
    }

    public void hook(Vector3 start, Vector3 end)
    {
        startPos = new Vector3(start.x, yOffset, start.z);
        endPos = new Vector3(end.x, yOffset, end.z);
        
        agentVisual.transform.position = startPos;
        startTravel = true;
        
        LineRenderer lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    void Update()
    {
        if (startTravel)
        {
            agentVisual.transform.position = Vector3.Lerp(agentVisual.transform.position, endPos, lifeTimer);
        }
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTimer);
        Destroy(this.gameObject);
    }
    
}