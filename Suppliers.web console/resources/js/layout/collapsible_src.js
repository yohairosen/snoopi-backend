(function () {
    var utils = dgTools, dom = utils.dom;
    function SetupCollapsible(headerId, bodyId, cookie) {
        var header = utils.$(headerId);
        var body = utils.$(bodyId);
        if (!header || !body) {
            if (arguments[3]) return;
            utils.observe(document, 'dom:onLoad', function () {
                SetupCollapsible(headerId, bodyId, cookie, true);
            });
            return;
        }
        utils.observe(header, 'click', function () {
            if (dom.hasClassName(header, 'open')) {
                dom.removeClassName(header, 'open');
                dom.hide(body);
                if (cookie) utils.Cookies.save(cookie, 'closed', 365);
            } else {
                dom.addClassName(header, 'open');
                dom.show(body);
                if (cookie) utils.Cookies.save(cookie, 'open', 365);
            }
        });
        if (cookie) {
            var value = utils.Cookies.read(cookie);
            if (value == 'open') {
                dom.addClassName(header, 'open');
                dom.show(body);
            } else {
                dom.removeClassName(header, 'open');
                dom.hide(body);
            }
        } else {
            dom[dom.hasClassName(header, 'open') ? 'show' : 'hide'](body);
        }
    }
    /** @expose **/
    window.SetupCollapsible = SetupCollapsible;
})();