using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class ZXingQR : MonoBehaviour
{
    #region Public methods

    /// <summary>
    /// Генератор QR кода
    /// </summary>
    /// <param name="text"> Текст для шифрования </param>
    /// <returns> Вернет текстуру QR </returns>
    public Texture2D GenerateQR(string text)
    {
        try
        {
            var encoded = new Texture2D(256, 256);
            var color32 = Encode(text, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
            return encoded;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message + ": text is null");
            return null;
        }
    }

    /// <summary>
    /// Дешифрование QR кода
    /// </summary>
    /// <param name="texture"> Текстура QR для дешифрования </param>
    /// <returns> Возвращает зашифрованное сообщение </returns>
    public string ReGenerateQR(Texture2D texture)
    {
        IBarcodeReader barcodeReader = new BarcodeReader();
        var result = barcodeReader.Decode(texture.GetPixels32(),
             texture.width, texture.height);

        if (result != null)
        {
            Debug.Log("Decoded text from QR: " + result.Text);
            return result.Text;
        }
        else
        {
            Debug.Log("Not found QR code");
            return null;
        }
    }

    /// <summary>
    /// Дешифрование QR кода
    /// </summary>
    /// <param name="webTexture"> Текстура QR для дешифрования </param>
    /// <returns> Возвращает зашифрованное сообщение </returns>
    public string ReGenerateQR(WebCamTexture webTexture)
    {
        IBarcodeReader barcodeReader = new BarcodeReader();
        var result = barcodeReader.Decode(webTexture.GetPixels32(),
             webTexture.width, webTexture.height);

        if (result != null)
        {
            Debug.Log("Decoded text from QR: " + result.Text);
            return result.Text;
        }
        else
        {
            Debug.Log("Not found QR code");
            return null;
        }
    }

    #region Encryption

    /// <summary>
    /// Кодирование текста CryptoStream
    /// </summary>
    /// <param name="data"> текст кодирования </param>
    /// <param name="passPhrase"> секретный ключ </param>
    /// <param name="saltValue"> соль </param>
    /// <param name="hashAlgorithm"> MD5 или SHA1 </param>
    /// <param name="passwordIterations"> любое число </param>
    /// <param name="keySize"> может быть 192 или 128 </param>
    /// <returns></returns>
    public string Encrypt(string data, string passPhrase = "test", string saltValue = "test", string hashAlgorithm = "SHA1",
        int passwordIterations = 7, int keySize = 128)
    {
        byte[] bytes = Encoding.ASCII.GetBytes("~1B2c3D4e5F6g7H8");
        byte[] rgbSalt = Encoding.ASCII.GetBytes(saltValue);
        byte[] buffer = Encoding.UTF8.GetBytes(data);
        byte[] rgbKey = new PasswordDeriveBytes(passPhrase, rgbSalt, hashAlgorithm, passwordIterations).GetBytes(keySize / 8);
        RijndaelManaged managed = new RijndaelManaged();
        managed.Mode = CipherMode.CBC;
        ICryptoTransform transform = managed.CreateEncryptor(rgbKey, bytes);
        MemoryStream stream = new MemoryStream();
        CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
        stream2.Write(buffer, 0, buffer.Length);
        stream2.FlushFinalBlock();
        byte[] inArray = stream.ToArray();
        stream.Close();
        stream2.Close();
        return Convert.ToBase64String(inArray);
    }

    /// <summary>
    /// Декодирование текста CryptoStream
    /// </summary>
    /// <param name="data"> текст кодирования </param>
    /// <param name="passPhrase"> секретный ключ </param>
    /// <param name="saltValue"> соль </param>
    /// <param name="hashAlgorithm"> MD5 или SHA1 </param>
    /// <param name="passwordIterations"> любое число </param>
    /// <param name="keySize"> может быть 192 или 128 </param>
    /// <returns></returns>
    public string Decrypt(string data, string passPhrase = "test", string saltValue = "test", string hashAlgorithm = "SHA1",
        int passwordIterations = 7, int keySize = 128)
    {
        byte[] bytes = Encoding.ASCII.GetBytes("~1B2c3D4e5F6g7H8");
        byte[] rgbSalt = Encoding.ASCII.GetBytes(saltValue);
        byte[] buffer = Convert.FromBase64String(data);
        byte[] rgbKey = new PasswordDeriveBytes(passPhrase, rgbSalt, hashAlgorithm, passwordIterations).GetBytes(keySize / 8);
        RijndaelManaged managed = new RijndaelManaged();
        managed.Mode = CipherMode.CBC;
        ICryptoTransform transform = managed.CreateDecryptor(rgbKey, bytes);
        MemoryStream stream = new MemoryStream(buffer);
        CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
        byte[] buffer5 = new byte[buffer.Length];
        int count = stream2.Read(buffer5, 0, buffer5.Length);
        stream.Close();
        stream2.Close();
        return Encoding.UTF8.GetString(buffer5, 0, count);
    }
    
    #endregion

    #endregion

    #region Private methods
    //encoder
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width,
                Margin = 0
            }
        };
        return writer.Write(textForEncoding);
    }

    #endregion
}
