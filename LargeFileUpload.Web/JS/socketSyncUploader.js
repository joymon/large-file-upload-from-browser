function readSyncInWorkerAndSendviaSocket(blob) {
    var worker = new Worker('../JS/socketSyncUploaderWorker.js');

    worker.onmessage = function (e) {
        switch (e.data.type) {
            case "success":
                document.getElementById('upload-dialog').close();
                console.timeEnd("upload - readSyncInWorkerAndSendviaSocket");
                break;
            case "progress":
                document.getElementById('upload-progress').value = e.data.value;
                break;
            case "error":
                console.log(e.data.value);
        }
    };

    worker.onerror = function (e) {
        document.querySelector('#error').textContent = [
            'ERROR: Line ', e.lineno, ' in ', e.filename, ': ', e.message].join('');
    };
    console.time("upload - readSyncInWorkerAndSendviaSocket");
    worker.postMessage({ blob: blob, rootLocation: window.location.host });
}
