using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools.Graphics
{
    public static class GraphicsHelper
    {

        /// <summary>
        /// Create a Texture representing a circle
        /// </summary>
        public static Texture2D CreateCircle(int radius, GraphicsDevice graphicD, Color color)
        {
            int outerRadius = radius*2 + 2; // So circle doesn't go out of bounds
            var circle = new Texture2D(graphicD, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f/radius;

            for (double angle = 0; angle < Math.PI*2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = color;
            }
            circle.SetData(data);
            return circle;
        }


        /// <summary>
        /// Create a texture of 1 pixel per 1 pixel
        /// </summary>
        public static Texture2D PixelTexture(GraphicsDevice graphicD, Color color)
        {
            var pixelT = new Texture2D(graphicD, 1, 1);

            pixelT.SetData<Color>(new Color[1] {color});

			return pixelT;
        }

        public static void DrawLine()
        {

        }

        public static void DrawLine(GraphicsDevice device, SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, int width = 2)
        {
            var lineTexture = PixelTexture(device, color);

            var r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length()+width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(lineTexture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }


}
