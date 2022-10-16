using SkiaSharp;
using System.Diagnostics.CodeAnalysis;

namespace Tamarix.Views
{
    public class MultiLineTextField : ClickableView
    {
        private SKPaint Paint = new SKPaint()
        {
            Color = Theme.Current.TextColor.SkColor,
            IsAntialias = true,
            TextSize = 20
        };
        private SKPaint CursorPaint = new SKPaint()
        {
            Color = Theme.Current.AccentColor.SkColor,
            IsAntialias = true,
            TextSize = 20,
        };

        private SKPaint CursorPaintInactive = new SKPaint()
        {
            Color = Theme.Current.AccentColorInactive.SkColor,
            IsAntialias = true,
            TextSize = 20,
        };
        public StyleBox? Background { get; set; }


        private List<string> value = new List<string>();
        public string Value { get => string.Join(",", value); set { this.value = value.Split('\n').ToList(); } }
        public bool Multiline { get; set; } = false;
        public bool ReadOnly { get; set; } = false;
        public string? Hint { get; set; } = null;
        public bool EnterIsSend { get; set; } = true;
        public bool IsPassword { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public bool IsSelecting { get; set; }
        public override MouseCursor? Cursor { get; set; } = MouseCursor.Edit;
        public override Padding Padding { get; set; } = new Padding(8);

        /// Selection props
        public CursorPosition SelectStart;
        public CursorPosition SelectEnd;
        public CursorPosition CursorPosition;

        private bool isDragging = false;
        private bool isPressed = false;

        private bool draggingEnd = true;

        // Animations
        private float cX=0, cY=0, cXto, cYto;
        private int cursorFadeTimer = 0, cursorFadeTime = 30;
        private bool blink = true;

        public MultiLineTextField(string value = "", bool multiline = false, bool readOnly = true, string? hint = "", bool enterIsSend = true, bool isPassword = false)
        {
            Value = value;
            Multiline = multiline;
            ReadOnly = readOnly;
            Hint = hint;
            EnterIsSend = enterIsSend;
            IsPassword = isPassword;

            //MinWidth = 512;
            MinHeight = 32;
            Width = 256;
            Height = 32;
        }

        const int cursorPadSize = 4;
        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
            canvas.Save();
            SKRect rect = new SKRect(X - Padding.Left, Y - Padding.Top, X-Padding.Left+MeasuredWidth, Y-Padding.Top+MeasuredHeight);
            canvas.ClipRect(rect);
            SKRect letterSize = new SKRect();
            Paint.MeasureText("A", ref letterSize);
            (Background ?? Theme.Current.TextFieldBackground).Draw(canvas, X-Padding.Left, Y-Padding.Top, MeasuredWidth, MeasuredHeight);


            float ypos = Y;
            var paint = IsActive ? CursorPaint : CursorPaintInactive;
            for (var i = 0; i < value.Count; i++)
            {
                if (i > SelectStart.Line && i < SelectEnd.Line)
                {
                    // Entire line is selected
                    canvas.DrawRect(
                        X + Padding.Left,
                        ypos + Padding.Top  - cursorPadSize,
                        Math.Max(Paint.MeasureText(value[i]),4),
                        Paint.FontSpacing,
                        paint);
                }
                if (SelectStart.Line == SelectEnd.Line && SelectEnd.Line == i)
                {
                    // Singe line select
                    canvas.DrawRect(
                        X + Padding.Left + Paint.MeasureText(value[i].Substring(0, SelectStart.Pos)),
                        ypos + Padding.Top  - cursorPadSize,
                        Paint.MeasureText(value[i].Substring(SelectStart.Pos, SelectEnd.Pos - SelectStart.Pos)),
                        Paint.FontSpacing,
                        paint);
                }
                else
                {
                    if (i == SelectStart.Line)
                    {
                        // Starting select line
                        canvas.DrawRect(
                            X + Padding.Left + Paint.MeasureText(value[i].Substring(0, SelectStart.Pos)),
                            ypos + Padding.Top  - cursorPadSize,
                            Math.Max(4,Paint.MeasureText(value[i].Substring(SelectStart.Pos))),
                        Paint.FontSpacing,
                            paint);
                    }
                    if (i == SelectEnd.Line)
                    {
                        // Last select line
                        canvas.DrawRect(
                            X + Padding.Left,
                            ypos + Padding.Top - cursorPadSize,
                            Math.Max(4,Paint.MeasureText(value[i].Substring(0, SelectEnd.Pos))),
                            Paint.FontSpacing,
                            paint);
                    }
                }
                canvas.DrawText(value[i],
                    X + Padding.Left,
                    ypos + Padding.Top + letterSize.Height,
                    Paint);





                // Draw the cursor
                if (IsActive && i == CursorPosition.Line && !blink)
                {
                    cXto = Paint.MeasureText(value[i].Substring(0, CursorPosition.Pos));
                    cYto = ypos - Y;
                    canvas.DrawRect(
                        X + Padding.Left + cX,
                        Y + Padding.Top + cY - cursorPadSize,
                        1,
                        letterSize.Height + cursorPadSize*2,
                        Paint);
                }

                ypos += Paint.FontSpacing;
            }




            // Debug
            //if (View.DrawDebug)
            //{
            //    ypos = Y + Padding.Top; 
            //    float xpos;
            //    for (var line = 0; line < value.Count; line++)
            //    {
            //        Paint.MeasureText(value[line], ref textSize);
            //        {
            //            string words = "";
            //            float newWidth = 0;
            //            xpos = X + Padding.Left;
            //            for (var letter = 0; letter < value[line].Length; letter++)
            //            {

            //                words =""+ value[line][letter];
            //                newWidth = Paint.MeasureText(words, ref textSize);
            //                canvas.DrawRect(xpos, ypos, newWidth, Paint.FontSpacing, new SKPaint() { Color = new SKColor(0, 200, 0, 150), Style = SKPaintStyle.Stroke });

            //                xpos += newWidth;
            //            }
            //        }
            //        Paint.MeasureText(value[line], ref textSize);
            //        //canvas.DrawRect(X + Padding.Left, ypos, textSize.Width, Paint.FontSpacing, new SKPaint() { Color = new SKColor(0, 0, 200, 255), Style=SKPaintStyle.Stroke });
            //        ypos += Paint.FontSpacing;
            //    }
            //}
            canvas.Restore();
        }

