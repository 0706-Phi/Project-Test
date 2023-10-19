using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameSate
{
    wait,
    move
}
public class Board : MonoBehaviour
{
    public GameSate currnetState = GameSate.move;
    public int Width;
    public int Hieght;
    public int offSet;
    private TilesBackGround[,] tiles;

    public GameObject tileBackground;
    public GameObject[] item;
    public GameObject[,] allitems;
    public GameObject destroyEffect;

    public FindMatches FindMatches;
    public Items currentItem;
    void Start()
    {
        FindMatches = FindObjectOfType<FindMatches>();
        tiles = new TilesBackGround[Width, Hieght];
        allitems = new GameObject[Width, Hieght];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Hieght; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject backgroundTiles = Instantiate(tileBackground, tempPosition, Quaternion.identity);
                backgroundTiles.transform.parent= this.transform;
                backgroundTiles.name = "(" +i+ "," +j+ ")";
                int items = Random.Range(0, item.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, item[items]) && maxIterations<100)
                {
                    items = Random.Range(0, item.Length);
                    maxIterations++;
                }
                GameObject Items = Instantiate(item[items], tempPosition, Quaternion.identity);
                Items.GetComponent<Items>().row = j;
                Items.GetComponent<Items>().column = i;
                Items.transform.parent = transform;
                Items.name = "(" + i + "," + j + ")";
                allitems[i, j] = Items;
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allitems[column - 1 ,row].tag == piece.tag && allitems[column-2, row].tag == piece.tag)
            {
                return true;
            }
            if (allitems[column ,row-1].tag == piece.tag && allitems[column, row-2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <=1 || row <=1)
        {
            if(row > 1)
            {
                if (allitems[column,row-1].tag == piece.tag && allitems[column,row-2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allitems[column - 1, row].tag == piece.tag && allitems[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Items firstPiece = FindMatches.currentMatches[0].GetComponent<Items>();
        if(firstPiece != null)
        {
            foreach (GameObject currentPiece in FindMatches.currentMatches)
            {
                Items items= currentPiece.GetComponent<Items>();
                if(items.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if(items.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
       return (numberVertical==5 || numberHorizontal==5);
    }

    private void CheckToMakeBom()
    {
        if(FindMatches.currentMatches.Count == 4 || FindMatches.currentMatches.Count == 7)
        {
            FindMatches.CheckBom();
        }
        if(FindMatches.currentMatches.Count == 5 || FindMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                if(currentItem != null)
                {
                    if(currentItem.isMatched)
                    {
                        if(!currentItem.isColorBom)
                        {
                            currentItem.isMatched= false;
                            currentItem.MakeColorBom();
                        }
                    }
                    else
                    {
                        if(currentItem.ortherItem != null)
                        {
                            Items ortheritem =currentItem.ortherItem.GetComponent<Items>();
                            if(ortheritem.isMatched)
                            {
                                if (!ortheritem.isColorBom)
                                {
                                    ortheritem.isMatched= false;
                                    ortheritem.MakeColorBom();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentItem != null)
                {
                    if (currentItem.isMatched)
                    {
                        if (!currentItem.isAdjacentBom)
                        {
                            currentItem.isMatched = false;
                            currentItem.MakeAdjacentBom();
                        }
                    }
                    else
                    {
                        if (currentItem.ortherItem != null)
                        {
                            Items ortheritem = currentItem.ortherItem.GetComponent<Items>();
                            if (ortheritem.isMatched)
                            {
                                if (!ortheritem.isAdjacentBom)
                                {
                                    ortheritem.isMatched = false;
                                    ortheritem.MakeAdjacentBom();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void DestroyMacthesAt(int column , int row)
    {
        if (allitems[column,row].GetComponent<Items>().isMatched)
        {
            if(FindMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBom();
            }
            GameObject partycle = Instantiate(destroyEffect, allitems[column, row].transform.position,Quaternion.identity);
            Destroy(partycle,.5f);
            Destroy(allitems[column,row]);
            allitems[column,row] = null;
        }
    }

    public void DestroyMacthes()
    {
        for(int i = 0; i<Width ; i++)
        {
            for(int j = 0; j<Hieght; j++)
            {
                if (allitems[i, j]!= null)
                {
                    DestroyMacthesAt(i,j);
                }
            }
        }
        FindMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Hieght; j++)
            {
                if (allitems[i, j] == null)
                {
                   nullCount++;
                }
                else if(nullCount > 0)
                {
                    allitems[i, j].GetComponent<Items>().row -= nullCount;
                    allitems[i,j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Hieght; j++)
            {
                if (allitems[i, j] == null)  
                {
                    Vector2 tempPosition =new Vector2(i,j + offSet);
                    int items = Random.Range(0,item.Length);
                    GameObject piece = Instantiate(item[items] , tempPosition, Quaternion.identity);
                    allitems[i, j] = piece;
                    piece.GetComponent<Items>().row = j;
                    piece.GetComponent<Items>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Hieght; j++)
            {
                if (allitems[i,j] != null)
                {
                    if (allitems[i, j].GetComponent<Items>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMacthes();
        }
        FindMatches.currentMatches.Clear();
        currentItem= null;
        yield return new WaitForSeconds(.5f);
        currnetState = GameSate.move;
    }

}
