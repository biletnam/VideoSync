# VideoSync
#### Watch videos in sync with friends over WebSockets! 
VideoSync is a web app which uses Websockets to synchronize one video player to any number of clients' video players.
The project provides both a server and client solution.
The client is written in standard HTML/CSS/JS with a few JS libraries: VideoJS and Bootstrap.
The server is written in C# and relies on the .NET Framework version 4.5.  On unix based systems, the server can still be compiled and run using Mono.

Within one room, there is one owner and any number of clients.  The owner has full control of the video playback and the video source, while the clients can only pause their own video players (useful if the video is having trouble buffering), but the pause is not broadcast globally like the owner's.
The server is written to essentially be just a messenger, passing around WebSockets messages to the correct clients and keeping track of connections and rooms.  It should use very little bandwidth because no video is ever streamed through the server.
All video is externally hosted.  Currently, the client is able to handle URLs directly linking to a video file, youtube or youtu.be video URLs and magnet links and URLs pointing to torrent files.  In addition, thanks to WebTorrent and drag-drop, local .mp4 files can be dragged onto the page and will be turned into a torrent which is then seeded by the owner and the magnet url is passed to all clients for streaming.

## Installation
- Clone the repo
- Make sure mono-complete is installed. Check their website for info on how to install: http://www.mono-project.com/docs/getting-started/install/
- Unix-based: run `./run_local.sh` this will attempt to start the server and if it can't find compiled binaries, it will run `mcs_build.sh` to attempt to create them.
- Windows: run `run_local.bat` which will do the same as above with `mcs_build.bat`.
- NOTE: To upgrade the binary after modifying server code, run `mcs_build.sh` or `mcs_build.bat` to compile updated binaries.
- To compile the minified HTML/JS/CSS files, you need to have grunt. To install, first make sure you have npm installed and then just run `npm install`. To minify all files, simply run `grunt`. While you are working, you can run `grunt watch` to automatically update the minified files as you make changes to the source files.

## Usage
- Unix-based: run `sudo ./run_local.sh` and it will prompt you for an admin password. Running without sudo will not work as the server runs on port 80 which needs root access.
- Windows: run `mcs_build.bat` from command prompt or from explorer.

## Contributing
1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request

## License
TODO: Write license