        public void MousePosToCursorPos(int x, int y, out int posNum, out int lineNum)
        {
            float ypos = Y + Padding.Top;
            float xpos = 0;
            lineNum = 0; posNum = 0;
            for (var line = 0; line < value.Count; line++)
            {
                if (y > ypos && y < ypos + Paint.FontSpacing)
                {
                    lineNum = line;

                    var lineWidth = Paint.MeasureText(value[line]);
                    xpos = X + Padding.Left;
                    if (x < xpos + lineWidth)
                    {
                        for (int letter = 0; letter < value[line].Length; letter++)
                        {
                            var letterChar = value[line][letter];

                            float letterWidth = Paint.MeasureText("" + letterChar);
                            if (x > xpos && x <= xpos + letterWidth)
                            {
                                if (x <= xpos + letterWidth / 2f)
                                    posNum = letter;
                                else
                                    posNum = letter + 1;
                            }

                            xpos += letterWidth;
                        }
                    }
                    else
                    {
                        posNum = value[line].Length;
                    }
                }
                ypos += Paint.FontSpacing;
            }

            // End of file
            if(ypos < y)
            {
                lineNum = value.Count-1;
                posNum = value[lineNum].Length;
                xpos =  X + Padding.Left; 
                for (int letter = 0; letter < value[lineNum].Length; letter++)
                {
                    var letterChar = value[lineNum][letter];

                    float letterWidth = Paint.MeasureText("" + letterChar);
                    if (x > xpos && x <= xpos + letterWidth)
                    {
                        if (x <= xpos + letterWidth / 2f)
                            posNum = letter;
                        else
                            posNum = letter + 1;
                    }

                    xpos += letterWidth;
                }
            }
        }

        public void CursorPosToMousePos(int pos, int line, out int x, out int y)
        {
            var text = value[line].Substring(0, pos);
            SKRect textSize = new SKRect();
            Paint.MeasureText(text, ref textSize);
            x = X + (int)textSize.Left;
            y = Y + (int)textSize.Height * line;
        }

