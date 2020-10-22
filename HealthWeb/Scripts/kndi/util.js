

String.format = function (src) {
    if (arguments.length === 0) return null;
    var args = Array.prototype.slice.call(arguments, 1);
    return src.replace(/\{(\d+)\}/g, function (m, i) {
        return args[i];
    });
};

// 對Date的擴充套件，將 Date 轉化為指定格式的String
// 月(M)、日(d)、小時(h)、分(m)、秒(s)、季度(q) 可以用 1-2 個佔位符，
// 年(y)可以用 1-4 個佔位符，毫秒(S)只能用 1 個佔位符(是 1-3 位的數字)
// 例子：
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423
// (new Date()).Format("yyyy-M-d h:m:s.S")   ==> 2006-7-2 8:9:4.18
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};


Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf()); // (1)
    dat.setDate(dat.getDate() + days);  // (2)
    return dat;
};

Date.prototype.toLocalISOTime = function () {
    var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
    //console.info(this)
    return new Date(this - tzoffset).toISOString().slice(0, -1);
};


function showSpinner() {
    if (spinnerContainer) {
        ej.popups.showSpinner(spinnerContainer);
    }
}


function hideSinner() {
    if (spinnerContainer)
        ej.popups.hideSpinner(spinnerContainer);
}

var spinnerContainer;
$(function () {

    $(document).bind('ajaxSend', function (event, request, options) {
        showSpinner();
    }).bind('ajaxError', function (event, request, settings, thrownError) {
        //onetwui.showToast({ title: 'AJax', content: String.format('Error! {0}', thrownError) });
    }).bind('ajaxComplete', function (event, request, settings) {
        hideSinner();
    });

    ej.base.enableRipple(true);
    spinnerContainer = document.getElementsByTagName('body')[0];
    ej.popups.createSpinner({
        target: spinnerContainer
    });
});


function loginLineOAuth(sessionId, hostname, client_id) {
    var redirect_uri = String.format('http://{0}/CS/LineOAuthCallback', hostname);
    var url = String.format('https://access.line.me/oauth2/v2.1/authorize?response_type=code&client_id={0}&redirect_uri={1}&state={2}&scope=openid%20profile%20email',
        client_id, redirect_uri, client_id);
    console.log(url);
    window.location.href = url;
}

function facebookAuth() {
}

function wechatAuth() {
}