using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace HttpManager
{
    public class DownLoadHandler : DownloadHandlerScript
    {

        private string savePath;
        private string fileName;
        private FileStream fileStream;
        public long fileTotalSize;
        public long fileCurSize;

        public event Action<long> GetFileTotalSizeCallBack;
        public event Action<float> GetProgressCallBack;

        public float progress;
        /// <summary>
        /// 初始化下载器
        /// </summary>
        /// <param name="preallocatedBuffer">每次下载的byte[] 容器（自定义大小）</param>
        /// <param name="savePath">保存的路径</param>
        public DownLoadHandler(byte[] preallocatedBuffer,string savePath,Action<long> GetFileTotalSizeCallBack=null,Action<float> GetProgressCallBack=null) : base(preallocatedBuffer)
        {
            this.savePath = savePath.Replace("\\","/");
            fileName = savePath.Substring(savePath.LastIndexOf('/')+1);
            fileStream = new FileStream(savePath,FileMode.Append,FileAccess.Write);
            fileCurSize = fileStream.Length;
            this.GetFileTotalSizeCallBack = GetFileTotalSizeCallBack;
            this.GetProgressCallBack = GetProgressCallBack;
        }

        /// <summary>
        /// 文件下载完成
        /// </summary>
        protected override void CompleteContent()
        {
            fileStream.Close();
            fileStream.Dispose();
        }

        protected override float GetProgress()
        {
            progress = base.GetProgress();
            return base.GetProgress();
        }

        /// <summary>
        /// 请求下载时的第一个回调函数，返回需要接收的文件总长度
        /// </summary>
        /// <param name="contentLength"></param>
        protected override void ReceiveContentLength(int contentLength)
        {
            if (contentLength == 0)
            {
                Debug.LogFormat("【下载完成】文件{0}下载完成...",fileName);
                CompleteContent();
            }
            fileTotalSize = contentLength;
            GetFileTotalSizeCallBack(fileTotalSize);
        }

        /// <summary>
        /// 从网络获取数据时候的回调，每帧调用一次
        /// </summary>
        /// <param name="data">接收到的数据字节流，总长度为构造函数定义的长度，并非所有的数据都是新的</param>
        /// <param name="dataLength">接收到的数据长度，表示data字节流数组中有多少数据是新接收到的，即0-dataLength之间的数据是刚接收到的</param>
        /// <returns>返回true为继续下载，返回false为中断下载</returns>
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length <= 0)
            {
                Debug.LogFormat("【下载中】文件{0}，没有获取到数据...", fileName);
                return false;
            }
            fileStream.Write(data, 0, dataLength);
            fileCurSize += dataLength;

            float progress= fileCurSize / fileTotalSize;

            GetProgressCallBack(progress);

            return true;
        }

        public void ErrorDispose()
        {
            fileStream.Close();
            fileStream.Dispose();
        }
    }
}

