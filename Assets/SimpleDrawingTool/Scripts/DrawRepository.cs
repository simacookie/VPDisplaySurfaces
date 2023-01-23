using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using SimpleDrawCanvas.Domain;

namespace SimpleDrawCanvas.Infrastructure
{
    public class DrawRepository : IDrawRepository
    {
        void IDrawRepository.Save(RenderTexture texture)
        {
            int w = texture.width;
            int h = texture.height;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);

            RenderTexture tmp = RenderTexture.active;

            RenderTexture.active = texture;
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            RenderTexture.active = tmp;

            byte[] data = tex.EncodeToPNG();

            string savePath = $"{UnityEngine.Application.dataPath}/../canvas.png";

            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(data);
                    bw.Close();
                }
                fs.Close();
            }
        }
    }
}
