***********************************************************************
【 ソフト名 】 MoonGate
【バージョン】 0.0.1
【最終更新日】 2013年7月2日
【 作 成 者 】 MayaTakimoto（山田知輝）
【 動作環境 】 XP,Vista,7（.NET Framework 4 フルパッケージが必要）
【 メ ー ル 】 allerbmu.reverse@gmail.com
【 Ｕ Ｒ Ｌ 】 https://sites.google.com/site/exceptionlaboratory/home
***********************************************************************


【はじめに】

MoonGateはクラウドストレージにファイルを暗号化してアップロードしたり、
逆にダウンロードして復号したりするソフトウェアです。

クラウドストレージは非常に便利であり、また無くてはならないものに
なりつつあります。しかしこの手のサービスには、どうしてもセキュリティの
問題がつきまといます。見られては困るデータが流出したり、非公開に設定
したはずのフォルダが公開設定になってしまったり、といったインシデントは
現実に頻繁に起こっています。クラウドの力を活用したくても、これでは躊躇
せざるを得ません。

そこで、アップロード前に暗号化することが必要になります。しかし、暗号化
とアップロードは別のソフトウェアが個別に担うことになりがちで、時間と
手間がかかっていました。MoonGateはこの2つの役割を1つのソフトでやろう、
というコンセプトのもとに開発したソフトです。

MoonGateはGPLv3ライセンスのもと、フリーのオープンソースソフトとして
配布しています。本ソフト、およびmgcrypt.dllとmgcloud.dllの著作権は
私、MayaTakimotoこと山田知輝が保持します。ただし、本ソフトで
参照している各種DLLの著作権は、それらDLLの開発元にあります。
また、ツールバーアイコンにはフリーのアイコン素材、GLYPHICONS FREEを
利用させていただいています（配布サイト：http://glyphicons.com/）。
GLYPHICONS FREEの著作権は作者であるJan Kovařík 氏にあります。
詳しくは、Lisenseフォルダ内の各テキストを参照してください。


【使い方】

本ソフトを使用する前に、アプリ登録を行なってIDとSecretKeyを取得する必要が
あります。ここでは例としてGoogleドライブを利用可能にしてみます。

参考URL
http://www.eisbahn.jp/yoichiro/2012/10/google-drive-api-ruby-on-rails.html


１．https://code.google.com/apis/console/ にアクセスし、Create Projectを
    クリックする。

２．左のメニューからServicesを選択する。

３．表示された中にあるDriveAPIをONにする。

４．左のメニューからAPI Accessを選択する。

５．Create an OAuth 2.0 client IDをクリックする。

６．Application typeはInstalled Applicationを選択する。
    Installed application typeはOtherを選択する。

７．Create Client IDをクリックし、表示されるClient IDとClient secretを
    保持する。

８．MoonGate.exeを起動し、Registet ConsumerInfo（ペンのマークのボタン）
    をクリックする。または、ConsumerSignup.exeを起動する。

９．Storage TypeからGoogle Driveを選択する。

１０．Keyに取得したClient IDを、Secretに取得したClient Secretを入力し、
      OKをクリックする。


以上の手順を踏むことで、GoogleドライブをMoonGateから利用できるように
なります。

本ソフトでは、アップロードするときもダウンロードするときも、一度リストに
登録する必要があります。
アップロード時は、ファイル選択ダイアログから対象ファイルを選択してください。
ダウンロード時は、Add Files From Cloudボタン（雲のマークのボタン）をクリック
するか、メニューからAdd Files From Cloudを選択してください。
リスト上のアイテムを選択してUpload

（現状ではアップロード先はGoogleドライブのマイドライブ直下しか選択出来ません。
また、ダウンロード時はマイドライブ直下の対象となり得るファイルをすべてリストに
追加します。この点については今後改善して行きたいと考えています。）
