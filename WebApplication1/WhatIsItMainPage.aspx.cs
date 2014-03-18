using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using RestSharp;


namespace WhatIsIt
{

    #region // |===== WordSoundService ==========================================================================================|

    public class WordSoundService
    {
        public class Attributes
        {
            public int Total { get; set; }
        }

        public class Item
        {
            public int Id { get; set; }
            public string Word { get; set; }
            public string Addtime { get; set; }
            public int Hits { get; set; }
            public string Username { get; set; }
            public string Sex { get; set; }
            public string Country { get; set; }
            public string Code { get; set; }
            public string Langname { get; set; }
            public string Pathmp3 { get; set; }
            public string Pathogg { get; set; }
            public object Rate { get; set; }
            public object NumVotes { get; set; }
            public int NumPositiveVotes { get; set; }
        }

        public class SoundResult
        {
            public Attributes Attributes { get; set; }
            public List<Item> Items { get; set; }
        }

        public class SoundRequest
        {
            public string SiteUrl { get; set;}
            public string ApiKey { get; set; }
            public string Format { get; set; }
            public string Action { get; set; }
            
            public string Language { get; set; }
            public string Sex { get; set; }
            public string Order { get; set; }
            public string Limit { get; set; }
            public string Type { get; set; }
        }
        

        //Constants
        public const string ApiKey = "114b9ec1f5cb114497cb5b00514bbdc6";

        private const string BaseUrl =
           "http://apifree.forvo.com" +
           "/key/" + 
           ApiKey +
           "/format/json/action/word-pronunciations/language/en/sex/f/order/rate-desc/limit/1/word/";
//         "/format/json/action/word-pronunciations/sex/f/order/rate-desc/limit/1/word/";
        public const string AudioFileType = ".mp3";
        public const string StorageFolderForSounds = @"UploadedSounds\";

        public static string WordToSound = String.Empty;
        public static string UrlToGetSoundFile = String.Empty;

        public static string GetSoundFileToPlay(string word)
        {
            if (word == null) throw new ArgumentNullException("word");
            var fullPathSoundStorageLocationForApp = System.AppDomain.CurrentDomain.BaseDirectory +
                                                     StorageFolderForSounds;
            
            // Check if sound file is already there, if so...just return it
            if (File.Exists(fullPathSoundStorageLocationForApp + word + AudioFileType))
                return StorageFolderForSounds + word + AudioFileType;

            // Sound file for this word is not there. We need to get it
            WordToSound = word;

            // Construct the request parameter settings
            var soundRequest = new SoundRequest { 
                                                  SiteUrl = "http://apifree.forvo.com",
                                                  ApiKey = "114b9ec1f5cb114497cb5b00514bbdc6",
                                                  Format = "json",
                                                  Action = "word-pronunciations",
                                                  Language = "en", 
                                                  Sex = "f", 
                                                  Limit = "1", 
                                                  Order = "rate-desc", 
                                                  Type = "word"
                                                };
            
            UrlToGetSoundFile = BaseUrl + WordToSound;
            var soundFileUri = GetSoundFromForvo();
            
            if (soundFileUri == String.Empty) return String.Empty;
            
            var soundFileName = DownloadSoundFile(soundFileUri);
            
            if (soundFileName == String.Empty) return String.Empty;
            
            return StorageFolderForSounds + soundFileName;
        }

        public static string GetSoundFromForvo()
        {
            var request = new RestRequest();
            var soundresult = Execute<SoundResult>(request);
            if (soundresult != null)
            {
                if (soundresult.Items.Count > 0)
                {
                    var pathmp3 = soundresult.Items[0].Pathmp3;
                    if (!pathmp3.IsNullOrWhiteSpace())
                    {
                        return pathmp3;
                    }
                }
            }
            return String.Empty;
        }