        public override void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            if (!evt.Handled)
            {
                base.OnMouseButton(ref evt, button, pressed);
                if (!pressed)
                    Console.WriteLine($"OnMouseButton TextField: content: {value[0]} {evt.Handled}");
                if (pressed)
                {
                    if (Window.Current != null)
                        Window.Current!.CurrentViewFocus = this;
                    isDragging = false;
                    isPressed = true;
                    IsSelecting = false;
                    MousePosToCursorPos(evt.x, evt.y, out SelectStart.Pos, out SelectStart.Line);
                    MousePosToCursorPos(evt.x, evt.y, out CursorPosition.Pos, out CursorPosition.Line);
                    MousePosToCursorPos(evt.x, evt.y, out SelectEnd.Pos, out SelectEnd.Line);
                    IsActive = true;
                }
                else
                {
                    isDragging = false;
                    isPressed = false;
                }
                if (IsActive)
                {
                    cursorFadeTimer = 0;
                    blink = false;
                    evt.Handled = true;
                }
            }
        }

        public int CursorPosToOffset(int pos, int line)
        {
            var result = 0;
            for (var i = 0; i <= line; i++)
            {
                if (i < line)
                {
                    result += value[i].Length;
                }
                else
                {
                    result += pos;
                }
            }
            return result;
        }

        public override void OnMouseMove(ref UIEvent evt)
        {
            base.OnMouseMove(ref evt);
            if (!IsActive) return;
            if (isPressed && !isDragging)
            {
                isDragging = true;
                MousePosToCursorPos(evt.x, evt.y, out SelectStart.Pos, out SelectStart.Line);
                MousePosToCursorPos(evt.x, evt.y, out CursorPosition.Pos, out CursorPosition.Line);
                MousePosToCursorPos(evt.x, evt.y, out SelectEnd.Pos, out SelectEnd.Line);
                IsSelecting = false;
            }

            if (isDragging)
            {
                MousePosToCursorPos(evt.x, evt.y, out CursorPosition.Pos, out CursorPosition.Line);
                //MousePosToCursorPos(evt.x, evt.y, out var newPos, out var newLine);

                if (draggingEnd)
                {
                    if (CursorPosToOffset(CursorPosition.Pos, CursorPosition.Line) <= CursorPosToOffset(SelectStart.Pos, SelectStart.Line))
                    {
                        draggingEnd = false;
                        SelectEnd.Pos = SelectStart.Pos;
                        SelectEnd.Line = SelectStart.Line;
                    }
                }
                else
                    if (CursorPosToOffset(CursorPosition.Pos, CursorPosition.Line) > CursorPosToOffset(SelectEnd.Pos, SelectEnd.Line))
                    draggingEnd = true;

                if (draggingEnd)
                    MousePosToCursorPos(evt.x, evt.y, out SelectEnd.Pos, out SelectEnd.Line);
                else
                    MousePosToCursorPos(evt.x, evt.y, out SelectStart.Pos, out SelectStart.Line);
            }

            if (SelectStart == SelectEnd)
            {
                IsSelecting = false;
            }
            else
            {
                IsSelecting = true;
            }

        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            cX += (cXto - cX) / 2.0f;
            cY += (cYto - cY) / 2.0f;

            cursorFadeTimer++;
            if(cursorFadeTimer >= cursorFadeTime)
            {
                cursorFadeTimer = 0;
                blink = !blink;
            }

            if(Window.Current != null)
            IsActive = Window.Current!.CurrentViewFocus == this;
        }

        public void RemoveTextOnSelection()
        {
            if (IsSelecting)
            {
                if (SelectStart.Line != SelectEnd.Line)
                {
                    // Clears whatever in selection
                    var newStart = value[SelectStart.Line].Remove(SelectStart.Pos);
                    var newEnd = value[SelectEnd.Line].Remove(0, SelectEnd.Pos);
                    for (var l = SelectStart.Line+1; l <= SelectEnd.Line; l++)
                    {
                        value.RemoveAt(SelectStart.Line + 1);
                    }
                    value[SelectStart.Line] = newStart + newEnd;
                }
                else
                {
                    value[SelectStart.Line] = value[SelectStart.Line].Remove(SelectStart.Pos, SelectEnd.Pos - SelectStart.Pos);
                }

                CursorPosition = SelectStart;
                SelectEnd = SelectStart;
                IsSelecting = false;
            }
        }

