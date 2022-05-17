#region License
// <copyright file="Program.cs" company="Michael R. Schwab">
//   Copyright 2022
// </copyright>
//
// License: https://github.com/Fyzel/FlickrRemovePhotosFromSet/blob/master/LICENSE
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using CommandLine;
using FlickrNet;

namespace FlickrRemovePhotosFromSet
{
    class Program
    {
        static int Main(string[] args)
        {
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            var flickrConfig = new FlickrConfiguration();

            var results = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    flickrConfig.ApiKey = o.ApiKey;
                    flickrConfig.ApiSecret = o.ApiSecret;
                    flickrConfig.SourcePhotoSetIdToKeep = o.SourceSetId;
                    flickrConfig.TargetPhotoSetIdForRemoval = o.TargetSetId;
                    flickrConfig.Verbose = o.Verbose;
                }
            );

            RemovePhotosFromSet(flickrConfig);

            return 0;
        }

        ///// <summary>
        ///// Display a formatted error message to the error stream
        ///// </summary>
        ///// <param name="app">Command line application object</param>
        ///// <param name="message">Error message to display</param>
        //private static void DisplayErrorMessage(CommandLineApplication app, string message)
        //{
        //    app.ShowHelp();
        //    app.Error.WriteLine($"\r\nERROR: {message}");
        //}

        /// <summary>
        /// Removes the photos from the specified target photoset that exist in the source photoset
        /// </summary>
        /// <param name="flickrConfig">Flickr configuration for remote calls</param>
        private static void RemovePhotosFromSet(FlickrConfiguration flickrConfig)
        {
            Flickr flickr = new Flickr();
            flickr.ApiKey = flickrConfig.ApiKey;
            flickr.ApiSecret = flickrConfig.ApiSecret;

            // set the callback to oob for user authorization
            OAuthRequestToken oauthRequesToken = flickr.OAuthGetRequestToken("oob");
            Console.Clear();
            string authorizationURL = flickr.OAuthCalculateAuthorizationUrl(oauthRequesToken.Token, AuthLevel.Write);
            Console.WriteLine($"Go to {authorizationURL} and copy the code");
            try
            {
                Process.Start(new ProcessStartInfo(authorizationURL) { UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                {
                    MessageBox.Show(noBrowser.Message);
                }
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }

            string verifier = Console.ReadLine();
            Console.Clear();

            OAuthAccessToken oauthAccessToken = flickr.OAuthGetAccessToken(oauthRequesToken, verifier);

            // get the source photos
            List<string> sourceSetPhotoIDList = GetAllPhotoIDsFromSet(flickr, flickrConfig.SourcePhotoSetIdToKeep, flickrConfig.Verbose);
            List<string> photosToBeRemoved = new List<string>();
            Photoset targetPhotoSetInfo = GetPhotoSet(flickr, flickrConfig.TargetPhotoSetIdForRemoval);

            List<List<string>> photoGroupsToBeRemoved = new List<List<string>>();

            photoGroupsToBeRemoved.Add(sourceSetPhotoIDList);

            foreach (List<string> photoList in photoGroupsToBeRemoved)
            {
                int count = 0;

                foreach (string photoID in photoList)
                {
                    try
                    {
                        count++;

                        if (flickrConfig.Verbose == true)
                        {
                            Console.WriteLine($"Removing Photo: {photoID} from '{targetPhotoSetInfo.Title}' set ({count} of {photoList.Count})");
                        }

                        flickr.PhotosetsRemovePhoto(flickrConfig.TargetPhotoSetIdForRemoval, photoID);
                    }
                    catch (Exception e)
                    {
                        if (flickrConfig.Verbose == true)
                        {
                            Console.WriteLine($"Error deleting: {e.Message}");
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Retrieves a list of photo identifiers for a specific photoset
        /// </summary>
        /// <param name="flickrContext">The flickr context</param>
        /// <param name="photoSetId">The flickr photoset</param>
        /// <param name="verbose">Verbosity is enabled</param>
        /// <returns>list of image identifiers in photoset</returns>
        private static List<string> GetAllPhotoIDsFromSet(Flickr flickrContext, string photoSetId, bool verbose)
        {
            List<string> output = new List<string>();

            Photoset photoSetInfo = GetPhotoSet(flickrContext, photoSetId);
            int perPage = 500;
            int totalPageCount = photoSetInfo.Total / perPage + 1;

            for (int currentPage = 1; currentPage < totalPageCount + 1; currentPage++)
            {
                if (verbose == true)
                {
                    Console.WriteLine($"  Processing '{photoSetInfo.Title}' page {currentPage} of {totalPageCount}");
                }

                PhotosetPhotoCollection setPhotoPage = flickrContext.PhotosetsGetPhotos(photoSetId, currentPage, perPage);

                if (verbose == true)
                {
                    Console.WriteLine($"    Got {setPhotoPage.Count}");
                }

                foreach (var photo in setPhotoPage)
                {
                    output.Add(photo.PhotoId);
                }
            }

            if (verbose == true)
            {
                Console.WriteLine($"  Finished Processing '{photoSetInfo.Title}'");
            }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flickrContext">The flickr context</param>
        /// <param name="photoSetId">The flickr photoset</param>
        /// <returns>The photoset info</returns>
        private static Photoset GetPhotoSet(Flickr flickrContext, string photoSetId)
        {
            return flickrContext.PhotosetsGetInfo(photoSetId);
        }
    }
}
