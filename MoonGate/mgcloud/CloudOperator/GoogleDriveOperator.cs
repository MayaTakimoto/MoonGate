﻿//-----------------------------------------------------------------------
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
using System.Collections.Generic;
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
        /// MimeType（フォルダ）
        /// </summary>
        private const string MIME_DIR = @"mimeType = 'application/vnd.google-apps.folder'";

        /// <summary>
        /// MimeType（ファイル）
        /// </summary>
        private const string MIME_BINARY = @"application/octet-stream";

        /// <summary>
        /// ファイルリスト取得時の条件
        /// </summary>
        private const string QUERY_GETFILES = @"fullText contains 'mgenf'";

        /// <summary>
        /// ファイルに付加する説明
        /// </summary>
        private const string FILE_DESCRIPTION = @"MoonGate Encrypted File";

        /// <summary>
        /// スコープ定義
        /// </summary>
        private const string OAUTH_SCOPE = @"https://www.googleapis.com/auth/drive.file";

        ///// <summary>
        ///// 認証情報保持ファイルのパス
        ///// </summary>
        //private const string AUTH_PATH = @"GoogleDriveAuthInfo.xml";

        /// <summary>
        /// RSAキーコンテナ名
        /// </summary>
        private const string KEY_CONTAINER_NAME = @"GDR_AUTH";

        //private const string REDIRECT_URI = @"urn:ietf:wg:oauth:2.0:oob";
        
        /// <summary>
        /// 
        /// </summary>
        private IAuthorizationState state = null;
        
        /// <summary>
        /// 
        /// </summary>
        private DriveService dServ = null;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cKey"></param>
        /// <param name="cSec"></param>
        public GoogleDriveOperator(char[] cKey, char[] cSec)
            : base(cKey, cSec) 
        {
            authPath = @"./user/GoogleDrive.xml"; 
        }

        
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fileList"></param>
        ///// <returns>アップロードしたファイル数</returns>
        //public override int UploadFiles(string[] fileList)
        //{
        //    int OKCount = 0;

        //    LoadAuthInfo(AUTH_PATH);

        //    dServ = InitConnection();

        //    foreach (string file in fileList)
        //    {
        //        //int iRet = UploadFile(file);
        //        //if (iRet == 0)
        //        //{
        //        //    OKCount++;
        //        //}
        //    }

        //    // 認証情報の保存。失敗した場合は既存の認証情報を削除する
        //    if (!SaveAuthInfo(AUTH_PATH) && System.IO.File.Exists(AUTH_PATH))
        //    {
        //        System.IO.File.Delete(AUTH_PATH);
        //    }

        //    return OKCount;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override HybridDictionary GetDirList()
        {
            if (dServ == null)
            {
                dServ = InitConnection();
            }

            HybridDictionary CloudDirList = new HybridDictionary(true);

            FilesResource.ListRequest listRequest = dServ.Files.List();
            listRequest.Q = MIME_DIR;

            do
            {
                try
                {
                    FileList fileList = listRequest.Fetch();
                    foreach (var file in fileList.Items)
                    {
                        CloudDirList.Add(file.OriginalFilename, file.Id);
                    }
                    
                    listRequest.PageToken = fileList.NextPageToken;
                }
                catch
                {
                    listRequest.PageToken = null;
                    return null;
                }
            }
            while (!String.IsNullOrEmpty(listRequest.PageToken));

            return CloudDirList;
        }


        /// <summary>
        ///MoonGate互換暗号化ファイルを一覧取得する
        /// </summary>
        /// <returns></returns>
        public override HybridDictionary GetFileList()
        {
            if (dServ == null)
            {
                dServ = InitConnection();
            }

            HybridDictionary DownloadFileList = new HybridDictionary(true);

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
                    return null;
                }
            }
            while (!String.IsNullOrEmpty(listRequest.PageToken));

            return DownloadFileList;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fileList"></param>
        ///// <returns></returns>
        //public override int DownloadFiles(string[] fileList)
        //{
        //    int OKCount = 0;

        //    LoadAuthInfo(AUTH_PATH);

        //    dServ = InitConnection();

        //    foreach (string file in fileList)
        //    {
        //        //int iRet = DownloadFile(file);
        //        //if (iRet == 0)
        //        //{
        //        //    OKCount++;
        //        //}
        //    }

        //    // 認証情報の保存。失敗した場合は既存の認証情報を削除する
        //    if (!SaveAuthInfo(AUTH_PATH) && System.IO.File.Exists(AUTH_PATH))
        //    {
        //        System.IO.File.Delete(AUTH_PATH);
        //    }

        //    return OKCount;
        //}


        /// <summary>
        /// アップロード（メイン）
        /// </summary>
        /// <param name="fileName">アップロードするファイル</param>
        /// <returns></returns>
        public override int UploadFile(string fileName, byte[] data)
        {
            // 引数が空の場合はエラー
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
            {
                return -1;
            }

            // ファイルが存在しない場合はエラー
            if (!System.IO.File.Exists(fileName))
            {
                return -2;
            }

            if (dServ == null)
            {
                dServ = InitConnection();
            }

            // アップロードファイルのひな形を作る
            var uploadFile = new Google.Apis.Drive.v2.Data.File();
            uploadFile.Title = Path.GetFileName(Path.ChangeExtension(fileName, FILE_EXT));
            uploadFile.Description = FILE_DESCRIPTION;
            uploadFile.MimeType = MIME_BINARY;

            MemoryStream ms = new MemoryStream(data);

            FilesResource.InsertMediaUpload uploadRequest = dServ.Files.Insert(uploadFile, ms, MIME_BINARY);
            
            uploadRequest.Upload();
            var file = uploadRequest.ResponseBody;

            return 0;
        }


        /// <summary>
        /// ダウンロード（メイン）
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public override int DownloadFile(string fileUrl, out byte[] data)
        {
            if (dServ == null)
            {
                dServ = InitConnection();
            }

            HttpWebRequest reqh = (HttpWebRequest)WebRequest.Create(fileUrl);
            var auth = dServ.Authenticator;
            auth.ApplyAuthenticationToRequest(reqh);

            HttpWebResponse res = (HttpWebResponse)reqh.GetResponse();

            using (Stream fsSave = res.GetResponseStream())
            {
                using(MemoryStream msDl = new MemoryStream())
                {
                    try
                    {
                        byte[] bytes = new byte[1024];
                        int iReadLength = 0;
                        while ((iReadLength = fsSave.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            msDl.Write(bytes, 0, iReadLength);
                        }
                    }
                    catch
                    {
                        data = null;
                        return -1;
                    }

                    data = msDl.ToArray();
                }
            }
            
            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void SetAuthInfo()
        {
            AuthInfoCipher cipher = new AuthInfoCipher();

            EntAuth.AccessToken = cipher.EncryptRsa(state.AccessToken.ToCharArray(), KEY_CONTAINER_NAME);
            EntAuth.RefreshToken = cipher.EncryptRsa(state.RefreshToken.ToCharArray(), KEY_CONTAINER_NAME);
            EntAuth.TokenLimit = (DateTime)state.AccessTokenExpirationUtc;
        }


        /// <summary>
        /// GogleDrive接続準備
        /// </summary>
        /// <returns></returns>
        private DriveService InitConnection()
        {
            // DriverServiceオブジェクトを初期化する
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, new string(ConsumerKey), new string(ConsumerSecret));
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
            // stateオブジェクトの初期化
            state = new AuthorizationState(new[] { DriveService.Scopes.Drive.GetStringValue() });
             
            // 初回認証時
            if (FirstAuthFlg)
            {
                state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
                return FirstAuthorize(arg);
            }
            // 過去に認証に成功している場合
            else
            {
                // まだ接続していない場合
                if (!ReadyConFlg)
                {
                    AuthInfoCipher cipher = new AuthInfoCipher();
                    char[] accessToken = cipher.DecryptRsa(EntAuth.AccessToken, KEY_CONTAINER_NAME);
                    char[] refreshToken = cipher.DecryptRsa(EntAuth.RefreshToken, KEY_CONTAINER_NAME);

                    state.AccessToken = new string(accessToken);             // アクセストークンをセット
                    state.RefreshToken = new string(refreshToken);           // リフレッシュトークンをセット
                    state.AccessTokenExpirationUtc = EntAuth.TokenLimit;     // アクセストークンの有効期限をセット

                    cipher.DeleteKeys(KEY_CONTAINER_NAME);
                }

                // アクセストークンの有効期限が切れていたら再取得する
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
        private IAuthorizationState FirstAuthorize(NativeApplicationClient arg)
        {
            Uri authUri = arg.RequestUserAuthorization(state);

            // ブラウザを立ち上げ、認証画面へ遷移させる
            Process.Start(authUri.ToString());

            // アクセスコードを取得
            string authCode = Interaction.InputBox("Type Authrorization Code!", "Authorize");

            FirstAuthFlg = false;
            ReadyConFlg = true;
            return arg.ProcessUserAuthorization(authCode, state);
        }


        /// <summary>
        /// 後始末メソッド
        /// </summary>
        public override void Dispose()
        {
            state = null;
            dServ = null;
            base.Dispose();
        }
    }
}
