using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using static UnityEditor.Progress;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches= new List<GameObject>();
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBom(Items items1, Items items2, Items items3)
    {
        List<GameObject> currentItems = new List<GameObject>();
        if (items1.isAdjacentBom)
        {
            currentMatches.Union(GetAdjacentPieces(items1.column,items1.row));
        }
        if (items2.isAdjacentBom)
        {
            currentMatches.Union(GetAdjacentPieces(items2.column, items2.row));
        }
        if (items3.isAdjacentBom)
        {
            currentMatches.Union(GetAdjacentPieces(items3.column, items3.row));
        }
        return currentItems;
    }

    private List<GameObject> IsRowBom(Items items1,Items items2,Items items3 )
    {
        List<GameObject> currentItems = new List<GameObject>();
        if (items1.isRowBom)
        {
            currentMatches.Union(GetRowPieces(items1.row));
        }
        if (items2.isRowBom)
        {
            currentMatches.Union(GetRowPieces(items2.row));
        }
        if (items3.isRowBom)
        {
            currentMatches.Union(GetRowPieces(items3.row));
        }
        return currentItems;
    }

    private List<GameObject> IsColumnBom(Items items1, Items items2, Items items3)
    {
        List<GameObject> currentItems = new List<GameObject>();
        if (items1.isColumnBom)
        {
            currentMatches.Union(GetColumnPieces(items1.column));
        }
        if (items2.isColumnBom)
        {
            currentMatches.Union(GetColumnPieces(items2.column));
        }
        if (items3.isColumnBom)
        {
            currentMatches.Union(GetColumnPieces(items3.column));
        }
        return currentItems;
    }

    private void AddToListAndMatch(GameObject items)
    {
        if (!currentMatches.Contains(items))
        {
            currentMatches.Add(items);
        }
        items.GetComponent<Items>().isMatched = true;
    }
    private void GetNearByPieces(GameObject items1, GameObject items2, GameObject items3)
    {
       AddToListAndMatch(items1);
       AddToListAndMatch(items2);
       AddToListAndMatch(items3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i=0; i<board.Width; i++)
        {
            for(int j=0; j<board.Hieght; j++)
            {
                GameObject currentItem = board.allitems[i,j];
                
                if(currentItem != null)
                {
                    Items currentItemItem = currentItem.GetComponent<Items>();
                    if (i>0 && i< board.Width - 1)
                    {
                        GameObject lefItem = board.allitems[i-1,j];
                        GameObject rightItem = board.allitems[i+1,j];
                        if(lefItem!= null && rightItem != null)
                        {
                            Items rightItemTem = rightItem.GetComponent<Items>();
                            Items lefItemItem = lefItem.GetComponent<Items>();
                            if (lefItem != null && rightItem != null)
                            {
                                if (lefItem.tag == currentItem.tag && rightItem.tag == currentItem.tag)
                                {
                                    currentMatches.Union(IsRowBom(lefItemItem, currentItemItem, rightItemTem));

                                    currentMatches.Union(IsColumnBom(lefItemItem, currentItemItem, rightItemTem));
                                    currentMatches.Union(IsAdjacentBom(lefItemItem, currentItemItem, rightItemTem));

                                    GetNearByPieces(lefItem, currentItem, rightItem);
                                }
                            }
                        }
                        
                    }
                    if (j > 0 && j < board.Hieght - 1)
                    {
                        GameObject upItem = board.allitems[i , j+1 ];                        
                        GameObject downItem = board.allitems[i , j-1];
                        if(upItem!= null && downItem != null)
                        {
                            Items downItemItem = downItem.GetComponent<Items>();
                            Items upItemItem = upItem.GetComponent<Items>();
                            if (upItem != null && downItem != null)
                            {
                                if (upItem.tag == currentItem.tag && downItem.tag == currentItem.tag)
                                {
                                    currentMatches.Union(IsColumnBom(upItemItem, currentItemItem, downItemItem));
                                    currentMatches.Union(IsRowBom(upItemItem, currentItemItem, downItemItem));
                                    currentMatches.Union(IsAdjacentBom(upItemItem, currentItemItem, downItemItem));

                                    GetNearByPieces(upItem, currentItem, downItem);
                                }
                            }
                        }
                        
                    }
                }
            }
        }
    }
      
    List<GameObject> GetAdjacentPieces(int column , int row)
    {
        List<GameObject> items = new List<GameObject>();
        for(int i=column -1; i <= column + 1; i++)
        {
            for(int j = row -1 ; j<=row + 1;j++)
            {
                if(i>=0 && i< board.Width && j>=0 && j< board.Hieght)
                {
                    items.Add(board.allitems[i,j]);
                    board.allitems[i, j].GetComponent<Items>().isMatched = true;
                }
            }
        }
        return items;
    }
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> items = new List<GameObject>();
        for(int i = 0 ; i < board.Hieght; i++) 
        {
            if (board.allitems[column, i]!= null)
            {
                items.Add(board.allitems[column, i]);
                board.allitems[column,i].GetComponent<Items>().isMatched = true;
            }
        }
        return items;
    }

    public void MatchPieccesOfColor(string color)
    {
        for(int i=0;i<board.Width;i++)
        {
            for(int j = 0; j < board.Hieght; j++)
            {
                if (board.allitems[i,j]!= null)
                {
                    if (board.allitems[i,j].tag == color)
                    {
                        board.allitems[i,j].GetComponent<Items>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> items = new List<GameObject>();
        for (int i = 0; i < board.Width; i++)
        {
            if (board.allitems[i, row] != null)
            {
                items.Add(board.allitems[i, row]);
                board.allitems[i,row].GetComponent<Items>().isMatched = true;
            }
        }
        return items;
    }

    public void CheckBom()
    {
        if(board.currentItem != null)
        {
            if(board.currentItem.isMatched)
            {
                board.currentItem.isMatched = false;
                //int typOfBom = Random.Range(0, 100);
                //if(typOfBom < 50)
                //{
                //    board.currentItem.MakeRowBom();
                //}
                //else if(typOfBom >= 50)
                //{
                //    board.currentItem.MakeColumnBom();
                //}
                if((board.currentItem.swipeAngle > -45 && board.currentItem.swipeAngle <=45) ||
                    (board.currentItem.swipeAngle < -135 || board.currentItem.swipeAngle >=135))
                {
                    board.currentItem.MakeColumnBom();
                }
                else
                {
                    board.currentItem.MakeRowBom();
                }
            }
            else if(board.currentItem.ortherItem != null)
            {
                Items ortherItem =board.currentItem.ortherItem.GetComponent<Items>();
                if (ortherItem.isMatched)
                {
                    ortherItem.isMatched = false;
                }
                //int typOfBom = Random.Range(0, 100);
                //if (typOfBom < 50)
                //{
                //    ortherItem.MakeRowBom(); 
                //}
                //else if (typOfBom >= 50)
                //{
                //    ortherItem.MakeColumnBom();
                //}
                if ((board.currentItem.swipeAngle > -45 && board.currentItem.swipeAngle <= 45) ||
                    (board.currentItem.swipeAngle < -135 || board.currentItem.swipeAngle >= 135))
                {
                    ortherItem.MakeColumnBom();
                }
                else
                {
                    ortherItem.MakeRowBom();
                }
            }
        }
    }

}
