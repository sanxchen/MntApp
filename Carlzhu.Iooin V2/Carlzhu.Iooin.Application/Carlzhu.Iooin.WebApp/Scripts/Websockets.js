
var ws;
var SocketCreated = false;
var isUserloggedout = false;
var server = "192.168.0.211";
var noSupportMessage = "Your browser cannot support WebSocket!";




//连接和关闭现在连接
function connectSocketServer(requestConnect,wSonOpen, wSonMessage, wSonClose, wSonError) {
    var support = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);
    if (support == null) {
        return null;
    }


    if (SocketCreated && (ws.readyState === 0 || ws.readyState === 1)) {
        SocketCreated = false;
        isUserloggedout = true;
        ws.close();
    } else {
        requestConnect();
        try {
            if ("WebSocket" in window) {
                ws = new WebSocket("ws://" + server + ":4141/carlzhu");
            } else if ("MozWebSocket" in window) {
                ws = new window.MozWebSocket("ws://" + server + ":4141/carlzhu");
            }

            SocketCreated = true;
            isUserloggedout = false;
        } catch (ex) {
            wSonError();
            return null;
        }
        ws.onopen = wSonOpen;
        ws.onmessage = wSonMessage;
        ws.onclose = wSonClose;
        ws.onerror = wSonError;
    }

    
    return ws;
};





