using UnityEngine;

public class DragDrop : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;
    Collider2D collider2D;
    public static bool isDragDropOn = true;
    void Start()
    {
        cam = Camera.main;
        collider2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    Vector3 distance;
    private void OnMouseDown()
    {
        distance = transform.position - getMousePos();
    }

    private void OnMouseUp()
    {
        collider2D.enabled = false;
        Vector3 raycastStart = transform.position;
        Vector3 raycastDirection = Vector3.forward;

        RaycastHit2D raycastHit;
        if (raycastHit = Physics2D.Raycast(raycastStart, raycastDirection))
        {
            if (raycastHit.transform.tag == "dropable")
            {
                transform.position = raycastHit.transform.position + new Vector3(0, 0, -0.01f);
            }
            else if (raycastHit.transform.tag == "pitchArea" || raycastHit.transform.tag == "spawnArea")
            {
                foreach (Scraper.Player player in Scraper.team1.players)
                {
                    if (player.attachedGameObject == gameObject)
                    {
                        if (raycastHit.transform.tag == "pitchArea")
                        {
                            player.attachedGameObject.GetComponent<PlayerObject>().isInPitch = true;
                            player.isInPitch = true;
                        }
                        if (raycastHit.transform.tag == "spawnArea")
                        {
                            player.attachedGameObject.GetComponent<PlayerObject>().isInPitch = false;
                            player.isInPitch = false;
                        }
                    }
                }
            }
        }
        collider2D.enabled = true;
    }

    Vector3 getMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePos);
    }

    private void OnMouseDrag()
    {
        if (isDragDropOn)
        {
            transform.position = getMousePos() + distance;
        }
    }
}
