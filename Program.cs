#region License
// <copyright file="Program.cs" company="Michael R. Schwab">
//   Copyright 2017
// </copyright>
//
// License: https://www.gnu.org/licenses/gpl.html
#endregion

using System;
using System.Collections.Generic;
using FlickrNet;

namespace FlickrRemovePhotosFromSet
{
    class Program
    {
        static int Main(string[] args)
        {
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            #region CommandLineApplication
            //var app = new CommandLineApplication
            //{
            //    Name = assemblyName,
            //    Description = "Removes photos from a target Flickr set if they are in the source Flickr set",
            //    Syntax = "syntax",
            //    ExtendedHelpText = $"\r\nObtain a Flickr API Key and Secret from https://www.flickr.com/services/developer/api/. \r\n\r\nExample: {assemblyName} --api-key 9a0554259914a86fb9e7eb014e4e5d52 --api-secret 000005fab4534d05 --source-set-id 72157675935916250 --target-set-id 72157676616452862",
            //    ShowInHelpText = true
            //};

            //// define the command line arguments
            //var apiKeyArg = new CommandArgument { Name = "--api-key", Description = "Flickr API Key", MultipleValues = false, ShowInHelpText = true };
            //var apiSecretArg = new CommandArgument { Name = "--api-secret", Description = "Flickr API Secret", MultipleValues = false, ShowInHelpText = true };
            //var sourceSetIdArg = new CommandArgument { Name = "--source-set-id", Description = "Source photoset ID", MultipleValues = false, ShowInHelpText = true };
            //var targetSetIdArg = new CommandArgument { Name = "--target-set-id", Description = "Target photoset ID", MultipleValues = false, ShowInHelpText = true };

            //// define the command line options
            //var verboseOpt = new CommandOption("--verbose", CommandOptionType.SingleValue);
            //verboseOpt.Description = "Enable verbose reporting";

            //// bind the command line arguments
            //app.Arguments.Add(apiKeyArg);
            //app.Arguments.Add(apiSecretArg);
            //app.Arguments.Add(sourceSetIdArg);
            //app.Arguments.Add(targetSetIdArg);

            //// bind the command line options
            //app.Options.Add(verboseOpt);

            //app.HelpOption("-h");

            //app.OnExecute(() =>
            //{
            //    // check the command line arguments

            //    if (String.IsNullOrEmpty(apiKeyArg.Value))
            //    {
            //        DisplayErrorMessage(app, "--api-key must be specified.");
            //        return 1;
            //    }

            //    if (String.IsNullOrEmpty(apiSecretArg.Value))
            //    {
            //        DisplayErrorMessage(app, "--api-secret must be specified.");
            //        return 2;
            //    }

            //    if (String.IsNullOrEmpty(sourceSetIdArg.Value))
            //    {
            //        DisplayErrorMessage(app, "--source-set-id must be specified.");
            //        return 3;
            //    }

            //    if (String.IsNullOrEmpty(targetSetIdArg.Value))
            //    {
            //        DisplayErrorMessage(app, "--target-set-id must be specified.");
            //        return 4;
            //    }

            //    // command line arguments are present
            //    var flickrConfig = new FlickrConfiguration
            //    {
            //        ApiKey = apiKeyArg.Value,
            //        ApiSecret = apiSecretArg.Value,
            //        SourcePhotoSetIdToKeep = sourceSetIdArg.Value,
            //        TargetPhotoSetIdForRemoval = targetSetIdArg.Value,
            //        Verbose = verboseOpt.HasValue()
            //    };

            //    RemovePhotosFromSet(flickrConfig);

            //    return 0;
            //});

            //return app.Execute(args);
            #endregion

            var options = new Options();

            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                var flickrConfig = new FlickrConfiguration
                {
                    ApiKey = options.ApiKey,
                    ApiSecret = options.ApiSecret,
                    SourcePhotoSetIdToKeep = options.SourceSetId,
                    TargetPhotoSetIdForRemoval = options.TargetSetId,
                    Verbose = options.Verbose
                };

                RemovePhotosFromSet(flickrConfig);
            }
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
            System.Diagnostics.Process.Start(authorizationURL);
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
            int totalPageCount = photoSetInfo.TotalNumberOfPhotosAndVideos / perPage + 1;

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
