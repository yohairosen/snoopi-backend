(function () {

    var utils = dgTools, dom = utils.dom;

    var Dashboard = {
        InitControls: function () {
            var self = this;

            var ulChartFilters = utils.$('ulChartFilters');
            var ulChartRanges = utils.$('ulChartRanges');

            var filterLis = utils.select('>li[data-filter]', ulChartFilters);
            filterLis.forEach(function (item) {
                var itemFilter = item.getAttribute('data-filter');
                utils.observe(item, 'click', function () {
                    Dashboard.ToggleFilter(itemFilter);
                });
            });

            var rangeLis = utils.select('>li[data-range]', ulChartRanges);
            rangeLis.forEach(function (item) {
                var itemRange = item.getAttribute('data-range');
                utils.observe(item, 'click', function () {
                    Dashboard.ToggleRange(itemRange);
                });
            });

            rangeLis = utils.select('li[data-range-dates],a[data-range-dates]', ulChartRanges);
            rangeLis.forEach(function (item) {
                utils.observe(item, 'click', function () {
                    Dashboard.ToggleRange(item);
                });
            });

            var toggles = utils.select('li[data-toggle],a[data-toggle]', ulChartRanges);
            toggles.forEach(function (item) {
                var toggle = item.getAttribute('data-toggle');
                utils.observe(item, 'click', function () {

                    var affect = utils.select(toggle, ulChartRanges)[0];
                    if (affect) {
                        var lis = utils.select('>li', ulChartRanges);
                        if (dom.isVisible(affect)) {
                            lis.forEach(function (item) {
                                dom[item == affect ? 'hide' : 'show'](item);
                            });
                        } else {
                            lis.forEach(function (item) {
                                dom[item == affect ? 'show' : 'hide'](item);
                            });
                        }
                    }

                });
            });

            return self;
        },
        ToggleFilter: function (filter) {
            var ulChartFilters = utils.$('ulChartFilters');
            var filterLis = utils.select('>li[data-filter]', ulChartFilters);
            var filterLisActive = utils.select('>li[data-filter].active', ulChartFilters);
            var hasChanges = false;
            if (filterLisActive.length == 0 || filterLisActive.length > 1 || filterLisActive[0].getAttribute('data-filter') != filter) {
                filterLis.forEach(function (item) {
                    var itemFilter = item.getAttribute('data-filter');
                    if (itemFilter == filter) {
                        dom[dom.hasClassName(item, 'active') ? 'removeClassName' : 'addClassName'](item, 'active');
                        hasChanges = true;
                    }
                });
            }
            if (hasChanges) {
                this.ReloadChart();
            }
            return this;
        },
        ToggleRange: function (range) {
            var self = this;
            var hasChanges = false;
            var dateItems = (typeof(range) == typeof('')) ? null : range.getAttribute('data-range-dates').split(',');
            if (dateItems && dateItems.length) {
                var from = utils.$(dateItems[0]), to = utils.$(dateItems[1]);
                from = from ? from.value : null;
                to = to ? to.value : null;
                function dateFromString(str) {
                    if (dgDatePicker) {
                        return new dgDatePicker().parseDate(str);
                    } else {
                        return new Date(str);
                    }
                }
                from = from ? dateFromString(from) : null;
                to = to ? dateFromString(to) : null;
                if (from && to) {
                    self._customRange = true;
                    self._customRangeFrom = from;
                    self._customRangeTo = to;
                    self._customRangeTo.setUTCDate(self._customRangeTo.getUTCDate() + 1);
                    self._customRangeTo.setUTCSeconds(self._customRangeTo.getUTCSeconds() - 1);
                    hasChanges = true;
                }
                else {
                    self._customRange = false;
                }
            }
            else {
                self._customRange = false;
                var ulChartRanges = utils.$('ulChartRanges');
                var rangeLis = utils.select('>li[data-range]', ulChartRanges);
                rangeLis.forEach(function (item) {
                    var itemRange = item.getAttribute('data-range');
                    if (itemRange == range) {
                        if (dom.hasClassName(item, 'active')) return;
                        dom.addClassName(item, 'active');
                        hasChanges = true;
                    } else {
                        dom.removeClassName(item, 'active');
                    }
                });
            }
            if (hasChanges) {
                this.ReloadChart();
            }
            return self;
        },
        ReloadChart: function () {
            var self = this;

            var ulChartFilters = utils.$('ulChartFilters');
            var ulChartRanges = utils.$('ulChartRanges');

            var requestParams = {};

            var filterLis = utils.select('>li[data-filter].active', ulChartFilters);
            filterLis.forEach(function (item) {
                var itemFilter = item.getAttribute('data-filter');
                requestParams[itemFilter] = 'yes';
            });

            var oneDay = 24 * 60 * 60 * 1000;
            var rangeTo = new Date();
            var rangeFrom = new Date(+rangeTo - oneDay);

            if (self._customRange) {

                rangeFrom = self._customRangeFrom;
                rangeTo = self._customRangeTo;

            }
            else {

                var rangeLis = utils.select('>li[data-range].active', ulChartRanges);
                rangeLis.forEach(function (item) {
                    var itemRange = item.getAttribute('data-range');

                    var matches = itemRange.match(/^([0-9.]+)([DWM])$/);
                    if (matches) {
                        if (matches[2] == 'D') {
                            rangeFrom = new Date(+rangeTo - oneDay * parseFloat(matches[1]));
                        } else if (matches[2] == 'W') {
                            rangeFrom = new Date(+rangeTo - oneDay * 7 * parseFloat(matches[1]));
                        } else if (matches[2] == 'M') {
                            rangeFrom = new Date(rangeTo);
                            rangeFrom.setUTCMonth(rangeFrom.getUTCMonth() - Math.floor(parseFloat(matches[1])))
                        }
                    } else {
                        // TODO
                    }
                });

            }

            if (rangeFrom) {
                requestParams['from'] = this.UtcDateFromDate(rangeFrom);
            }
            if (rangeTo) {
                requestParams['to'] = this.UtcDateFromDate(rangeTo);
            }

            requestParams['request-type'] = 'chart-data';
            requestParams['_cachestamp'] = +new Date;

            if (self._currentRequest) {
                self._currentRequest.abort();
            }

            var url = utils.ajax.obj2url(requestParams, window.location.href.replace(window.location.hash, '').replace('#', ''));
            self._currentRequest = utils.ajax.getUrl(url, function (json) {
                if (json) {
                    self._LoadedData = json;
                    if (self._GoogleChartsLoaded) self.LoadChartData();
                } else {
                    // Nothing to do
                }
                self._currentRequest = null;
            }, 'json');

            return self;
        },
        LoadChartData: function () {
            var self = this;

            var data = self._LoadedData;
            self._LoadedData = null;

            var types = [];
            var dataTable = new google.visualization.DataTable();
            dataTable.addColumn('date', '');
            if (data['Sales']) {
                types.push(data['Sales']);
                dataTable.addColumn('number', 'Sales');
            }
            if (data['Checkins']) {
                types.push(data['Checkins']);
                dataTable.addColumn('number', 'Checkins');
            }
            if (data['Revenue']) {
                types.push(data['Revenue']);
                dataTable.addColumn('number', 'Revenue');
            }
            if (data['ActiveUsers']) {
                types.push(data['ActiveUsers']);
                dataTable.addColumn('number', 'Active Users');
            }

            var minDate = null, maxDate = null;
            for (var j = 0, t, d; j < types.length; j++) {
                t = types[j];
                if (!t.Minimum) continue;
                d = new Date(t.Minimum.Year, t.Minimum.Month - 1, t.Minimum.Day);
                if (minDate == null || d < minDate) minDate = d;
                d = new Date(t.Maximum.Year, t.Maximum.Month - 1, t.Maximum.Day);
                if (maxDate == null || d > maxDate) maxDate = d;
            }

            var date = minDate;
            if (date) {
                var k, row;
                var oneDay = 24 * 60 * 60 * 1000;
                while (date <= maxDate) {
                    row = [];

                    row.push(new Date(date));

                    k = date.getFullYear() + ',' + (date.getMonth() + 1) + ',' + date.getDate();
                    for (var j = 0, t; j < types.length; j++) {
                        t = types[j].Data;
                        if (t[k] !== undefined) row.push(t[k]);
                        else row.push(0);
                    }

                    dataTable.addRow(row);

                    date.setTime(+date + oneDay);
                }
            }

            self._ChartData = dataTable;

            if (!self._Chart) {
                self._Chart = new google.visualization.AreaChart(document.getElementById('dvChart'));
                self._ChartReady = true;
                google.visualization.events.addListener(self._Chart, 'ready', function () { self.GoogleChartReady(); });
            }

            self.GoogleChartReady();

            return self;
        },
        GoogleChartReady: function () {
            var self = this;
            self._ChartReady = true;
            if (self._ChartData) {
                var chart = self._Chart;
                self._ChartReady = false;
                var dataTable = self._LastChartData = self._ChartData;
                self._ChartData = null;
                chart.draw(dataTable, { chartArea: { left: '5%', top: '5%', width: '90%', height: '75%' }, 'legend': { 'position': 'bottom'} });
            }
            return self;
        },
        GoogleChartResize: function () {
            var self = this;
            if (self._ChartReady && (self._ChartData || self._LastChartData)) {
                var chart = self._Chart;
                self._ChartReady = false;
                var dataTable = self._LastChartData = self._LastChartData || self._ChartData;
                self._ChartData = null;
                chart.draw(dataTable, { chartArea: { left: '5%', top: '5%', width: '90%', height: '75%' }, 'legend': { 'position': 'bottom'} });
            }
            return self;
        },
        GoogleChartsLoadedCallback: function () {
            var self = this;
            self._GoogleChartsLoaded = true;
            if (self._LoadedData) {
                self.LoadChartData();
            }
            return self;
        },
        UtcDateFromDate: (function () {
            function pad(number) {
                var r = String(number);
                if (r.length === 1) {
                    r = '0' + r;
                }
                return r;
            }

            return function (date) {
                return date.getUTCFullYear()
                    + '-' + pad(date.getUTCMonth() + 1)
                    + '-' + pad(date.getUTCDate())
                    + 'T' + pad(date.getUTCHours())
                    + ':' + pad(date.getUTCMinutes())
                    + ':' + pad(date.getUTCSeconds())
                    + 'Z';
            }
        })()
    };

    /** @expose **/
    window.Dashboard = Dashboard;

    utils.observe(document, 'dom:onLoad', function () {
        google.load("visualization", "1", { packages: ["corechart"], callback: function () { Dashboard.GoogleChartsLoadedCallback() } });
        Dashboard.InitControls().ReloadChart();

        var lastW = document.body.clientWidth;
        var timeout = null;
        utils.observe(window, 'resize', function () {
            if (timeout) clearTimeout(timeout);
            timeout = setTimeout(function () {
                if (document.body.clientWidth != lastW) {
                    lastW = document.body.clientWidth;
                    Dashboard.GoogleChartResize();
                }
            }, 150);
        });
    });

})();