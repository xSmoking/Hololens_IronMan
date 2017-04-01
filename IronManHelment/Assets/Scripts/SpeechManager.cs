using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();


    // Use this for initialization
    void Start()
    {
        keywords.Add("Jarvis Target", () =>
        {
            BroadcastMessage("OnWakeUp");
        });

        keywords.Add("Jarvis Scan", () =>
        {
            BroadcastMessage("OnScan");
        });
        
        keywords.Add("Jarvis Reset", () =>
        {
            BroadcastMessage("OnReset");
        });

        keywords.Add("Jarvis Toggle", () =>
        {
            BroadcastMessage("OnToggle");
        });
        
        keywords.Add("Jarvis Activate", () =>
        {
            BroadcastMessage("OnActivate");
            BroadcastMessage("Rotate");
        });
        keywords.Add("Jarvis Deactivate", () =>
        {
            BroadcastMessage("OnDeActivate");
            BroadcastMessage("StopRotate");
        });

        keywords.Add("Jarvis Initiate", () =>
        {
            BroadcastMessage("OnInitiate");
        });

        keywords.Add("Jarvis Clear", () =>
        {
            BroadcastMessage("OnClear");
        });

        //keywords.Add("Drop Sphere", () =>
        //{
        //    var focusObject = GazeGestureManager.Instance.FocusedObject;
        //    if (focusObject != null)
        //    {
        //        // Call the OnDrop method on just the focused object.
        //        focusObject.SendMessage("OnDrop");
        //    }
        //});

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}