        public override void OnKeyDown(ref UIEvent evt)
        {
            base.OnKeyDown(ref evt);
            if (!IsActive) return;
            cursorFadeTimer = 0;
            blink = false;
            if (evt.Key == Key.Enter || evt.Key == Key.KeypadEnter)
            {
                // Insert a new line
                if (Multiline)
                {
                    RemoveTextOnSelection();
                    var text = value[CursorPosition.Line].Substring(CursorPosition.Pos);
                    value[CursorPosition.Line] = value[CursorPosition.Line].Substring(0, CursorPosition.Pos);
                    value.Insert(CursorPosition.Line + 1, text);
                    CursorPosition.Line += 1;
                    CursorPosition.Pos = 0;
                }
            }
            else if (evt.Key == Key.Backspace || evt.Key == Key.Delete)
            {
                if (IsSelecting)
                {
                    RemoveTextOnSelection();
                }
                else
                {
                    if (CursorPosition.Pos > 0)
                    {
                        // Remove the character
                        if (evt.Key == Key.Backspace)
                            CursorPosition.Pos -= 1;
                        value[CursorPosition.Line] = value[CursorPosition.Line].Remove(CursorPosition.Pos, 1);
                    }
                    else if (CursorPosition.Line > 0)
                    {
                        // Remove the line \n
                        var whatsLeft = value[CursorPosition.Line];
                        value.RemoveAt(CursorPosition.Line);
                        CursorPosition.Line -= 1;
                        CursorPosition.Pos = value[CursorPosition.Line].Length;

                        value[CursorPosition.Line] += whatsLeft;
                    }
                }
                SelectStart = CursorPosition;
                SelectEnd = CursorPosition;
            }
            else if (evt.Key == Key.Right)
            {
                SetCursorPosition(CursorPosition.Line, CursorPosition.Pos + 1);
            }
            else if (evt.Key == Key.Left)
            {
                SetCursorPosition(CursorPosition.Line, CursorPosition.Pos - 1);
            }
            else if (evt.Key == Key.Up && Multiline)
            {
                SetCursorPosition(CursorPosition.Line - 1, CursorPosition.Pos);
            }
            else if (evt.Key == Key.Down && Multiline)
            {
                SetCursorPosition(CursorPosition.Line + 1, CursorPosition.Pos);
            }
        }

        public override void OnKeyChar(ref UIEvent evt)
        {
            base.OnKeyChar(ref evt);
            if (!IsActive) return;
            RemoveTextOnSelection();
            string ins = evt.c?.ToString() ?? "";
            value[CursorPosition.Line] = value[CursorPosition.Line].Insert(CursorPosition.Pos, ins);
            CursorPosition.Pos += 1;
        }

        public void SetCursorPosition(CursorPosition value)
        {
            SelectStart = value;
            SelectEnd = value;
            CursorPosition = value;
            cursorFadeTimer = 0;
            blink = false;
        }


        public void SetCursorPosition(int line, int pos)
        {
            if (line <= 0) line = 0;
            if (pos <= 0) pos = 0;
            if (pos > value[line].Length - 1) pos = value[line].Length;
            CursorPosition.Line = line; CursorPosition.Pos = pos;
            SelectStart = CursorPosition;
            SelectEnd = CursorPosition;
            cursorFadeTimer = 0;
            blink = false;
        }
    }

    public struct CursorPosition
    {
        public int Pos = 0, Line = 0;

        public CursorPosition()
        {
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is CursorPosition)
                return Pos == ((CursorPosition)obj).Pos && Line == ((CursorPosition)obj).Line;
            return false;
        }

        public static bool operator ==(CursorPosition left, CursorPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CursorPosition left, CursorPosition right)
        {
            return !(left == right);
        }
    }
}