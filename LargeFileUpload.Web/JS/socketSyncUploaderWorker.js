self.addEventListener('message', function (e) {

    var rootLocation = e.data.rootLocation;

    var ftInfo = {};
    this.BYTES_PER_CHUNK = 2 * 1024 * 1024;
    // Size of the file

    // The total number of file chunks
    var blob = e.data.blob;
    var Total_Number_of_Chunks = Math.ceil(blob.size / BYTES_PER_CHUNK);
    // Array used to hold the total number of chunks, the number of chunks that have been uploaded,
    // and the current chunk. This information is sent to the web worker that uploads the file chunks
    var chunkCount = {
        currentNumber: -1,
        numberOfChunks: Total_Number_of_Chunks,
        numberOfUploadedChunks: 0
    };

    var SIZE = blob.size;
    ftInfo.FileName = blob.name;
    ftInfo.TotalFileSize = SIZE;

    var part = 0;
    var start = 0;
    var end = BYTES_PER_CHUNK;
    var socket;
    try {
        socket = new WebSocket("ws://" + rootLocation + "/LargeFileUploadWithChunking/home/SendData");
    }
    catch (ex) {
        postMessage(ex);
    }
    if (socket.readyState > 1) {
        postMessage({ type: "error", value: "Server dont support WebSockets." });
        //postMessage('close');
    }
    var blobSize = 0;
    var chunkSize = 2 * 1024 * 1024;
    socket.onopen = function (event) {
        
        var uploadEntry = {
            FileName: blob.name
        }
        socket.send(JSON.stringify(uploadEntry));
        socket.onmessage = function (event) {
            var data = JSON.parse(event.data);
            if (data.accepted) {// The soket ready send data
                var fileReader = new FileReaderSync();
                var reader = new FileReaderSync();
                console.time("read");
                while (start < SIZE) {
                    console.time(`fileRead ${chunkCount.currentNumber}`);
                    var chunk = blob.slice(start, end);
                    // Read the chunk into another variable to calculate the checksum
                    var buffer = fileReader.readAsArrayBuffer(chunk);
                    console.time(`fileRead ${chunkCount.currentNumber}`);
                    start = end;
                    end = start + BYTES_PER_CHUNK;
                    chunkCount.currentNumber++;
                    chunkCount.numberOfUploadedChunks++;
                    transferProgress = Math.ceil((start * 100) / SIZE);
                    part = chunkCount.currentNumber;
                    var uInt8Array = new Uint8Array(buffer.byteLength);
                    var blobToSend = new Blob([new Uint8Array(buffer)], { type: "text/application" });
                    //var blobToSend = new Blob([uInt8Array], { type: "text/application" });
                    if (socket.readyState === 1) {
                        console.log(`Sending Chunk ${chunkCount.currentNumber}`);
                        console.time(`socketSend ${chunkCount.currentNumber}`);
                        socket.send(blobToSend);
                        postMessage({ type: "progress", value: transferProgress});
                        console.timeEnd(`socketSend ${chunkCount.currentNumber}`);
                    } else {
                        postMessage({ type: "error", value: "Socket closed unexpectedly. Exiting the upload" });
                        break;
                    }
                }
            } else if (data.received) {
                //document.getElementById('upload-progress').value = Math.round(data.received * 100 / blobSize);
                if (data.received >= blobSize) {
                    socket.close();
                    postMessage({ type: "success",value:"Done" });
                }
            }
        }
    }
}, false);
