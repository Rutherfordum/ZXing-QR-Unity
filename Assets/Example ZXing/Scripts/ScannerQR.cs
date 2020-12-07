using UnityEngine;
using UnityEngine.UI;

public class ScannerQR : ZXingQR
{
    [SerializeField] private RawImage outRawImageQR;
    [SerializeField] private InputField inputTextToQR;
    [SerializeField] private RawImage outCameraWebTexture;
    [SerializeField] private Text outDecodeText;
    [SerializeField] private AspectRatioFitter fit;

    private string textToQR;
    private WebCamTexture webCam;

    // получаем разрешение к камере 
    private void Awake()
    {
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
    }

    // запуск камеры устройства
    void Start()
    {
        openCameraDevice();
    }

    //получаем текст для кодирования 
    public void OnGetText()
    {
        textToQR = inputTextToQR.text;
    }

    // кодирование текста в QR код
    public void OnGenerateQR()
    {
        outRawImageQR.texture = GenerateQR(textToQR);
    }

    // декодирование QR кода в текст
    public void OnRegenerateQR()
    {
        if (ReGenerateQR(webCam) != null)
            outDecodeText.text = ReGenerateQR(webCam);
        else
            outDecodeText.text = "Not found QR code";
    }

    private void openCameraDevice()
    {
        webCam = new WebCamTexture();
        webCam.Play();


        if (webCam.isPlaying)
            outCameraWebTexture.texture = webCam;
        else
            Debug.LogWarning(webCam.name + "is not available");

        float ratio = (float)webCam.width / (float)webCam.height;
        fit.aspectRatio = ratio;
        float scaleY = webCam.videoVerticallyMirrored ? -1f : 1f;
        outCameraWebTexture.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -webCam.videoRotationAngle;
        outCameraWebTexture.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }
}
