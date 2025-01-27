using UnityEngine;
//using UnityEngine.UIElements;

public class DrawingOnScreen : MonoBehaviour
{
    public Camera cam;
    public GameObject inkPrefab;
    public GameObject drawCanvas;
    public GameObject pitchArea;
    public GameObject chooseMenu;
    public bool isPenSelected;
    public bool isArrowSelected;
    public bool showDrawings;
    public bool isHandSelected;
    public GameObject arrowPrefab;
    public LineRenderer sizeDisplay;


    // Start is called before the first frame update
    void Start()
    {
        sizeDisplay.colorGradient = inkPrefab.GetComponent<LineRenderer>().colorGradient;
    }

    // Update is called once per frame

    float timePassed = 0;
    void Update()
    {
        if (isPenSelected && showDrawings && isInDrawableArea())
        {
            mouseInput();
        }
        else if (isArrowSelected && showDrawings && isInDrawableArea())
        {
            createArrow();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            isInDrawableArea();
        }


        if (isHoldingIncrease)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= 0.1f)
            {
                float multi = Input.GetKey(KeyCode.LeftShift) ? 5f : 1f;
                if ((0.01f * multi + sizeDisplay.startWidth) < 1f)
                {
                    sizeDisplay.startWidth += 0.01f * multi;
                    inkPrefab.GetComponent<LineRenderer>().startWidth = sizeDisplay.startWidth;
                }
                else if ((0.01f * multi + sizeDisplay.startWidth) > 1f)
                {
                    sizeDisplay.startWidth = 1f;
                    inkPrefab.GetComponent<LineRenderer>().startWidth = sizeDisplay.startWidth;
                }
                timePassed = 0;

            }
        }
        else if (isHoldingDecrease)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= 0.1f)
            {
                float multi = Input.GetKey(KeyCode.LeftShift) ? 5f : 1f;
                if ((-0.01f * multi +sizeDisplay.startWidth) > 0.05f)
                {
                    sizeDisplay.startWidth -= 0.01f * multi;
                    inkPrefab.GetComponent<LineRenderer>().startWidth = sizeDisplay.startWidth;
                }
                else if ((-0.01f * multi + sizeDisplay.startWidth) < 0.05f)
                {
                    sizeDisplay.startWidth = 0.05f;
                    inkPrefab.GetComponent<LineRenderer>().startWidth = sizeDisplay.startWidth;
                }
                timePassed = 0;
            }
        }


        bool isAnySelected = isPenSelected || isArrowSelected || !isHandSelected;
        DragDrop.isDragDropOn = !isAnySelected;

        if(chooseMenu.activeInHierarchy)
        {
            isPenSelected = false;
            isArrowSelected = false;
        }

    }
    

    LineRenderer lR;

    bool isInDrawableArea()
    {
        Vector2 pos = mousePos();
        BoxCollider2D collider = pitchArea.GetComponent<BoxCollider2D>();
        Vector2 max = collider.bounds.max;
        Vector2 min = collider.bounds.min;
        if (pos.x < max.x && pos.x > min.x && pos.y < max.y && pos.y > min.y)
        {
            return true;
        }
        return false;
    }

    void mouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //first click
            GameObject ink = Instantiate(inkPrefab);
            ink.transform.SetParent(drawCanvas.transform, false);
            lR = ink.GetComponent<LineRenderer>();
        }
        if (Input.GetMouseButton(0))
        {
            //drag
            Vector3 pos = mousePos();
            lR.positionCount++;
            lR.SetPosition(lR.positionCount - 1, pos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                lR.Simplify(0.1f);
            }
        }
    }

    GameObject arrow;
    Vector3 firstPos;
    void createArrow()
    {

        if (Input.GetMouseButtonDown(0))
        {
            firstPos = mousePos();
            arrow = Instantiate(arrowPrefab);
            arrow.transform.SetParent(drawCanvas.transform, false);
            arrow.transform.position = firstPos;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 currentPos = mousePos();
            Vector3 difference = currentPos - firstPos;
            arrow.transform.localScale = new Vector3(difference.magnitude * 1.09f, 1, 1);
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }


    Vector2 mousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    


    //button funcs
    public void penSelectToggle()
    {
        isPenSelected = !isPenSelected;
        isArrowSelected = false;
        isHandSelected = false;
    }

    public void showDrawingToggle()
    {
        showDrawings = !showDrawings;
        drawCanvas.SetActive(showDrawings);
    }

    public void selectPenColor(GameObject inkType)
    {
        if (isPenSelected && showDrawings)
        {
            inkPrefab = inkType;
            sizeDisplay.colorGradient = inkType.GetComponent<LineRenderer>().colorGradient;
            inkPrefab.GetComponent<LineRenderer>().startWidth = sizeDisplay.startWidth;
        }
    }

    public void arrowSelectToggle()
    {
        isArrowSelected = !isArrowSelected;
        isPenSelected = false;
        isHandSelected = false;
    }

    public void selectArrowColor(GameObject arrowType)
    {
        if (isArrowSelected && showDrawings)
        {
            arrowPrefab = arrowType;
        }
    }

    public void handSelectToggle()
    {
        isHandSelected = !isHandSelected;
        isPenSelected = false;
        isArrowSelected = false;
    }

    bool isHoldingIncrease;
    bool isHoldingDecrease;
    public void holdIncrease()
    {
        isHoldingIncrease = !isHoldingIncrease;
    }

    public void holdDecrease()
    {
        isHoldingDecrease = !isHoldingDecrease;
    }


}
