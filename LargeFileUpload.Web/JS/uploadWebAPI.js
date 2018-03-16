
$('#submit').click(function (evt) {
    if (!$('#upload-form')[0].checkValidity()) {
        alert('Not all inputs are given. Exiting...');
        return true;
    }
    evt.stopPropagation();
    evt.preventDefault();

    document.getElementById('upload-dialog').showModal();
    if (document.getElementById('multipartCheckBox').checked) {
        if (document.getElementById('fileReadingCheckBox').checked) {
            readAsyncAndSendAsMultiPartFormDataviaWebAPI(document.getElementById('uploadFile').files[0]);
        } else {
            readSyncInWorkerAndSendviaWebAPI(document.getElementById('uploadFile').files[0]);
        }
    }
    else {
        if (document.getElementById('fileReadingCheckBox').checked) {
            alert('not implemented');
            //readSyncInWorkerAndSendviaWebAPI(document.getElementById('uploadFile').files[0]);
        } else {
            readSyncInWorkerAndSendviaWebAPI(document.getElementById('uploadFile').files[0]);
        }
    }
    document.getElementById('upload-dialog').close();
});