using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ImageMagick;

namespace ImgToConsole
{
    class Utils
    {
        public struct Vector2 //numerics dosent work so yea
        {
            public float X { get; set; }
            public float Y { get; set; }

            public Vector2(float x, float y)
            {
                X = x;
                Y = y;
            }

            public float Magnitude => (float)Math.Sqrt(X * X + Y * Y);

            public Vector2 Normalize()
            {
                float magnitude = Magnitude;
                if (magnitude > 0)
                {
                    return new Vector2(X / magnitude, Y / magnitude);
                }
                return new Vector2(0, 0);
            }

            public static Vector2 operator +(Vector2 a, Vector2 b)
            {
                return new Vector2(a.X + b.X, a.Y + b.Y);
            }

            public static Vector2 operator -(Vector2 a, Vector2 b)
            {
                return new Vector2(a.X - b.X, a.Y - b.Y);
            }

            public static Vector2 operator *(Vector2 a, float scalar)
            {
                return new Vector2(a.X * scalar, a.Y * scalar);
            }

            public static Vector2 operator /(Vector2 a, float scalar)
            {
                return new Vector2(a.X / scalar, a.Y / scalar);
            }

            public static float Dot(Vector2 a, Vector2 b)
            {
                return a.X * b.X + a.Y * b.Y;
            }

            public override string ToString()
            {
                return $"({X}, {Y})";
            }
    }
    class Renderer
    {
            public static Vector2 startcursorpos;
            public static Vector2 step;
            public static Vector2 pixeltoread = new Vector2(0, 0);
            public static bool RenderImage(string path)
            {
                if (!File.Exists(path))
                {
                    throw new Exception($"File {path} was not found.");
                }

                startcursorpos = new Vector2(Console.CursorLeft, Console.CursorTop);

                using (var image = new MagickImage(path))
                {
                    step.X = (image.Width > Console.WindowWidth) ? (float)(image.Width - 1) / (Console.WindowWidth - 1) : 1;
                    step.Y = (image.Height > Console.WindowHeight) ? (float)(image.Height - 1) / (Console.WindowHeight - 1) : 1;

                    if (pixeltoread.X >= 0 && pixeltoread.X < image.Width)
                    {
                        var pixel = image.GetPixels().GetPixel((int)pixeltoread.X, (int)pixeltoread.Y);
                        var color = pixel.ToColor();
                        pixeltoread.X = pixeltoread.X + step.X;
                    }else if (pixeltoread.Y >= 0 && pixeltoread.Y < image.Height)
                    {
                        pixeltoread.X = 0;
                        pixeltoread.Y = pixeltoread.Y + step.Y;
                        var pixel = image.GetPixels().GetPixel((int)pixeltoread.X, (int)pixeltoread.Y);
                        var color = pixel.ToColor();

                    }
                    else
                    {
                        return true;
                    }
                }

                return true;
            }

            public static ConsoleColor magiktoconsole(MagickColor color)
            {
                Dictionary<ConsoleColor, MagickColor> keyValuePairs = new Dictionary<ConsoleColor, MagickColor>()
                {
                    { ConsoleColor.Black, new MagickColor(0, 0, 0) },
                    { ConsoleColor.DarkBlue, new MagickColor(0, 0, 139) },
                    { ConsoleColor.DarkGreen, new MagickColor(0, 100, 0) },
                    { ConsoleColor.DarkCyan, new MagickColor(0, 139, 139) },
                    { ConsoleColor.DarkRed, new MagickColor(139, 0, 0) },
                    { ConsoleColor.DarkMagenta, new MagickColor(139, 0, 139) },
                    { ConsoleColor.DarkYellow, new MagickColor(139, 139, 0) },
                    { ConsoleColor.Gray, new MagickColor(211, 211, 211) },
                    { ConsoleColor.DarkGray, new MagickColor(169, 169, 169) },
                    { ConsoleColor.Blue, new MagickColor(0, 0, 255) },
                    { ConsoleColor.Green, new MagickColor(0, 255, 0) },
                    { ConsoleColor.Cyan, new MagickColor(0, 255, 255) },
                    { ConsoleColor.Red, new MagickColor(255, 0, 0) },
                    { ConsoleColor.Magenta, new MagickColor(255, 0, 255) },
                    { ConsoleColor.Yellow, new MagickColor(255, 255, 0) },
                    { ConsoleColor.White, new MagickColor(255, 255, 255) }
                };

                ConsoleColor closestcolor = ConsoleColor.Black;
                double shortestdistance = double.MaxValue;

                foreach (var item in keyValuePairs)
                {
                    double distance = Math.Sqrt(
                        Math.Pow(color.R - item.Value.R / 256, 2) +
                        Math.Pow(color.G - item.Value.G / 256, 2) +
                        Math.Pow(color.B - item.Value.B / 256, 2)
                    );

                    if (distance < shortestdistance)
                    {
                        shortestdistance = distance;
                        closestcolor = item.Key;
                    }
                }
                return closestcolor;
            }

            public static bool RenderPixel(MagickColor color,Vector2 cordinate)
            {
                var defpos = new Vector2(Console.CursorLeft, Console.CursorTop);
                Console.SetCursorPosition((int)cordinate.X, (int)cordinate.Y);

                Console.BackgroundColor = magiktoconsole(color);
                Console.Write(" ");

                Console.SetCursorPosition((int)defpos.X, (int)defpos.Y);
                return true;
            }
        }
    }
}
