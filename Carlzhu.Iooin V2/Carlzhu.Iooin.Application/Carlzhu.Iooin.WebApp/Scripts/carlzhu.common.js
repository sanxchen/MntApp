
//消息


$(function ($j) {
    //加载通用布局
    $("<div></div>").appendTo("body").load("/Default/GetLayoutComm");

    $("*:not(div)").tooltip({
        placement: "top",
        delay: { show: 500, hide: 100 }
    });

    $(".datetimepicker").datetimepicker({
        dateFormat: "yy/mm/dd"
    });

    $(".datepicker").datepicker({
        dateFormat: "yy/mm/dd",
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true
    });

    $(".dialog").dialog({
        autoOpen: false,
        modal: true,
        relsize: false,
        closeOnEscape: true,
        minWidth: 400,
        minHeight: 200,
        draggable: false,
        resizable: false,
        show: {
            effect: "blind",
            duration: 300
        },
        hide: {
            effect: "clip",
            duration: 300
        }


    });





    //黑色提示靠右
    $("form *").tooltipValidation({
        placement: "right"
    });

    //Input只能输入数值
    $(":input.number").keyup(function () {
        $(this).val($(this).val().replace(/\D/g, ""));
    });

    //阻止Input提交表单
    $(":input.stopsubmit").keypress(function (e) {
        return e.keyCode !== 13;
    });


    $("#rmbUpper").text(function () {
        var rmb = $(this).data("rmb");

        return upDigit(rmb);
    });


    //全屏
    $('#fullscreenbtn').bind("click", function () {
        if (window.fullScreenApi.supportsFullScreen) {
            window.fullScreenApi.requestFullScreen(document.getElementById('fullscreenbox'));
        } else {
            alert('浏览器版本较低，请在首页进行下载安装');
        }
    });
    if ($("#InvolvingUserPanel").length > 0) {
        var invoEle = $("#InvolvingUserPanel");
        $("<div>" +
            "<p>" +
            "<input type='text' placeholder='请输入工号按添加' id='InvolvingTagEmployee' class='number' />" +
            "<input type='button' value='&nbsp;&nbsp;添&nbsp;加&nbsp;&nbsp;' id='addInvolvingTag' />" +
            "<input type='button' value='更新名单' class='pull-right' id='updateInvolvingTag' />" +
            "</p>" +
            "<p class='help-block text-warning'>请在上面输入工号，然后点添加，待所有人员添加修改完毕后，按[更新名单]保存<small class='pull-right'>by Minicut</small></p>" +
            "<input type='text' class='InvolvingTag'  data-guid='" + invoEle.attr("data-guid") + "' /></div>").appendTo(invoEle);
    }

    $(":input.InvolvingTag").tagsInput({
        width: "auto",
        interactive: false,
        onAddTag: function (tag) {
            var va = $(this).val();
            if (((va.split(tag)).length) > 2) {
                rightCornerMsg("该标签已存在啦！！！", "error");
                $(this).removeTag(tag);
                $(this).importTags(va.substring(0, (va.lastIndexOf(tag) - 1)));
            }
        },
        Initialization: (function (e) {
            //是否需要添加元素
            var involvingTag = $(".InvolvingTag");
            var guid = involvingTag.attr("data-guid");
            var emp = $("#InvolvingTagEmployee");

            //用于修改时初始化名单
            $.post("/eForm/Tracking/GetJsonInvolvingTag", { guid: guid }, function (e) {
                involvingTag.importTags(e);
            });


            $("#addInvolvingTag").bind("click", function () {
                $.post("/CommonModule/Employee/SetForm", { keyValue: emp.val() }, function (data) {
                    data = eval("(" + data + ")");
                    involvingTag.addTag(data.EmpNo + ":" + data.RealName);
                    emp.val("");
                });

            });

            $("#updateInvolvingTag").bind("click", function () {
                if (involvingTag.val() !== "") {
                    $.post("/eForm/F/UpdateInvolvingTag", { employees: involvingTag.val(), guid: guid }, function (t) {
                        (t === "True") ? rightCornerMsg("名单更新成功", "success") : rightCornerMsg("名单更新失败", "error");
                    });
                } else {
                    rightCornerMsg("名单不可为空", "info");
                }
            });

        })()
    });


});

//div滚动

function startmarquee(lh, speed, delay) {
    var t;
    var oHeight = 300;/** div的高度 **/
    var p = false;
    var o = document.getElementById("show");
    var preTop = 0;
    o.scrollTop = 0;
    function start() {
        t = setInterval(scrolling, speed);
        o.scrollTop += 1;
    }
    function scrolling() {
        if (o.scrollTop % lh !== 0
                && o.scrollTop % (o.scrollHeight - oHeight - 1) !== 0) {
            preTop = o.scrollTop;
            o.scrollTop += 1;
            if (preTop >= o.scrollHeight || preTop === o.scrollTop) {
                o.scrollTop = 0;
            }
        } else {
            clearInterval(t);
            setTimeout(start, delay);
        }
    }
    setTimeout(start, delay);
}
//文件上传窗体

