$('#getLocation').click(function () {
    window.navigator.geolocation.getCurrentPosition(function (result) {
        $('#location').val(result.coords.latitude + ", " + result.coords.longitude);
    });
    return false;
});
$('#getImage').click(function () {
    var hdConstraints = {
        video: {
            mandatory: {
                minWidth: 1280,
                minHeight: 720
            }
        }
    };
    var feed = Modernizr.prefixed('getUserMedia', navigator)(hdConstraints, function (stream) {
        $('#takeShot').show();
        document.getElementById('feed').src = window.URL.createObjectURL(stream);
        $('#takePicture').click(function () {
            $('#shotpreview').show();
            $('#feed').hide();
            document.getElementById('shotpreview').getContext("2d").drawImage(document.getElementById('feed'), 0, 0, 384, 216);
            document.getElementById('shot').getContext("2d").drawImage(document.getElementById('feed'), 0, 0, 1280, 720);
            $('#cameraImage').val(document.getElementById('shot').toDataURL("image/png"));
            stream.stop();
        });
    }, function () { });
});
$('#submit').click(function (evt) {
    if (!$('#upload-form')[0].checkValidity()) {
        return true;
    }
    if ($('#uploadUrl').val() !== '') {
        return true;
    }
    evt.stopPropagation();
    evt.preventDefault();

    document.getElementById('upload-dialog').showModal();
    readSyncInWorkerAndSendviaSocket(document.getElementById('uploadFile').files[0]);
    //processSocket()
});