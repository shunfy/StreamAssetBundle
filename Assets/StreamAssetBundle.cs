using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class EncryptionStream : FileStream
{
    public byte[] EncryKey { get; private set; }

    public static byte[] UInt32ToByte4(uint vl)
    {
        byte[] ret = new byte[4];
        ret[0] = (byte)(vl >> 0);
        ret[1] = (byte)(vl >> 8);
        ret[2] = (byte)(vl >> 16);
        ret[3] = (byte)(vl >> 24);
        return ret;
    }

    public EncryptionStream(string path, FileMode mode, uint encryKey)
        :base(path, mode)
    { EncryKey = UInt32ToByte4(encryKey); }
    public EncryptionStream(string path, FileMode mode, FileAccess access, uint encryKey)
        : base(path, mode, access)
    { EncryKey = UInt32ToByte4(encryKey); }
    public EncryptionStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, uint encryKey)
        : base(path, mode, access, share, bufferSize)
    { EncryKey = UInt32ToByte4(encryKey); }
    public EncryptionStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync, uint encryKey)
        :base(path, mode, access, share, bufferSize, useAsync)
    { EncryKey = UInt32ToByte4(encryKey); }

    public override int Read(byte[] array, int offset, int count)
    {
        int ret = base.Read(array, offset, count);

        for(int i = 0; i < ret; ++i)
        {
            array[i] ^= EncryKey[i % 4];
        }

        return ret;
    }

    public override bool CanSeek => true;
}

public static class StreamEncryption
{
    static uint encryKey = 0x6F7A81F3;

    public static void EcryptAssetBundle(string abPath)
    {
        byte[] EncryKey = EncryptionStream.UInt32ToByte4(encryKey);
        byte[] bytes = File.ReadAllBytes(abPath);
        for (int i = 0; i < bytes.Length; ++i)
        {
            bytes[i] ^= EncryKey[i % 4];
        }
        File.WriteAllBytes(abPath, bytes);
    }

    public static void DecryptAssetBundle(string abPath, string name, Action<UnityEngine.Object> onLoadedCall)
    {
        var fileStream = new EncryptionStream(abPath, FileMode.Open, FileAccess.Read, encryKey);
        var myLoadedAssetBundle = AssetBundle.LoadFromStream(fileStream);
        var obj = myLoadedAssetBundle.LoadAsset<GameObject>(name);
        try
        {
            onLoadedCall(obj);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        myLoadedAssetBundle.Unload(false);
        fileStream.Close();
    }

    public static IEnumerator AsyncLoad(string abPath, string name, Action<UnityEngine.Object> onLoadedCall)
    {
        var fileStream = new EncryptionStream(abPath, FileMode.Open, FileAccess.Read, encryKey);
        var bundleLoadRequest = AssetBundle.LoadFromStreamAsync(fileStream);
        yield return bundleLoadRequest;
        var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync(name);
        yield return assetLoadRequest;
        try
        {
            onLoadedCall(assetLoadRequest.asset);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        myLoadedAssetBundle.Unload(false);
        fileStream.Close();
    }
}

