using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HttpManager
{
    public class HttpRequest : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            StartCoroutine(TestWebRequest(@"http://127.0.0.1:8080/test/injectfix/Android_10.0.7_1"));
        }

       private IEnumerator TestWebRequest(string url)
        {
            DownLoadHandler downLoadHandler = new DownLoadHandler(new byte[1024 * 200], "d:/aaa",x=>Debug.LogFormat("文件大小：{0}",x),x=>Debug.LogFormat("下载进度：{0}",x*100));
            using (UnityWebRequest uwr = UnityWebRequest.Get(url))
            {
                uwr.chunkedTransfer = true;
                uwr.disposeDownloadHandlerOnDispose = true;
                uwr.SetRequestHeader("Range", "bytes=" + downLoadHandler.fileCurSize+"-");
                uwr.downloadHandler = downLoadHandler;
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                    downLoadHandler.Dispose();
                }
            }
        }
    }
}

