using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public class TextureFactory
    {
        private static Dictionary<Color, Texture2D> colors = new Dictionary<Color, Texture2D>();
        public static Texture2D GetTexture(Color color)
        {
            if (colors.ContainsKey(color)) return colors[color];
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.SetPixel(0,0,color);
            texture2D.Apply();
            colors[color] = texture2D;
            return texture2D;
        }
    }
}