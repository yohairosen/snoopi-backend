/* Javascript by Daniel Cohen Gindi (c) danielgindi@gmail.com 054-5655765 */
/* Version: 2013-02-26 */
/* No dependencies */
(function(){var g={$:function(a){if(1<arguments.length){for(var b=0,c=[],f=arguments.length;b<f;b++)c.push(this._$(arguments[b]));return c}return!a?null:a&&1==a.nodeType?a:"string"==typeof a?document.getElementById?document.getElementById(a):document.layers?document.layers[a]:document.all[a]:null},extend:function(a,b){if(!b||!a)return a;for(var c in b)b.hasOwnProperty(c)&&(a[c]=b[c]);return a},observe:function(a,b,c,f){if(null!=a){if(0==b.indexOf("dom:",0)){var e=function(a){a=a||event;b==a.eventName&&
c(a)};a.addEventListener?a.addEventListener("dataavailable",e,f?!0:!1):a.attachEvent&&a.attachEvent("ondataavailable",e)}else a.addEventListener?(a.addEventListener(b,c,f?!0:!1),"mousewheel"==b&&a.addEventListener("DOMMouseScroll",c,f?!0:!1)):a.attachEvent&&a.attachEvent("on"+b,c);return c}},unobserve:function(a,b,c,f){if(null!=a)return f=f?!0:!1,a.removeEventListener?(a.removeEventListener(b,c,f),"mousewheel"==b&&a.removeEventListener("DOMMouseScroll",c,f)):a.detachEvent&&a.detachEvent("on"+b,c),
c},fire:function(a,b){a==document&&(document.createEvent&&!a.dispatchEvent)&&(a=document.documentel);var c;document.createEvent?(c=document.createEvent("HTMLEvents"),c.initEvent("dataavailable",!0,!0)):(c=document.createEventObject(),c.eventType="ondataavailable");c.eventName=b;document.createEvent?a.dispatchEvent(c):a.fireEvent(c.eventType,c);return c},isArray:function(a){return null!=a&&"object"==typeof a&&"splice"in a&&"join"in a}},h={_autoUpdates:[],realTimezoneOffset:(new Date).getTimezoneOffset(),
currentDate:new Date,ISORegex:/([0-9]{4,4})-([0-9]{2,2})-([0-9]{2,2})(T([0-9]{2,2})(:([0-9]{2,2}))?(:([0-9]{2,2}))?(\.([0-9]{1,3}))?)?(Z||(\+([0-9]{2,2})(:?([0-9]{2,2}))?)||(-([0-9]{2,2})(:?([0-9]{2,2}))?))?/,__updateTimeout:null,updateInterval:0,defaultFormat:"default",defaultCulture:"en-us",_cultureReady:!1,Culture:"auto",DefaultFormats:{"default":"ddd MMM dd yyyy HH:mm:ss",shortDate:"M/d/yy",normalDate:"MM/dd/yy",mediumDate:"MMM d, yyyy",mediumDateShortTime:"MMM d, yyyy h:mm TT",longDate:"MMM d, yyyy",
fullDate:"dddd, MMMM d, yyyy",shortTime:"h:mm TT",mediumTime:"h:mm:ss TT",longTime:"h:mm:ss TT Z",normalDateTime:"MM/dd/yy HH:mm:ss",isoDate:"yyyy-MM-dd",isoTime:"HH:mm:ss",isoDateTime:"yyyy-MM-dd'T'HH:mm:ss",isoUtcDateTime:"UTC:yyyy-MM-dd'T'HH:mm:ss'Z'",relative:"REL"},Cultures:{en:{dayNames:"Sunday Monday Tuesday Wednesday Thursday Friday Saturday Sun Mon Tue Wed Thu Fri Sat".split(" "),monthNames:"January February March April May June July August September October November December Jan Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec".split(" "),
relative:{now:"now","-1min":"1 minute ago","-mins":"# minutes ago","-1sec":"1 second ago","-secs":"# seconds ago","-1hr":"about an hour ago","-hrs":"# hours ago","-1day":"about a day ago","-days":"# days ago","+1min":"in 1 minute","+mins":"in # minutes","+1sec":"in 1 second","+secs":"in # seconds","+1hr":"in about an hour","+hrs":"in # hours","+1day":"in about a day","+days":"in # days"}},he:{dayNames:"\u05e8\u05d0\u05e9\u05d5\u05df \u05e9\u05e0\u05d9 \u05e9\u05dc\u05d9\u05e9\u05d9 \u05e8\u05d1\u05d9\u05e2\u05d9 \u05d7\u05de\u05d9\u05e9\u05d9 \u05e9\u05d9\u05e9\u05d9 \u05e9\u05d1\u05ea \u05e8\u05d0\u05e9\u05d5\u05df \u05e9\u05e0\u05d9 \u05e9\u05dc\u05d9\u05e9\u05d9 \u05e8\u05d1\u05d9\u05e2\u05d9 \u05d7\u05de\u05d9\u05e9\u05d9 \u05e9\u05d9\u05e9\u05d9 \u05e9\u05d1\u05ea".split(" "),
monthNames:"\u05d9\u05e0\u05d5\u05d0\u05e8 \u05e4\u05d1\u05e8\u05d5\u05d0\u05e8 \u05de\u05e8\u05e5 \u05d0\u05e4\u05e8\u05d9\u05dc \u05de\u05d0\u05d9 \u05d9\u05d5\u05e0\u05d9 \u05d9\u05d5\u05dc\u05d9 \u05d0\u05d5\u05d2\u05d5\u05e1\u05d8 \u05e1\u05e4\u05d8\u05de\u05d1\u05e8 \u05d0\u05d5\u05e7\u05d8\u05d5\u05d1\u05e8 \u05e0\u05d5\u05d1\u05de\u05d1\u05e8 \u05d3\u05e6\u05de\u05d1\u05e8 \u05d9\u05e0\u05d5 \u05e4\u05d1\u05e8 \u05de\u05e8\u05e5 \u05d0\u05e4\u05e8 \u05de\u05d0\u05d9 \u05d9\u05d5\u05e0 \u05d9\u05d5\u05dc \u05d0\u05d5\u05d2 \u05e1\u05e4\u05d8 \u05d0\u05d5\u05e7 \u05e0\u05d5\u05d1 \u05d3\u05e6\u05de".split(" "),
relative:{now:"\u05e2\u05db\u05e9\u05d9\u05d5","-1min":"\u05dc\u05e4\u05e0\u05d9 \u05d3\u05e7\u05d4","-mins":"\u05dc\u05e4\u05e0\u05d9 # \u05d3\u05e7\u05d5\u05ea","-1sec":"\u05dc\u05e4\u05e0\u05d9 \u05e9\u05e0\u05d9\u05d4","-secs":"\u05dc\u05e4\u05e0\u05d9 # \u05e9\u05e0\u05d9\u05d5\u05ea","-1hr":"\u05dc\u05e4\u05e0\u05d9 \u05e9\u05e2\u05d4","-hrs":"\u05dc\u05e4\u05e0\u05d9 # \u05e9\u05e2\u05d5\u05ea","-1day":"\u05dc\u05e4\u05e0\u05d9 \u05d9\u05d5\u05dd","-days":"\u05dc\u05e4\u05e0\u05d9 # \u05d9\u05de\u05d9\u05dd",
"+1min":"\u05d1\u05e2\u05d5\u05d3 \u05d3\u05e7\u05d4","+mins":"\u05d1\u05e2\u05d5\u05d3 # \u05d3\u05e7\u05d5\u05ea","+1sec":"\u05d1\u05e2\u05d5\u05d3 \u05e9\u05e0\u05d9\u05d4","+secs":"\u05d1\u05e2\u05d5\u05d3 # \u05e9\u05e0\u05d9\u05d5\u05ea","+1hr":"\u05d1\u05e2\u05d5\u05d3 \u05e9\u05e2\u05d4","+hrs":"\u05d1\u05e2\u05d5\u05d3 # \u05e9\u05e2\u05d5\u05ea","+1day":"\u05d1\u05e2\u05d5\u05d3 \u05d9\u05d5\u05dd","+days":"\u05d1\u05e2\u05d5\u05d3 # \u05d9\u05de\u05d9\u05dd"},formats:{shortDate:"d/M/yy",
normalDate:"dd/MM/yy",normalDateTime:"dd/MM/yy HH:mm:ss"}}},findCulture:function(a,b){if(!a)return b?this.findCulture(this.defaultCulture):null;if("string"!=typeof a)return a;if("auto"==a)for(var c=document.getElementsByTagName("meta"),f=0;f<c.length;f++){var e=c[f];if("content"==(e.getAttribute("http-equiv")||"").toLowerCase()){e=(e.getAttribute("content")||"").split(/,/g);for(c=0;c<e.length;c++)if(a=e[c],a=this.findCulture(a,b))return a;break}}if(this.Cultures[a])return this.Cultures[a];f=a.lastIndexOf("-");
return-1<f?(a=a.substr(0,f),this.findCulture(a,b)):b?this.findCulture(this.defaultCulture):null},prepareCulture:function(){for(var a=this.Culture,b=document.getElementsByTagName("script"),c=0;c<b.length;c++){var f=b[c];if(f.src&&f.src.match(/dgDateManager((_src)|(\-min))?\.js(\?.*)?$/)){a=(b=f.src.match(/\?.*culture=([a-z\-]*)/i))?b[1]:a;break}}"string"==typeof a&&(a=this.findCulture(a,!0));this.Culture=a;this.ActualFormats=g.extend(g.extend({},this.DefaultFormats),a.formats);this._cultureReady=!0;
return this},fixTimezoneOffset:function(a){a.setMinutes(a.getMinutes()-a.getTimezoneOffset()+h.realTimezoneOffset);return a},setFormat:function(){if(1==arguments.length){var a=arguments[0];this.defaultFormat=a;return this}if(2==arguments.length){var b=arguments[0],a=arguments[1],b=g.$(b);if(!b||!b.__dgDateManager)return!1;b.__dgDateManager.format=a;return this}return!1},setUpdate:function(){if(1==arguments.length){var a=arguments[0];0>a&&(a=0);if(a==this.updateInterval)return this;0==a?(this.updateInterval=
a,this.__updateTimeout&&(clearTimeout(this.__updateTimeout),this.__updateTimeout=null)):0<a&&(this.updateInterval=a,this.__updateTimeout&&clearTimeout(this.__updateTimeout),this._updateAllTimeoutF||(this.__updateAllTimeoutF=function(){h._updateAllTimeout()}),this.__updateTimeout=setTimeout(this.__updateAllTimeoutF,this.updateInterval));return this}if(2==arguments.length){var b=arguments[0],a=arguments[1],b=g.$(b);if(!b||!b.__dgDateManager)return!1;if(a==b.__dgDateManager.update)return this;if("auto"==
a)b.__dgDateManager.__updateTimeout&&(clearTimeout(b.__dgDateManager.__updateTimeout),b.__dgDateManager.__updateTimeout=null);else{0>a&&(a=0);if(a==b.__dgDateManager.update)return this;0==a&&0<b.__dgDateManager.update?(b.__dgDateManager.update=a,b.__dgDateManager.__updateTimeout&&(clearTimeout(b.__dgDateManager.__updateTimeout),b.__dgDateManager.__updateTimeout=null)):0<a&&(b.__dgDateManager.update=a,b.__dgDateManager.__updateTimeout&&clearTimeout(b.__dgDateManager.__updateTimeout),b.__dgDateManager.__updateTimeout=
setTimeout(function(){h.update(b)},b.__dgDateManager.update))}return this}return!1},_updateAllTimeout:function(){this.updateAll();0<this.updateInterval&&(this.__updateTimeout=setTimeout(this.__updateAllTimeoutF,this.updateInterval));return this},updateAll:function(){this.currentDate=new Date;this.realTimezoneOffset=this.currentDate.getTimezoneOffset();for(var a=0;a<this._autoUpdates.length;a++)"auto"==this._autoUpdates[a].__dgDateManager.update&&this.update(this._autoUpdates[a])},update:function(a){"string"==
typeof a&&(a=g.$(a));var b=a.__dgDateManager.format;"auto"==b&&(b=this.defaultFormat);"auto"==b&&(b="default");this.ActualFormats[b]&&(b=this.ActualFormats[b]);b=this.formatDate(a.__dgDateManager.originalDate,b);void 0!==a.innerText?a.innerText=b:void 0!==a.textContent?a.textContent=b:void 0!==a.value&&(a.value=b);"auto"!=a.__dgDateManager.update&&0<a.__dgDateManager.update&&(a.__dgDateManager.__updateTimeout&&clearTimeout(a.__dgDateManager.__updateTimeout),a.__dgDateManager.__updateTimeout=setTimeout(function(){h.update(a)},
a.__dgDateManager.update))},start:function(a,b){this._cultureReady||this.prepareCulture();a=g.$(a);if(g.isArray(a))for(var c=[],f=0;f<a.length;f++)c.push(this.start(a[f],b));else{if(!a)return null;c=this._opts=g.extend(g.extend({},{update:"auto",format:"auto"}),b);f=a.textContent||a.innerText||a.value;if(!f)return!1;f=h.parseDate(f);if(!f)return!1;a.__dgDateManager={originalDate:f,update:c.update,format:c.format};a.updateDate=function(){h.update(a)};a.setUpdate=function(b){h.setUpdate(a,b)};a.setFormat=
function(b){h.setFormat(a,b)};this._autoUpdates.push(a);this.update(a)}return this},startAll:function(a){this._cultureReady||this.prepareCulture();var b=/(^|\s)(dgDateManager)\s*(\{[^}]*\})?/i;a=a||document;a=a.getElementsByTagName("*");for(var c,f,e=0;e<a.length;e++){c=a[e];if(!c.__dgDateManager&&c.className&&(f=c.className.match(b))){var g={};if(f[3])try{g=(new Function("return ("+f[3]+")"))()}catch(j){}this.start(c,g)&&e--}c=null}0<this.updateInterval&&!this.__updateTimeout&&(this.__updateTimeout&&
clearTimeout(this.__updateTimeout),this._updateAllTimeoutF||(this.__updateAllTimeoutF=function(){h._updateAllTimeout()}),this.__updateTimeout=setTimeout(this.__updateAllTimeoutF,this.updateInterval));return this},formatDate:function(a,b,c){return c?a.toUTCString(b):a.toString(b)},parseDate:function(a){a=a.match(this.ISORegex);if(!a)return null;void 0===a[1]&&(a[1]=0);void 0===a[2]&&(a[2]=0);void 0===a[3]?a[3]=0:a[2]-=1;parsedDate=new Date(a[1],a[2],a[3],a[5]||0,a[7]||0,a[9]||0,a[11]||0);var b=0;void 0!==
a[14]?(b=60*-(parseInt(a[14],10)||0),void 0!==a[16]&&(b-=parseInt(a[16],10)||0),b+=parsedDate.getTimezoneOffset()):void 0!==a[18]?(b=60*(parseInt(a[18],10)||0),void 0!==a[20]&&(b+=parseInt(a[20],10)||0),b+=parsedDate.getTimezoneOffset()):"Z"==a[12]&&(b+=parsedDate.getTimezoneOffset());parsedDate.setMinutes(parsedDate.getMinutes()-b);return h.fixTimezoneOffset(parsedDate)}};window.dgDateManager=h;var q=/d{1,4}|M{1,4}|yy(?:yy)?|([HhmsTt])\1?|[LloSZ]|UTC|"[^"]*"|'[^']*'/g,r=/\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
t=/[^-+\dA-Z]/g,j=function(a,b){a=String(a);for(b=b||2;a.length<b;)a="0"+a;return a},u={d:function(a){return a.getDate()},D:function(a){return a.getDay()},M:function(a){return a.getMonth()},y:function(a){return a.getFullYear()},H:function(a){return a.getHours()},m:function(a){return a.getMinutes()},s:function(a){return a.getSeconds()},L:function(a){return a.getMilliseconds()},o:function(){return 0},utcd:function(a){return(String(a).match(r)||[""]).pop().replace(t,"")},utc:function(a){a=a.getTimezoneOffset();
s=0<a?"-":"+";a=0>a?-a:a;var b=a%60;return s+j((a-b)/60,2)+(b?j(b,2):"")}},v={d:function(a){return a.getUTCDate()},D:function(a){return a.getUTCDay()},M:function(a){return a.getUTCMonth()},y:function(a){return a.getUTCFullYear()},H:function(a){return a.getUTCHours()},m:function(a){return a.getUTCMinutes()},s:function(a){return a.getUTCSeconds()},L:function(a){return a.getUTCMilliseconds()},o:function(){return d.getTimezoneOffset()},utcd:function(){return"UTC"},utc:function(){return"Z"}},n={d:function(a,
b){return b.d(a)},dd:function(a,b){return j(b.d(a))},ddd:function(a,b){return h.Culture.dayNames[b.D(a)+7]},dddd:function(a,b){return h.Culture.dayNames[b.D(a)]},M:function(a,b){return b.M(a)+1},MM:function(a,b){return j(b.M(a)+1)},MMM:function(a,b){return h.Culture.monthNames[b.M(a)+12]},MMMM:function(a,b){return h.Culture.monthNames[b.M(a)]},yy:function(a,b){return String(b.y(a)).slice(2)},yyyy:function(a,b){return b.y(a)},h:function(a,b){return b.H(a)%12||12},hh:function(a,b){return j(b.H(a)%12||
12)},H:function(a,b){return b.H(a)},HH:function(a,b){return j(b.H(a))},m:function(a,b){return b.m(a)},mm:function(a,b){return j(b.m(a))},s:function(a,b){return b.s(a)},ss:function(a,b){return j(b.s(a))},l:function(a,b){return j(b.L(a),3)},L:function(a,b){var c=b.L(a);return j(99<c?Math.round(c/10):c)},t:function(a,b){return 12>b.H(a)?"a":"p"},tt:function(a,b){return 12>b.H(a)?"am":"pm"},T:function(a,b){return 12>b.H(a)?"A":"P"},TT:function(a,b){return 12>b.H(a)?"AM":"PM"},Z:function(a,b){return b.utc(a)},
UTC:function(a,b){return b.utcd(a)},o:function(a,b){a=b.o(a);return(0<a?"-":"+")+j(100*Math.floor(Math.abs(a)/60)+Math.abs(a)%60,4)},S:function(a,b){var c=b.d(a);return["th","st","nd","rd"][3<c%10?0:(10!=c%100-c%10)*c%10]}};h.formatDate=function(a,b,c,f){1==arguments.length&&("[object String]"==Object.prototype.toString.call(a)&&!/\d/.test(a))&&(b=a,a=void 0);a?"string"==typeof a?a=h.parseDate(a):a instanceof Date&&(a=new Date(a)):a=new Date;if(isNaN(a))throw SyntaxError("invalid date");if(!b)return c?
a.toUTCString(b):a.toString(b);var e=b.slice(0,4);"REL:"==e&&(f=!0,b=b.slice(4),e=b.slice(0,4));"UTC:"==e&&(b=b.slice(4),c=!0);if(f){e=Math.floor(new Date-a);if(0==e)return h.Culture.relative.now;var g;0>e?(e/=-1E3,g="+"):(e/=1E3,g="-");e=Math.floor(e);if(60>e)return 1==e?h.Culture.relative[g+"1sec"]:h.Culture.relative[g+"secs"].replace("#",e);e=Math.floor(e/60);if(60>e)return 1==e?h.Culture.relative[g+"1min"]:h.Culture.relative[g+"mins"].replace("#",e);e/=60;e=Math.floor(e);if(60>e)return 1==e?h.Culture.relative[g+
"1hr"]:h.Culture.relative[g+"hrs"].replace("#",e);e/=24;e=Math.floor(e);if(24>e)return 1==e?h.Culture.relative[g+"1day"]:h.Culture.relative[g+"days"].replace("#",e);b=h.DefaultFormats.mediumDateShortTime}var j=c?v:u;return b.replace(q,function(b){return b in n?n[b](a,j):b.slice(1,b.length-1)})};var k=function(){document.$domOnLoadFired||(l&&window.clearInterval(l),document.$domOnLoadFired=!0,document.isDomLoaded=!0,g.fire(document,"dom:onLoad"))},l;if(document.addEventListener)window.webkit?(l=window.setInterval(function(){/loaded|complete/.test(document.readyState)&&
k()},0),g.observe(window,"load",k,!1)):document.addEventListener("DOMContentLoaded",k,!1);else{document.write("<script id=__onDOMContentLoaded defer src=//:>\x3c/script>");var m=g.$("__onDOMContentLoaded"),p=function(){"complete"==m.readyState&&(g.unobserve(m,"readystatechange",p),k())};g.observe(m,"readystatechange",p)}g.observe(document,"dom:onLoad",function(){h.startAll()})})();