UDCsocket = function (options) {
    var defaults =
    {
        ip: "127.0.0.1",
        //onSearchComplete:localSearchResult,
        port: 8989,
        conn: null,
        recData: null,
        close: null,
        jump: null,
        error: null
    }
    try {
        var ws;
        var settakon = this.settakon;
        var SOCKECT_ADDR = "";
        if (options.ip.indexOf("ws") >= 0)
            SOCKECT_ADDR = "" + options.ip + ":" + options.port + "";
        else
            SOCKECT_ADDR = "ws://" + options.ip + ":" + options.port + "";

        this.jump = options.jump;
        ws = new WebSocket(SOCKECT_ADDR);
        ws.binaryType = "arraybuffer";

        var token;
        ws.onopen = function (event) { options.conn(this.readyState); };
        var timeout;
        ws.onmessage = function (evt) {
            try {
                var bytesRead = evt.data.length;
                var text;
                //evt.data blob类型
                if (/^\[object (?:Uint8Array|ArrayBuffer)(?:Constructor)?\]$/.test(evt.data)) {
                    var tempbtye = new Uint8Array(evt.data);//转成byte[] 类型，或者其他array类型
                    //if (tempbtye[0] == 0x99) {
                    //    var myDate = new Date();
                    //    timeout = myDate;//记录当前时间
                    //    if (bytesRead > 1) {
                    //        var b = new byte[bytesRead - 1];
                    //        var t = tempbtye;
                    //        Array.Copy(t, 1, b, 0, b.Length);//拷贝数据到b
                    //        tempbtye = b;
                    //        bytesRead = bytesRead - (1);
                    //    }
                    //    return;
                    //}
                    var a = tempbtye[1];
                    var temp2 = Utf8ArrayToStr(tempbtye, 2, a);//通过index起点，和len 长度从btye[] 转成utf8 
                    var len = parseInt(temp2);
                    if (tempbtye.length == (len + 2 + a))
                        text = Utf8ArrayToStr(tempbtye, 2 + a, len);//通过index起点，和len 长度从btye[] 转成utf8 
                    //else
                    //    alert('长度不足');
                }
                if (tempbtye[0] == 0xff) {
                    if (text.indexOf("token") >= 0) {

                        settakon(text.split('|')[1]);

                        //settakon(token);
                        options.conn('token', token);
                    }
                    if (text.indexOf("jump") >= 0)
                        if (options.jump != null)
                            options.jump(text.split('|')[1]);
                }
                else if (tempbtye[0] == 0x99) {
                    return;
                }
                else
                    if (options.recData != null)
                        options.recData(text);
            } catch (e) { alert(e.message); }
            //   alert("接收到服务器发送的数据：\r\n" + text);



        };
        ws.onclose = function (event) { if (options.close != null) options.close(this.readyState, token); };
        ws.onerror = function (event) {
            if (options.error != null)
                options.error(event.data);

        };
        this.soc = ws;

    } catch (ex) {
        alert(ex.message);
    }



    function Utf8ArrayToStr(array, i, len) {
        var out, c;
        var char2, char3;

        out = "";

        len = parseInt(i) + parseInt(len);
        while (i < len) {
            c = array[i++];
            switch (c >> 4) {
                case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
                    // 0xxxxxxx
                    out += String.fromCharCode(c);
                    break;
                case 12: case 13:
                    // 110x xxxx   10xx xxxx
                    char2 = array[i++];
                    out += String.fromCharCode(((c & 0x1F) << 6) | (char2 & 0x3F));
                    break;
                case 14:
                    // 1110 xxxx  10xx xxxx  10xx xxxx
                    char2 = array[i++];
                    char3 = array[i++];
                    out += String.fromCharCode(((c & 0x0F) << 12) |
                        ((char2 & 0x3F) << 6) |
                        ((char3 & 0x3F) << 0));
                    break;
            }
        }

        return out;
    }



}
UDCsocket.prototype = {
    listListen: new Array(),
    soc: null,
    takon: "",
    jump: null,
    AddListenmethod: function (fun) {
        //  alert(str);
        this.listListen.push(fun);
    },
    settakon: function (str) {
        // alert(str);
        this.takon = str;
    },
    SendData: function (command, Request) {

        try {
            //  var takon = this.takon;
            // alert(this.takon);
            //alert(takon);
            //debugger;
            try {
                if (takon == "" || takon == null) {
                    //alert("takon不存在！");
                    takon = "bbbb";
                }
            } catch (e) { takon = "bbbb"; }
            var test = Request;
            //test.Request = Request;
            //test.Root = JSON.stringify(Root);
            //test.Parameter = JSON.stringify(Parameter);
            try {
                test.Token = takon;
            } catch (e) {
                test.Token = "bbbb";
            }

            test.Querycount = 0;
            test.Number = null;
            // alert(JSON.stringify(test));

            var sendb = (JSON.stringify(test)).getBytes();
            var lens = (sendb.length + '').getBytes();
            //不清楚getBytes 是不是utf8格式
            //byte[] b = new byte[2 + lens.Length + sendb.Length];
            var bytes = [2 + lens.length + sendb.length];
            // var b = new Int8Array(bytes);
            bytes[0] = command;
            bytes[1] = lens.length;
            //lens.CopyTo(b, 2);
            for (var i = 0; i < lens.length; i++) {
                bytes[i + 2] = lens[i];
            }
            for (var i = 0; i < sendb.length; i++) {
                bytes[i + 2 + lens.length] = sendb[i];
            }
            var b = new Int8Array(bytes);
            //sendb.CopyTo(b, 2 + lens.Length);

            this.soc.send(b);
        } catch (ex) {
            alert("SendData" + ex.message);
        }
    }

}

String.prototype.getBytes = function () {
    //var bytes = [];
    //for (var i = 0; i < this.length; i++) {
    //    var charCode = this.charCodeAt(i);
    //    var cLen = Math.ceil(Math.log(charCode) / Math.log(256));
    //    for (var j = 0; j < cLen; j++) {
    //        bytes.push((charCode << (j * 8)) & 0xFF);
    //    }
    //}

    //return bytes;
    var str = this;
    var byteArray = [];
    for (var i = 0; i < str.length; i++)
        if (str.charCodeAt(i) <= 0x7F)
            byteArray.push(str.charCodeAt(i));
        else {
            var h = encodeURIComponent(str.charAt(i)).substr(1).split('%');
            for (var j = 0; j < h.length; j++)
                byteArray.push(parseInt(h[j], 16));
        }
    return byteArray;
}
