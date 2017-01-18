#region License
// <copyright file="Options.cs" company="Michael R. Schwab">
//   Copyright 2017
// </copyright>
//
// License: https://www.gnu.org/licenses/gpl.html
#endregion

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

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
