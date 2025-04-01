using Steamworks;
using UnityEngine;

namespace Assets.Scripts.Framework.Utilities
{
    public class GetSteamInfo : MonoBehaviour
    {
        public static Texture2D SteamImageToUnityImage(int imageId)
        {
            bool isValid = SteamUtils.GetImageSize(imageId, out uint width, out uint height);
            if (!isValid) return null;

            byte[] imageData = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(imageId, imageData, (int)(width * height * 4));
            if (!isValid) return null;

            Texture2D texture = new((int)width, (int)height, TextureFormat.RGBA32, false, false);

            for (int i = 0; i < imageData.Length; i += 4)
                imageData[i + 3] = 255;

            texture.LoadRawTextureData(imageData);
            texture.Apply();

            Texture2D fixedTexture = FlipTextureVertically(texture);
            Destroy(texture);

            return fixedTexture;
        }

        private static Texture2D FlipTextureVertically(Texture2D original)
        {
            Texture2D flipped = new(original.width, original.height, original.format, false);

            for (int x = 0; x < original.width; x++)
                for (int y = 0; y < original.height; y++)
                    flipped.SetPixel(x, original.height - y - 1, original.GetPixel(x, y));

            flipped.Apply();
            return flipped;
        }
    }
}