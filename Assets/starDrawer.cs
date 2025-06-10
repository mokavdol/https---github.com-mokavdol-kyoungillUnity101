using System;
using System.Collections.Generic;
using System.IO;
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
    [SerializeField]
    private List<StarPhase> starPhases;
    void Start()
    {
        string allText = "";
        foreach (var starPhase in starPhases)
        {
            allText += starPhase.Draw();
            allText += "\n========================================\n"; // 구분선 추가
        }
        
        // txt 파일로 저장
        SaveToTextFile(allText);
    }
    private void SaveToTextFile(string content)
    {
        string fileName = "StarOutput_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
        string filePath = Path.Combine(Application.dataPath, fileName);
        
        try
        {
            File.WriteAllText(filePath, content);
            Debug.Log($"Text file saved: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save text file: {e.Message}");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    [Serializable]
    private struct StarPhase
    {
        public List<int2> dots;
        public StarPhase(List<int2> dots)
        {
            this.dots = dots;
        }
        public readonly String Draw()
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
            int[] grid = new int[(xMax + 1) * (yMax + 1)];
            Array.Fill(grid, 0);
            foreach (var dot in dots)
            {
                int2 line;
                line = dots[(dots.IndexOf(dot) + 1) % dots.Count] - dot;
                //Debug.Log(line.x + ", " + line.y);
                int maxLength = math.max(math.abs(line.x), math.abs(line.y));
                for (int i = 0; i <= maxLength; i++)
                {
                    //Debug.Log($"Drawing line from {dot.x}, {dot.y} to {line.x}, {line.y} at step {i}");
                    int sign = math.sign(line.x) == 0? 1 : math.sign(line.x);
                    int x = dot.x + Mathf.RoundToInt(((float)line.x * i / maxLength) + 0.00001f*math.sign(line.x* line.y));
                    int y = dot.y + Mathf.RoundToInt(((float)line.y * i / maxLength) + 0.00001f*math.sign(line.x* line.y));
                    if (x >= 0 && y >= 0 && x <= xMax && y <= yMax)
                    {
                        grid[x + y * (xMax + 1)]++;
                        
                        //Debug.Log($"Drawing line at {x}, {y}");
                        if (i - sign >= 0 && i - sign <= maxLength)
                        {
                            if (math.abs(line.x) > math.abs(line.y) && (Mathf.RoundToInt((float)line.y * (i - sign) / maxLength) == Mathf.RoundToInt((float)line.y * i / maxLength))) //x로 이어진 선 체크.... 길어짐...
                            {
                                //Debug.Log($"double Check,Drawing line at {x}, {y}");
                                grid[x + y * (xMax + 1)]++;
                                //Debug.Log($");
                            }
                        }
                    }
                }
                int2 lastLine = dots[(dots.IndexOf(dot) + dots.Count - 1) % dots.Count] - dot;
                if (math.sign(lastLine.y * line.y) <= 0)
                {
                    //Debug.Log($"triple Check,Drawing line at {dot.x}, {dot.y}");
                    grid[dot.x + dot.y * (xMax + 1)]++;
                    continue;
                }
            }
            bool isInside = false;
            for (int y = 0; y <= yMax; y++)
            {
                isInside = false;
                for (int x = 0; x <= xMax; x++)
                {
                    if (grid[x + y * (xMax + 1)] != 0)
                    {
                        while (grid[x + y * (xMax + 1)] > 0)
                        {
                            grid[x + y * (xMax + 1)]--;
                            //Debug.Log($"Drawing star at {x}, {y}");
                            isInside = !isInside;
                        }
                        text += "★";
                    }
                    else if (isInside)
                        text += "★";
                    else
                        text += "　";
                }
                text += "\n";
            }
            Debug.Log(text);
            return text;
        }
    }
}
