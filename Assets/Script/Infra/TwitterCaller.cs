using UnityEngine;
using UnityEngine.UI;

public static class TwitterCaller {
    private static readonly string linkUrl = "https://unityroom.com/games/poppincubes";
    private static readonly string hashtags = "unity1week,gamedev,POPPIN'CUBES";

    public static void Tweeting (string message) {
        string text = message;
        var url = "https://twitter.com/intent/tweet?" +
            "text=" + text +
            "&url=" + linkUrl +
            "&hashtags=" + hashtags;

#if UNITY_EDITOR
        Application.OpenURL (url);
#elif UNITY_WEBGL
        Application.ExternalEval (string.Format ("window.open('{0}','_blank')", url));
#else
        Application.OpenURL (url);
#endif
    }
}