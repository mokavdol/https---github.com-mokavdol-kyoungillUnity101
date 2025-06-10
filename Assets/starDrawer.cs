using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class starDrawer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<int2> dots = new List<int2>();
        dots.Add(new int2(4, 0));
        dots.Add(new int2(0, 4));
        dots.Add(new int2(0, 0));
        StarPhase starPhase = new StarPhase(dots);
        starPhase.Draw();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private struct StarPhase
    {
        public List<int2> dots;
        public StarPhase(List<int2> dots)
        {
            this.dots = dots;
        }
        public readonly void Draw()
        {
            int xMax = 0;
            int yMax = 0;
            String text = "";
            foreach (var dot in dots)
            {
                if (dot.x > xMax)
                    xMax = dot.x;
                if (dot.y > yMax)
                    yMax = dot.y;
            }
            int[] grid = new int[xMax * yMax];
            Array.Fill(grid, 0);
            foreach (var dot in dots)
            {
                int2 line;
                line = dot - dots[(dots.IndexOf(dot) +1)% dots.Count];
                Debug.Log(line.x + ", " + line.y);
                for (int i = 0; i < math.max(math.abs(line.x), math.abs(line.y)); i++)
                {
                    Debug.Log($"Drawing line from {dot.x}, {dot.y} to {line.x}, {line.y} at step {i}");
                    int x = dot.x + Convert.ToInt32(line.x * i / math.max(math.abs(line.x), math.abs(line.y)));
                    int y = dot.y + Convert.ToInt32(line.y * i / math.max(math.abs(line.x), math.abs(line.y)));
                    if (x >= 0 && y >= 0 && x < xMax && y < yMax)
                    {
                        Debug.Log($"Drawing line at {x}, {y}");
                        if (Convert.ToInt32(line.y * (i - 1 * math.sign(line.x)) / math.max(math.abs(line.x), math.abs(line.y))) != Convert.ToInt32(line.y * i / math.max(math.abs(line.x), math.abs(line.y)))) //x로 이어진 선 체크.... 길어짐...
                        {
                            Debug.Log($"Check,");
                            grid[x + y * xMax]++;
                        }
                    }
                }
            }
            bool isInside = false;
            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    if (isInside)
                    {
                        text += "★";
                        while (grid[x + y * xMax] > 0)
                        {
                            grid[x + y * xMax]--;
                            Debug.Log($"Drawing star at {x}, {y}");
                        }
                    }
                    else
                        text += "　";
                    while (grid[x + y * xMax] > 0)
                    {
                        grid[x + y * xMax]--;
                        Debug.Log($"Drawing star at {x}, {y}");
                        isInside = !isInside;
                    }
                }
                text += "\n";
            }
            Debug.Log(text);
        }
    }
}
