
function readAsyncAndSendviaSocket(){
    var socket;
    try {
        socket = new WebSocket("ws://" + window.location.host + "/LargeFileUploadWithChunking/home/SendData");
    }
    catch (ex) {
        alert(ex);
    }
    if (socket.readyState > 1) {
        alert("Server dont support WebSockets.");
        document.getElementById('upload-dialog').close();
    }
    var blobSize = 0;
    var chunkSize = 2 * 1024 * 1024;
    socket.onopen = function (event) {
        console.time("upload - readAsyncAndSendviaSocket");
        var uploadEntry = {
            EmailAddress: $('#emailAddress').val(),
            DatePublished: $('#datePublished').val(),
            Location: $('#location').val(),
            FileName: document.getElementById('uploadFile').files[0].name
        }
        socket.send(JSON.stringify(uploadEntry));
        socket.onmessage = function (event) {
            var data = JSON.parse(event.data);
            if (data.accepted) {
                getBlob(function (blob) {
                    blobSize = blob.size || blob.arrayLength;
                    for (var i = 0, count = 1; i < blobSize; i += chunkSize, count += 1) {
                        var segment = blob.slice(i, i + chunkSize);
                        console.log(`Sending chunk ${count} of size ${segment.size}`);
                        if (socket.readyState === 1) {
                            socket.send(segment);

                        } else {
                            alert("Socket closed unexpectedly. Exiting the upload");
                            break;
                        }
                    }
                });
            } else if (data.received) {
                document.getElementById('upload-progress').value = Math.round(data.received * 100 / blobSize);
                if (data.received >= blobSize) {
                    socket.close();
                }
            }
        }
        socket.onerror = (err) => {
            alert(err);
        };
        socket.onclose = function () {

            document.getElementById('upload-dialog').close();
            //window.location.href = "/LargeFileUploadWithChunking";
            console.timeEnd("upload - readAsyncAndSendviaSocket");
            alert("upload completed");
        }
    }
}
function getBlob(callBack) {
    var selectedFiles = document.getElementById('uploadFile').files;
    if (selectedFiles.length == 1 /*&& selectedFiles[0].type.match("*")*/) {
        var reader = new FileReader();
        reader.onloadend = function (e) {
            console.timeEnd("read");
            console.time("convert");
            var blob = dataURLToBlob(e.target.result);
            console.timeEnd("convert");
            callBack(blob);
        }
        console.time("read");
        reader.readAsDataURL(selectedFiles[0]);
    } else if ($('#cameraImage').val() !== "") {
        callBack(dataURLToBlob($('#cameraImage').val()));
    }
}

function dataURLToBlob(dataURL) {
    var BASE64_MARKER = ';base64,';
    if (dataURL.indexOf(BASE64_MARKER) == -1) {
        var parts = dataURL.split(',');
        var contentType = parts[0].split(':')[1];
        var raw = decodeURIComponent(parts[1]);

        return new Blob([raw], { type: contentType });
    }

    var parts = dataURL.split(BASE64_MARKER);
    var contentType = parts[0].split(':')[1];
    var raw = window.atob(parts[1]);
    var rawLength = raw.length;

    var uInt8Array = new Uint8Array(rawLength);

    for (var i = 0; i < rawLength; ++i) {
        uInt8Array[i] = raw.charCodeAt(i);
    }

    return new Blob([uInt8Array], { type: contentType });
}
