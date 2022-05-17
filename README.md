# FlickrRemovePhotosFromSet

This is a command line utility to remove photos that are in one photoset from another.

Example batch file to remove photos from the *Auto Upload* photoset that appear in the given photoset ID.

```Batchfile
echo off
set arg1=%1
shift

FlickrRemovePhotosFromSet.exe -v -k [api-key] -a [api-secret] -t [Auto Upload Photoset ID] -s %arg1%
```

Licensed under [GNU Lesser General Public License v3.0](https://github.com/Fyzel/FlickrRemovePhotosFromSet/blob/master/LICENSE).
