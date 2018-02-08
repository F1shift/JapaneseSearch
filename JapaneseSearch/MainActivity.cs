using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using JapaneseSearch.Views;
using JapaneseSearch.Models;

namespace JapaneseSearch
{
    [Activity(Label = "日本語検索", MainLauncher = true, Theme = "@android:style/Theme.Material")]
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
            String newUA_desktop = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9.0.4) Gecko/20100101 Firefox/4.0";
            String newUA_mobile = "Mozilla/5.0 (Linux; Android 5.1.1; Nexus 5 Build/LMY48B; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/43.0.2357.65 Mobile Safari/537.36";
            webView.Settings.UserAgentString = newUA_desktop;
            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.SetSupportZoom(true);
            webView.Settings.DefaultZoom = WebSettings.ZoomDensity.Far;
            webView.SetWebViewClient(new HybridWebViewClient());

            Action hideKeyboard = new Action(() =>
            {
                InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(
                        this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
            });

            var editText = (EditText)FindViewById<EditText>(Resource.Id.editText);
            editText.KeyPress += (o, e) =>
            {
                if (e.KeyCode == Keycode.Enter ||
                    e.KeyCode == Keycode.Search)
                {
                    webView.Settings.UserAgentString = newUA_desktop;
                    webView.LoadUrl("https://dict.hjenglish.com/jp/jc/" + editText.Text);
                    hideKeyboard();
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
                {
                    webView.Settings.UserAgentString = newUA_desktop;
                    webView.LoadUrl("https://dict.hjenglish.com/jp/jc/" + editText.Text);
                    hideKeyboard();
                }
            };
            var buttonIMG = (Button)FindViewById<Button>(Resource.Id.buttonIMG);
            buttonIMG.Click += (o, e) =>
            {
                if (editText.Text != null)
                {
                    webView.Settings.UserAgentString = newUA_mobile;
                    webView.LoadUrl("https://www.google.co.jp/search?q=" + editText.Text + "&tbm=isch");
                    hideKeyboard();
                }
            };
            var buttonGoogle = (Button)FindViewById<Button>(Resource.Id.buttonGoogle);
            buttonGoogle.Click += (o, e) =>
            {
                if (editText.Text != null)
                {
                    webView.Settings.UserAgentString = newUA_mobile;
                    webView.LoadUrl("https://www.google.co.jp/search?q=" + editText.Text);
                    hideKeyboard();
                }
            };
            var buttonInverse = (Button)FindViewById<Button>(Resource.Id.buttonInverse);
            buttonInverse.Click += (o, e) =>
            {
                if (editText.Text != null)
                {
                    webView.Settings.UserAgentString = newUA_mobile;
                    webView.LoadUrl("https://www.google.co.jp/search?q=" + editText.Text + "+日文");
                    hideKeyboard();
                }
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

