using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class Items : MonoBehaviour
{
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    [Header("Swipe Stuff")]
    public float swipeAngle=0;
    public float swipeResist=1f;
    [Header("Power Stuff")]
    public bool isColumnBom;
    public bool isRowBom;
    public bool isColorBom;
    public bool isAdjacentBom;
    public GameObject adjacentMarker;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBom;

    [Header("Board Variable")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public GameObject ortherItem;
    private Board board;
    private FindMatches findMatches;

    public bool isMatched = false;
    void Start()
    {
        isColumnBom= false;
        isRowBom= false;
        isAdjacentBom= false;
        findMatches = FindObjectOfType<FindMatches>();
        board = FindObjectOfType<Board>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousColumn = column;
        //previousRow = row;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            isAdjacentBom = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }
    
    void Update()
    {
        //FindMatched();
        //if (isMatched)
        //{
        //    SpriteRenderer mysprite = GetComponent<SpriteRenderer>();
        //    mysprite.color = new Color(1f,1f,1f,.2f);
        //}
        targetX = column;
        targetY=row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX,transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allitems[column,row] != this.gameObject) 
            {
                board.allitems[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
           
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x,targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allitems[column, row] != this.gameObject)
            {
                board.allitems[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches(); 
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
       
        }
    }
    private void OnMouseDown()
    {
        if(board.currnetState == GameSate.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

    }
    private void OnMouseUp()
    {
        if(board.currnetState == GameSate.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            calculateAngle();
        }
       
    }
    void calculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currnetState = GameSate.wait; 
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            movePieces();
            board.currentItem = this;
        }
        else
        {
            board.currnetState= GameSate.move;
           
        }
        
    }

    public IEnumerator CheckMoveCo()
    {
        if (isColorBom)
        {
            findMatches.MatchPieccesOfColor(ortherItem.tag);
            isMatched = true;
        }
        else if(ortherItem.GetComponent<Items>().isColorBom)
        {
            findMatches.MatchPieccesOfColor(this.gameObject.tag);
            ortherItem.GetComponent<Items>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if(ortherItem!= null)
        {
            if(!isMatched && !ortherItem.GetComponent<Items>().isMatched)
            {
                ortherItem.GetComponent<Items>().row =row;
                ortherItem.GetComponent<Items>().column =column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentItem = null;
                board.currnetState = GameSate.move;
            }
            else
            {
                board.DestroyMacthes();
            }
            //ortherItem = null;
        }
        
    }

    void MovePieceActual(Vector2 direction)
    {
        ortherItem = board.allitems[column + (int)direction.x, row + (int)direction.y];
        previousColumn = column;
        previousRow = row;
        ortherItem.GetComponent<Items>().column += -1 * (int)direction.x;
        ortherItem.GetComponent<Items>().row += -1 * (int)direction.y;
        column += (int)direction.x;
        row += (int)direction.y;
        StartCoroutine(CheckMoveCo());
    }
    void movePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.Width -1)
        {
            //ortherItem = board.allitems[column+1, row];
            //previousColumn = column;
            //previousRow = row;
            //ortherItem.GetComponent<Items>().column -= 1;
            //column+= 1;
            //StartCoroutine(CheckMoveCo());
            MovePieceActual(Vector2.right);
            
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.Hieght - 1)
        {
            //ortherItem = board.allitems[column , row + 1];
            //previousColumn = column;
            //previousRow = row;
            //ortherItem.GetComponent<Items>().row -= 1;
            //row += 1;
            //StartCoroutine(CheckMoveCo());
            MovePieceActual(Vector2.up);
           
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //ortherItem = board.allitems[column - 1, row];
            //previousColumn = column;
            //previousRow = row;
            //ortherItem.GetComponent<Items>().column += 1;
            //column -= 1;
            //StartCoroutine(CheckMoveCo());
            MovePieceActual(Vector2.left);

        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //ortherItem = board.allitems[column, row - 1];
            //previousColumn = column;
            //previousRow = row;
            //ortherItem.GetComponent<Items>().row += 1;
            //row -= 1;
            //StartCoroutine(CheckMoveCo());
            MovePieceActual(Vector2.down);

        }
        else
        {
            board.currnetState = GameSate.move;
        }
    }

    void FindMatched()
    {
        if(column > 0 && column < board.Width - 1)
        {
            GameObject lefItem1= board.allitems[column - 1, row];
            GameObject rightItem1= board.allitems[column + 1, row];
            if(lefItem1!= null && rightItem1!= null) 
            {
                if (lefItem1.tag == this.gameObject.tag && rightItem1.tag == this.gameObject.tag)
                {
                    lefItem1.GetComponent<Items>().isMatched = true;
                    rightItem1.GetComponent<Items>().isMatched = true;
                    isMatched = true;
                }
            }
            
        }

        if (row > 0 && row < board.Hieght - 1)
        {
            GameObject upItem1 = board.allitems[column, row + 1];
            GameObject downItem1 = board.allitems[column, row - 1];
            if(upItem1!= null && downItem1!= null)
            {
                if (upItem1.tag == this.gameObject.tag && downItem1.tag == this.gameObject.tag)
                {
                    upItem1.GetComponent<Items>().isMatched = true;
                    downItem1.GetComponent<Items>().isMatched = true;
                    isMatched = true;
                }
            }
           
        }
    }

    public void MakeRowBom()
    {
        isRowBom = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBom()
    {
        isColumnBom = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColorBom()
    {
        isColorBom = true;
        GameObject color = Instantiate(colorBom, transform.position, Quaternion.identity);
        color.transform.parent = this.transform;
    }
    
    public void MakeAdjacentBom()
    {
        isAdjacentBom = true;
        GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }
}
