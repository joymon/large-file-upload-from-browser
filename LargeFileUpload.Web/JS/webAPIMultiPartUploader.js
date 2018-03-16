function readAsyncAndSendAsMultiPartFormDataviaWebAPI(file) {

    $.get(uploadConfigurations.rootUrl + "/api/upload/new", (data, status) => {
        //alert(data);
        UploadFile(file, data);
    }
    );
}

function UploadFile(file,id) {
    console.time("upload - readAsyncAndSendAsMultiPartFormDataviaWebAPI");
    // create array to store the buffer chunks
    var FileChunk = [];
    // the file object itself that we will work with
    var BufferChunkSize = uploadConfigurations.chunkSize
    //var ReadBuffer_Size = 1024;
    var fileStreamPos = 0;
    // set the initial chunk length
    var EndPos = BufferChunkSize;
    var Size = file.size;

    // add to the FileChunk array until we get to the end of the file
    while (fileStreamPos < Size) {
        // "slice" the file from the starting position/offset, to  the required length
        FileChunk.push(file.slice(fileStreamPos, EndPos));
        fileStreamPos = EndPos; // jump by the amount read
        EndPos = fileStreamPos + BufferChunkSize; // set next chunk length
    }
    // get total number of "files" we will be sending
    var TotalParts = FileChunk.length;
    var PartCount = 0;
    // loop through, pulling the first item from the array each time and sending it
    var promises = [];
    console.log("Uploading started.");
    console.time("upload");
    
    while (chunk = FileChunk.shift()) {
        PartCount++;
        // file name convention
        //var FilePartName = file.name + ".part_" + PartCount + "." + TotalParts;
        var FilePartName = PartCount;
        // send the file
        promises.push(UploadFileChunk(id,chunk, FilePartName));
    }
    $.when.apply(this, promises).then(function (responses) {
        // Each argument is an array with the following structure: [ data, statusText, jqXHR ]
        console.timeEnd("upload");
        var responseArgsArray = Array.prototype.slice.call(this, arguments);
        console.log("Upload completed. Going to merge");
        console.time("merge");
        mergeFiles(id, file.name).then(function (response) {
            console.timeEnd("merge");
            console.timeEnd("upload - readAsyncAndSendAsMultiPartFormDataviaWebAPI");
            document.getElementById('upload-dialog').close();
            alert('Upload and merge done');
        });
    });
}
function mergeFiles(id, name) {
    return $.get(uploadConfigurations.rootUrl  +  `/api/upload/merge/${id}/${name}/`);
}
function UploadFileChunk(id,Chunk, FileName) {
    var FD = new FormData();
    FD.append('file', Chunk, FileName);
    return $.ajax({
        type: "POST",
        url: uploadConfigurations.rootUrl +'/api/upload/'+ id,
        contentType: false,
        processData: false,
        data: FD
    });
}