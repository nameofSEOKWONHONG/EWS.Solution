window.download = (options) => {
    var fileUrl = "data:" + options.mimeType + ";base64," + options.byteArray;
    fetch(fileUrl)
        .then(response => response.blob())
        .then(blob => {
            var link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, { type: options.mimeType });
            link.download = options.fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
}
window.downloadFromUrl = (options) => {
    var xhr = new XMLHttpRequest();
    xhr.responseType = 'blob';
    xhr.onload = (event) => {
        var blob = xhr.response;
        var downloaded = document.createElement('a');
        var downloadedurl = window.URL.createObjectURL(blob);
        downloaded.href = downloadedurl;
        downloaded.download = options.filename;
        document.body.append(downloaded);
        downloaded.click();
        downloaded.remove();
        window.URL.revokeObjectURL(downloadedurl);
    };
    xhr.open('GET', options.url);
    xhr.send();
}