using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Services.Description;
using Microsoft.Ajax.Utilities;
using RestSharp;


namespace WhatIsIt
{
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
            public string SiteUrlName = "siteUrl";
            public string SiteUrlValue = "http://apifree.forvo.com/";
            public string ApiKeyName = "key";
            public string ApiKeyValue = "114b9ec1f5cb114497cb5b00514bbdc6";
            public string FormatName = "format";
            public string FormatValue = "json";
            public string ActionName = "action";
            public string ActionValue = "word-pronunciations";
            public string LanguageName = "language";
            public string LanguageValue { get; set; }
            public string SexName = "sex";
            public string SexValue { get; set; }
            public string OrderName = "order";
            public string OrderValue = "rate-desc";
            public string LimitName = "limit";
            public string LimitValue = "1";
            public string TypeName = "type";
            public string TypeValue { get; set; }
            public string WordName = "word";
            public string WordValue { get; set; }

        }
        

        //Constants
        public const string AudioFileType = ".mp3";
        public const string StorageFolderForSounds = @"UploadedSounds\";

        public static string WordToSound = String.Empty;
        public static string UrlToGetSoundFile = String.Empty;

        public static string GetSoundFileToPlay(string word)
        {
            if (word == null) throw new ArgumentNullException("word");
            var fullPathSoundStorageLocationForApp = System.AppDomain.CurrentDomain.BaseDirectory + StorageFolderForSounds;

            // Check if sound file is already there, if so...just return it
            string existingFileLocation;
            if (CheckIfSoundfileAlreadyExists(word, fullPathSoundStorageLocationForApp, out existingFileLocation)) return existingFileLocation;

            // Sound file for this word is not there. We need to get it
            WordToSound = word;

            // Construct the request parameter settings
            var soundRequest = new SoundRequest
            {
                ApiKeyName = "key",
                ActionName = "action",
                ActionValue = "word-pronunciations",
                LanguageName = "language",
                LanguageValue = "en",
                SexName = "sex",
                SexValue = "m",
                OrderName = "order",
                OrderValue = "rate-desc",
                LimitName = "limit",
                LimitValue = "1",
                TypeName = "type",
                TypeValue = "word",
                WordValue = word
            };

            var soundFileUri = GetSoundFromForvo(soundRequest);
            
            if (soundFileUri == String.Empty) return String.Empty;
            
            var soundFileName = DownloadSoundFile(soundFileUri);
            
            if (soundFileName == String.Empty) return String.Empty;
            
            return StorageFolderForSounds + soundFileName;
        }

        private static bool CheckIfSoundfileAlreadyExists(string word, string fullPathSoundStorageLocationForApp, out string existingFileLocation)
        {
            existingFileLocation = null;
            if (!File.Exists(fullPathSoundStorageLocationForApp + word + AudioFileType)) return false;
            existingFileLocation = StorageFolderForSounds + word + AudioFileType;
            return true;
        }

        public static string GetSoundFromForvo(SoundRequest soundRequest)
        {
            var request = new RestRequest("/" + soundRequest.ApiKeyName   + "/{" + soundRequest.ApiKeyName   + "}" +
                                          "/" + soundRequest.ActionName   + "/{" + soundRequest.ActionName   + "}" +
                                          "/" + soundRequest.LanguageName + "/{" + soundRequest.LanguageName + "}" +
                                          "/" + soundRequest.FormatName   + "/{" + soundRequest.FormatName   + "}" +
                                          "/" + soundRequest.SexName      + "/{" + soundRequest.SexName      + "}" +
                                          "/" + soundRequest.OrderName    + "/{" + soundRequest.OrderName    + "}" +
                                          "/" + soundRequest.LimitName    + "/{" + soundRequest.LimitName    + "}" +
                                          "/" + soundRequest.TypeName     + "/{" + soundRequest.TypeName     + "}" +
                                          "/" + soundRequest.WordName     + "/{" + soundRequest.WordName     + "}");
                                         // "/language/{language}/format/{format}/sex/{sex}/order/{order}/limit/{limit}/type/{type}/word/{word}");
            var soundresult = Execute<SoundResult>(request, soundRequest);
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

        public static T Execute<T>(RestRequest request, SoundRequest soundRequest) where T : new()
        {
            IRestClient client = new RestClient(soundRequest.SiteUrlValue);
            request.AddParameter(soundRequest.ApiKeyName, soundRequest.ApiKeyValue, ParameterType.UrlSegment);
            request.AddParameter(soundRequest.ActionName, soundRequest.ActionValue, ParameterType.UrlSegment);
            request.AddParameter(soundRequest.LanguageName, soundRequest.LanguageValue, ParameterType.UrlSegment);
            request.AddParameter(soundRequest.FormatName, soundRequest.FormatValue, ParameterType.UrlSegment);
            request.AddParameter(soundRequest.SexName, soundRequest.SexValue, ParameterType.UrlSegment);
            request.AddParameter(soundRequest.OrderName, soundRequest.OrderValue, ParameterType.UrlSegment);
            request.AddParameter(soundRequest.LimitName, soundRequest.LimitValue, ParameterType.UrlSegment);
            request.AddParameter(soundRequest.TypeName, soundRequest.TypeValue, ParameterType.UrlSegment);
            request.AddParameter(soundRequest.WordName, soundRequest.WordValue, ParameterType.UrlSegment);

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
    }
}