using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using JapaneseSearch.Views;
using JapaneseSearch.Models;

namespace JapaneseSearch
{
    [Activity(Label = "日本語を調べよう", MainLauncher = true, Theme = "@android:style/Theme.Material")]
    public class MainActivity : Activity
    {
        bool searched = false;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;

            //デスクトップ版のウェブサイトに導くため、デスクトップブラウザーのインフォメーションを送る。
            String newUA = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9.0.4) Gecko/20100101 Firefox/4.0";
            webView.Settings.UserAgentString = newUA;
            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.SetSupportZoom(true);
            webView.Settings.DefaultZoom = WebSettings.ZoomDensity.Far;
            webView.SetWebViewClient(new HybridWebViewClient());

            var editText = (EditText)FindViewById<EditText>(Resource.Id.editText);
            editText.Click += (o, e) =>
            {
                if (searched)
                {
                    editText.Text = null;
                }
            };
            editText.KeyPress += (o, e) =>
            {
                if (e.KeyCode == Keycode.Enter ||
                    e.KeyCode == Keycode.Search)
                {
                    webView.LoadUrl("https://dict.hjenglish.com/jp/jc/" + editText.Text);
                    webView.RequestFocus();
                    webView.FindFocus();
                }
                else
                {
                    e.Handled = false;
                }
            };

            var buttonEdo = (Button)FindViewById<Button>(Resource.Id.buttonEdo);
            buttonEdo.Click += (o, e) =>
            {
                if (editText.Text != null)
                    webView.LoadUrl("https://dict.hjenglish.com/jp/jc/" + editText.Text);
            };
            var buttonIMG = (Button)FindViewById<Button>(Resource.Id.buttonIMG);
            buttonIMG.Click += (o, e) =>
            {
                if (editText.Text != null)
                    webView.LoadUrl("https://www.google.co.jp/search?q=" + editText.Text + "&tbm=isch&client=ms-android-htc");
            };
            var buttonGoogle = (Button)FindViewById<Button>(Resource.Id.buttonGoogle);
            buttonGoogle.Click += (o, e) =>
            {
                if (editText.Text != null)
                    webView.LoadUrl("https://www.google.co.jp/search?q=" + editText.Text + "&client=ms-android-htc");
            };
        }

        public class HybridWebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {

                view.LoadUrl(url);
                return true;
            }
            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
            }
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
            }
            public override void OnReceivedError(WebView view, [GeneratedEnum] ClientError errorCode, string description, string failingUrl)
            {
                base.OnReceivedError(view, errorCode, description, failingUrl);
            }




        }
    }
}

