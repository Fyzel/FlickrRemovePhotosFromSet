#region License
// <copyright file="Options.cs" company="Michael R. Schwab">
//   Copyright 2022
// </copyright>
//
// License: https://github.com/Fyzel/FlickrRemovePhotosFromSet/blob/master/LICENSE
#endregion

using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace FlickrRemovePhotosFromSet
{
    class Options
    {
        [Option('k', "api-key", Required = true, HelpText = "Flickr API Key")]
        public string ApiKey { get; set; }

        [Option('a', "api-secret", Required = true, HelpText = "Flickr API Secret")]
        public string ApiSecret { get; set; }

        [Option('s', "source-set-id", Required = true, HelpText = "Source photoset ID")]
        public string SourceSetId { get; set; }

        [Option('t', "target-set-id", Required = true, HelpText = "Target photoset ID")]
        public string TargetSetId { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "verbose")]
        public bool Verbose { get; set; }

        [Usage(ApplicationAlias = "FlickrRemovePhotosFromSet")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Verbose Usage",
                                         new Options
                                         {
                                             ApiKey = "[api-key]",
                                             ApiSecret = "[api-secret]",
                                             SourceSetId = "72157698344344474",
                                             TargetSetId = "72157650408948124",
                                             Verbose = true
                                         });
                yield return new Example("Non-Verbose Usage",
                                        new Options
                                        {
                                            ApiKey = "[api-key]",
                                            ApiSecret = "[api-secret]",
                                            SourceSetId = "72157698344344474",
                                            TargetSetId = "72157650408948124",
                                            Verbose = false
                                        });
            }
        }
    }
}
