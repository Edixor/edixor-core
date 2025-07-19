using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace Markit.Markdown
{ 
    public static class MarkdownImageLoader
    {
        public static async void Load(Image image, string url)
        {
            using var req = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url);
            var op = req.SendWebRequest();
            while (!op.isDone)
                await System.Threading.Tasks.Task.Yield();

            if (req.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                image.image = UnityEngine.Networking.DownloadHandlerTexture.GetContent(req);
        }
    }
}
