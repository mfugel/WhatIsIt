using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WhatIsIt
{
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
                //upload file to server
                string determineFilePath = DetermineFilePath(FileUpload1.FileName, txtImageName.Text);
                FileUpload1.PostedFile.SaveAs(determineFilePath);

                Response.Redirect("./WhatIsItMainPage.aspx");
            }
        }

        private string DetermineFilePath(string sourceFilename, string desiredFileName = null) 
        {
            var resultingFilename = "";
            var ext = Path.GetExtension(sourceFilename);

            if (!string.IsNullOrEmpty(desiredFileName))
            {
                resultingFilename = desiredFileName + ext;
            }
            else
            {
                resultingFilename = sourceFilename;
            }
            return Server.MapPath("~/UploadedImages/" + resultingFilename);
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
}