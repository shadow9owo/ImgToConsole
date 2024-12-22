using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;

namespace ImgToConsole
{
    public class Utils
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
    }
public class Renderer
{
        public static Utils.Vector2 startcursorpos;
        public static Utils.Vector2 step;
        public static Utils.Vector2 pixeltoread = new Utils.Vector2(0, 0);
        public static bool RenderImage(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception($"File {path} was not found.");
            }

            startcursorpos = new Utils.Vector2(Console.CursorLeft, Console.CursorTop);

            using (var image = new MagickImage(path))
            {

                step.X = (image.Width > Console.WindowWidth) ? (float)(image.Width - 1) / (Console.WindowWidth - 1) : 1;
                step.Y = (image.Height > Console.WindowHeight) ? (float)(image.Height - 1) / (Console.WindowHeight - 1) : 1;

                for (float y = 0; y < Console.WindowHeight; y++)
                {
                    for (float x = 0; x < Console.WindowWidth; x++)
                    {
                        int imageX = (int)(x * step.X);
                        int imageY = (int)(y * step.Y);

                        if (imageX < image.Width && imageY < image.Height)
                        {
                            var pixel = image.GetPixels().GetPixel(imageX, imageY);
                            var color = pixel.ToColor();
                            RenderPixel(new MagickColor(color.R,color.G,color.B), new Utils.Vector2(x, y));
                        }
                    }
                }
            }

            Console.SetCursorPosition(Console.CursorLeft, (int)(Console.WindowHeight / step.Y));

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
                    Math.Pow(color.R - item.Value.R, 2) +
                    Math.Pow(color.G - item.Value.G, 2) +
                    Math.Pow(color.B - item.Value.B, 2)
                );

                if (distance < shortestdistance)
                {
                    shortestdistance = distance;
                    closestcolor = item.Key;
                }
            }

            return closestcolor;
        }


        public static bool RenderPixel(MagickColor color, Utils.Vector2 coordinate)
        {
            var defpos = new Utils.Vector2(Console.CursorLeft, Console.CursorTop);
            Console.SetCursorPosition((int)coordinate.X, (int)coordinate.Y);

            Console.BackgroundColor = magiktoconsole(color);

            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write(".");

            Console.ResetColor();

            return true;
        }

    }
}