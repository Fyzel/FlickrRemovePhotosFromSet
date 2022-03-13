#region License
// <copyright file="Options.cs" company="Michael R. Schwab">
//   Copyright 2022
// </copyright>
//
// License: https://www.gnu.org/licenses/gpl.html
#endregion

namespace FlickrRemovePhotosFromSet
{
    class FlickrConfiguration
    {
        /// <summary>
        /// The Flickr API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The Flickr API secret value
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// The photoset to keep the photos in
        /// </summary>
        public string SourcePhotoSetIdToKeep { get; set; }

        /// <summary>
        /// The photoset to remove the photos from if they are in SourcePhotoSetIdToKeep
        /// </summary>
        public string TargetPhotoSetIdForRemoval { get; set; }

        /// <summary>
        /// Verbose output
        /// </summary>
        public bool Verbose { get; set; }

        public override string ToString()
        {
            return $"ApiKey: {ApiKey}, ApiSecret: {ApiSecret}, SourcePhotoSetIdToKeep: {SourcePhotoSetIdToKeep}, TargetPhotoSetIdForRemoval: {TargetPhotoSetIdForRemoval}, Verbose: {Verbose}";
        }
    }
}
