using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Speech;
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
        int pressedButtonID;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            #region find UI
            var webView = FindViewById<WebView>(Resource.Id.webView);
            var editText = FindViewById<EditText>(Resource.Id.editText);
            var buttonEdo = FindViewById<Button>(Resource.Id.buttonEdo);
            var buttonIMG = FindViewById<Button>(Resource.Id.buttonIMG);
            var buttonGoogle = FindViewById<Button>(Resource.Id.buttonGoogle);
            var buttonInverse = FindViewById<Button>(Resource.Id.buttonInverse);
            #endregion

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
            editText.LongClick += (o, e) =>
            {
                InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
                inputManager.ShowInputMethodPicker();
                inputManager.ShowSoftInput(editText, ShowFlags.Implicit);
            };

            #region Set Click Event
            buttonEdo.Click += (o, e) =>
            {
                if (editText.Text != null)
                {
                    webView.Settings.UserAgentString = newUA_desktop;
                    webView.LoadUrl("https://dict.hjenglish.com/jp/jc/" + editText.Text);
                    hideKeyboard();
                }
            };
            buttonIMG.Click += (o, e) =>
            {
                if (editText.Text != null)
                {
                    webView.Settings.UserAgentString = newUA_mobile;
                    webView.LoadUrl("https://www.google.co.jp/search?q=" + editText.Text + "&tbm=isch");
                    hideKeyboard();
                }
            };
            buttonGoogle.Click += (o, e) =>
            {
                if (editText.Text != null)
                {
                    webView.Settings.UserAgentString = newUA_mobile;
                    webView.LoadUrl("https://www.google.co.jp/search?q=" + editText.Text);
                    hideKeyboard();
                }
            };
            buttonInverse.Click += (o, e) =>
            {
                if (editText.Text != null)
                {
                    webView.Settings.UserAgentString = newUA_mobile;
                    webView.LoadUrl("https://www.google.co.jp/search?q=" + editText.Text + "+日文");
                    hideKeyboard();
                }
            };
            #endregion

            #region LongClick
            Action callRecognizerIntent_JPN = 
                new Action(
                () =>
                {
                    Intent intent = new Intent(
                        RecognizerIntent.ActionRecognizeSpeech);
                    intent.PutExtra(RecognizerIntent.ExtraLanguage, "ja-JP");

                    try
                    {
                        StartActivityForResult(intent, 1);
                        editText.Text = "";
                    }
                    catch (ActivityNotFoundException a)
                    {
                        Toast t = Toast.MakeText(ApplicationContext,
                                "Opps! Your device doesn't support Speech to Text",
                                ToastLength.Short);
                        t.Show();
                    }
                }
                );
            Action callRecognizerIntent_tCHN = 
                new Action(
                () =>
                {
                    Intent intent = new Intent(
                        RecognizerIntent.ActionRecognizeSpeech);

                    intent.PutExtra(RecognizerIntent.ExtraLanguage, "zh-TW");

                    try
                    {
                        StartActivityForResult(intent, 1);
                        editText.Text = "";
                    }
                    catch (ActivityNotFoundException a)
                    {
                        Toast t = Toast.MakeText(ApplicationContext,
                                "Opps! Your device doesn't support Speech to Text",
                                ToastLength.Short);
                        t.Show();
                    }
                }
                );
            buttonEdo.LongClick += (o, e) => { pressedButtonID = Resource.Id.buttonEdo; callRecognizerIntent_JPN(); };
            buttonIMG.LongClick += (o, e) => { pressedButtonID = Resource.Id.buttonIMG; callRecognizerIntent_JPN(); }; ;
            buttonGoogle.LongClick += (o, e) => { pressedButtonID = Resource.Id.buttonGoogle; callRecognizerIntent_JPN(); }; ;
            buttonInverse.LongClick += (o, e) => { pressedButtonID = Resource.Id.buttonInverse; callRecognizerIntent_tCHN(); }; ;
            webView.LongClick += (o, e) =>
            {
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(webView.Url));
                StartActivity(intent);
            };
            #endregion

            editText.RequestFocus();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (requestCode)
            {
                case 1:
                    {
                        if (null != data)
                        {
                            System.Collections.Generic.IList<String> text = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                            Toast t = Toast.MakeText(ApplicationContext,
                                "Searching...",
                                ToastLength.Short);
                            t.Show();
                            var editText = (EditText)FindViewById<EditText>(Resource.Id.editText);
                            editText.Text = text[0];
                            var buttonPressed = FindViewById<Button>(pressedButtonID);
                            buttonPressed.PerformClick();
                        }
                        else
                        {
                            Toast t = Toast.MakeText(ApplicationContext,
                                "Data is null",
                                ToastLength.Short);
                            t.Show();
                        }
                        break;
                    }

            }
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                var webView = FindViewById<WebView>(Resource.Id.webView);
                if (webView.CanGoBack())
                {
                    webView.GoBack();
                    return true;
                }
                else
                {
                    Intent intent = new Intent();
                    intent.SetAction(Intent.ActionMain);
                    intent.AddCategory(Intent.CategoryHome);
                    StartActivity(intent);
                    return false;
                }
            }
            return false;
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

