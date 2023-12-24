using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class TextureUtil
{

    public static Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>($"Textures/{name}");
    }

}