        public static T Execute<T>(RestRequest request) where T : new()
        {
            //**** TBD: turn this into client.add parameters instead of hardcoded BaseUrl****
            //IRestClient client = new RestClient("http://apifree.com/");
            //client.AddDefaultHeader("key", ApiKey);
            //client.AddDefaultUrlSegment("key", ApiKey);
            //client.AddDefaultUrlSegment("language", "en");
            //client.AddDefaultUrlSegment("format", "json");
            //client.AddDefaultUrlSegment("action", "word-pronunciations");
            //client.AddDefaultUrlSegment("limit", "1");
            //client.AddDefaultUrlSegment("type", "word");

            var client = new RestClient {BaseUrl = UrlToGetSoundFile};
            var response = client.Execute<T>(request);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response from Forvo api. Check inner details for more info.";
                var forvoException = new ApplicationException(message, response.ErrorException);
                throw forvoException;
            }
            return response.Data;
        }

        public static string DownloadSoundFile(string soundFileUri)
        {
            var u = new Uri(soundFileUri);
            var fileName = WordToSound + AudioFileType;
            var pathWithSoundFileName = System.AppDomain.CurrentDomain.BaseDirectory + "UploadedSounds\\" + fileName;
            using (var client = new WebClient())
            {
                client.DownloadFile(soundFileUri, pathWithSoundFileName);
            }
            return fileName;
        }

        #endregion
    }

    #region // |===== WhatIsItMainPage ==========================================================================================|

    public partial class WhatIsItMainPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateImages();
            InitializePage();
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string filename = string.Empty;
                string ext = string.Empty;
                filename = FileUpload1.FileName;
                ext = Path.GetExtension(filename);

                if (!string.IsNullOrEmpty(txtImageName.Text))
                {
                    filename = txtImageName.Text + ext;
                }

                //upload file to server
                FileUpload1.PostedFile.SaveAs(Server.MapPath("~/UploadedImages/" + filename));
            }
            Response.Redirect("./WhatIsItMainPage.aspx");
        }

        private void PopulateImages()
        {
            foreach (string strFileName in Directory.GetFiles(Server.MapPath("~/UploadedImages/")))
            {
                var imageButton = new ImageButton();
                var fileInfo = new FileInfo(strFileName);
                imageButton.ImageUrl = "~/UploadedImages/" + fileInfo.Name;
                imageButton.Width = Unit.Pixel(200);
                imageButton.Height = Unit.Pixel(200);
                imageButton.Style.Add("padding", "5px");
                imageButton.Click += imageButton_Click;
                Panel1.Controls.Add(imageButton);
            }
        }

        private void imageButton_Click(object sender, ImageClickEventArgs e)
        {
            var wordToPronounce = (Path.GetFileNameWithoutExtension(((ImageButton) sender).ImageUrl.ToUpper()));

            //show the image of the file just selected
            FocusedImage.ImageUrl = ((ImageButton) sender).ImageUrl;
            FocusedImageDesc.Text = string.Format("{0}<BR>", wordToPronounce);

            //get the sound file to play
            var soundFileToPlay = WordSoundService.GetSoundFileToPlay(wordToPronounce);

            if (soundFileToPlay == String.Empty)
            {
                //indicate that the sound could not be found for this word
                FocusedSound.Text =
                    string.Format(
                        "Sound file for the word {0} could not be found!",
                        wordToPronounce);
            }
            else
            {
                //construct audio html tag with file to autoplay
                FocusedSound.Text =
                    string.Format(
                        "<audio controls='controls' autoPlay='autoPlay'><source src='{0}' type='audio/mp3'/></audio>",
                        soundFileToPlay);    
            }
        }

        protected void btnShowHideNewImage_Click(object sender, EventArgs e)
        {
            if (UploadPanel.Visible)
            {
                // UploadPanel is currently visible
                InitializePage();
            }
            else // UploadPanel is currently not visible
            {
                btnShowHideNewImage.Visible = false;
                UploadPanel.Visible = true;
            }
        }

        public void InitializePage()
        {
            btnShowHideNewImage.Visible = true;
            btnShowHideNewImage.Text = "Upload a New Image ";
            UploadPanel.Visible = false;
        }

        protected void CancelUpload_Click(object sender, EventArgs e)
        {
        }
    }

    #endregion ///////////////////////////////////////////////////////////////////////////////////
}