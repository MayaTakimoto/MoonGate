//-----------------------------------------------------------------------
// <summary>GoogleDrive通信クラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  GMT $</date>
// <copyright file="$Name: GoogleDriveConnector.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util;
using mgcloud.Config;
using Microsoft.VisualBasic;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace mgcloud.CloudOperator
{
    /// <summary>
    /// GoogleDriveとの通信を担うクラス
    /// </summary>
    public class GoogleDriveOperator : BaseCloudOperator
    {
        /// <summary>
        /// 定数フィールド
        /// </summary>
        private const string MIME_FDSEARCH = @"mimeType = 'application/vnd.google-apps.folder'";
        private const string MIME_GETFILES = @"application/octet-stream";
        private const string QUERY_GETFILES = @"fullText contains 'zip'";
        private const string FILE_DESCRIPTION = @"MoonGate Encrypted File";
        //private const string CLIENT_ID = @"815568550348.apps.googleusercontent.com";
        //private const string CLIENT_SECRET = @"yRDhSlYssPrYYjTNuVRpsiEh";
        //private const string REDIRECT_URI = @"urn:ietf:wg:oauth:2.0:oob";
        private const string OAUTH_SCOPE = @"https://www.googleapis.com/auth/drive.file";
        private const string AUTH_PATH = @"GoogleDriveAuthInfo.xml";
        private const string KEY_CONTAINER_NAME = @"GDR_AUTH";
        
        /// <summary>
        /// 
        /// </summary>
        private IAuthorizationState state;
        
        /// <summary>
        /// 
        /// </summary>
        private DriveService dServ;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cKey"></param>
        /// <param name="cSec"></param>
        public GoogleDriveOperator(string cKey, string cSec)
            : base(cKey, cSec) { }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns>アップロードしたファイル数</returns>
        public override int UploadFiles(string[] fileList)
        {
            int OKCount = 0;

            LoadAuthInfo(AUTH_PATH);

            dServ = InitConnection();

            foreach (string file in fileList)
            {
                int iRet = UploadFile(file);
                if (iRet == 0)
                {
                    OKCount++;
                }
            }

            // 認証情報の保存。失敗した場合は既存の認証情報を削除する
            if (!SaveAuthInfo(AUTH_PATH) && System.IO.File.Exists(AUTH_PATH))
            {
                System.IO.File.Delete(AUTH_PATH);
            }

            return OKCount;
        }



        /// <summary>
        ///MoonGate互換暗号化ファイルを一覧取得する
        /// </summary>
        /// <returns></returns>
        public override int GetFileList()
        {
            LoadAuthInfo(AUTH_PATH);

            dServ = InitConnection();

            DownloadFileList = new HybridDictionary();

            FilesResource.ListRequest listRequest = dServ.Files.List();
            listRequest.Q = QUERY_GETFILES;

            do
            {
                try
                {
                    FileList fileList = listRequest.Fetch();
                    foreach (var file in fileList.Items)
                    {
                        DownloadFileList.Add(file.OriginalFilename, file.DownloadUrl);
                    }

                    listRequest.PageToken = fileList.NextPageToken;
                }
                catch
                {
                    listRequest.PageToken = null;
                    return -1;
                }
            }
            while (!String.IsNullOrEmpty(listRequest.PageToken));

            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public override int DownloadFiles(string[] fileList)
        {
            int OKCount = 0;

            LoadAuthInfo(AUTH_PATH);

            dServ = InitConnection();

            foreach (string file in fileList)
            {
                int iRet = DownloadFile(file);
                if (iRet == 0)
                {
                    OKCount++;
                }
            }

            // 認証情報の保存。失敗した場合は既存の認証情報を削除する
            if (!SaveAuthInfo(AUTH_PATH) && System.IO.File.Exists(AUTH_PATH))
            {
                System.IO.File.Delete(AUTH_PATH);
            }

            return OKCount;
        }


        /// <summary>
        /// アップロード（メイン）
        /// </summary>
        /// <param name="filePath">アップロードするファイル</param>
        /// <returns></returns>
        protected override int UploadFile(string filePath)
        {
            // 引数が空の場合はエラー
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath))
            {
                return -1;
            }

            // ファイルが存在しない場合はエラー
            if (!System.IO.File.Exists(filePath))
            {
                return -2;
            }

            // アップロードファイルのひな形を作る
            var uploadFile = new Google.Apis.Drive.v2.Data.File();
            uploadFile.Title = filePath;
            uploadFile.Description = FILE_DESCRIPTION;
            uploadFile.MimeType = MIME_GETFILES;

            byte[] ulData = System.IO.File.ReadAllBytes(filePath);
            MemoryStream ms = new MemoryStream(ulData);

            FilesResource.InsertMediaUpload uploadRequest = dServ.Files.Insert(uploadFile, ms, MIME_GETFILES);
            
            uploadRequest.Upload();
            var file = uploadRequest.ResponseBody;

            return 0;
        }


        /// <summary>
        /// ダウンロード（メイン）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected override int DownloadFile(string fileName)
        {
            string downloadUrl = DownloadFileList[fileName].ToString();

            HttpWebRequest reqh = (HttpWebRequest)WebRequest.Create(downloadUrl);
            var auth = dServ.Authenticator;
            auth.ApplyAuthenticationToRequest(reqh);

            HttpWebResponse res = (HttpWebResponse)reqh.GetResponse();

            using (Stream fsSave = res.GetResponseStream())
            {
                using (FileStream dlFile = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        byte[] bytes = new byte[1024];
                        int iReadLength = 0;
                        while ((iReadLength = fsSave.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            dlFile.Write(bytes, 0, iReadLength);
                        }
                    }
                    catch
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }



        /// <summary>
        /// GogleDrive接続準備
        /// </summary>
        /// <returns></returns>
        private DriveService InitConnection()
        {
            // GoogleDriveとコネクトするためのお決まり
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, ConsumerKey, ConsumerSecret);
            var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
            dServ = new DriveService(auth);

            return dServ;
        }


        /// <summary>
        /// OAuth認証を取得する
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            // 認証URLの取得
            state = new AuthorizationState(new[] { DriveService.Scopes.Drive.GetStringValue() });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            if (FirstAuthFlg)
            {
                return FirstAuthorize(arg, authUri);
            }
            else
            {
                if (!ReadyConFlg)
                {
                    state.AccessToken = AuthInfoCipher.DecryptRsa(EntAuth.AccessToken, KEY_CONTAINER_NAME);     // アクセストークンをセット
                    state.RefreshToken = AuthInfoCipher.DecryptRsa(EntAuth.RefreshToken, KEY_CONTAINER_NAME);   // リフレッシュトークンをセット
                    state.AccessTokenExpirationUtc = EntAuth.TokenLimit;                                        // アクセストークンの有効期限をセット

                    AuthInfoCipher.DeleteKeys(KEY_CONTAINER_NAME);
                }

                if (state.AccessTokenExpirationUtc < DateTime.Now)
                {
                    arg.RefreshToken(state);
                }

                ReadyConFlg = true;
                return state;
            }
        }


        /// <summary>
        /// 初回認証時の処理
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="authUri"></param>
        /// <returns></returns>
        private IAuthorizationState FirstAuthorize(NativeApplicationClient arg, Uri authUri)
        {
            // ブラウザを立ち上げ、認証画面へ遷移させる
            Process.Start(authUri.ToString());

            // アクセスコードを取得
            string authCode = Interaction.InputBox("Type Authrorization Code!", "Authorize");

            FirstAuthFlg = false;
            ReadyConFlg = true;
            return arg.ProcessUserAuthorization(authCode, state);
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            state = null;
            dServ = null;
            base.Dispose();
        }
    }
}
