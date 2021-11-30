using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Terrain
{
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

    class TextureGenerator
    {
        public static float[] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {
            if (scale <= 0) scale = 0.0001f;

            int size = width * height;

            float[] noiseMap = new float[size];

            int halfWidth = width / 2;
            int halfHeight = height / 2;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            Random random = new Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int o = 0; o < octaves; o++)
            {
                float offsetX = random.Next(-100000, 100000) + offset.X;
                float offsetY = random.Next(-100000, 100000) + offset.Y;
                octaveOffsets[o] = new Vector2(offsetX, offsetY);
            }

            for (int i = 0; i < size; i++)
            {

                float frequency = 1;
                float amplitude = 1;
                float noiseHeight = 0;

                int x = i % width;
                int y = i / width;

                for (int o = 0; o < octaves; o++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[o].X;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[o].Y;

                    float noiseValue = NoiseHelpers.GustavsonNoise(sampleX, sampleY, false, true);
                    noiseHeight += noiseValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;

                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[i] = noiseHeight;
            }

            for (int i = 0; i < size; i++)
            {
                noiseMap[i] = (noiseMap[i] - minNoiseHeight) / (maxNoiseHeight - minNoiseHeight);
            }

            return noiseMap;
        }

        public static Texture2D TextureFromHeightMap(int width, int height, float[] noiseMap)
        {
            Color[] colorMap = new Color[width * height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float value = noiseMap[j * width + i];
                    colorMap[j * width + i] = Color.Lerp(Color.White, Color.Black, value);
                }
            }

            return TextureFromColorMap(width, height, colorMap);
        }

        public static Texture2D TextureFromColorMap(int width, int height, Color[] colorMap)
        {
            Texture2D texture = new Texture2D(Globals.graphics.GraphicsDevice, width, height);
            texture.SetData(colorMap);
            return texture;
        }

    }
}