function UpFile(element, limits, fileType, reqFileExp) {
    //effects:blind,bounce,clip,drop,explode,fade,fold,highlight,pulsate,puff,scale,size,shake,slide,transfer
    var array = ['blind', 'bounce', 'clip', 'drop', 'explode', 'fade', 'fold', 'highlight', 'pulsate', 'puff', 'scale', 'size', 'shake', 'slide'];
    var num = Math.floor(Math.random() * (15));
    $("#upFileDig").load('/eForm/F/UpFilePage?limits=' + limits + '&filetype=' + fileType + '' + "&reqFileExp=" + reqFileExp).dialog({
        modal: true,
        minWidth: 640,
        minHeight: 320,
        resizable: false,
        draggable: false,
        closeOnEscape: true,
        show: {
            effect: array[num],
            duration: 300
        },
        hide: {
            effect: "clip",
            duration: 300
        }
    }).dialog("option", "element", element);
}

function Finish(rt) {
    var dialogEle = $("#upFileDig");
    var result = rt.split("|");
    alert(result[0]);
    if (result[0] === "上传成功！") {
        $(dialogEle.dialog("option", "element")).val(result[1]).focus();
        dialogEle.dialog("close");
        $("#upFileList").text(result[2]);
    }
}


function rightCornerMsg(msg, type) {
    $.globalMessenger().post({
        message: msg,
        type: type
    });
}

function LoadQueryData(status, word) {
    var img = $("#idCarlzhuProgressImage");
    var mask = $("#idCarlzhuMaskImage");

    switch (status) {
        case "onbegin":
            $("#idCarlzhuLoadingChar").text(word);
            img.show().removeClass("hide").css({
                "position": "fixed",
                "top": "50%",
                "left": "50%",
                "margin-top": function () { return -1 * img.height() / 2; },
                "margin-left": function () { return -1 * img.width() / 2; }
            });
            mask.show().removeClass("hide").css("opacity", "0.1");
            break;
        default:
            img.hide();
            mask.hide();
            break;
    }
}


$.ajaxLoading = function (options) {
    var complete = options.complete;
    options.complete = function (httpRequest, status) {
        LoadQueryData("oncomplete");
        if (complete) {
            complete(httpRequest, status);
        }
    };
    options.async = true;
    options.type = "Post";
    LoadQueryData("onbegin", options.loadingdata);
    $.ajax(options);
};


function formatJsonDateTime(val, formatType) {
    var re = /-?\d+/;
    var m = re.exec(val);
    var d = new Date(parseInt(m[0]));

    return d.format(formatType);
}
// 用于格式化日期显示
Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month 
        "d+": this.getDate(),    //day 
        "h+": this.getHours(),   //hour 
        "m+": this.getMinutes(), //minute 
        "s+": this.getSeconds(), //second 
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter 
        "S": this.getMilliseconds() //millisecond 
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
    (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (o.hasOwnProperty(k))
            if (new RegExp("(" + k + ")").test(format))
                format = format.replace(RegExp.$1,
                    RegExp.$1.length === 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}


function showTime() {
    var today = new Date();
    var day;
    if (today.getDay() === 0) day = "星期日";
    if (today.getDay() === 1) day = "星期一";
    if (today.getDay() === 2) day = "星期二";
    if (today.getDay() === 3) day = "星期三";
    if (today.getDay() === 4) day = "星期四";
    if (today.getDay() === 5) day = "星期五";
    if (today.getDay() === 6) day = "星期六";
    var date = (today.getFullYear()) + "年" + (today.getMonth() + 1) + "月" + today.getDate() + "日 " +
        day + " " + today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
    document.getElementById("time").innerHTML = date;
}


function scrollToBottom(target) {
    target.animate({ scrollTop: target[0].scrollHeight });
}


function upDigit(n) {
    var fraction = ['角', '分'];
    var digit = ['零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖'];
    var unit = [['元', '万', '亿'], ['', '拾', '佰', '仟']];
    var head = n < 0 ? '欠' : '';
    n = Math.abs(n);

    var s = '';
    var i;
    for (i = 0; i < fraction.length; i++) {
        s += (digit[Math.floor(n * 10 * Math.pow(10, i)) % 10] + fraction[i]).replace(/零./, '');
    }
    s = s || '整';
    n = Math.floor(n);

    for (i = 0; i < unit[0].length && n > 0; i++) {
        var p = '';
        for (var j = 0; j < unit[1].length && n > 0; j++) {
            p = digit[n % 10] + unit[1][j] + p;
            n = Math.floor(n / 10);
        }
        s = p.replace(/(零.)*零$/, '').replace(/^$/, '零') + unit[0][i] + s;
    }
    return head + s.replace(/(零.)*零元/, '元').replace(/(零.)+/g, '零').replace(/^整$/, '零元整');
}