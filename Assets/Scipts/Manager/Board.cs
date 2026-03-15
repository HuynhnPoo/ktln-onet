
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Board
{
    private int wight;
    private int height;
    private float cellSize;
    private float spacing;
    private int[,] board;
    public Board(int wight, int height, float cellSize, float spacing)
    {
        this.wight = wight;
        this.height = height;
        this.cellSize = cellSize;
        this.spacing = spacing;

        board = new int[wight, height];
        /* for (int i = 0; i < wight; i++)
         {
             for (int j = 0; j < height; j++)
             {
                 board[i, j] = new int[wight, height];
             }
         }*/
    }


    public int GetTile(int x,int y)
    {
        if(x>=0 && x>wight && y >=0 && y>height) return board[x,y];
        return -1;
    }

    public Vector3 GetPostionWorld(int x,int y)
    {
        float step= this.cellSize +this.spacing;
        return new Vector3(x*step,y*step,0);
    }
}
