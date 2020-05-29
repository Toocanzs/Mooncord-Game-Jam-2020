using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBuffer
{
    private RenderTexture A;
    private RenderTexture B;

    private bool swapped = false;

    public RenderTexture Current
    {
        get => swapped ? A : B;
    }

    public void Swap()
    {
        swapped = !swapped;
    }

    public bool enableRandomWrite
    {
        set
        {
            A.enableRandomWrite = value;
            B.enableRandomWrite = value;
        }
    }

    public void Create()
    {
        A.Create();
        B.Create();
    }

    public TextureWrapMode wrapMode
    {
        set
        {
            A.wrapMode = value;
            B.wrapMode = value;
        }
    }

    public void Release()
    {
        A.Release();
        B.Release();
    }
    
    public DoubleBuffer(RenderTexture a)
    {
        A = a;
        B = new RenderTexture(a);
    }
}

public static class RenderTextureExtensions
{
    public static DoubleBuffer ToDoubleBuffer(this RenderTexture rt)
    {
        return new DoubleBuffer(rt);
    }
}