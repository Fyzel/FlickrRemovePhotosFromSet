# FlickrRemovePhotosFromSet

Example batch file to remove photos from the *Auto Upload* photoset that appear in the given photoset ID.

```Batchfile
echo off
set arg1=%1
shift

FlickrRemovePhotosFromSet.exe -v -k [api-key] -a [api-secret] -t [Auto Upload Photoset ID] -s %arg1%
```