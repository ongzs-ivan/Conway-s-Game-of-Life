using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellManager : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    public bool randomizeAtStart = false;
    public bool start = true;

    public GameObject cameraObject;

    public InputField widthField;
    public InputField heightField;

    public Slider simSpeedSlider;

    [Range(0.00001f, 1.5f)] public float updateRate = 0.1f;

    private float timer = 0.0f;
    private bool isPaused = true;
    private const int maxWidth = 192;
    private const int maxHeight = 108;


    [Space]
    public Sprite tile;

    private Cell[,] cells = new Cell[maxWidth, maxHeight];
    private int[,] states = new int[maxWidth, maxHeight];
    private int[,] rule = new int[maxWidth, maxHeight];
    
    public void PlayPauseFunction()
    {
        if (isPaused)
        {
            isPaused = false;
        }
        else
        {
            isPaused = true;
        }
    }

    void Start()
    {
        CreateGrid(gridSizeX, gridSizeY);
        if (randomizeAtStart)
        {
            RandomizeGrid();
        }
    }

    private void ClickOnCell()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f))
            {
                hit.collider.gameObject.GetComponent<Cell>().SwitchState();
            }
        }
    }

    private void RepositionCamera()
    {
        float offsetY = -10;
        Vector3 tempPos = new Vector3(transform.position.x + gridSizeX / 2, offsetY + transform.position.y + gridSizeY / 2, -10.0f);
        cameraObject.transform.position = tempPos;
    }

    public void QuitGameButton()
    {
        //Debug.Log("Quitting Game...");
        Application.Quit();
    }

    private void Update()
    {
        ClickOnCell();
        
        timer += Time.deltaTime;
        if (timer >= updateRate)
        {
            if (!isPaused)
                UpdateStates();

            timer = 0.0f;
        }
    }
    
    public void RandomizeGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //0 or 1
                cells[x, y].SetState(Random.Range(0, 2), 0);
            }
        }
    }

    void UpdateStates()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                int state = cells[x, y].state;
                int result = state;
                int rl = cells[x, y].currentRl;
                int count = GetLivingNeighbours(x, y);
                
                if (state == 1 && count < 2)
                {
                    result = 0;
                    rl = 0;
                }
                if (state == 1 && (count == 2 || count == 3))
                {
                    result = 1;
                    rl = 1;
                }
                if (state == 1 && count > 3)
                {
                    result = 0;
                    rl = 2;
                }
                if (state == 0 && count == 3)
                {
                    result = 1;
                    rl = 3;
                }

                states[x, y] = result;
                rule[x, y] = rl;
            }
        }
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                cells[x, y].SetState(states[x, y], rule[x, y]);
            }
        }
    }
    
    void CreateGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject temp = new GameObject("x: " + x + " y: " + y);
                temp.AddComponent<Cell>().CreateCell(new int[] { x, y }, tile);
                cells[x, y] = temp.GetComponent<Cell>();
            }
        }
        RepositionCamera();
    }

    private void DeleteGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Destroy(cells[x, y].gameObject);
            }
        }
    }

    public void KillAliveCells()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                states[x, y] = 0;
                rule[x, y] = 0;
                
                cells[x, y].SetState(states[x, y], rule[x, y]);
            }
        }
    }

    int GetLivingNeighbours(int x, int y)
    {
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int col = (x + i + gridSizeX) % gridSizeX;
                int row = (y + j + gridSizeY) % gridSizeY;
                
                count += cells[col, row].state;
            }
        }
        count -= cells[x, y].state;

        return count;
    }

    public void ApplyGridSizeChange()
    {
        KillAliveCells();
        DeleteGrid();
        gridSizeX = int.Parse(widthField.text);
        gridSizeY = int.Parse(heightField.text);

        if (gridSizeX < 1)
        {
            gridSizeX = 1;
        }
        if (gridSizeY < 1)
        {
            gridSizeY = 1;
        }

        if (gridSizeX > maxWidth)
        {
            gridSizeX = maxWidth;
        }
        if (gridSizeY > maxHeight)
        {
            gridSizeY = maxHeight;
        }

        //Debug.Log("New Width: " + gridSizeX);
        //Debug.Log("New Height: " + gridSizeY);

        CreateGrid(gridSizeX, gridSizeY);
        RandomizeGrid();
    }

    public void SimSpeedChange()
    {
        updateRate = simSpeedSlider.value;
    }
}
