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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace mgcloud.Connector
{
    /// <summary>
    /// GoogleDriveとの通信を担うクラス
    /// </summary>
    public class GoogleDriveConnector : BaseConnector
    {
        /// <summary>
        /// 定数フィールド
        /// </summary>
        private const string QUERY_FDSEARCH = @"mimeType = 'application/vnd.google-apps.folder'";
        private const string CLIENT_ID = @"815568550348.apps.googleusercontent.com";
        private const string CLIENT_SECRET = @"yRDhSlYssPrYYjTNuVRpsiEh";
        private const string REDIRECT_URI = @"urn:ietf:wg:oauth:2.0:oob";
        private const string OAUTH_SCOPE = @"https://www.googleapis.com/auth/drive.file";
        private const string AUTH_PATH = @"GoogleDriveAuthInfo.xml";
        private const string KEY_CONTAINER_NAME = @"GDR_AUTH";
        
        
        /// <summary>
        /// 
        /// </summary>
        private IAuthorizationState state;


        /// <summary>
        /// 認証情報保持エンティティ
        /// </summary>
        private AuthInfoEntity entAuth;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GoogleDriveConnector()
            : base()
        {
            FirstAuthFlg = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns>アップロードしたファイル数</returns>
        public override int UploadFiles(string[] fileList)
        {
            int OKCount = 0;

            entAuth = new AuthInfoEntity();
            AuthInfoSerializer aiSerializer = new AuthInfoSerializer(AUTH_PATH);

            if (aiSerializer.TryDeserialize(out entAuth))
            {
                FirstAuthFlg = true;
            }

            foreach (string file in fileList)
            {
                int iRet = UploadFile(file);
                if (iRet == 0)
                {
                    OKCount++;
                }
            }

            return OKCount;
        }


        /// <summary>
        ///MoonGate互換暗号化ファイルを一覧取得する
        /// </summary>
        /// <returns></returns>
        public override List<string> GetFileList()
        {
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public override int DownloadFiles(string[] fileList)
        {
            int OKCount = 0;

            entAuth = new AuthInfoEntity();
            AuthInfoSerializer aiSerializer = new AuthInfoSerializer(AUTH_PATH);

            if (aiSerializer.TryDeserialize(out entAuth))
            {
                FirstAuthFlg = true;
            }

            foreach (string file in fileList)
            {
                int iRet = DownloadFile(file);
                if (iRet == 0)
                {
                    OKCount++;
                }
            }

            return OKCount;
        }



        /// <summary>
        /// アップロード
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

            // 
            DriveService dServ = InitConnection();

            // アップロードファイルのひな形を作る
            Google.Apis.Drive.v2.Data.File uploadFile = new Google.Apis.Drive.v2.Data.File();
            uploadFile.Title = filePath;
            uploadFile.Description = "MoonGate Encrypted File";
            uploadFile.MimeType = "application/octet-stream";

            byte[] byteArray = System.IO.File.ReadAllBytes(filePath);
            MemoryStream stream = new MemoryStream(byteArray);

            FilesResource.InsertMediaUpload request = dServ.Files.Insert(uploadFile, stream, "application/octet-stream");


            request.Upload();
            Google.Apis.Drive.v2.Data.File file = request.ResponseBody;

            return 0;
        }

        
        protected override int DownloadFile(string filePath)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// GogleDrive接続準備
        /// </summary>
        /// <returns></returns>
        private DriveService InitConnection()
        {
            // GoogleDriveとコネクトするためのお決まり
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, CLIENT_ID, CLIENT_SECRET);
            var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
            DriveService dServ = new DriveService(auth);

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
                state.AccessToken = entAuth.AccessToken;                // アクセストークンをセット
                state.RefreshToken = entAuth.RefreshToken;              // リフレッシュトークンをセット
                state.AccessTokenExpirationUtc = entAuth.TokenLimit;    // アクセストークンの有効期限をセット

                if (state.AccessTokenExpirationUtc < DateTime.Now)
                {
                    arg.RefreshToken(state);
                }

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

            return arg.ProcessUserAuthorization(authCode, state);
        }


    }
}
