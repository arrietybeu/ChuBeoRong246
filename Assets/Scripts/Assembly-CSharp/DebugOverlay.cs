using System.Collections.Generic;
using UnityEngine;

public static class DebugOverlay
{
    private static readonly List<string> logs = new List<string>();
    private static bool visible = true;

    private static readonly int maxLines = 20;
    private static readonly float lineHeight = 20f;
    private static readonly float padding = 8f;


    private static GUIStyle labelStyle;
    private static Rect inputRect;
    private static Rect buttonRect;
    private static string inputText = "";


    public static void Log(string message)
    {
        logs.Add(message);
        if (logs.Count > 100) logs.RemoveAt(0);
    }

    public static void Toggle()
    {
        visible = !visible;
    }

    public static void Draw(mGraphics g)
    {
        if (!visible) return;

        int count = Mathf.Min(logs.Count, maxLines);
        float boxHeight = lineHeight * (count + 2) + padding * 3 + 30; // + dòng input
        float boxWidth = 400f;
        float startX = 10;
        float startY = 10;

        // Khởi tạo GUIStyle nếu cần
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.UpperLeft;
        }

        // Nền đen bán trong: ARGB = 0xAARRGGBB (0x66 = alpha ~ 40%)
        g.setColor(0x66000000);
        g.fillRect((int)startX, (int)startY, (int)boxWidth, (int)boxHeight);

        // Input
        inputRect = new Rect(startX + padding, startY + padding, boxWidth - 120, 25);
        g.setColor(13524492); // trắng viền
        g.drawRect((int)inputRect.x, (int)inputRect.y, (int)inputRect.width, (int)inputRect.height);
        g.drawString(inputText, (int)inputRect.x + 5, (int)inputRect.y + 5, labelStyle);

        // Button
        buttonRect = new Rect(inputRect.x + inputRect.width + 10, inputRect.y, 80, 25);
        g.setColor(13524492); // xanh dương nhạt
        g.fillRect((int)buttonRect.x, (int)buttonRect.y, (int)buttonRect.width, (int)buttonRect.height);
        g.setColor(13524492);
        g.drawString("Xác Nhận", (int)buttonRect.x + 10, (int)buttonRect.y + 5, labelStyle);

        // Log
        for (int i = 0; i < count; i++)
        {
            string line = logs[logs.Count - count + i];
            g.drawString(line, (int)startX + 8, (int)(startY + 40 + i * lineHeight), labelStyle);
        }
    }


    // Xử lý input (gọi từ Unity Input system)
    public static void HandleInput()
    {
        if (!visible) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b') // backspace
            {
                if (inputText.Length > 0)
                    inputText = inputText.Substring(0, inputText.Length - 1);
            }
            else if (c == '\n' || c == '\r') // enter
            {
                SubmitInput();
            }
            else
            {
                inputText += c;
            }
        }

        // Xử lý click
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y; // chuyển hệ trục

            if (buttonRect.Contains(mouse))
            {
                SubmitInput();
            }
        }
    }

    private static void SubmitInput()
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            Log(inputText);
            inputText = "";
        }
    }
}
