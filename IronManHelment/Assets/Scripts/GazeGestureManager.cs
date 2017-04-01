using UnityEngine;
using UnityEngine.VR.WSA.Input;
using UnityEngine.VR.WSA.WebCam;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }
    public GameObject facePrefab = null;
    public Texture testTexture = null;
    public GameObject FocusedObject { get; private set; }

    GestureRecognizer recognizer = null;
    Resolution cameraResolution;
    PhotoCapture photoCaptureObject = null;

    Vector3 cameraPosition = Vector3.zero;
    Quaternion cameraRotation = new Quaternion();

    public GameObject textPrefab = null;
    public GameObject status = null;
    public GameObject framePrefab = null;

    string FaceAPIKey = "630f9a1163b94bc7a60f712e0248c94c";
    string EmotionAPIKey = "f6a8f69e941f4863bf3f3175b91c80bd";

    // Update is called once per frame
    void Update()
    {
        // Figure out which hologram is focused this frame.
        GameObject oldFocusObject = FocusedObject;

        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            FocusedObject = hitInfo.collider.gameObject;
        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            FocusedObject = null;
        }

        // If the focused object changed this frame,
        // start detecting fresh gestures again.
        if (FocusedObject != oldFocusObject)
        {
            recognizer.CancelGestures();
            recognizer.StartCapturingGestures();
        }
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.PNG;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    }

    IEnumerator<object> PostToFaceAPI(byte[] imageData, Matrix4x4 cameraToWorldMatrix, Matrix4x4 pixelToCameraMatrix)
    {
        var url = "https://api.projectoxford.ai/face/v1.0/detect?returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses";
        var headers = new Dictionary<string, string>() {
            { "Ocp-Apim-Subscription-Key", FaceAPIKey },
            { "Content-Type", "application/octet-stream" }
        };

        WWW www = new WWW(url, imageData, headers);
        yield return www;
        string responseString = www.text;

        JSONObject j = new JSONObject(responseString);
        Debug.Log(j);

        if (j.list.Count == 0)
        {
            status.GetComponent<TextMesh>().text = "No faces found";
            yield break;
        }
        status.GetComponent<TextMesh>().text = "Ready";

        var faceRectangles = "";
        Dictionary<string, TextMesh> textmeshes = new Dictionary<string, TextMesh>();

        int count = 0;
        float offsetY = 0;
        foreach (var result in j.list)
        {
            GameObject txtObject = Instantiate(textPrefab);
            TextMesh txtMesh = txtObject.GetComponent<TextMesh>();
            var a = result.GetField("faceAttributes");
            var f = a.GetField("facialHair");
            var p = result.GetField("faceRectangle");

            string id = string.Format("{0},{1},{2},{3}", p.GetField("left"), p.GetField("top"), p.GetField("width"), p.GetField("height"));
            textmeshes[id] = txtMesh;

            try
            {
                var source = new Texture2D(0, 0);
                source.LoadImage(imageData);
                var dest = new Texture2D((int)p["width"].i, (int)p["height"].i);
                dest.SetPixels(source.GetPixels((int)p["left"].i, cameraResolution.height - (int)p["top"].i - (int)p["height"].i, (int)p["width"].i, (int)p["height"].i));
                byte[] justThisFace = dest.EncodeToPNG();
                string filepath = Path.Combine(Application.persistentDataPath, "face_" + count.ToString() + ".png");
                File.WriteAllBytes(filepath, justThisFace);

                GameObject parent = GameObject.Find("Faces");
                parent.transform.position = Camera.main.transform.position;
                parent.transform.position += Camera.main.transform.right * 0.35f;
                parent.transform.position += Camera.main.transform.up * 0.17f;
                parent.transform.position += Camera.main.transform.forward * 2.0f;

                Texture2D texture = GameObject.FindGameObjectWithTag("GameManager").GetComponent<IMG2Sprite>().LoadTexture(filepath);
                GameObject newGo = Instantiate(facePrefab, parent.transform.position, Quaternion.Euler(90, 180, 0));
                newGo.transform.LookAt(Camera.main.transform);
                newGo.transform.rotation = Quaternion.Euler(90 + newGo.transform.eulerAngles.x, newGo.transform.eulerAngles.y, newGo.transform.eulerAngles.z);
                newGo.transform.position = new Vector3(newGo.transform.position.x, newGo.transform.position.y + offsetY, newGo.transform.position.z);
                newGo.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
                newGo.GetComponent<MeshRenderer>().material.mainTexture = texture;
                newGo.name = "face_" + count.ToString();
                newGo.tag = "facePicture";
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (faceRectangles == "")
                faceRectangles = id;
            else
                faceRectangles += ";" + id;

            GameObject pictureParent = GameObject.Find("face_" + count.ToString());
            txtObject.transform.LookAt(Camera.main.transform);
            txtObject.transform.position = new Vector3(pictureParent.transform.position.x + 0.13f, pictureParent.transform.position.y + 0.1f, pictureParent.transform.position.z);
            txtObject.tag = "faceText";
            
            txtMesh.text = string.Format("Gender: {0}\nAge: {1}\nMoustache: {2}\nBeard: {3}\nSideburns: {4}\nSmile: {5}", a.GetField("gender").str, a.GetField("age"), f.GetField("moustache"), f.GetField("beard"), f.GetField("sideburns"), a.GetField("smile"));
            count++;
            offsetY -= 0.28f;
        }

        // Emotion API
        url = "https://api.projectoxford.ai/emotion/v1.0/recognize?faceRectangles=" + faceRectangles;
        headers["Ocp-Apim-Subscription-Key"] = EmotionAPIKey;
        www = new WWW(url, imageData, headers);
        yield return www;
        responseString = www.text;

        j = new JSONObject(responseString);

        foreach (var result in j.list)
        {
            var p = result.GetField("faceRectangle");
            string id = string.Format("{0},{1},{2},{3}", p.GetField("left"), p.GetField("top"), p.GetField("width"), p.GetField("height"));
            var txtMesh = textmeshes[id];
            var obj = result.GetField("scores");
            string highestEmote = "Unknown";
            float highestC = 0;
            for (int i = 0; i < obj.list.Count; i++)
            {
                string key = obj.keys[i];
                float c = obj.list[i].f;
                if (c > highestC)
                {
                    highestEmote = key;
                    highestC = c;
                }
            }
            txtMesh.text += "\nEmotion: " + highestEmote;
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            List<byte> imageBufferList = new List<byte>();
            // Copy the raw IMFMediaBuffer data into our empty byte list.
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

            var cameraToWorldMatrix = new Matrix4x4();
            photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);

            cameraPosition = cameraToWorldMatrix.MultiplyPoint3x4(new Vector3(0, 0, -1));
            cameraRotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));

            Matrix4x4 projectionMatrix;
            photoCaptureFrame.TryGetProjectionMatrix(Camera.main.nearClipPlane, Camera.main.farClipPlane, out projectionMatrix);
            Matrix4x4 pixelToCameraMatrix = projectionMatrix.inverse;

            status.GetComponent<TextMesh>().text = "Processing";

            StartCoroutine(PostToFaceAPI(imageBufferList.ToArray(), cameraToWorldMatrix, pixelToCameraMatrix));
        }
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);

    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    void Awake()
    {
        Camera.main.nearClipPlane = 100.0f;
        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            OnScan();
        };
        recognizer.StartCapturingGestures();
    }

    void OnScan()
    {
        OnClear();
        status.GetComponent<TextMesh>().text = "Scanning";
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void OnClear()
    {
        status.GetComponent<TextMesh>().text = "Ready";

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("faceBounds");
        foreach (GameObject enemy in gameObjects)
            Destroy(enemy);

        gameObjects = GameObject.FindGameObjectsWithTag("faceText");
        foreach (GameObject enemy in gameObjects)
            Destroy(enemy);

        gameObjects = GameObject.FindGameObjectsWithTag("facePicture");
        foreach (GameObject enemy in gameObjects)
            Destroy(enemy);

        gameObjects = GameObject.FindGameObjectsWithTag("emoteText");
        foreach (GameObject enemy in gameObjects)
            Destroy(enemy);
    }

    void OnReset()
    {
        Camera.main.nearClipPlane = 100;
    }

    void OnInitiate()
    {
        Camera.main.nearClipPlane = 0.85f;
    }
}
