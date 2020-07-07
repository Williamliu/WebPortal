var wliuRowModel = 'ng-model="db.tables[tb].GuidColumn(guid, col).value" ';
var wliuRowCommon = [
    'ng-change="db.tables[tb].Change(guid, col)" ',
    'ng-disabled="!db.tables[tb].GuidColumn(guid, col)" ',
    'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" '
].join('');
var wliuRowToolTip = [
    'tooltip-toggle="hover" ',
    'tooltip-body="{{db.tables[tb].GuidColumnErrorMsg(guid, col)}}" ',
    'tooltip-target="wliuToolTip" ',
    'tooltip-placement="down" '
].join('');
var wliuRowBounce = 'ng-model-options="{ updateOn:\'default blur\', debounce:{default: 500, blur:0} }" ';
var WLIU_NG = angular.module("wliu_database", []);
WLIU_NG.directive("wliu.label", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<label wliu ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
            '{{db.tables[tb].metas[col].title}}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.head", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<label wliu ',
                'for="{{tb}}-{{col}}-head" ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
                '{{ db.tables[tb].metas[col].title ? db.tables[tb].metas[col].title : col }} ',
                '<a wliu btn16 sort ',
                    'id="{{tb}}-{{col}}-head" ',
                    'ng-click="changeOrder(col)" ',
                    'ng-if="db.tables[tb].metas[col].order && db.tables[tb].metas[col].order!=\'\'" ',
                    'ng-attr="{\'sort-asc\': db.tables[tb].orderState(col, \'ASC\'), \'sort-desc\': db.tables[tb].orderState(col, \'DESC\') }" ',
                    'title = "Sort by {{db.tables[tb].metas[col].title}}"',
                '>',
                '</a>',
            '</label>'
        ].join(''),
        controller: function ($scope) {
            $scope.changeOrder = function (colName) {
                $scope.db.tables[$scope.tb].orderChange(colName).then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
        }
    };
});
WLIU_NG.directive("wliu.hicon", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:         "=",
            tb:         "@",
            icons:      "@",
            actionadd:  "&",
            actionsave: "&",
            actioncancel:"&"
        },
        template: [
            '<span>',
            '<a wliu btn24 add ',
                'ng-if="addState(db.tables[tb].state)" ',
                'ng-click="addAction()" ',
                'title="{{Words(\'button.new\')}}"',
            '></a>',
            '<a wliu btn24 cancel ',
                'ng-click="cancelAction()" ',
                'ng-if="saveState(db.tables[tb].state)" ',
                'title="{{Words(\'button.cancelall\')}}"',
            '></a>',
            '<a wliu btn24 save ',
                'ng-click="saveAction()" ',
                'ng-if="saveState(db.tables[tb].state)" ',
                'title="{{Words(\'button.saveall\')}}"',
            '></a>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.icons = $scope.icons || "";
            $scope.Words = $window.Words;
            $scope.addState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("add") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.add) flag = false;
                if (state === undefined || state===null) flag = false;
                if (state !== 0) flag = false;
                return flag;
            };
            $scope.saveState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("save") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.save) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state === 0) flag = false;
                return flag;
            };
            $scope.cancelState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("cancel") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.save) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state === 0) flag = false;
                return flag;
            };
            $scope.saveAction = function () {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.save) {
                    $scope.db.tables[$scope.tb].SaveAll()
                        .then(data => {
                            if ($scope.actionsave) if ($.isFunction($scope.actionsave)) $scope.actionsave();
                            $scope.$apply();
                        })
                        .catch(data => {
                            $scope.$apply();
                        });
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.save.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }

            };
            $scope.addAction = function () {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.add) {
                    $scope.db.tables[$scope.tb].NewRowB();
                    if ($scope.actionadd) if ($.isFunction($scope.actionadd)) $scope.actionadd();
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.add.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
            };
            $scope.cancelAction = function () {
                if ($scope.actioncancel) if ($.isFunction($scope.actioncancel)) $scope.actioncancel();
                $scope.db.tables[$scope.tb].CancelAll();
            };
        }
    };
});
WLIU_NG.directive("wliu.micon", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:         "=",
            tb:         "@",
            icons:      "@",
            actionadd:  "&",
            actionsave: "&"

        },
        template: [
            '<span>',
            '<a wliu btn24 add ',
                'ng-if="addState(db.tables[tb].state)" ',
                'ng-click="addAction()" ',
                'title="{{Words(\'button.new\')}}"',
            '></a>',
            '<a wliu btn24 save ',
                'ng-click="saveAction()" ',
                'ng-if="saveState(db.tables[tb].state)" ',
                'title="{{Words(\'button.saveall\')}}"',
            '></a>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.icons = $scope.icons || "";
            $scope.Words = $window.Words;
            $scope.addState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("add") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.add) flag = false;
                if (state === undefined || state === null) flag = false;
                return flag;
            };
            $scope.saveState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("save") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.save) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state === 0) flag = false;
                return flag;
            };
            $scope.saveAction = function () {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.save) {
                    $scope.db.tables[$scope.tb].SaveAll()
                        .then(data => {
                            if ($scope.actionsave) if ($.isFunction($scope.actionsave)) $scope.actionsave();
                            $scope.$apply();
                        })
                        .catch(data => {
                            $scope.$apply();
                        });
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.save.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }

            };
            $scope.addAction = function () {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.add) {
                    $scope.db.tables[$scope.tb].NewRowB();
                    if ($scope.actionadd) if ($.isFunction($scope.actionadd)) $scope.actionadd();
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.add.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
            };
        }
    };
});
WLIU_NG.directive("wliu.bicon", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:             "=",
            tb:             "@",
            guid:           "@",
            icons:          "@",
            actiondetail:   "&",
            actiondelete:   "&",
            actionsave:     "&",
            actioncancel:   "&"
        },
        template: [
            '<span wliu style="white-space:nowrap;">',
            '<a wliu btn24 detail ',
                'ng-click="detailAction(guid)" ',
                'ng-if="detailState(db.tables[tb].GuidRow(guid).state)" ',
                'title="{{Words(\'button.detail\')}}"',
            '></a>',
            '<a wliu btn24 delete ',
                'ng-click="deleteAction(guid)" ',
                'ng-if="deleteState(db.tables[tb].GuidRow(guid).state)" ',
                'title="{{Words(\'button.delete\')}}"',
            '></a>',
            '<a wliu btn24 cancel ',
                'ng-click="cancelAction(guid)" ',
                'ng-if="cancelState(db.tables[tb].GuidRow(guid).state)" ',
                'title="{{Words(\'button.cancel\')}}"',
            '></a>',
            '<a wliu btn24 save ',
                'ng-click="saveAction(guid)" ',
                'ng-if="saveState(db.tables[tb].GuidRow(guid).state)" ',
                'title="{{Words(\'button.save\')}}"',
            '></a>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.icons = $scope.icons || "";
            $scope.Words = $window.Words;
            $scope.detailState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("detail") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.detail) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state !== 0) flag = false;
                return flag;
            };
            $scope.deleteState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("delete") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.delete) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state !== 0) flag = false;
                return flag;
            };
            $scope.saveState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("save") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.save) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state === 0) flag = false;

                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.add) {
                    if (state === 2) flag = true;
                }
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.delete) {
                    if (state === 3) flag = true;
                }
                return flag;
            };
            $scope.cancelState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("cancel") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.save) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state === 0) flag = false;

                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.add) {
                    if (state === 2) flag = true;
                }
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.delete) {
                    if (state === 3) flag = true;
                }
                return flag;
            };
            $scope.detailAction = function (guid) {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.detail) {
                    $scope.db.tables[$scope.tb].rowGuid = guid;
                    if ($scope.actiondetail) if ($.isFunction($scope.actiondetail)) {
                        $scope.actiondetail();
                    }
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.detail.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
            };
            $scope.deleteAction = function (guid) {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.delete) {
                    if ($scope.db.tables[$scope.tb].GuidRow(guid)) {
                        switch ($scope.db.tables[$scope.tb].GuidRow(guid).state) {
                            case 0:
                            case 1:
                                $scope.db.tables[$scope.tb].Detach(guid);
                                break;
                            case 2:
                                $scope.db.tables[$scope.tb].Remove(guid);
                                break;
                            case 3:
                                break;
                        }
                        if ($scope.actiondelete) if ($.isFunction($scope.actiondelete)) $scope.actiondelete();
                    }
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.delete.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
               
            };
            $scope.saveAction = function (guid) {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.save) {
                    $scope.db.tables[$scope.tb].Save(guid)
                        .then(data => {
                            if ($scope.actionsave) if ($.isFunction($scope.actionsave)) $scope.actionsave();
                            $scope.$apply();
                        })
                        .catch(data => {
                            $scope.$apply();
                        });
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.save.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
            };
            $scope.cancelAction = function (guid) {
                if ($scope.db.tables[$scope.tb].GuidRow(guid)) {
                    switch ($scope.db.tables[$scope.tb].GuidRow(guid).state) {
                        case 0:
                            break;
                        case 1:
                        case 3:
                            $scope.db.tables[$scope.tb].Cancel(guid);
                            break;
                        case 2:
                            $scope.db.tables[$scope.tb].Remove(guid);
                            break;
                    }
                    if ($scope.actioncancel) if ($.isFunction($scope.actioncancel)) $scope.actioncancel();
                }
            };
        }
    };
});
WLIU_NG.directive("wliu.badd", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            show:   "=",
            arr:    "=",
            actionadd: "&"
        },
        template: [
            '<span wliu style="white-space:nowrap;">',
            '<a wliu btn24 add ',
                'ng-click="addAction(guid)" ',
                'ng-if="addState(guid)" ',
                'title="{{Words(\'button.new\')}}"',
            '></a>',
            '<a wliu btn24 okey ',
                'ng-if="okeyState(guid)" ',
                'title="{{Words(\'class.enrolled\')}}"',
            '></a>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.icons = $scope.icons || "";
            $scope.Words = $window.Words;
            $scope.addAction = function (guid) {
                $scope.db.tables[$scope.tb].rowGuid = guid;
                if ($scope.actionadd) if ($.isFunction($scope.actionadd)) $scope.actionadd();
            };

            $scope.addState = function (guid) {
                let flag = true;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.add) flag = false;
                if ($scope.show) {
                    if (!$.inList($scope.arr, $scope.db.tables[$scope.tb].GuidKey(guid)))
                        flag = true;
                    else
                        flag = false;

                    if ($scope.db.tables[$scope.tb].GuidState(guid) == null) flag = false;
                    if ($scope.db.tables[$scope.tb].GuidState(guid) == 1) flag = false;
                    if ($scope.db.tables[$scope.tb].GuidState(guid) == 2) flag = false;
                    if ($scope.db.tables[$scope.tb].GuidState(guid) == 3) flag = false;

                } else {
                    flag = false;
                }
                return flag;
            };
            $scope.test = function (guid) {
                return $.inList($scope.arr, $scope.db.tables[$scope.tb].GuidKey(guid));
            };
            $scope.okeyState = function (guid) {
                let flag = true;
                if ($scope.show)
                    if ($.inList($scope.arr, $scope.db.tables[$scope.tb].GuidKey(guid)))
                        flag = true;
                    else
                        flag = false;
                else
                    flag = false;

                return flag;
            };

        }
    };
});
WLIU_NG.directive("wliu.search", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:         "=",
            tb:         "@",
            label:      "@",
            action:     "&"
        },
        template: [
            '<a wliu button green ',
                'ng-click="searchAction()"',
            '>{{(label?label:Words("button.search"))}}</a>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.searchAction = function () {
                if ($scope.db.tables[$scope.tb]) {
                    $scope.db.tables[$scope.tb].Reload().then(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    }).catch(e => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    });
                }
            };
        }
    };
});
// only for filter type EInput.Image
WLIU_NG.directive("wliu.scan", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            label:  "@",
            action: "&"
        },
        template: [
            '<a wliu button mint ',
                'ng-click="searchAction()"',
            '>{{(label?label:Words("button.search"))}}</a>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.searchAction = function () {
                if ($scope.db.tables[$scope.tb]) {
                    $scope.db.tables[$scope.tb].Scan().then(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    });
                }
            };
        }
    };
});

WLIU_NG.directive("wliu.rowno", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@"
        },
        template: [
            '<span wliu>',
                '{{db.tables[tb].GuidIndex(guid) - 0 + 1}}',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.rowstate", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@"
        },
        template: [
            '<span wliu>',
            '<a wliu btn16 rowstate-save    ng-if="db.tables[tb].GuidRow(guid).error.code==0 && db.tables[tb].GuidRow(guid).state==1" title="Changed"></a>',
            '<a wliu btn16 rowstate-add     ng-if="db.tables[tb].GuidRow(guid).error.code==0 && db.tables[tb].GuidRow(guid).state==2" title="New"></a>',
            '<a wliu btn16 rowstate-delete  ng-if="db.tables[tb].GuidRow(guid).error.code==0 && db.tables[tb].GuidRow(guid).state==3" title="Deleted"></a>',
            '<a wliu btn16 rowstate-error   ng-if="db.tables[tb].GuidRow(guid).error.code!=0" ',
                'tooltip-toggle="hover" ',
                'tooltip-body="{{db.tables[tb].GuidRow(guid).error.Message().nl2br()}}" ',
                'tooltip-target="wliuToolTip" ',
                'tooltip-placement="down"',
            '></a>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.rowstatus", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: false,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@"
        },
        template: [
            '<span><span ng-if="db.tables[tb].GuidRow(guid).state>=0">',
                '<wliu.rowno    ng-if="db.tables[tb].GuidRow(guid).state==0" db="db" tb="{{tb}}" guid="{{guid}}"></wliu.rowno>',
                '<wliu.rowstate ng-if="db.tables[tb].GuidRow(guid).state!=0" db="db" tb="{{tb}}" guid="{{guid}}"></wliu.rowstate>',
            '</span></span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});

WLIU_NG.directive("wliu.text", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: '<span wliu text>{{db.tables[tb].GuidColumn(guid, col).value}}</span>',
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.template", function ($sce) {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: '<span wliu text ng-bind-html="getHTML(db.tables[tb].GuidRow(guid).columns)"></span>',
        controller: function ($scope, $sce) {
            $scope.getHTML = function (columns) {
                let colKeys = ('' + $scope.col).holderArray();
                let values = {};
                for (let i = 0; i < colKeys.length; i++) {
                    let colName = colKeys[i];
                    if (colName.indexOf(",") < 0) {
                        if (columns) if (columns[colName]) {
                            values[colName] = columns[colName].value;
                        }
                    }
                }
                return $sce.trustAsHtml(('' + $scope.col).replaceHolder(values));
            };
        }
    };
});
WLIU_NG.directive("wliu.textbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<input wliu fit type="text" ',
                wliuRowModel,
                wliuRowCommon,
                wliuRowBounce,
                wliuRowToolTip,
            '/>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.validate", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<input wliu fit type="text" ',
                'ng-blur="Validate(guid, col)" ',
                wliuRowModel,
                wliuRowCommon,
                wliuRowToolTip,
            '/>'
        ].join(''),
        controller: function ($scope) {
            $scope.Validate = function (guid, colName) {
                $scope.db.tables[$scope.tb].Validate(guid, colName).then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
        }
    };
});

WLIU_NG.directive("wliu.password", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<input wliu fit type="password" ',
                wliuRowModel,
                wliuRowCommon,
                wliuRowBounce,
                wliuRowToolTip,
            '/> '
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.confirm", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<input wliu fit type="password" ',
                'ng-model="db.tables[tb].GuidColumn(guid, col).value1" ',
                wliuRowCommon,
                wliuRowBounce,
                wliuRowToolTip,
            '/> '
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.textarea", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@",
            hh:     "@"
        },
        template: [
            '<textarea wliu fit style="height:{{hh}};" ',
                wliuRowModel,
                wliuRowCommon,
                wliuRowBounce,
                wliuRowToolTip,
            '></textarea>'
        ].join(''),
        controller: function ($scope) {
            $scope.hh = $scope.hh || "80px";
        },
        link: function (sc, el, attr) {
        }
    };
});
WLIU_NG.directive("wliu.hidden", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: '<input type="hidden" wliu fit ng-model="db.tables[tb].GuidColumn(guid, col).value" />',
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.bool", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<span wliu ',
                'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                wliuRowToolTip,
            '>',
                '<input wliu type="checkbox" id="{{db.tables[tb].GuidColumn(guid, col).guid}}" ',
                    wliuRowModel,
                    wliuRowCommon,
                '/>',
                '<label wliu checkbox ',
                    'for="{{db.tables[tb].GuidColumn(guid, col).guid}}"',
                '>',
                    '{{ db.tables[tb].GuidColumn(guid, col).value? db.tables[tb].metas[col].description.labelYes(): db.tables[tb].metas[col].description.labelNo() }}',
                '</label>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
//booltext,  请将 yes, no 的值放在 Meta description  "有效|无效"
WLIU_NG.directive("wliu.booltext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<label wliu ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
                '{{ db.tables[tb].GuidColumn(guid, col).value? db.tables[tb].metas[col].description.labelYes(): db.tables[tb].metas[col].description.labelNo() }}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<select wliu fit ',
                'ng-options="sObj.value as sObj.title for sObj in db.GetCollection(tb, col).items" ',
                wliuRowModel,
                wliuRowCommon,
                wliuRowToolTip,
            '>',
                '<option value=""></option>',
            '</select>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.fselect", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@",
            fcol:   "@"
        },
        template: [
            '<select wliu fit ',
                'ng-options="sObj.value as sObj.title for sObj in db.GetCollection(tb, col).items | filter: {\'refValue\': db.tables[tb].GuidColumn(guid, fcol).value }" ',
                wliuRowModel,
                wliuRowCommon,
                wliuRowToolTip,
            '>',
                '<option value=""></option>',
            '</select>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.stext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: '<label wliu>{{(db.GetCollection(tb, col).FindItem(db.tables[tb].GuidColumn(guid, col).value)?db.GetCollection(tb, col).FindItem(db.tables[tb].GuidColumn(guid, col).value).title:"")}}</label>',
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("wliu.radio", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@",
            colnum: "@"
        },
        template: [
            '<span wliu text ',
                'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                wliuRowToolTip,
            '>',
            '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(tb, col).items"></span>',
            '<input type="radio" wliu ',
                'id="{{db.tables[tb].GuidColumn(guid, col).guid + \'-rd-\' + rdObj.value}}" ',
                'ng-value="rdObj.value" ',
                wliuRowModel,
                wliuRowCommon,
            '/>',
            '<label wliu radio ',
                'for="{{db.tables[tb].GuidColumn(guid, col).guid + \'-rd-\' + rdObj.value}}" title="{{rdObj.title}}">',
                '{{rdObj.title}}',
            '</label><br ng-if="breakLine($index)">',
            '<span style="display:none;" ng-repeat-end></span>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.colnum = parseInt($scope.colnum || 0);
            $scope.breakLine = function (idx) {
                $scope.colnum = parseInt($scope.colnum ? $scope.colnum : 0);
                if ($scope.colnum > 0) {
                    if ((idx + 1) % $scope.colnum === 0)
                        return true;
                    else
                        return false;
                } else {
                    return false;
                }
            };
        }
    };
});
WLIU_NG.directive("wliu.checkbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@",
            colnum: "@"
        },
        template: [
            '<span wliu text ',
            'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                'ng-init="db.tables[tb].rows[db.tables[tb].GuidIndex(guid)].columns[col].value=db.tables[tb].rows[db.tables[tb].GuidIndex(guid)].columns[col].value || {}" ',
                 wliuRowToolTip,
            '>',
                '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(tb, col).items"></span>',
                    '<input type="checkbox" wliu ',
                        'id="{{db.tables[tb].GuidColumn(guid, col).guid + \'-ck-\' + rdObj.value}}" ',
                        'ng-model="db.tables[tb].GuidColumn(guid, col).value[rdObj.value]" ',
                        'ng-value="rdObj.value" ',
                        wliuRowCommon,
                    '/>',
                    '<label wliu checkbox ',
                        'for= "{{db.tables[tb].GuidColumn(guid, col).guid + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}" > ',
                        '{{rdObj.title}}',
                    '</label><br ng-if="breakLine($index)">',
                '<span style="display:none;" ng-repeat-end></span>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.colnum = parseInt($scope.colnum || 0);
            $scope.breakLine = function (idx) {
                $scope.colnum = parseInt($scope.colnum || 0);
                if ($scope.colnum > 0) {
                    if ((idx + 1) % $scope.colnum === 0)
                        return true;
                    else
                        return false;
                } else {
                    return false;
                }
            };
        }
    };
});
WLIU_NG.directive("wliu.radio1", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<span><input wliu radiolist fit readonly type="text" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                wliuRowToolTip,
                'value="{{valueText()}}" ',
                'title="{{valueText()}}" ',
                'ng-click="triggerDiag(db.tables[tb].metas[col].listRef.collection, tb, guid, col)" ',
                'diag-toggle="wliuDiagRadioList" ',
            '/></span>'
        ].join(''),
        controller: function ($scope) {
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };

            $scope.valueText = function () {
                var text = "";
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].metas)
                        if ($scope.db.tables[$scope.tb].metas[$scope.col])
                            if ($scope.db.GetCollection($scope.tb, $scope.col)) {
                                text = $.map($scope.db.GetCollection($scope.tb, $scope.col).items, function (n) {
                                    if ($scope.db.tables[$scope.tb].GuidColumn($scope.guid, $scope.col)) {
                                        if ($scope.db.tables[$scope.tb].GuidColumn($scope.guid, $scope.col).value===n.value)
                                            return n.title;
                                        else
                                            return null;
                                    } else {
                                        return null;
                                    }

                                }).join("; ");
                            }
                return text;
            }
        },
        link: function (sc, el, attr) {
            $("#wliuDiagRadioList").diag({});
        }
    };
});
WLIU_NG.directive("wliu.checkbox1", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<span><input wliu checklist fit readonly type="text" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                'ng-init="db.tables[tb].rows[db.tables[tb].GuidIndex(guid)].columns[col].value=db.tables[tb].rows[db.tables[tb].GuidIndex(guid)].columns[col].value || {}" ',
                 wliuRowToolTip,
                'value="{{valueText()}}" ',
                'title="{{valueText()}}" ',
                'ng-click="triggerDiag(db.tables[tb].metas[col].listRef.collection, tb, guid, col)" ',
                'diag-toggle="wliuDiagCheckboxList" ',
            '/></span>'
        ].join(''),
        controller: function ($scope) {
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };
            $scope.valueText = function () {
                var text = "";
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].metas)
                        if ($scope.db.tables[$scope.tb].metas[$scope.col])
                            if ($scope.db.GetCollection($scope.tb, $scope.col)) {
                                text = $.map($scope.db.GetCollection($scope.tb, $scope.col).items, function (n) {
                                    if ($scope.db.tables[$scope.tb].GuidColumn($scope.guid, $scope.col)) {
                                        if ($scope.db.tables[$scope.tb].GuidColumn($scope.guid, $scope.col).value[n.value])
                                            return n.title;
                                        else
                                            return null;
                                    } else {
                                        return null;
                                    }

                                }).join("; ");
                            }
                return text;
            };
        },
        link: function (sc, el, attr) {
            $("#wliuDiagCheckboxList").diag({});
        }
    };
});
WLIU_NG.directive("wliu.navi", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@"
        },
        template: [
            '<div>',
                '<div ng-if="db.tables[tb].navi.isActive" style="display:block; height:40px; line-height:40px;">',

                    '<div style="float:left;">',
                        '<span style="vertical-align:middle;">{{Words("navi.page")}}: </span>',
                        '<input type="text" wliu style="text-align:center; width:60px; vertical-align:middle;" ng-keypress="keypress($event)" ng-model="db.tables[tb].navi.pageNo" />',
                        '<span style="font-size:1.5em; vertical-align:middle;"> / </span>',
                        '<span style="font-size:1.2em; vertical-align:middle; font-weight:600;">{{db.tables[tb].navi.pageTotal}}</span>',

                        '<span style="font-size:1.2em; vertical-align:middle;" > | </span >', 
                        '<a wliu btn24 first    ng-click="firstPage()"    ng-attr="{\'disabled\': db.tables[tb].firstState()}"      title="{{Words(\'navi.first\')}}"></a>',
                        '<a wliu btn24 previous ng-click="previousPage()" ng-attr="{\'disabled\': db.tables[tb].previousState()}"   title="{{Words(\'navi.previous\')}}"></a>',
                        '<a wliu btn24 next     ng-click="nextPage()"     ng-attr="{\'disabled\': db.tables[tb].nextState()}"       title="{{Words(\'navi.next\')}}"></a>',
                        '<a wliu btn24 last     ng-click="lastPage()"     ng-attr="{\'disabled\': db.tables[tb].lastState()}"       title="{{Words(\'navi.last\')}}"></a>',
                        '<span style="font-size:1.2em; vertical-align:middle;" > | </span >', 

                        '<a wliu btn24 reload ng-click="reloadPage()" title="{{Words(\'navi.reload\')}}" ng-hide="db.tables[tb].navi.isLoading==1"></a>',
                        '<i class="fa fa-spinner fa-lg fa-pulse fa-fw" aria-hidden="true" ng-show="db.tables[tb].navi.isLoading==1"></i>',

                        '<span style="font-size:1.2em; vertical-align:middle;" > | </span >', 
                        '<span style="vertical-align:middle;">{{Words("navi.total")}}: </span>',
                        '<span style="font-size:1.2em; vertical-align:middle; font-weight:600;">{{db.tables[tb].navi.rowTotal}}</span>',
                    '</div>',

                    '<div style="float:right;">',
                        '<span style="vertical-align:middle;">{{Words("navi.size")}}: </span>',
                        '<input type="text" wliu style="text-align:center; width:40px; vertical-align:middle;" ng-model="db.tables[tb].navi.pageSize" ng-keypress="keypress($event)" />',
                        '<a wliu btn24 reload ng-click="reloadPage()" title="{{Words(\'navi.reload\')}}" ng-hide="db.tables[tb].navi.isLoading==1"></a>',
                        '<i class="fa fa-spinner fa-lg fa-pulse fa-fw" aria-hidden="true" ng-show="db.tables[tb].navi.isLoading==1"></i>',
                    '<div>',

                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.firstPage = function () {
                $scope.db.tables[$scope.tb].firstPage().then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
            $scope.previousPage = function () {
                $scope.db.tables[$scope.tb].previousPage().then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
            $scope.nextPage = function () {
                $scope.db.tables[$scope.tb].nextPage().then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
            $scope.lastPage = function () {
                $scope.db.tables[$scope.tb].lastPage().then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
            $scope.reloadPage = function () {
                $scope.db.tables[$scope.tb].Reload().then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
            $scope.keypress = function (ev) {
                if (ev.keyCode === 13) {
                    $scope.db.tables[$scope.tb].Reload().then(data => {
                        $scope.$apply();
                    }).catch(data => {
                        $scope.$apply();
                    });
                }
            };
        }
    };
});

WLIU_NG.directive("wliu.intdate", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@",
            format: "@"
        },
        template: [
            '<label wliu ',
                'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
            '{{ db.tables[tb].GuidColumn(guid, col).value?(db.tables[tb].GuidColumn(guid, col).value>0?(db.tables[tb].GuidColumn(guid, col).value * 1000 | date : (format?format:"yyyy-MM-dd HH:mm") ):"") :"" }}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    }
});
WLIU_NG.directive("wliu.imgurl", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@",
            ww:     "@",
            hh:     "@",
            view:   "@",
            edit:   "@"
        },
        template: [
                '<div style="display:block;width:{{ww}};height:{{hh}}">',
                    '<img wliu ',
                        'rowkey="{{db.tables[tb].rows[row].key}}" ',
                        'class="img-responsive" style="cursor:pointer;width:100%;height:100%;border-radius:6px;" ',
                        'src="{{(db.tables[tb].GuidColumn(guid, col).value==\'\'?\'\':db.tables[tb].GuidColumn(guid, col).value+\'/\'+size)}}" ',
                    '/>',
                '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.ww = $scope.ww || "100%"; 
            $scope.hh = $scope.hh || "auto"; 
            $scope.view = $scope.view || "small";
            $scope.edit = $scope.edit || "large";
        },
        link: function (sc, el, attr) {
            $('img[wliu][rowkey]', el).off("click").on("click", function (ev) {
                let url = sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value;
                $("#img_viewer", "#wliuDiagImageViewer").attr("src", url + '/' + sc.edit + '?ts=' + CreateUUID());
            });
        }
    };
});
WLIU_NG.directive("wliu.imgcontent", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@",
            rowkey: "@",
            ww:     "@",
            hh:     "@",
            view:   "@",
            edit:   "@"
        },
        template: [
            '<div style="display:block;width:{{ww}};height:{{hh}}">',
            '<img wliu ',
                'rowkey="{{db.tables[tb].GuidColumn(guid, rowkey).value}}" ',
                'class="img-responsive" style="cursor:pointer;width:100%;height:100%;border-radius:6px;" ',
                'src="{{db.tables[tb].GuidColumn(guid, col).value}}" ',
            '/>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.ww = $scope.ww || "100%";
            $scope.hh = $scope.hh || "auto";
            $scope.view = $scope.view || "small";
            $scope.edit = $scope.edit || "large";
            $scope.rowkey = $scope.rowkey ? $scope.rowkey : $scope.col; 
        },
        link: function (sc, el, attr) {
            $('img[wliu][rowkey]', el).off("click").on("click", function (ev) {
                let url = sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value1;
                $("#img_viewer", "#wliuDiagImageViewer").attr("src", url + '/' + sc.edit + '?ts=' + CreateUUID());
            });
        }
    };
});
WLIU_NG.directive("table.error", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            label: "@"
        },
        template: [
            '<div id="wliuDiagTableError" wliu diag maskable movable diag-toggle="wliuDiagTableError">',
            '<div head>',
            '<a warning></a>',
            '{{(label?label:Words("error.message"))}}',
            '</div>',
            '<div body style="min-height:60px;">',
            '</div>',
            '<center><a wliu button blue diag-toggle="wliuDiagTableError">CLOSE</a></center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        },
        link: function (sc, el, attr) {
        }
    };
});
WLIU_NG.directive("form.error", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            label: "@"
        },
        template: [
            '<div id="wliuDiagFormError" wliu diag maskable movable diag-toggle="wliuDiagFormError">',
            '<div head>',
            '<a warning></a>',
            '{{(label?label:Words("error.message"))}}',
            '</div>',
            '<div body style="min-height:60px;">',
            '</div>',
            '<center><a wliu button blue diag-toggle="wliuDiagFormError">CLOSE</a></center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        }
    };
});
WLIU_NG.directive("wliu.loading", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            oper:     "@"
        },
        template: [
            '<div wliu loading method="{{oper}}"></div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        },
        link: function (sc, el, attr) {
            $(function () {
                $("div[wliu][loading][method]").loading({ text: sc.Words("please.wait") });
            });
        }
    };
});
WLIU_NG.directive("wliu.tooltip", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {},
        template: [
            '<div wliu-tooltip tooltip-target="wliuToolTip"></div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        }
    };
});
WLIU_NG.directive("wliu.hint", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {},
        template: [
            '<div id="wliuHint" wliu hint></div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        }
    };
});
WLIU_NG.directive("wliu.date", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<input type="text" wliu date-picker ',
                wliuRowModel,
                wliuRowCommon,
                wliuRowBounce,
                wliuRowToolTip,
            '/>'
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                let today = new Date();
                $(el).pickadate({
                    format: "yyyy-mm-dd",
                    formatSubmit: "yyyy-mm-dd",
                    closeOnSelect: true,
                    disable: [{ from: [2016, 9, 1], to: [2016, 9, 10] }, [2016, 10, 5]],
                    //min: new Date(2015,3,20),
                    //max: new Date(2016,11,14),
                    selectYears: 100,
                    min: new Date(today.getFullYear() - 90, 1, 1),
                    max: new Date(today.getFullYear() + 10, 12, 31),
                    onSet: function (dobj) {
                        if (dobj.select) {
                            let dt = new Date(dobj.select);
                            sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value = dt.format("Y-m-d");
                        } else {
                            sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value = null;
                        }
                        sc.db.tables[sc.tb].SetChange(sc.guid, sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});
WLIU_NG.directive("wliu.time", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<input type="text" wliu time-picker ',
                wliuRowModel,
                wliuRowCommon,
                wliuRowBounce,
                wliuRowToolTip,
            '/>'
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                $(el).pickatime({
                    twelvehour: false,
                    afterDone: function (tobj) {
                        if(tobj)
                            sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value = tobj;
                        else 
                            sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value = null;
                        sc.db.tables[sc.tb].SetChange(sc.guid, sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});
WLIU_NG.directive("wliu.datetime", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@",
            h1:     "@",
            h2:     "@"
        },
        template: [
            '<span ',
                'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                wliuRowToolTip,
            '>',
            '<input type="text" ng-attr="{\'h1\':h1, \'h2\':h2}" wliu date-picker ',
                'ng-model="db.tables[tb].GuidColumn(guid, col).value" ',
                'ng-click="db.tables[tb].SetChange(guid, col)" ',
                'ng-disabled="!db.tables[tb].GuidColumn(guid, col)" ',
            '/>',
            '<input type="text" ng-attr="{\'h1\':h1, \'h2\':h2}" wliu time-picker ',
                'ng-model="db.tables[tb].GuidColumn(guid, col).value1" ',
                'ng-click="db.tables[tb].SetChange(guid, col)" ',
                'ng-disabled="!db.tables[tb].GuidColumn(guid, col)" ',
            '/>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.h1 = $scope.h1 || 0;
            $scope.h2 = $scope.h2 || 0;
        },
        link: function (sc, el, attr) {
            $(function () {
                let today = new Date();
                $("input[wliu][date-picker]", el).pickadate({
                    format: "yyyy-mm-dd",
                    formatSubmit: "yyyy-mm-dd",
                    closeOnSelect: true,
                    disable: [{ from: [2016, 9, 1], to: [2016, 9, 10] }, [2016, 10, 5]],
                    //min: new Date(2015,3,20),
                    //max: new Date(2016,11,14),
                    selectYears: 100,
                    min: new Date(today.getFullYear() - 90, 1, 1),
                    max: new Date(today.getFullYear() + 10, 12, 31),
                    onSet: function (dobj) {
                        if (dobj.select) {
                            let dt = new Date(dobj.select);
                            sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value = dt.format("Y-m-d");
                        }
                        else
                        {
                            sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value = null;
                        }
                        sc.db.tables[sc.tb].SetChange(sc.guid, sc.col);
                        sc.$apply();
                    }
                });

                $("input[wliu][time-picker]", el).pickatime({
                    twelvehour: false,
                    afterDone: function (tobj) {
                        if(tobj)
                            sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value1 = tobj;
                        else 
                            sc.db.tables[sc.tb].GuidColumn(sc.guid, sc.col).value1 = null;

                        sc.db.tables[sc.tb].SetChange(sc.guid, sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});


/*** Current ***/
var formRowModel = 'ng-model="db.tables[tb].CurrentColumn(col).value" ';
var formRowCommon = [
    'ng-change="db.tables[tb].Change(db.tables[tb].CurrentGuid(), col)" ',
    'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
    'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" '
].join('');
var formRowToolTip = [
    'tooltip-toggle="hover" ',
    'tooltip-body="{{db.tables[tb].CurrentColumnErrorMsg(col)}}" ',
    'tooltip-target="wliuToolTip" ',
    'tooltip-placement="down" '
].join('');
var formRowBounce = 'ng-model-options="{ updateOn:\'default blur\', debounce:{default: 500, blur:0} }" ';


WLIU_NG.directive("form.label", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<label wliu ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
                '{{db.tables[tb].metas[col].title}}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("confirm.label", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: '<label wliu ng-attr="{\'need\': db.tables[tb].metas[col].required}" title="{{db.tables[tb].metas[col].description?db.tables[tb].metas[col].description:col}}">{{db.tables[tb].metas[col].description}}</label>',
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.rowno", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@"
        },
        template: [
            '<label wliu>',
            '{{ (db.tables[tb].CurrentIndex() - 0 + 1) }}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.rowstate", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@"
        },
        template: [
            '<span wliu>',
            '<a wliu btn16 rowstate-save ng-if="db.tables[tb].CurrentRow().error.code==0 && db.tables[tb].CurrentRow().state==1" title="Changed"></a>',
            '<a wliu btn16 rowstate-add ng-if="db.tables[tb].CurrentRow().error.code==0 && db.tables[tb].CurrentRow().state==2" title="New"></a>',
            '<a wliu btn16 rowstate-delete ng-if="db.tables[tb].CurrentRow().error.code==0 && db.tables[tb].CurrentRow().state==3" title="Deleted"></a>',
            '<a wliu btn16 rowstate-error ng-if="db.tables[tb].CurrentRow().error.code!=0" ',
                'tooltip-toggle="hover" ',
                'tooltip-body="{{db.tables[tb].CurrentRow().error.Message().nl2br()}}" ',
                'tooltip-target="wliuToolTip" ',
                'tooltip-placement="down" ',
            '></a>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.rowstatus", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: false,
        scope: {
            db:     "=",
            tb:     "@"
        },
        template: [
            '<span wliu>',
            '<form.rowno ng-if="db.tables[tb].CurrentRow().state==0" db="db" tb="{{tb}}"></form.rowno>',
            '<form.rowstate  ng-if="db.tables[tb].CurrentRow().state!=0" db="db" tb="{{tb}}"></form.rowstate>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.status", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: false,
        scope: {
            db:     "=",
            tb:     "@"
        },
        template: [
            '<span>',
            '<label wliu>{{Words("col.status")}}</label> : ',
            '<label wliu ng-if="db.tables[tb].CurrentRow().state==0" style="color:#ff4400;font-weight:bold;">{{Words("status.normal")}}</label>',
            '<label wliu ng-if="db.tables[tb].CurrentRow().state==1" style="color:#ff4400;font-weight:bold;">{{Words("status.changed")}}</label>',
            '<label wliu ng-if="db.tables[tb].CurrentRow().state==2" style="color:#ff4400;font-weight:bold;">{{Words("status.add")}}</label>',
            '<label wliu ng-if="db.tables[tb].CurrentRow().state==3" style="color:#ff0000;font-weight:bold;">{{Words("status.delete")}}</label>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        }
    };
});
WLIU_NG.directive("form.text", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: '<span wliu>{{db.tables[tb].CurrentColumn(col).value}}</span>',
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.textbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            action: "&"
        },
        template: [
            '<input wliu fit type="text" ',
                'ng-keypress="keypress($event)" ',
                formRowModel,
                formRowCommon,
                //formRowBounce,
                formRowToolTip,
            '/>'
        ].join(''),
        controller: function ($scope) {
            $scope.keypress = function (ev) {
                if($scope.action) if ($.isFunction($scope.action)) {
                    if (ev.keyCode === 13) {
                        $scope.db.tables[$scope.tb].Save($scope.db.tables[$scope.tb].CurrentGuid())
                            .then(data => {
                                $scope.action();
                                $scope.$apply();
                            })
                            .catch(data => {
                                $scope.$apply();
                            });
                    }
                }
            };
        }
    };
});
WLIU_NG.directive("form.validate", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<input wliu fit type="text" ',
                'ng-blur="Validate(db.tables[tb].CurrentGuid(), col)" ',
                formRowModel,
                formRowCommon,
                formRowToolTip,
            '/>'
        ].join(''),
        controller: function ($scope) {
            $scope.Validate = function (guid, colName) {
                $scope.db.tables[$scope.tb].Validate(guid, colName).then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
        }
    };
});
WLIU_NG.directive("form.password", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            action: "&"
        },
        template: [
            '<input wliu fit type="password" ',
                'ng-keypress="keypress($event)" ',
                formRowModel,
                formRowCommon,
                //formRowBounce,
                formRowToolTip,
            '/> '
        ].join(''),
        controller: function ($scope) {
            $scope.keypress = function (ev) {
                if ($scope.action) if ($.isFunction($scope.action)) {
                    if (ev.keyCode === 13) {
                        $scope.db.tables[$scope.tb].Save($scope.db.tables[$scope.tb].CurrentGuid())
                            .then(data => {
                                $scope.action();
                                $scope.$apply();
                            })
                            .catch(data => {
                                $scope.$apply();
                            });
                    }
                }
            };
        }
    };
});
WLIU_NG.directive("form.confirm", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@"
        },
        template: [
            '<input wliu fit type="password" ',
                'ng-model="db.tables[tb].CurrentColumn(col).value1" ',
                formRowCommon,
                formRowBounce,
                formRowToolTip,
            '/> '
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.textarea", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            hh:     "@"
        },
        template: [
            '<textarea wliu fit style="height:{{hh}};" ',
                formRowModel,
                formRowCommon,
                formRowBounce,
                formRowToolTip,
            '></textarea>'
        ].join(''),
        controller: function ($scope) {
            $scope.hh = $scope.hh || "80px";
        }
    };
});
WLIU_NG.directive("form.hidden", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: '<input type="hidden" wliu fit ng-model="db.tables[tb].CurrentColumn(col).value" />',
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.bool", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<span wliu ',
                'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                formRowToolTip,
            '>',
            '<input wliu type="checkbox" id="{{db.tables[tb].CurrentColumn(col).guid}}" ',
                formRowModel,
                formRowCommon,
            '/>',
            '<label wliu checkbox ',
                'for="{{db.tables[tb].CurrentColumn(col).guid}}"',
            '>',
                '{{ db.tables[tb].CurrentColumn(col).value? db.tables[tb].metas[col].description.labelYes():db.tables[tb].metas[col].description.labelNo() }}',
            '</label>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.booltext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<label wliu ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
                '{{ db.tables[tb].CurrentColumn(col).value? db.tables[tb].metas[col].description.labelYes():db.tables[tb].metas[col].description.labelNo() }}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<select wliu fit ',
                'ng-options="sObj.value as sObj.title for sObj in db.GetCollection(tb, col).items" ',
                formRowModel,
                formRowCommon,
                formRowToolTip,
            '>',
            '<option value=""></option>',
            '</select>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.fselect", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            fcol:   "@"
        },
        template: [
            '<select wliu fit ',
                'ng-options="sObj.value as sObj.title for sObj in db.GetCollection(tb, col).items | filter: {\'refValue\': db.tables[tb].CurrentColumn(fcol).value }" ',
                formRowModel,
                formRowCommon,
                formRowToolTip,
            '>',
            '<option value=""></option>',
            '</select>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.stext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: '<label wliu>{{(db.GetCollection(tb, col).FindItem(db.tables[tb].CurrentColumn(col).value)?db.GetCollection(tb, col).FindItem(db.tables[tb].CurrentColumn(col).value).title:"")}}</label>',
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.radio", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<span wliu text ',
                'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                formRowToolTip,
            '>',
            '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(tb, col).items"></span>',
            '<input type="radio" wliu ',
                'id="{{db.tables[tb].CurrentColumn(col).guid + \'-rd-\' + rdObj.value}}" ',
                'ng-value="rdObj.value" ',
                formRowModel,
                formRowCommon,
            '/>',
            '<label wliu radio ',
                'for="{{db.tables[tb].CurrentColumn(col).guid + \'-rd-\' + rdObj.value}}" title="{{rdObj.title}}">',
                '{{rdObj.title}}',
            '</label>',
            '<span style="display:none;" ng-repeat-end></span>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.breakLine = function (idx) {
                $scope.colnum = parseInt($scope.colnum ? $scope.colnum : 0);
                if ($scope.colnum > 0) {
                    if ((idx + 1) % $scope.colnum === 0)
                        return true;
                    else
                        return false;
                } else {
                    return false;
                }
            };
        }
    };
});
WLIU_NG.directive("form.checkbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<span wliu text ',
                'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                'ng-init="db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col].value=db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col].value || {}" ',
                formRowToolTip,
            '>',
                '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(tb, col).items"></span>',
                    '<input type="checkbox" wliu ',
                        'id="{{db.tables[tb].CurrentColumn(col).guid + \'-ck-\' + rdObj.value}}" ',
                        'ng-model="db.tables[tb].CurrentColumn(col).value[rdObj.value]" ',
                        'ng-value="rdObj.value" ',
                        formRowCommon,
                    '/>',
                    '<label wliu checkbox ',
                        'for="{{db.tables[tb].CurrentColumn(col).guid + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}">',
                        '{{rdObj.title}}',
                    '</label>',
                '<span style="display:none;" ng-repeat-end></span>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.breakLine = function (idx) {
                $scope.colnum = parseInt($scope.colnum ? $scope.colnum : 0);
                if ($scope.colnum > 0) {
                    if ((idx + 1) % $scope.colnum === 0)
                        return true;
                    else
                        return false;
                } else {
                    return false;
                }
            };
        }
    };
});
WLIU_NG.directive("form.checkcom", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            icon:   "@"
        },
        template: [
            '<div>',
                '<table><tr><td style="white-space:nowrap;" valign="top">',
                    '<form.label db="db" tb="{{tb}}" col="{{col}}" style="vertical-align:middle;"></form.label>: ',
                    '<a wliu btn16 detail ',
                        'ng-if="icon" ',
                        'ng-click="triggerDiag(db.tables[tb].metas[col].listRef.collection, tb, db.tables[tb].CurrentGuid(), col)" ',
                        'diag-toggle="wliuDiagCheckComList"',
                    '></a>',
                '</td><td valign="top">',
                '<ul wliu checkcom>',
                    '<li ng-repeat="rdObj1 in valueItems()">',
                        '{{rdObj1.title}} ',
                            '<a wliu btn16 close disabled ',
                            'ng-click="db.tables[tb].CurrentColumn(col).value[rdObj1.value]=false; db.tables[tb].Change(db.tables[tb].CurrentGuid(), col);"></a>',
                    '</li>',
                '</ul>',
                '</td></tr></table>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.db.tables[$scope.tb].CurrentColumn($scope.col).value = $scope.db.tables[$scope.tb].CurrentColumn($scope.col).value || {}; 
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };

            $scope.valueItems = function () {
                var selectItem = [];
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].metas)
                        if ($scope.db.tables[$scope.tb].metas[$scope.col])
                            if ($scope.db.GetCollection($scope.tb, $scope.col)) {
                                selectItem = $.grep($scope.db.GetCollection($scope.tb, $scope.col).items, function (n) {
                                    if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col)) {
                                        if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value[n.value])
                                            return true;
                                        else
                                            return false;
                                    } else {
                                        return false;
                                    }

                                });
                            }
                return selectItem;
            }
        }
    };
});
WLIU_NG.directive("form.checklist", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            maxww:  "@",
            maxhh:  "@"
        },
        template: [
            '<div wliu checklist style="max-width:{{maxww}};max-height:{{maxhh}};" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                'ng-init="db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col].value=db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col].value || {}" ',
                formRowToolTip,
            '>',
                '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(tb, col).items"></span>',
                    '<input type="checkbox" wliu ',
                        'id="{{db.tables[tb].CurrentColumn(col).guid + \'-ck-\' + rdObj.value}}" ',
                        'ng-model="db.tables[tb].CurrentColumn(col).value[rdObj.value]" ',
                        'ng-value="rdObj.value" ',
                        formRowCommon,
                    '/>',
                    '<label wliu checkbox ',
                        'for="{{db.tables[tb].CurrentColumn(col).guid + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}"',
                    '>',
                        '{{rdObj.title}}',
                    '</label><br>',
                '<span style="display:none;" ng-repeat-end></span>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.db.tables[$scope.tb].CurrentColumn($scope.col).value = $scope.db.tables[$scope.tb].CurrentColumn($scope.col).value || {}; 
            $scope.maxww = $scope.maxww || "auto";
            $scope.maxhh = $scope.maxhh || "auto";
        }
    };
});
WLIU_NG.directive("form.radio1", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<input wliu radiolist fit readonly type="text" ',
                'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                 formRowToolTip,
                'value="{{valueText()}}" ',
                'title="{{valueText()}}" ',
                'ng-click="triggerDiag(db.tables[tb].metas[col].listRef.collection, tb, db.tables[tb].CurrentGuid(), col)" ',
                'diag-toggle="wliuDiagRadioList" ',
            '/>'
        ].join(''),
        controller: function ($scope) {
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };

            $scope.valueText = function () {
                var text = "";
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].metas)
                        if ($scope.db.tables[$scope.tb].metas[$scope.col])
                            if ($scope.db.GetCollection($scope.tb, $scope.col)) {
                                text = $.map($scope.db.GetCollection($scope.tb, $scope.col).items, function (n) {
                                    if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col)) {
                                        if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value===n.value)
                                            return n.title;
                                        else
                                            return null;
                                    } else {
                                        return null;
                                    }

                                }).join("; ");
                            }
                return text;
            }
        }
    };
});
WLIU_NG.directive("form.checkbox1", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<input wliu checklist fit readonly type="text" ',
                'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                'ng-init="db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col].value=db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col].value || {}" ',
                formRowToolTip,
                'value="{{valueText()}}" ',
                'title="{{valueText()}}" ',
                'ng-click="triggerDiag(db.tables[tb].metas[col].listRef.collection, tb, db.tables[tb].CurrentGuid(), col)" ',   
                'diag-toggle="wliuDiagCheckboxList" ',
            '/>'
        ].join(''),
        controller: function ($scope) {
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };

            $scope.valueText = function () {
                var text = "";
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].metas)
                        if ($scope.db.tables[$scope.tb].metas[$scope.col])
                            if ($scope.db.GetCollection($scope.tb, $scope.col)) {
                                text = $.map($scope.db.GetCollection($scope.tb, $scope.col).items, function (n) {
                                    if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col)) {
                                        if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value[n.value])
                                            return n.title;
                                        else
                                            return null;
                                    } else {
                                        return null;
                                    }

                                }).join("; ");
                            }
                return text;
            }
        }
    };
});
WLIU_NG.directive("form.save", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            label:  "@",
            action: "&"
        },
        template: [
            '<a wliu button blue ',
                'ng-click="saveAction(db.tables[tb].CurrentGuid())" ',
            '>{{label}}</a>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.saveAction = function (guid) {
                if ($scope.db.tables[$scope.tb].CurrentRow()) {
                    switch ($scope.db.tables[$scope.tb].CurrentRow().state) {
                        case 0:
                        case 1:
                            if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.save) {
                                $scope.db.tables[$scope.tb].Save(guid)
                                    .then(data => {
                                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                                        $scope.$apply();
                                    })
                                    .catch(data => {
                                        $scope.$apply();
                                    });
                            }
                            else
                            {
                                $scope.db.tables[$scope.tb].error.Clear();
                                $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.save.na"));
                                $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                            }
                            break;
                        case 2:
                            if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.add) {
                                $scope.db.tables[$scope.tb].Save(guid)
                                    .then(data => {
                                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                                        $scope.$apply();
                                    })
                                    .catch(data => {
                                        $scope.$apply();
                                    });
                            } else {
                                $scope.db.tables[$scope.tb].error.Clear();
                                $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.add.na"));
                                $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                            }
                            break;
                        case 3:
                            if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.delete) {
                                $scope.db.tables[$scope.tb].Save(guid)
                                    .then(data => {
                                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                                        $scope.$apply();
                                    })
                                    .catch(data => {
                                        $scope.$apply();
                                    });
                            }
                            else {
                                $scope.db.tables[$scope.tb].error.Clear();
                                $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.delete.na"));
                                $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                            }
                            break;
                    }
                }
            };
        }
    };
});
WLIU_NG.directive("form.add", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            label:  "@",
            action: "&"
        },
        template: [
            '<a wliu button green ',
                'ng-click="addAction(db.tables[tb].CurrentGuid())" ',
            '>{{label}}</a>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.addAction = function (guid) {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.add) {
                    $scope.db.tables[$scope.tb].Empty(guid);
                    if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.add.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
            };
        }
    };
});
WLIU_NG.directive("form.cancel", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            label:  "@",
            action: "&"
        },
        template: [
            '<a wliu button orange ',
            'ng-click="cancelAction(db.tables[tb].CurrentGuid())"',
            '>',
                '{{label}}',
            '</a>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.cancelAction = function (guid) {
                if ($scope.db.tables[$scope.tb].GuidRow(guid)) {
                    switch ($scope.db.tables[$scope.tb].GuidRow(guid).state) {
                        case 0:
                            break;
                        case 1:
                        case 3:
                            $scope.db.tables[$scope.tb].Cancel(guid);
                            break;
                        case 2:
                            $scope.db.tables[$scope.tb].Cancel(guid);
                            break;
                    }
                    //if ($scope.db.tables[$scope.tb].rows.length <= 0) $scope.db.tables[$scope.tb].NewRowB();

                    if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                }
            };
        }
    };
});
WLIU_NG.directive("form.intdate", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@",
            format: "@"
        },
        template: [
            '<label wliu ',
            'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
            '{{ db.tables[tb].CurrentColumn(col).value?(db.tables[tb].CurrentColumn(col).value>0?(db.tables[tb].CurrentColumn(col).value * 1000 | date : (format?format:"yyyy-MM-dd HH:mm") ):"") :"" }}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("form.ymd", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<span ng-init="db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col+\'YY\'].value=db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col+\'YY\'].value<=0?\'\':db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col+\'YY\'].value">',
            '<form.textbox db="db" tb="{{tb}}" col="{{col+\'YY\'}}" style="width:60px; text-align:center;" placeholder="{{Words(\'year\')}}" /></form.textbox> - ',
            '<form.select db="db" tb="{{tb}}"  col="{{col+\'MM\'}}" style="width:60px;" placeholder="{{Words(\'month\')}}" /></form.select> - ',
            '<form.select db="db" tb="{{tb}}"  col="{{col+\'DD\'}}" style="width:60px;" placeholder="{{Words(\'day\')}}" /></form.select>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        }
    };
});
WLIU_NG.directive("form.date", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<input type="text" wliu date-picker ',
                formRowModel,
                formRowCommon,
                formRowToolTip,
            '/>'
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                let today = new Date();
                $(el).pickadate({
                    format: "yyyy-mm-dd",
                    formatSubmit: "yyyy-mm-dd",
                    closeOnSelect: true,
                    disable: [{ from: [2016, 9, 1], to: [2016, 9, 10] }, [2016, 10, 5]],
                    //min: new Date(2015,3,20),
                    //max: new Date(2016,11,14),
                    selectYears: 100,
                    min: new Date(today.getFullYear() - 90, 1, 1),
                    max: new Date(today.getFullYear() + 10, 12, 31),
                    onSet: function (dobj) {
                        if (dobj.select) {
                            let dt = new Date(dobj.select);
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = dt.format("Y-m-d");
                        } else {
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = null;
                        }
                        sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});
WLIU_NG.directive("form.time", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<input type="text" wliu time-picker ',
                formRowModel,
                formRowCommon,
                formRowToolTip,
            '/>'
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                $(el).pickatime({
                    twelvehour: false,
                    afterDone: function (tobj) {
                        if (tobj) 
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = tobj;
                        else 
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = "";
                        sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});
WLIU_NG.directive("form.datetime", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            h1:     "@",
            h2:     "@"
        },
        template: [
            '<span ',
                'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                formRowToolTip,
            '>',
            '<input type="text" ng-attr="{\'h1\':h1, \'h2\':h2}" wliu date-picker ',
                'ng-model="db.tables[tb].CurrentColumn(col).value" ',
                'ng-click="db.tables[tb].SetChange(db.tables[tb].CurrentGuid(), col)" ',
                'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
            '/>',
            '<input type="text" ng-attr="{\'h1\':h1, \'h2\':h2}" wliu time-picker ',
                'ng-model="db.tables[tb].CurrentColumn(col).value1" ',
                'ng-click="db.tables[tb].SetChange(db.tables[tb].CurrentGuid(), col)" ',
                'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
            '/>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.h1 = $scope.h1 || 0;
            $scope.h2 = $scope.h2 || 0;
        },
        link: function (sc, el, attr) {
            $(function () {
                let today = new Date();
                $("input[wliu][date-picker]", el).pickadate({
                    format: "yyyy-mm-dd",
                    formatSubmit: "yyyy-mm-dd",
                    closeOnSelect: true,
                    disable: [{ from: [2016, 9, 1], to: [2016, 9, 10] }, [2016, 10, 5]],
                    //min: new Date(2015,3,20),
                    //max: new Date(2016,11,14),
                    selectYears: 100,
                    min: new Date(today.getFullYear() - 90, 1, 1),
                    max: new Date(today.getFullYear() + 10, 12, 31),
                    onSet: function (dobj) {
                        if (dobj.select) {
                            let dt = new Date(dobj.select);
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = dt.format("Y-m-d");
                        }
                        else
                        {
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = null;
                        }
                        sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                        sc.$apply();
                    }
                });

                $("input[wliu][time-picker]", el).pickatime({
                    twelvehour: false,
                    afterDone: function (tobj) {
                        if (tobj)
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value1 = tobj;
                        else 
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value1 = null;
                        sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});
WLIU_NG.directive("form.ckeditor", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            label:  "@",
            hh:     "@"
        },
        template: [
            '<span>',
                '<label wliu fit h2 ng-if="label" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required, \'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                    'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
                '>',
                '{{db.tables[tb].metas[col].title}}',
                '</label>',
                '<input type="hidden" ',
                    formRowModel,
                ' />',
                '<textarea wliu fit id="{{tb}}_{{col}}_ckeditor"></textarea>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.label = $scope.label || 0;
            $scope.hh = $scope.hh || 500;
            $scope.guid = "";
            $scope.modelChange = function () {
                if ($scope.db.tables) if ($scope.db.tables[$scope.tb]) if ($scope.db.tables[$scope.tb].navi)
                    if ($scope.db.tables[$scope.tb].CurrentRow()) {
                        if ($scope.db.tables[$scope.tb].CurrentGuid() !== $scope.guid) {
                            if (CKEDITOR.instances[`${$scope.tb}_${$scope.col}_ckeditor`])
                                if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value)
                                    CKEDITOR.instances[`${$scope.tb}_${$scope.col}_ckeditor`].setData($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value);
                                else
                                    CKEDITOR.instances[`${$scope.tb}_${$scope.col}_ckeditor`].setData("");
                            $scope.guid = $scope.db.tables[$scope.tb].CurrentGuid();
                        }
                    }
                    else  // row not exist
                    {
                        CKEDITOR.instances[`${$scope.tb}_${$scope.col}_ckeditor`].setData("");
                        $scope.guid = "";
                    }
            };
            $scope.$watch("db.tables[tb].CurrentColumn(col).value", $scope.modelChange);
        },
        link: function (sc, el, attr) {
            $(function () {
                htmlObj_cn = CKEDITOR.replace(`${sc.tb}_${sc.col}_ckeditor`, {
                    height: sc.hh
                });
                htmlObj_cn.on("change", function (evt) {
                });
                htmlObj_cn.on("key", function (evt) {
                    //if (!sc.$root.$$phase) {
                        if (sc.db.tables[sc.tb].CurrentColumn(sc.col)) {
                            if (sc.db.tables[sc.tb].CurrentColumn(sc.col).value !== CKEDITOR.instances[`${sc.tb}_${sc.col}_ckeditor`].getData()) {
                                sc.db.tables[sc.tb].CurrentColumn(sc.col).value = CKEDITOR.instances[`${sc.tb}_${sc.col}_ckeditor`].getData();
                                if (sc.db.tables[sc.tb].CurrentColumn(sc.col).value !== sc.db.tables[sc.tb].CurrentColumn(sc.col).current) {
                                    sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                                } else {
                                    sc.db.tables[sc.tb].CurrentColumn(sc.col).state = 0;
                                    sc.db.tables[sc.tb].ChangeState();
                                }
                                sc.$apply();

                            }
                        }
                    //}
                });

            });
        }
    };
});
WLIU_NG.directive("form.ckinline", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@",
            label: "@",
            hh: "@"
        },
        template: [
            '<span>',
            '<label wliu fit h2 ng-if="label" ',
            'ng-attr="{\'need\': db.tables[tb].metas[col].required, \'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
            'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
            '{{db.tables[tb].metas[col].title}}',
            '</label>',
            '<input type="hidden" ',
            formRowModel,
            ' />',
            '<textarea wliu fit id="{{tb}}_{{col}}_ckeditor"></textarea>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.label = $scope.label || 0;
            $scope.hh = $scope.hh || 500;
            $scope.guid = "";
            $scope.modelChange = function () {
                if ($scope.db.tables) if ($scope.db.tables[$scope.tb]) if ($scope.db.tables[$scope.tb].navi)
                    if ($scope.db.tables[$scope.tb].CurrentRow()) {
                        if ($scope.db.tables[$scope.tb].CurrentGuid() !== $scope.guid) {
                            if (CKEDITOR.instances[`${$scope.tb}_${$scope.col}_ckeditor`])
                                if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value)
                                    CKEDITOR.instances[`${$scope.tb}_${$scope.col}_ckeditor`].setData($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value);
                                else
                                    CKEDITOR.instances[`${$scope.tb}_${$scope.col}_ckeditor`].setData("");
                            $scope.guid = $scope.db.tables[$scope.tb].CurrentGuid();
                        }
                    }
                    else  // row not exist
                    {
                        CKEDITOR.instances[`${$scope.tb}_${$scope.col}_ckeditor`].setData("");
                        $scope.guid = "";
                    }
            };
            $scope.$watch("db.tables[tb].CurrentColumn(col).value", $scope.modelChange);
        },
        link: function (sc, el, attr) {
            $(function () {
                CKEDITOR.disableAutoInline = true;
                htmlObj_cn = CKEDITOR.inline(`${sc.tb}_${sc.col}_ckeditor`, {
                    height: sc.hh
                });
                /*
                htmlObj_cn = CKEDITOR.replace(`${sc.tb}_${sc.col}_ckeditor`, {
                    height: sc.hh
                });
                */
                htmlObj_cn.on("change", function (evt) {
                });
                htmlObj_cn.on("key", function (evt) {
                    //if (!sc.$root.$$phase) {
                    if (sc.db.tables[sc.tb].CurrentColumn(sc.col)) {
                        if (sc.db.tables[sc.tb].CurrentColumn(sc.col).value !== CKEDITOR.instances[`${sc.tb}_${sc.col}_ckeditor`].getData()) {
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = CKEDITOR.instances[`${sc.tb}_${sc.col}_ckeditor`].getData();
                            if (sc.db.tables[sc.tb].CurrentColumn(sc.col).value !== sc.db.tables[sc.tb].CurrentColumn(sc.col).current) {
                                sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                            } else {
                                sc.db.tables[sc.tb].CurrentColumn(sc.col).state = 0;
                                sc.db.tables[sc.tb].ChangeState();
                            }
                            sc.$apply();

                        }
                    }
                    //}
                });

            });
        }
    };
});



WLIU_NG.directive("assm.label", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@"
        },
        template: [
            '<div comm assm fixed>',
            '<label text for="assm_{{tb}}_{{col}}">',
                '{{db.tables[tb].CurrentColumn(col).value}}',
            '</label>',
            '<label assm id="assm_{{tb}}_{{col}}" ',
                'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
                '{{db.tables[tb].metas[col].title}}',
            '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.html", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            hh:     "@"
        },
        template: [
            '<div comm assm fixed>',
                '<label text for="assm_{{tb}}_{{col}}">',
                    '<div style="max-height:{{hh}};" ng-bind-html="getHTML(db.tables[tb].CurrentColumn(col).value)"></div>',
                '</label>',
                '<label assm id="assm_{{tb}}_{{col}}" ',
                    'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope, $sce) {
            $scope.hh = $scope.hh || "80px";
            $scope.getHTML = function (htmlText) {
                return $sce.trustAsHtml(htmlText);
            };
        }
    };
});
WLIU_NG.directive("assm.template", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            label:  "@"
        },
        template: [
            '<div comm assm fixed>',
                '<label text for="assm_{{tb}}_{{col}}" ng-bind-html="getHTML(db.tables[tb].CurrentRow(guid).columns)">',
                '</label>',
                '<label assm id="assm_{{tb}}_{{col}}" ',
                    'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
                '>',
                    '{{label}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope, $sce) {
            $scope.getHTML = function (columns) {
                let colKeys = ('' + $scope.col).holderArray();
                let values = {};
                for (let i = 0; i < colKeys.length; i++) {
                    let colName = colKeys[i];
                    if (colName.indexOf(",") < 0) {
                        if (columns) if (columns[colName]) {
                            values[colName] = columns[colName].value;
                        }
                    }
                }
                return $sce.trustAsHtml(('' + $scope.col).replaceHolder(values));
            };
        }
    };
});
WLIU_NG.directive("assm.intdate", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            format: "@"
        },
        template: [
            '<div comm assm fixed>',
                '<label text for="assm_{{tb}}_{{col}}">',
                    '{{db.tables[tb].CurrentColumn(col).value?(db.tables[tb].CurrentColumn(col).value>0?(db.tables[tb].CurrentColumn(col).value * 1000 | date : (format?format:"yyyy-MM-dd HH:mm") ):"") :""}}',
                '</label>',
                '<label assm id="assm_{{tb}}_{{col}}" ',
                    'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.ymd", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm nolh fixed>',
                '<span wliu text>',
                    '<form.ymd db="db" tb="{{tb}}" col="{{col}}"></form.ymd>',
                '</span>',
                '<label assm ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col+\'YY\'].required}"',
                '>',
                    '{{db.tables[tb].metas[col+\'YY\'].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.textbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm>',
                '<input assm type="text" id="assm_{{tb}}_{{col}}" ',
                    formRowModel,
                    formRowCommon,
                    formRowBounce,
                    formRowToolTip,
                '/>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.validate", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm>',
                '<input assm type="text" id="assm_{{tb}}_{{col}}" ',
                    'ng-blur="Validate(db.tables[tb].CurrentGuid(), col)" ',
                    formRowModel,
                    formRowCommon,
                    formRowToolTip,
                '/>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
            '</label>',

            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.Validate = function (guid, colName) {
                $scope.db.tables[$scope.tb].Validate(guid, colName).then(data => {
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
        }
    };
});
WLIU_NG.directive("assm.password", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm>',
                '<input assm type="password" id="assm_{{tb}}_{{col}}" ',
                    formRowModel,
                    formRowCommon,
                    formRowBounce,
                    formRowToolTip,
                '/>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.confirm", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm>',
                '<input assm type="password" id="assm_{{tb}}_{{col}}_confirm" ',
                    'ng-model="db.tables[tb].CurrentColumn(col).value1" ',
                    formRowCommon,
                    formRowBounce,
                    formRowToolTip,
                '/>',
                '<label assm for="assm_{{tb}}_{{col}}_confirm" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].description}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.textarea", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            hh:     "@"
        },
        template: [
            '<div comm assm nolh fixed>',
                '<span wliu text>',
                    '<textarea wliu fit style="height:{{hh}};margin:0px;padding:0px 2px;" ',
                        formRowModel,
                        formRowCommon,
                        formRowBounce,
                        formRowToolTip,
                    '>',
                    '</textarea>',
                '</span>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.hh = $scope.hh || "80px";
        }
    };
});
WLIU_NG.directive("assm.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm fixed>',
                '<select assm ',
                    'ng-options="sObj.value as sObj.title for sObj in db.GetCollection(tb, col).items" ',
                    formRowModel,
                    formRowCommon,
                    formRowToolTip,
                '>',
                    '<option value=""></option>',
                '</select>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.stext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm fixed>',
                '<label text for="assm_{{tb}}_{{col}}">',
                    '{{(db.GetCollection(tb, col).FindItem(db.tables[tb].CurrentColumn(col).value)?db.GetCollection(tb, col).FindItem(db.tables[tb].CurrentColumn(col).value).title:"")}}',
                '</label>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.fselect", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            fcol:   "@"
        },
        template: [
            '<div comm assm fixed>',
                '<select assm ',
                    'ng-options="sObj.value as sObj.title for sObj in db.GetCollection(tb, col).items | filter: {\'refValue\': db.tables[tb].CurrentColumn(fcol).value }" ',
                    formRowModel,
                    formRowCommon,
                    formRowToolTip,
                '>',
                    '<option value=""></option>',
                '</select>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.bool", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm fixed>',
                '<span wliu text ',
                    'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                    formRowToolTip,
                '>',
                    '<input wliu type="checkbox" id="{{db.tables[tb].CurrentColumn(col).guid}}" ',
                        formRowModel,
                        formRowCommon,
                    '/>',
                    '<label wliu checkbox ',
                        'for="{{db.tables[tb].CurrentColumn(col).guid}}"',
                    '>',
                        '{{ db.tables[tb].CurrentColumn(col).value? db.tables[tb].metas[col].description.labelYes():db.tables[tb].metas[col].description.labelNo() }}',
                    '</label>',
                '</span>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.booltext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@"
        },
        template: [
            '<div comm assm fixed>',
                '<label text for="assm_{{tb}}_{{col}}">',
                    '{{ db.tables[tb].CurrentColumn(col).value? db.tables[tb].metas[col].description.labelYes():db.tables[tb].metas[col].description.labelNo() }}',
                '</label>',
                '<label assm id="assm_{{tb}}_{{col}}" ',
                    'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("assm.radio", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            colnum: "@"
        },
        template: [
            '<div comm assm nolh fixed>',
                '<span wliu text ',
                    'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                    formRowToolTip,
                '>',
                    '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(tb, col).items"></span>',
                        '<input type="radio" wliu ',
                            'id="{{db.tables[tb].CurrentColumn(col).guid + \'-rd-\' + rdObj.value}}" ',
                            'ng-value="rdObj.value" ',
                            formRowModel,
                            formRowCommon,
                        '/>',
                        '<label wliu radio ',
                            'for="{{db.tables[tb].CurrentColumn(col).guid + \'-rd-\' + rdObj.value}}" title="{{rdObj.title}}"',
                        '>',
                            '{{rdObj.title}}',
                        '</label><br ng-if="breakLine($index)">',
                    '<span style="display:none;" ng-repeat-end></span>',
                '</span>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.colnum = parseInt($scope.colnum || 0);
            $scope.breakLine = function (idx) {
                $scope.colnum = parseInt($scope.colnum || 0);
                if ($scope.colnum > 0) {
                    if ((idx + 1) % $scope.colnum === 0)
                        return true;
                    else
                        return false;
                } else {
                    return false;
                }
            };
        }
    };
});
WLIU_NG.directive("assm.checkbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            colnum: "@"
        },
        template: [
            '<div comm assm nolh fixed>',
                '<span wliu text ',
                    'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                    'ng-init="db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col].value=db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[col].value || {}" ',
                    formRowToolTip,
                '>',
                    '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(tb, col).items"></span>',
                        '<input type="checkbox" wliu ',
                            'id="{{db.tables[tb].CurrentColumn(col).guid + \'-ck-\' + rdObj.value}}" ',
                            'ng-model="db.tables[tb].CurrentColumn(col).value[rdObj.value]" ',
                            'ng-value="rdObj.value" ',
                            formRowCommon,
                        '/>',
                        '<label wliu checkbox ',
                            'for="{{db.tables[tb].CurrentColumn(col).guid + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}"',
                         '>',
                            '{{rdObj.title}}',
                        '</label><br ng-if="breakLine($index)">',
                    '<span style="display:none;" ng-repeat-end></span>',
                '</span>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.colnum = parseInt($scope.colnum || 0);
            $scope.breakLine = function (idx) {
                $scope.colnum = parseInt($scope.colnum || 0);
                if ($scope.colnum > 0) {
                    if ((idx + 1) % $scope.colnum === 0)
                        return true;
                    else
                        return false;
                } else {
                    return false;
                }
            };
        }
    };
});
WLIU_NG.directive("assm.radio1", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@"
        },
        template: [
            '<div comm assm fixed>',
                '<input assm radiolist type="text" readonly ',
                    'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                    formRowToolTip,
                    'value="{{valueText()}}" ',
                    'title="{{valueText()}}" ',
                    'ng-click="triggerDiag(db.tables[tb].metas[col].listRef.collection, tb, db.tables[tb].CurrentGuid(), col)" ',
                    'diag-toggle="wliuDiagRadioList" ',
                '/>',
                '<label assm for="assm_{{tb}}_{{col}}" ',
                    'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                    formRowToolTip,
                '>',
                    '{{db.tables[tb].metas[col].title}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };

            $scope.valueText = function () {
                var text = "";
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].metas)
                        if ($scope.db.tables[$scope.tb].metas[$scope.col])
                            if ($scope.db.GetCollection($scope.tb, $scope.col)) {
                                text = $.map($scope.db.GetCollection($scope.tb, $scope.col).items, function (n) {
                                    if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col)) {
                                        if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value === n.value)
                                            return n.title;
                                        else
                                            return null;
                                    } else {
                                        return null;
                                    }

                                }).join("; ");
                            }
                return text;
            }
        },
        link: function (sc, el, attr) {
            $("#wliuDiagRadioList").diag({});
        }
    };
});
WLIU_NG.directive("assm.checkbox1", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@"
        },
        template: [
            '<div comm assm fixed>',
            '<input assm checklist type="text" readonly ',
            'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
            'ng-init="db.tables[tb].rows[db.tables[tb].CurrentGuid()].columns[col].value=db.tables[tb].rows[db.tables[tb].CurrentGuid()].columns[col].value || {}" ',
            formRowToolTip,
            'value="{{valueText()}}" ',
            'title="{{valueText()}}" ',
            'ng-click="triggerDiag(db.tables[tb].metas[col].listRef.collection, tb, db.tables[tb].CurrentGuid(), col)" ',
            'diag-toggle="wliuDiagCheckboxList" ',
            '/>',
            '<label assm for="assm_{{tb}}_{{col}}" ',
            'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
            formRowToolTip,
            '>',
            '{{db.tables[tb].metas[col].title}}',
            '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };

            $scope.valueText = function () {
                var text = "";
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].metas)
                        if ($scope.db.tables[$scope.tb].metas[$scope.col])
                            if ($scope.db.GetCollection($scope.tb, $scope.col)) {
                                text = $.map($scope.db.GetCollection($scope.tb, $scope.col).items, function (n) {
                                    if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col)) {
                                        if ($scope.db.tables[$scope.tb].CurrentColumn($scope.col).value[n.value])
                                            return n.title;
                                        else
                                            return null;
                                    } else {
                                        return null;
                                    }

                                }).join("; ");
                            }
                return text;
            }
        },
        link: function (sc, el, attr) {
            $("#wliuDiagCheckboxList").diag({});
        }
    };
});
WLIU_NG.directive("assm.date", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            h1:     "@",
            h2:     "@"
        },
        template: [
            '<div comm assm>',
            '<input type="text" assm date-picker  ng-attr="{\'h1\':h1, \'h2\':h2}" id="assm_{{tb}}_{{col}}" ',
                formRowModel,
                formRowCommon,
                formRowToolTip,
            '/>',
            '<label assm for="assm_{{tb}}_{{col}}" ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required, \'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                formRowToolTip,
            '>{{db.tables[tb].metas[col].title}}</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                let today = new Date();
                $("input[assm][date-picker]", el).pickadate({
                    format: "yyyy-mm-dd",
                    formatSubmit: "yyyy-mm-dd",
                    closeOnSelect: true,
                    disable: [{ from: [2016, 9, 1], to: [2016, 9, 10] }, [2016, 10, 5]],
                    //min: new Date(2015,3,20),
                    //max: new Date(2016,11,14),
                    selectYears: 100,
                    min: new Date(today.getFullYear() - 90, 1, 1),
                    max: new Date(today.getFullYear() + 10, 12, 31),
                    onSet: function (dobj) {
                        if (dobj.select) {
                            let dt = new Date(dobj.select);
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = dt.format("Y-m-d");
                        } else {
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = null;
                        }
                        sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});
WLIU_NG.directive("assm.time", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@",
            h1: "@",
            h2: "@"
        },
        template: [
            '<div comm assm>',
            '<input type="text" assm time-picker ng-attr="{\'h1\':h1, \'h2\':h2}" id="assm_{{tb}}_{{col}}" ',
                formRowModel,
                formRowCommon,
                formRowToolTip,
            '/>',
            '<label assm for="assm_{{tb}}_{{col}}" ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required, \'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                formRowToolTip,
            '>{{db.tables[tb].metas[col].title}}</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                $("input[assm][time-picker]", el).pickatime({
                    twelvehour: false,
                    afterDone: function (tobj) {
                        if (tobj)
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = tobj;
                        else
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = "";
                        sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});
WLIU_NG.directive("assm.datetime", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            h1:     "@",
            h2:     "@"
        },
        template: [
            '<div comm assm>',
            '<input type="text" ng-attr="{\'h1\':h1, \'h2\':h2}" assm date-picker ',
                'ng-model="db.tables[tb].CurrentColumn(col).value" ',
                'ng-click="db.tables[tb].SetChange(db.tables[tb].CurrentIndex(), col)" ',
                'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
            '/>',
            '<input type="text" ng-attr="{\'h1\':h1, \'h2\':h2}" assm time-picker ',
                'ng-model="db.tables[tb].CurrentColumn(col).value1" ',
                'ng-click="db.tables[tb].SetChange(db.tables[tb].CurrentIndex(), col)" ',
                'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
            '/>',
            '<label assm for="assm_{{tb}}_{{col}}" ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                formRowToolTip,
            '>{{db.tables[tb].metas[col].title}}</label>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                let today = new Date();
                $("input[assm][date-picker]", el).pickadate({
                    format: "yyyy-mm-dd",
                    formatSubmit: "yyyy-mm-dd",
                    closeOnSelect: true,
                    disable: [{ from: [2016, 9, 1], to: [2016, 9, 10] }, [2016, 10, 5]],
                    //min: new Date(2015,3,20),
                    //max: new Date(2016,11,14),
                    selectYears: 100,
                    min: new Date(today.getFullYear() - 90, 1, 1),
                    max: new Date(today.getFullYear() + 10, 12, 31),
                    onSet: function (dobj) {
                        if (dobj.select) {
                            let dt = new Date(dobj.select);
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = dt.format("Y-m-d");
                        }
                        else {
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value = null;
                        }
                        sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                        sc.$apply();
                    }
                });

                $("input[assm][time-picker]", el).pickatime({
                    twelvehour: false,
                    afterDone: function (tobj) {
                        if (tobj)
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value1 = tobj;
                        else
                            sc.db.tables[sc.tb].CurrentColumn(sc.col).value1 = null;
                        sc.db.tables[sc.tb].SetChange(sc.db.tables[sc.tb].CurrentGuid(), sc.col);
                        sc.$apply();
                    }
                });
            });
        }
    };
});

var filterToolTip = [
    'tooltip-toggle="hover" ',
    'tooltip-body="{{db.tables[tb].filters[col].error.Message().nl2br()}}" ',
    'tooltip-target="wliuToolTip" ',
    'tooltip-placement="down" '
].join('');

WLIU_NG.directive("filter.label", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@"
        },
        template: [
            '<label wliu ',
            'ng-attr="{\'need\': db.tables[tb].filters[col].required}" ',
            'title="{{db.tables[tb].filters[col].title?db.tables[tb].filters[col].title:col}}"',
            '>',
            '{{db.tables[tb].filters[col].title?db.tables[tb].filters[col].title:col}}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("filter.textbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            action: "&"
        },
        template: [
            '<span>',
            '<input wliu small type="text" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].filters[col].error.HasError()}" ',
                'ng-model="db.tables[tb].filters[col].value1" ',
                'ng-change="db.tables[tb].FilterChange(col)" ',
                'ng-keypress="keypress($event)" ',
                'ng-disabled="!db.tables[tb].filters[col]" ',
                filterToolTip,
            '/> ',
            '{{db.tables[tb].error.Message()}}',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.keypress = function (ev) {
                if (ev.keyCode === 13) {
                    $(ev.target).select();
                    $scope.db.tables[$scope.tb].Reload().then(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    }).catch(data => {
                        $scope.$apply();
                    });
                }
            };
        },
        link: function (sc, el, attr) {
            $(function () {
                $("input", el).off("focus").on("focus", function (ev) {
                    $(this).select();
                });
            });
        }
    };
});
WLIU_NG.directive("filter.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            reload: "@",
            action: "&"
        },
        template: [
            '<select wliu style="min-width:80px;" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].filters[col].error.HasError()}" ',
                'ng-model="db.tables[tb].filters[col].value1" ',
                'ng-change="db.tables[tb].FilterChange(col);ChangeKey()" ',
                'ng-options="sObj.value as sObj.title for sObj in db.collections[db.tables[tb].filters[col].listRef.collection].items" ',
                'ng-disabled="!db.collections[db.tables[tb].filters[col].listRef.collection]" ',
                filterToolTip,
            '>',
            '<option value=""></option>',
            '</select>'
        ].join(''),
        controller: function ($scope) {
            $scope.reload = $scope.reload || 1;
            $scope.ChangeKey = function () {
                if (parseInt($scope.reload)>0) {
                    $scope.db.tables[$scope.tb].filters[$scope.col].value1 = $scope.db.tables[$scope.tb].filters[$scope.col].value1 || 0;
                    $scope.db.tables[$scope.tb].navi.pageNo = 0;
                    $scope.db.tables[$scope.tb].firstPage().then(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    }).catch(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    });
                }
            };
        }
    };
});
WLIU_NG.directive("filter.fselect", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:         "=",
            tb:         "@",
            col:        "@",
            fcol:       "@",
            reload:     "@",
            action:     "&"
        },
        template: [
            '<select wliu style="min-width:80px;" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].filters[col].error.HasError()}" ',
                'ng-model="db.tables[tb].filters[col].value1" ',
                'ng-change="db.tables[tb].FilterChange(col);ChangeKey()" ',
                'ng-options="sObj.value as sObj.title for sObj in db.collections[db.tables[tb].filters[col].listRef.collection].items | filter: {\'refValue\': db.tables[tb].filters[fcol].value1 }" ',
                'ng-disabled="!db.collections[db.tables[tb].filters[col].listRef.collection]" ',
                filterToolTip,
            '>',
            '<option value=""></option>',
            '</select>'
        ].join(''),
        controller: function ($scope) {
            $scope.reload = $scope.reload || 1;
            $scope.ChangeKey = function () {
                if (parseInt($scope.reload) > 0) {
                    $scope.db.tables[$scope.tb].filters[$scope.col].value1 = $scope.db.tables[$scope.tb].filters[$scope.col].value1 || 0;
                    $scope.db.tables[$scope.tb].navi.pageNo = 0;
                    $scope.db.tables[$scope.tb].firstPage().then(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    }).catch(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    });
                }
            };
        }
    };
});
WLIU_NG.directive("filter.linkselect", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            action: "&"
        },
        template: [
            '<select wliu ',
                'ng-model="db.tables[tb].refKey" ',
                'ng-options="sObj.value as sObj.title for sObj in db.collections[db.tables[tb].filters[col].listRef.collection].items" ',
                'ng-change="db.tables[tb].FilterChange(col);ChangeKey()" ',
                'ng-disabled="!db.collections[db.tables[tb].filters[col].listRef.collection]" ',
                filterToolTip,
            '>',
            '<option value=""></option>',
            '</select>'
        ].join(''),
        controller: function ($scope) {
            $scope.ChangeKey = function () {
                $scope.db.tables[$scope.tb].navi.pageNo = 0;
                $scope.db.tables[$scope.tb].firstPage().then(data => {
                    if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                    $scope.$apply();
                }).catch(data => {
                    $scope.$apply();
                });
            };
        }
    };
});
// only for filter type EInput.Scan
WLIU_NG.directive("filter.scan", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            action: "&"
        },
        template: [
            '<span>',
            '<input wliu type="text" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].filters[col].error.HasError()}" ',
                'ng-model="db.tables[tb].filters[col].value1" ',
                'ng-change="db.tables[tb].FilterChange(col)" ',
                'ng-keypress="keypress($event)" ',
                'ng-disabled="!db.tables[tb].filters[col]" ',
                filterToolTip,
            '/> ',
            '{{db.tables[tb].error.Message()}}',
            '</span>'
        ].join(''),
        controller: function ($scope) {
            $scope.keypress = function (ev) {
                if (ev.keyCode === 13) {
                    $(ev.target).select();
                    $scope.db.tables[$scope.tb].Scan().then(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    }).catch(data => {
                        $scope.$apply();
                    });
                }
            };
        },
        link: function (sc, el, attr) {
            $(function () {
                $("input", el).off("focus").on("focus", function (ev) {
                    $(this).select();
                });
            });
        }
    };
});

// only for filter type EInput.Scan
/*
WLIU_NG.directive("filter.qrscan", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery:    "=",
            db:         "=",
            tb:         "@",
            col:        "@", //col should be EInput.Image type to different normal search
            action:     "&"
        },
        template: [
            '<span>',
            //'<input wliu type="text" ng-model="db.tables[tb].filters[col].value1" />',
                '<a wliu button ng-click="QRCodeScan()">{{Words(\'qr.code\')}}</a>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.QRCodeScan = function (ev) {
                $scope.gallery.startScanner(d => {
                    $scope.db.tables[$scope.tb].filters[$scope.col].value1 = d;
                    $scope.$apply();
                    $scope.db.tables[$scope.tb].Scan().then(data => {
                        if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                        $scope.$apply();
                    }).catch(data => {
                        $scope.$apply();
                    });
                });
            };
        }
    };
});
*/

// checkbox
WLIU_NG.directive("filter.checkcom", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@",
            icon: "@"
        },
        template: [
            '<div style="display:inline-block; vertical-align:middle;">',
            '<table><tr><td style="white-space:nowrap;" valign="top">',
            '<filter.label db="db" tb="{{tb}}" col="{{col}}" style="vertical-align:middle;"></filter.label>: ',
            '<a wliu btn16 detail ',
                'ng-if="icon" ',
                'ng-click="triggerDiag(db.tables[tb].filters[col].listRef.collection, tb, \'\', col)" ',
                'diag-toggle="wliuDiagCheckComFilter" ',
            '></a>',
            '</td><td valign="top">',
            '<ul wliu checkcom>',
            '<li ng-repeat="rdObj1 in valueItems()">',
                '{{rdObj1.title}} ',
                '<a wliu btn16 close disabled ',
                'ng-click="db.tables[tb].filters[col].value1[rdObj1.value]=false"></a>',
            '</li>',
            '</ul>',
            '</td></tr></table>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };
            $scope.valueItems = function () {
                var selectItem = [];
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].filters)
                        if ($scope.db.tables[$scope.tb].filters[$scope.col])
                            if ($scope.db.collections[$scope.db.tables[$scope.tb].filters[$scope.col].listRef.collection]) {
                                selectItem = $.grep($scope.db.collections[$scope.db.tables[$scope.tb].filters[$scope.col].listRef.collection].items, function (n) {
                                    if ($scope.db.tables[$scope.tb].filters[$scope.col].value1) {
                                        if ($scope.db.tables[$scope.tb].filters[$scope.col].value1[n.value])
                                            return true;
                                        else
                                            return false;
                                    } else {
                                        return false;
                                    }

                                });
                            }
                return selectItem;
            };
        }
    };
});
WLIU_NG.directive("checkcom.fdiag", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            maxww:  "@",
            maxhh:  "@"
        },
        template: [
            '<div id="wliuDiagCheckComFilter" wliu diag maskable movable  style="z-index:8802;" diag-toggle="wliuDiagCheckComFilter">',
            '<div head>',
                '{{Words(\'please.select\')}}',
            '</div>',

            '<div body>',
                '<div wliu checklist style="max-height:{{maxhh}};max-width:{{maxww}};" ',
                    'ng-attr="{\'input-invalid\': db.tables[db.checkList.tb].filters[db.checkList.col].error.HasError()}" ',
                    'ng-init="db.tables[db.checkList.tb].filters[db.checkList.col].value1=db.tables[db.checkList.tb].filters[db.checkList.col].value1 || {}" ',
                '>',
                '<span style="display:none;" ng-repeat-start="rdObj in db.collections[db.checkList.collect].items"></span>',
                    '<input type="checkbox" wliu ',
                        'id="{{\'filter-\' + db.checkList.col + \'-ck-\' + rdObj.value}}" ',
                        'ng-model="db.tables[db.checkList.tb].filters[db.checkList.col].value1[rdObj.value]" ',
                        'ng-value="rdObj.value" ',
                        'ng-disabled="!db.tables[db.checkList.tb].filters[db.checkList.col]" ',
                    '/>',
                    '<label wliu checkbox ',
                        'for="{{\'filter-\' + db.checkList.col + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}"',
                    '>',
                    '{{rdObj.title}}',
                    '</label><br>',
                '<span style="display:none;" ng-repeat-end></span>',
                '</div>',
            '</div>',

            '<center style="margin-top:6px;">',
                '<a wliu button diag-toggle="wliuDiagCheckComFilter" title="Words(\'button.close\')">{{Words(\'button.close\')}}</a>',
            '</center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.maxhh = $scope.maxhh || "auto";
            $scope.maxww = $scope.maxww || "auto";
            $scope.db.checkList = $scope.db.checkList || {};
            $scope.db.checkList.collect = $scope.db.checkList.collect || "";
            $scope.db.checkList.tb = $scope.db.checkList.tb || "";
            $scope.db.checkList.guid = $scope.db.checkList.guid || "";
            $scope.db.checkList.col = $scope.db.checkList.col || "";
        }
    };
});


WLIU_NG.directive("card.print", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:      "=",
            gallery: "=",
            action: "&"
        },
        template: [
            '<div style="display:block;text-align:left;margin-top:12px;">',
                '<a wliu button h2 green ng-if="printState()" ng-click="print()">{{Words("button.print")}}</a>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.Words = $window.Words;
            $scope.printState = function () {
                let flag = true;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.print) flag = false;
                return flag;
            };
            $scope.print = function (ev) {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.print) {
                    if ($scope.action) if ($.isFunction($scope.action)) $scope.action();
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.print.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
            };
        }
    };
});
WLIU_NG.directive("card.member", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery:    "=",
            db:         "=",
            tb:         "@",
            mid:        "@",
            photo:      "@",
            cert:       "@",
            alias:      "@"
        },
        template: [
            '<div style="display:block; padding:0px; margin:0px; position:absolute; top:1mm; left:1mm; border-radius:8px; overflow:hidden; width:52.5mm; height:82.5mm;">',
                '<img src="/resource/style/images/membercard/member-{{gallery.memberCard}}.jpg" border="1" style="width:100%; height:100%; object-fit:fill; border:1px solid #eeeeee;" />',
                '<div id="logo" style="display:block;position:absolute;width:16mm;height:16mm;top:0mm;left:0mm;z-index:9;">',
                    '<img src="/resource/style/images/membercard/member-card-logo.png" style="width:100%; height:100%; object-fit:contain;" />',
                '</div>',
                '<div style="display:block;position:absolute;top:24mm;left:0px;right:0px;z-index:9;width:52.5mm;text-align:center;">',
                    '<table border="0" style="min-height:42mm;width:52.5mm;" width="100%">',
                        '<tr>',
                            '<td valign="middle" align="center">',
                                '<img src="{{gallery.CurrentContent(gallery.edit)}}" style="width:26mm;max-height:26mm;object-fit:contain;" />',
                            '</td>',
                        '</tr>',
                        '<tr>',
                            '<td valign="middle" align="center">',
                                //'<div ng-if="db.tables[tb].rows[db.tables[tb].rowGuid].columns[cert].value" style="display:inline-block;position:relative;padding:2px 4px;">',
                                    '<div style="display:inline-block;position:relative;color:{{fontColor()}};text-shadow:1px 1px {{fontShadow()}};font-size:24px;font-weight:600;line-height:1.2;opacity:1;z-index:9;" ng-bind-html="getName(db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[cert].value)"></div>',
                                    //'<div style="display:block;position:absolute;top:0px;left:0px;bottom:0px;right:0px;background-color:#333333;border-radius:6px;opacity:0.5;"></div>',
                                //'</div>',
                            '</td>',
                        '</tr>',
                        '<tr>',
                            '<td valign="middle" align="center">',
                                //'<div ng-if="db.tables[tb].rows[db.tables[tb].rowGuid].columns[alias].value" style="display:inline-block;position:relative;padding:2px 4px;">',
                                    '<div style="display:inline-block;position:relative;color:{{fontColor()}};text-shadow:1px 1px {{fontShadow()}};font-size:18px;font-weight:600;line-height:1.2;opacity:1;z-index:9;" ng-bind-html="getName(db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[alias].value)"></div>',
                                    //'<div style="display:block;position:absolute;top:0px;left:0px;bottom:0px;right:0px;background-color:#333333;border-radius:6px;opacity:0.5;"></div>',
                                //'</div>',
                            '</td>',
                        '</tr>',
                    '</table>',
                '</div>',
                '<div id="id" style="display:block;position:absolute;bottom:1mm;left:1mm;z-index:9;text-align:left;">',
                    '<span style="display:block;color:{{fontColor()}};text-shadow:1px 1px {{fontShadow()}};font-size:14px;line-height:1.2;opacity:1;">{{Words("id.no")}}</span>',
                    '<span style="display:block;color:{{fontColor()}};text-shadow:1px 1px {{fontShadow()}};font-size:14px;line-height:1.2;opacity:1;">{{db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[mid].value}}</span>',
                '</div>',
                '<div id="qr" style="display:block;position:absolute;width:12mm;height:12mm;bottom:1mm;right:1mm;overflow:hidden;z-index:9;">',
                    '<img src="https://chart.googleapis.com/chart?chs=180x180&cht=qr&chl={{db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[mid].value}}&choe=UTF-8" style="position:absolute;top:-2mm;left:-2mm;width:16mm;height:16mm;object-fit:contain;opacity:0.5;" />',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.Words = $window.Words;
            $scope.gallery.memberCard = $scope.gallery.memberCard || "blue"; 
            $scope.fontColor = function () {
                let fcolor = "#0120ba";
                switch ($scope.gallery.memberCard) {
                    case "blue":
                        fcolor = "#ff7f00";
                        break;
                    case "green":
                        fcolor = "#0120ba";
                        break;
                    case "orange":
                        fcolor = "#0120ba";
                        break;
                }
                return fcolor;
            };

            $scope.fontShadow = function () {
                let scolor = "#999999";
                switch ($scope.gallery.memberCard) {
                    case "blue":
                        scolor = "#333333";
                        break;
                    case "green":
                        scolor = "#999999";
                        break;
                    case "orange":
                        scolor = "#999999";
                        break;
                }
                return scolor;
            };

            $scope.getName = function (name) {
                return $sce.trustAsHtml((''+name).nl2br());
            };
        }
    };
});
WLIU_NG.directive("card.shaolin", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery: "=",
            db: "=",
            tb: "@",
            mid: "@",
            photo: "@",
            cert: "@",
            alias: "@"
        },  //yellow:  fbd253   //orange: f8ac07
        template: [
            '<div style="display:block; padding:0px; margin:0px; position:absolute; top:1mm; left:1mm; border-radius:8px; overflow:hidden; width:52.5mm; height:82.5mm;">',
                '<img src="/resource/style/images/membercard/member-{{gallery.memberCard}}.jpg" style="width:100%; height:100%; object-fit:fill; border:1px solid #eeeeee;" />',
                '<div style="display:block;position:absolute;top:26mm;left:0px;right:0px;width:52.5mm;min-height:40mm;z-index:9;text-align:center;">',
                    '<table border="0" style="min-height:40mm;width:52.5mm;" width="100%">',
                    '<tr>',
                    '<td valign="middle" align="center">',
                         '<img src="{{gallery.CurrentContent(gallery.edit)}}" style="width:26mm;max-height:26mm;object-fit:contain;" />',
                    '</td>',
                    '</tr>',
                    '<tr>',
                    '<td valign="middle" align="center">',
                        //'<div style="display:inline-block;position:relative;padding:2px 4px;">',
                            '<div style="display:inline-block;position:relative;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:24px;font-weight:600;line-height:1.2;opacity:1;z-index:9;" ',
                                'ng-bind-html="getName(db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[cert].value)"',
                            '>',
                            '</div>',
                            //'<div style="display:block;position:absolute;top:0px;left:0px;bottom:0px;right:0px;background-color:#333333;border-radius:6px;opacity:0.5;"></div>',
                        //'</div>',
                    '</td>',
                    '</tr>',
                    '<tr>',
                    '<td valign="middle" align="center">',
                        //'<div ng-if="db.tables[tb].rows[db.tables[tb].rowGuid].columns[alias].value" style="display:inline-block;position:relative;padding:2px 4px;">',
                            '<div style="display:inline-block;position:relative;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:16px;font-weight:600;line-height:1.2;opacity:1;z-index:9;" ',
                                'ng-bind-html="getName(db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[alias].value)"',
                            '>',
                            '</div>',
                            //'<div style="display:block;position:absolute;top:0px;left:0px;bottom:0px;right:0px;background-color:#333333;border-radius:6px;opacity:0.5;"></div>',
                        //'</div>',
                    '</td>',
                    '</tr>',
                    '</table>',
                '</div>',
                '<div id="id" style="display:block;position:absolute;bottom:1mm;left:1mm;z-index:9;text-align:left;">',
                    '<span style="display:block;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:12px;font-weight:600;line-height:1.2;opacity:1;">12080 Bridgeport</span>',
                    '<span style="display:block;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:12px;font-weight:600;line-height:1.2;opacity:1;">Richmond, BC</span>',
                    '<span style="display:block;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:12px;font-weight:600;line-height:1.2;opacity:1;">Canada V6V 1J3</span>',
                '</div>',
                '<div id="qr" style="display:block;position:absolute;width:12mm;height:12mm;bottom:1mm;right:1mm;overflow:hidden;z-index:9;">',
                    '<img src="https://chart.googleapis.com/chart?chs=180x180&cht=qr&chl={{db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[mid].value}}&choe=UTF-8" style="position:absolute;top:-2mm;left:-2mm;width:16mm;height:16mm;object-fit:contain;opacity:0.5;" />',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.gallery.memberCard = $scope.gallery.memberCard || "shaolin";
            $scope.fontColor = "#0120ba";
            $scope.fontShadow = "#999999";
            $scope.Words = $window.Words;
            $scope.getName = function (name) {
                return $sce.trustAsHtml(('' + name).nl2br());
            };
        }
    };
});
WLIU_NG.directive("card.vcard", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery:    "=",
            db:         "=",
            tb:         "@",
            company:    "@",
            mid:        "@",
            photo:      "@",
            cert:       "@",
            alias:      "@"
        },  //yellow:  fbd253   //orange: f8ac07
        template: [
            '<div class="member-card-border" style="display:block; padding:0px; margin:0px; position:absolute; top:1mm; left:1mm; border-radius:8px; overflow:hidden; width:52.5mm; height:82.5mm;">',
                '<img src="/resource/style/images/membercard/member-card-logo.png" style="position:absolute;left:-1mm;top:0mm;width:18mm;height:18mm;object-fit:contain;" />',

                '<div id="company" style="display:block;position:absolute;top:18mm;left:0mm;bottom:18mm;width:12mm;text-align:center;z-index:9;vertical-align:middle;">',
                    '<div style="display:inline-block;writing-mode:vertical-rl;color:#d30404;font-size:18px;font-weight:600;vertical-align:middle;text-transform:uppercase;white-space:nowrap;">',
                        '{{company}}',
                    '</div>',
                '</div>',

                '<img src="https://chart.googleapis.com/chart?chs=180x180&cht=qr&chl={{db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[mid].value}}&choe=UTF-8" ',
                    'style="position:absolute;bottom:0px;left:-2mm;width:20mm;height:20mm;object-fit:contain;opacity:1;" />',

                /*
                '<div id="id" style="display:block;position:absolute;bottom:2mm;left:18mm;z-index:9;text-align:left;">',
                    '<span style="display:block;color:#d30404;font-size:12px;font-weight:600;line-height:1.2;opacity:1;">12080 Bridgeport</span>',
                    '<span style="display:block;color:#d30404;font-size:12px;font-weight:600;line-height:1.2;opacity:1;">Richmond, BC</span>',
                    '<span style="display:block;color:#d30404;font-size:12px;font-weight:600;line-height:1.2;opacity:1;">Canada V6V 1J3</span>',
                '</div>',
                */

                '<div id="id" style="display:block;position:absolute;bottom:2mm;left:18mm;z-index:9;text-align:left;">',
                    //'<span style="display:block;color:#d30404;font-size:14px;font-weight:600;line-height:1.2;opacity:1;">{{Words("id.no")}}</span>',
                    '<span style="display:block;color:#d30404;font-size:14px;font-weight:600;line-height:1.2;opacity:1;">{{db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[mid].value}}</span>',
                '</div>',

                '<div id="layout_right" style="display:block;position:absolute;top:2mm,left:12mm;right:1mm;bottom:1mm;">',
                    '<table border="0" style="position:relative; width:38mm; height:80mm;" width="100%">',
                        '<tr>',
                            '<td align="center">',
                                '<img src="{{gallery.CurrentContent(gallery.edit)}}" style="width:32mm;max-height:32mm;object-fit:contain;" />',
                            '</td>',
                        '</tr>',
                        '<tr>',
                            '<td align="center">',
                                '<div style="display:inline-block;position:relative;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:24px;font-weight:600;line-height:1.2;opacity:1;z-index:9;" ',
                                    'ng-bind-html="getName(db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[cert].value)"',
                                '></div>',
                            '</td>',
                        '</tr>',
                        '<tr>',
                            '<td align="center" valign="top">',
                                '<div style="display:inline-block;position:relative;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:18px;font-weight:600;line-height:1.2;opacity:1;z-index:9;" ',
                                    'ng-bind-html="getName(db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[alias].value)"',
                                '></div>',
                            '</td>',
                        '</tr>',
                        '<tr>',
                            '<td align="center">',
                                '<div style="display:block;position:relative;height:10mm;"></div>',
                            '</td>',
                        '</tr>',
                    '</table>',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.Words = $window.Words;
            $scope.fontColor    = "#0120ba";
            $scope.fontShadow   = "#bbbbbb";
            $scope.getName = function (name) {
                return $sce.trustAsHtml(('' + name).nl2br());
            };
        }
    };
});
WLIU_NG.directive("card.hcard", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery:    "=",
            db:         "=",
            tb:         "@",
            mid:        "@",
            photo:      "@",
            cert:       "@",
            alias:      "@"
        },
        template: [
            '<div style="display:block; padding:0px; margin:0px; position:absolute; top:1mm; left:1mm; border-radius:8px; overflow:hidden; width:82.5mm; height:52.5mm;">',
            '<img src="/resource/style/images/membercard/hcard-{{gallery.memberCard}}.jpg" border="1" style="width:100%; height:100%; object-fit:fill; border:1px solid #eeeeee;" />',
            '<div id="logo" style="display:block;position:absolute;width:16mm;height:16mm;top:0mm;left:0mm;z-index:9;">',
                '<img src="/resource/style/images/membercard/member-card-logo.png" style="width:100%; height:100%; object-fit:contain;" />',
            '</div>',
            '<div id="qr" style="display:block;position:absolute;width:16mm;height:16mm;top:1mm;right:1mm;overflow:hidden;z-index:9;">',
                '<img src="https://chart.googleapis.com/chart?chs=180x180&cht=qr&chl={{db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[mid].value}}&choe=UTF-8" style="position:absolute;top:-2mm;left:-2mm;width:20mm;height:20mm;object-fit:contain;opacity:1;" />',
            '</div>',
            '<div style="display:block;position:absolute;top:24mm;left:0px;right:0px;min-height:30mm;z-index:9;width:82.5mm">',
            '<table border="0" style="min-height:30mm;width:82.5mm;" width="100%">',
                '<tr>',
                    '<td rowspan="4" valign="top" align="center">',
                         '<img src="{{gallery.CurrentContent(gallery.edit)}}" style="width:26mm;max-height:26mm;margin:0px 3px;object-fit:contain;" />',
                    '</td>',
                '</tr>',
                '<tr>',
                    '<td valign="top" align="center" width="100%">',
                        '<div style="display:inline-block;position:relative;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:28px;font-weight:600;line-height:1.2;opacity:1;z-index:9;margin-right:6px;" ',
                            'ng-bind-html="getName(db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[cert].value)">',
                        '</div>',
                    '</td>',
                '</tr>',
                '<tr>',
                    '<td valign="top" align="center" width="100%">',
                        '<div ng-if="db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[alias].value" ',
                            'style="display:inline-block;position:relative;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-size:24px;font-weight:600;line-height:1.2;opacity:1;z-index:9;margin-right:6px;" ',
                            'ng-bind-html="getName(db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[alias].value)"',
                        '>',
                        '</div>',
                    '</td>',
                '</tr>',
                '<tr>',
                    '<td valign="top" align="center">',
                        '<div style="display:block;position:relative;height:20px;"></div>',
                    '</td>',
                '</tr>',
                '</table>',
                '</div>',
                '<div id="id" style="display:block;position:absolute;bottom:1mm;right:1mm;z-index:9;text-align:left;">',
                    '<span style="display:inline-block;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-weight:600;font-size:14px;line-height:1.2;opacity:1;">{{Words("id.no")}}: </span>',
                    '<span style="display:inline-block;color:{{fontColor}};text-shadow:1px 1px {{fontShadow}};font-weight:600;font-size:14px;line-height:1.2;opacity:1;">{{db.tables[tb].rows[db.tables[tb].CurrentIndex()].columns[mid].value}}</span>',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.Words = $window.Words;
            $scope.gallery.memberCard = $scope.gallery.memberCard || "member";
            $scope.fontColor = "#0120ba";
            $scope.fontShadow = "#999999";
            $scope.getName = function (name) {
                return $sce.trustAsHtml(('' + name).nl2br());
            };
        }
    };
});
WLIU_NG.directive("vcard.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery: "=",
            action: "&"
        },
        template: [
            '<div style="display:block;text-align:left;">',
                '<span style="font-size:18px;">{{Words(\'member.vcard\')}}</span><br>',
                    '<input type="radio" wliu ',
                    'id="member-color-vcard" ',
                    'ng-click="click()" ',
                    'ng-model="gallery.memberCard" ',
                    'ng-value="\'vcard\'" ',
                '/>',
                '<label wliu radio ',
                    'for="member-color-vcard" title="{{Words(\'vcard.member\')}}"',
                '>',
                    '{{Words("vcard.member")}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.gallery.memberCard = $scope.gallery.memberCard || "vcard";
            $scope.Words = $window.Words;
            $scope.click = function (ev) {
                if ($scope.action)
                    if ($.isFunction($scope.action))
                        $scope.action();
            };
        }
    };
});
WLIU_NG.directive("shaolin.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery: "=",
            action: "&"
        },
        template: [
            '<div style="display:block;text-align:left;">',
                '<span style="font-size:18px;">{{Words(\'shaolin.card\')}}</span><br>',
                '<input type="radio" wliu ',
                    'id="member-color-shaolin" ',
                    'ng-click="click()" ',
                    'ng-model="gallery.memberCard" ',
                    'ng-value="\'shaolin\'" ',
                '/>',
                '<label wliu radio ',
                    'for="member-color-shaolin" title="{{Words(\'shaolin.card\')}}"',
                '>',
                    '{{Words("shaolin.card")}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.gallery.memberCard = $scope.gallery.memberCard || "shaolin";
            $scope.Words = $window.Words;
            $scope.click = function (ev) {
                if ($scope.action)
                    if ($.isFunction($scope.action))
                        $scope.action();
            };
        }
    };
});
WLIU_NG.directive("member.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery:    "=",
            action:     "&"
        },
        template: [
            '<div style="display:block;text-align:left;">',
                '<span style="font-size:18px;">{{Words(\'member.card\')}}</span><br>',
                '<input type="radio" wliu ',
                    'id="member-color-blue" ',
                    'ng-click="click()" ',
                    'ng-value="\'blue\'" ',
                    'ng-model="gallery.memberCard" ',
                '/>',
                '<label wliu radio ',
                    'for="member-color-blue" title="{{Words(\'color.blue\')}}">',
                    '{{Words("color.blue")}}',
                '</label>',
                '<input type="radio" wliu ',
                    'id="member-color-green" ',
                    'ng-click="click()" ',
                    'ng-value="\'green\'" ',
                    'ng-model="gallery.memberCard" ',
                '/>',
                '<label wliu radio ',
                    'for="member-color-green" title="{{Words(\'color.green\')}}">',
                    '{{Words("color.green")}}',
                '</label>',
                '<input type="radio" wliu ',
                    'id="member-color-orange" ',
                    'ng-click="click()" ',
                    'ng-value="\'orange\'" ',
                    'ng-model="gallery.memberCard" ',
                '/>',
                '<label wliu radio ',
                    'for="member-color-orange" title="{{Words(\'color.orange\')}}">',
                    '{{Words("color.orange")}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.gallery.memberCard = $scope.gallery.memberCard || "blue";
            $scope.Words = $window.Words;
            $scope.click = function (ev) {
                if ($scope.action)
                    if ($.isFunction($scope.action))
                        $scope.action();
            };
        }
    };
});
WLIU_NG.directive("hcard.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery: "=",
            action: "&"
        },
        template: [
            '<div style="display:block;text-align:left;">',
                '<span style="font-size:18px;">{{Words(\'member.hcard\')}}</span><br>',
                '<input type="radio" wliu ',
                    'id="member-color-member" ',
                    'ng-click="click()" ',
                    'ng-value="\'member\'" ',
                    'ng-model="gallery.memberCard" ',
                '/>',
                '<label wliu radio ',
                    'for="member-color-member" title="{{Words(\'hcard.member\')}}">',
                    '{{Words("hcard.member")}}',
                '</label>',
                '<input type="radio" wliu ',
                    'id="member-color-employee" ',
                    'ng-click="click()" ',
                    'ng-value="\'employee\'" ',
                    'ng-model="gallery.memberCard" ',
                '/>',
                '<label wliu radio ',
                    'for="member-color-employee" title="{{Words(\'hcard.employee\')}}">',
                    '{{Words("hcard.employee")}}',
                '</label>',
                '<input type="radio" wliu ',
                    'id="member-color-volunteer" ',
                    'ng-click="click()" ',
                    'ng-value="\'volunteer\'" ',
                    'ng-model="gallery.memberCard" ',
                '/>',
                '<label wliu radio ',
                    'for="member-color-volunteer" title="{{Words(\'hcard.volunteer\')}}">',
                    '{{Words("hcard.volunteer")}}',
                '</label>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window, $sce) {
            $scope.gallery.memberCard = $scope.gallery.memberCard || "blue";
            $scope.Words = $window.Words;
            $scope.click = function (ev) {
                if ($scope.action)
                    if ($.isFunction($scope.action))
                        $scope.action();
            };
        }
    };
});

WLIU_NG.directive("image.gallery", function () {
    return {
        restrict: "E",
        replace: true,
        scope: {
            gallery: "=",
            icons: "@",
            ww: "@",
            hh: "@"
        },
        template: [
            '<div>',
            '<button wliu ',
                'style="margin-left:6px;"',
                'ng-if="gallery.images.length<gallery.maxCount" title="Words(\'add.image\')" ',
                'ng-click="gallery.NewRowB()" title="Words(\'add.image\')" ',
            '>',
                '<a wliu btn16 add></a> {{Words(\'add.image\')}}',
            '</button>',
            '<button wliu ng-click="gallery.SaveImages()" title="Words(\'save.image\')" style="margin-left:6px;"><a wliu btn16 save></a> {{Words(\'save.image\')}}</button>',
            '<span style="font-weight:bold;color:#ff4400;font-size:18px;margin-left:6px;vertical-align:middle;">{{imageNotes()}}</span>',
            '<ul style="padding:0px;margin:0px">',
                '<li style="display:inline-block;list-style:none;padding:0px;margin:4px;vertical-align:middle;" ',
                    'ng-repeat="imgObj in gallery.images" ',
                '>',
                    '<div wliu image gallery>',
                        '<div style="display:block;position:relative;width:{{ww}}px;height:{{hh}}px;text-align:center;border-bottom:1px solid #cccccc;background-color:#ffffff;border-radius:6px;">',
                            '<div wcenter ng-if="imgObj.error.HasError()">',
                                '{{imgObj.error.Message().nl2br()}}',
                            '</div>',
                            '<div wcenter ng-if="!imgObj.error.HasError() && imgObj.state==2 && gallery.GuidContent(imgObj.guid, gallery.view)==\'\'">',
                            '{{Words(\'add.image\')}}',
                            '</div>',
                            '<div wcenter ng-if="!imgObj.error.HasError() && imgObj.state==3 && gallery.GuidContent(imgObj.guid, gallery.view)==\'\'">',
                            '{{Words(\'delete.image\')}}',
                            '</div>',
                            '<img wliu ',
                                'ng-if="!gallery.GuidError(imgObj.guid).HasError()" ',
                                'ng-click="showEditor(imgObj.guid)" ',
                                'class="img-responsive" style="cursor:pointer;width:100%;height:100%;" ',
                                'src="{{gallery.GuidContent(imgObj.guid, gallery.view)}}" ',
                            '/>',
                        '</div>',
                        '<div style="display:block; position:relative; text-align:center;">',
                            '<div style="float:left;">',
                                '<label wliu for="gallery_image_upload_{{imgObj.guid}}" ng-if="uploadState()">',
                                    '<a wliu btn24 file-upload  title="{{Words(\'button.upload\')}}"></a>',
                                    '<input type="file" id="gallery_image_upload_{{imgObj.guid}}" guid="{{imgObj.guid}}" ',
                                        'onchange="angular.element(this).scope().selectFile(event, this.getAttribute(\'guid\'));" ',
                                        'style="display:block; position:absolute; opacity:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" ',
                                        'value="Browse..." ',
                                    '/>',
                                '</label>',
                                '<label wliu ng-if="cameraState()"><a wliu btn24 camera ng-click="initCamera(imgObj.guid)" title="{{Words(\'button.camera\')}}"></a></label>',
                                '<label wliu ng-if="textState()"><a wliu btn24 comments ng-click="showComment(imgObj.guid)" title="{{Words(\'button.comments\')}}"></a></label>',
                                '<label wliu ng-if="cancelState()"><a wliu btn24 restore ng-click="gallery.Cancel(imgObj.guid)" title="{{Words(\'button.cancel\')}}"></a></label>',
                            '</div>',
                            '<div style="float:right;">',
                                //'<label wliu><a wliu btn24 save ng-click="gallery.SaveImage()"  title="{{Words(\'button.save\')}}"></a></label>',
                                '<label wliu ng-if="deleteState()"><a wliu btn24 empty ng-click="gallery.Remove(imgObj.guid)"  title="{{Words(\'button.delete\')}}"></a></label>',
                            '</div>',
                        '</div>',
                    '</div>',
                '</li>',
            '</ul>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.icons = $scope.icons || "";
            $scope.Words = $window.Words;
            $scope.uploadState = function () {
                let flag = true;
                if ($scope.icons.indexOf("upload") < 0) flag = false;
                return flag;
            };
            $scope.cameraState = function () {
                let flag = true;
                if ($scope.icons.indexOf("camera") < 0) flag = false;
                return flag;
            };
            $scope.textState = function () {
                let flag = true;
                if ($scope.icons.indexOf("text") < 0) flag = false;
                return flag;
            };
            $scope.deleteState = function () {
                let flag = true;
                if ($scope.icons.indexOf("delete") < 0) flag = false;
                return flag;
            };
            $scope.cancelState = function () {
                let flag = true;
                if ($scope.icons.indexOf("cancel") < 0) flag = false;
                return flag;
            };
            $scope.imageNotes = function () {
                return $scope.Words("image.maxcount").replaceAll('{' + 'maxcount' + '}', $scope.gallery.maxCount);
            };
            $scope.selectFile = function (event, guid) {
                $scope.gallery.SetCurrent(guid);
                $scope.gallery.FromFile(event);
            };
            $scope.showEditor = function (guid) {
                $scope.gallery.SetCurrent(guid);
                $scope.gallery.ShowEditor();
            };
            $scope.initCamera = function (guid) {
                $scope.gallery.SetCurrent(guid);
                $scope.gallery.InitCamera();
            };
            $scope.showComment = function (guid) {
                $scope.gallery.SetCurrent(guid);
                $scope.gallery.ShowComment();
            };
        }
    };
});
WLIU_NG.directive("image.main", function () {
    return {
        restrict: "E",
        replace: true,
        scope: {
            gallery:    "=",
            icons:      "@",
            ww:         "@",
            hh:         "@"
        },
        template: [
            '<div wliu image gallery>',
                '<div style="display:block;position:relative;width:{{ww}}px;height:{{hh}}px;text-align:center;border-bottom:1px solid #cccccc;background-color:#ffffff;border-radius:6px;">',
                    '<div wcenter ng-if="gallery.CurrentError().HasError()">',
                        '{{gallery.CurrentError().Message().nl2br()}}',
                    '</div>',
                    '<div wcenter ng-if="!gallery.CurrentError().HasError() && gallery.CurrentImage().state==2 && gallery.CurrentContent(gallery.view)==\'\'">',
                        '{{Words(\'add.image\')}}',
                    '</div>',
                    '<div wcenter ng-if="!gallery.CurrentError().HasError() && gallery.CurrentImage().state==3 && gallery.CurrentContent(gallery.view)==\'\'">',
                        '{{Words(\'delete.image\')}}',
                    '</div>',
                    '<img wliu ',
                        'ng-if="!gallery.CurrentError().HasError()" ',
                        'ng-click="showEditor()" ',
                        'class="img-responsive" style="cursor:pointer;width:100%;height:100%;" ',
                        'src="{{gallery.CurrentContent(gallery.view)}}" ',
                    '/>',
                '</div>',
                '<div style="display:block; position:relative; text-align:center;">',
                    '<div style="float:left;">',
                        '<label wliu for="gallery_image_upload_{{gallery.CurrentGuid()}}" ng-if="uploadState()">',
                            '<a wliu btn24 file-upload  title="{{Words(\'button.upload\')}}"></a>',
                            '<input type="file" id="gallery_image_upload_{{gallery.CurrentGuid()}}" ',
                                'onchange="angular.element(this).scope().selectFile(event);" ',
                                'style="display:block; position:absolute; opacity:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" ',
                                'value="Browse..." ',
                            '/>',
                        '</label>',
                        '<label wliu ng-if="cameraState()"><a wliu btn24 camera ng-click="initCamera()" title="{{Words(\'button.camera\')}}"></a></label>',
                        '<label wliu ng-if="textState()"><a wliu btn24 comments ng-click="showMainComment()" title="{{Words(\'button.comments\')}}"></a></label>',
                        '<label wliu ng-if="cancelState()"><a wliu btn24 restore ng-click="gallery.Cancel(gallery.CurrentGuid())" title="{{Words(\'button.cancel\')}}"></a></label>',
                    '</div>',
                    '<div style="float:right;">',
                         '<label wliu ng-if="deleteState()"><a wliu btn24 empty ng-click="gallery.Detach(gallery.CurrentGuid())"  title="{{Words(\'button.delete\')}}"></a></label>',
                    '</div>',
              '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.icons = $scope.icons || "";
            $scope.Words = $window.Words;
            $scope.uploadState = function () {
                let flag = true;
                if ($scope.icons.indexOf("upload") < 0) flag = false;
                return flag;
            };
            $scope.cameraState = function () {
                let flag = true;
                if ($scope.icons.indexOf("camera") < 0) flag = false;
                return flag;
            };
            $scope.textState = function () {
                let flag = true;
                if ($scope.icons.indexOf("text") < 0) flag = false;
                return flag;
            };
            $scope.deleteState = function () {
                let flag = true;
                if ($scope.icons.indexOf("delete") < 0) flag = false;
                return flag;
            };
            $scope.cancelState = function () {
                let flag = true;
                if ($scope.icons.indexOf("cancel") < 0) flag = false;
                return flag;
            };

            $scope.selectFile = function (event) {
                $scope.gallery.FromFile(event);
            };
            $scope.showEditor = function () {
                $scope.gallery.ShowEditor();
            };
            $scope.initCamera = function () {
                $scope.gallery.InitCamera();
            };
            $scope.showMainComment = function () {
                $scope.gallery.ShowMainComment();
            };
        }
    };
});
WLIU_NG.directive("image.imgedit", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery:    "=",
            ww:         "@",
            hh:         "@"
        },
        template: [
            '<div id="wliuDiagImageEditor" wliu diag maskable movable style="z-index:8801;" diag-toggle="wliuDiagImageEditor">',
                '<div head>',
                    '{{Words("image.editor")}}',
                '</div>',
                '<div body style="min-width:60px;min-height:60px;padding:0px;margin:0px;text-align:center;">',
                    '<div class="wliu-image-frame">',
                        '<img wliu id="img_editor" style="max-height:420px;" src="" />',
                        '<div class="wliu-image-crop">',
                            '<div class="wliu-image-crop-h"></div>',
                            '<div class="wliu-image-crop-v"></div>',
                        '</div>',
                    '</div>',
                '</div>',
                '<center>',
                    '<button wliu diag-toggle="wliuDiagImageEditor" title="Words(\'image.okey\')"><a wliu btn16 save></a> {{Words(\'image.okey\')}}</button>',
                    '<button wliu ng-click="gallery.CropImage()" title="Words(\'image.crop\')"><a wliu btn16 crop></a> {{Words(\'image.crop\')}}</button>',
                    '<button wliu ng-click="gallery.RotateImage()" title="Words(\'image.rotate\')"><a wliu btn16 rotate-right></a> {{Words(\'image.rotate\')}}</button>',
                    '<button wliu ng-click="gallery.ResetImage()" title="Words(\'image.reset\')"><a wliu btn16 cancel></a> {{Words(\'image.reset\')}}</button>',
                '</center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.ww = $scope.ww || 100;
            $scope.hh = $scope.hh || 100;
        },
        link: function (sc, el, attr) {
            $(function () {
                let ratio = 1;
                let ww = parseInt(sc.ww) <= 0 ? 100 : parseInt(sc.ww);
                let hh = parseInt(sc.hh) <= 0 ? 100 : parseInt(sc.hh);
                ratio = parseInt(ww) / parseInt(hh);

                $("div.wliu-image-crop", el).draggable({
                    containment: "parent"
                });
                $("div.wliu-image-crop", el).resizable({
                    aspectRatio: ratio,
                    containment: "parent"
                });

                $("#img_editor", el).off("load").on("load", function (event) {
                    $("div.wliu-image-crop", "#wliuDiagImageEditor").css({ left: "5%", top: "5%", width: "90%", height: "90%" });
                    $("#wliuDiagImageEditor").diag("initShow");
                });

            });
        }
    };
});
WLIU_NG.directive("image.imgview", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {},
        template: [
            '<div id="wliuDiagImageViewer" wliu diag maskable movable  style="z-index:8802;" diag-toggle="wliuDiagImageViewer">',
            '<div head>',
            '{{Words("image.viewer")}}',
            '</div>',
            '<div body style="min-width:60px;min-height:60px;padding:0px;margin:0px;text-align:center;">',
            '<img wliu id="img_viewer" class="img-responsive" src="" />',
            '</div>',
            '<center style="margin-top:6px;">',
            '<a wliu button diag-toggle="wliuDiagImageViewer" title="Words(\'button.close\')">{{Words(\'button.close\')}}</a>',
            '</center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        },
        link: function (sc, el, attr) {
            $(function () {
                // 对于图片的加载是异步完成的，当加载完成会触发事件  load,  我们只需要改变 <img src="xxxx" />, 剩下的事情放在事件里
                $("#img_viewer", "#wliuDiagImageViewer").off("load").on("load", function (event) {
                    $("#wliuDiagImageViewer").diag("initShow");
                });
            });
        }
    };
});
WLIU_NG.directive("image.imgtext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery: "="
        },
        template: [
        '<div id="wliuDiagImageText" wliu diag maskable movable style="z-index:8804;" diag-toggle="wliuDiagImageText">',
            '<div head>',
                '{{Words("image.comments")}}',
            '</div>',
            '<div body style="min-width:60px;min-height:60px;padding:0px;margin:0px;">',
            '<table cellpadding="2" cellspacing="2" style="width:100%;">',
                '<tr>',
                    '<td><div fixed border comm assm><input assm type="text" ng-model="gallery.CurrentImage().title_en" ' + wliuRowBounce + ' /><label assm>{{Words(\'title.en\')}}</label></div></td>',
                    '<td><div fixed border comm assm><input assm type="text" ng-model="gallery.CurrentImage().title_cn" ' + wliuRowBounce + ' /><label assm>{{Words(\'title.cn\')}}</label></div></td>',
                '</tr>',
                '<tr>',
                    '<td><div fixed border comm assm nolh><span wliu text><textarea wliu fit type="text" ng-model="gallery.CurrentImage().detail_en" ' + wliuRowBounce + ' /></span><label assm>{{Words(\'detail.en\')}}</label></div></td>',
                    '<td><div fixed border comm assm nolh><span wliu text><textarea wliu fit type="text" ng-model="gallery.CurrentImage().detail_cn" ' + wliuRowBounce + ' /></span><label assm>{{Words(\'detail.cn\')}}</label></div></td>',
                '</tr>',
                '<tr>',
                    '<td>',
                        '<div fixed border comm assm nolh>',
                            '<span wliu text>',
                                '<input wliu checkbox type="checkbox" ng-model="gallery.CurrentImage().main" id="image_main_{{gallery.CurrentImage().guid}}" />',
                                '<label wliu checkbox for="image_main_{{gallery.CurrentImage().guid}}">{{Words(\'image.main\')}}</label>',
                            '</span>',
                            '<label assm>{{Words(\'image.main\')}}</label>',
                        '</div>',
                    '</td>',
                    '<td>',
                        '<div fixed border comm assm><input assm type="text" ng-model="gallery.CurrentImage().sort" ' + wliuRowBounce + ' style="width:60px;" /><label assm>{{Words(\'col.sort\')}}</label></div>',
                    '</td>',
                '</tr>',
            '</table>',
            '</div>',
            '<center style="margin-top:12px;">',
                '<a wliu button green  diag-toggle="wliuDiagImageText" ng-click="gallery.SaveCurrentText()" title="Words(\'button.save\')">{{Words(\'button.save\')}}</a>',
                '<a wliu button orange diag-toggle="wliuDiagImageText" title="Words(\'button.cancel\')">{{Words(\'button.cancel\')}}</a>',
            '</center>',
        '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        },
        link: function (sc, el, attr) {
        }
    };
});
WLIU_NG.directive("image.maintext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery: "="
        },
        template: [
            '<div id="wliuDiagImageMainText" wliu diag maskable movable style="z-index:8804;" diag-toggle="wliuDiagImageMainText">',
                '<div head>',
                    '{{Words("image.comments")}}',
                '</div>',
                '<div body style="min-width:60px;min-height:60px;padding:0px;margin:0px;">',
                    '<table cellpadding="2" cellspacing="2" style="width:100%;">',
                        '<tr>',
                            '<td><div fixed border comm assm><input assm type="text" ng-model="gallery.CurrentImage().title_en" ' + wliuRowBounce + ' /><label assm>{{Words(\'title.en\')}}</label></div></td>',
                            '<td><div fixed border comm assm><input assm type="text" ng-model="gallery.CurrentImage().title_cn" ' + wliuRowBounce + ' /><label assm>{{Words(\'title.cn\')}}</label></div></td>',
                        '</tr>',
                        '<tr>',
                            '<td><div fixed border comm assm nolh><span wliu text><textarea wliu fit type="text" ng-model="gallery.CurrentImage().detail_en" ' + wliuRowBounce + ' /></span><label assm>{{Words(\'detail.en\')}}</label></div></td>',
                            '<td><div fixed border comm assm nolh><span wliu text><textarea wliu fit type="text" ng-model="gallery.CurrentImage().detail_cn" ' + wliuRowBounce + ' /></span><label assm>{{Words(\'detail.cn\')}}</label></div></td>',
                        '</tr>',
                    '</table>',
                '</div>',
                '<center style="margin-top:12px;">',
                '<a wliu button green  diag-toggle="wliuDiagImageMainText" ng-click="gallery.SaveCurrentText()" title="Words(\'button.save\')">{{Words(\'button.save\')}}</a>',
                '<a wliu button orange diag-toggle="wliuDiagImageMainText" title="Words(\'button.cancel\')">{{Words(\'button.cancel\')}}</a>',
                '</center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        },
        link: function (sc, el, attr) {
        }
    };
});
WLIU_NG.directive("image.camera", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery: "="
        },
        template: [
            '<div id="wliuDiagCamera" wliu diag maskable movable style="z-index:8803;" diag-toggle="wliuDiagCamera">',
                '<div head>',
                '{{Words("image.camera")}}',
                '</div>',
                '<div body style="min-width:60px;min-height:60px;padding:0px;margin:0px;text-align:center;">',
                    '<video id="videoElement" class="img-responsive" style="height:420px;max-height:420px;object-fit:contain;" />',
                '</div>',
                '<center>',
                    '<button wliu ng-click="gallery.SnapShot()" title="Words(\'image.snap\')"><a wliu btn16 camera></a>{{Words(\'image.snap\')}}</button>',
                    '<button wliu diag-toggle="wliuDiagCamera" title="Words(\'image.close\')">{{Words(\'image.close\')}}</button>',
                '</center>',
           '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        },
        link: function (sc, el, attr) {
            // 对于视频的处理： 视频的加载是异步的， 加载完成以后会触发事件  loadeddata 
            $("#videoElement", el).off("loadeddata").on("loadeddata", function (event) {
                $("#wliuDiagCamera").diag({
                    close: function () {
                        sc.gallery.StopCamera();
                    }
                }).diag("initShow");
            });
        }
    };
});
WLIU_NG.directive("image.scanner", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            gallery:    "=",
            ww:         "@",
            maxww:      "@"
        },
        template: [
            '<div id="wliuDiagScanner" wliu float>',
                '<div head>',
                    '{{Words("image.scanner")}}',
                '</div>',
                '<div body>',
                    '<video id="scannerElement" style="width:{{ww}};max-width:{{maxww}};object-fit:contain;" />',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.ww = $scope.ww || "auto";
            $scope.maxww = $scope.maxww || "30vw";
            $scope.Words = $window.Words;
        },
        link: function (sc, el, attr) {
            $(function () {

                $(el).float({
                    close: function () {
                        sc.gallery.stopScanner();
                    }
                });

                $("#scannerElement", el).off("loadeddata").on("loadeddata", function (event) {
                    if (sc.gallery.scanner) $("#wliuDiagScanner").float("show");
                });

            });
        }
    };
});
WLIU_NG.directive("image.scanview", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:         "=",
            tb:         "@",
            photo:      "@",
            displayname:"@",
            aliasname:  "@",
            ww:         "@",
            hh:         "@",
            maxww:      "@",
            maxhh:      "@"
},
        template: [
            '<div id="wliuDiagScanView" wliu float>',
                '<div head>',
                    '{{Words("image.scanview")}}',
                '</div>',
                '<div body style="text-align:center;">',
                    '<img id="wliuDiagScanViewImage" ng-if="db.tables[tb].rows[db.tables[tb].rowGuid].columns[photo] && db.tables[tb].rows[db.tables[tb].rowGuid].columns[photo].value.length>0" src="{{db.tables[tb].rows[db.tables[tb].rowGuid].columns[photo].value}}" style="width:{{ww}};height:{{hh}};max-width:{{maxww}};max-height:{{maxhh}};object-fit:contain;" />',
                    '<div wliu nophoto ng-if="db.tables[tb].rows[db.tables[tb].rowGuid].columns[photo] && (!db.tables[tb].rows[db.tables[tb].rowGuid].columns[photo].value || db.tables[tb].rows[db.tables[tb].rowGuid].columns[photo].value.length<=0)">',
                        //'{{Words("image.notfound")}}',
                    '</div>',
                    '<div ng-if="!db.tables[tb].rows[db.tables[tb].rowGuid].columns[photo]" style="color:red;font-size:48px;text-align:center;">',
                        '{{Words("row.notfound")}}',
                    '</div>',
                '</div>',
                '<div style="display:block;text-align:center;">',
                    '<label ng-if="db.tables[tb].rows[db.tables[tb].rowGuid].columns[displayname]" style="display:inline-block;font-size:48px;font-weight:600;line-height:1.2;white-space:wrap;">{{db.tables[tb].CurrentRow().columns[displayname].value}}</label><br>',
                    '<label ng-if="db.tables[tb].rows[db.tables[tb].rowGuid].columns[aliasname]" style="display:inline-block;font-size:36px;font-weight:600;line-height:1.2;white-space:wrap;">{{db.tables[tb].CurrentRow().columns[aliasname].value}}</label>',
                '<div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.ww = $scope.ww || "auto";
            $scope.hh = $scope.hh || "auto";
            $scope.maxww = $scope.maxww || "50vw";
            $scope.maxhh = $scope.maxhh || "auto";
            $scope.Words = $window.Words;
        },
        link: function (sc, el, attr) {
            $(function () {
            });
        }
    };
});

WLIU_NG.directive("checkbox.diag", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "="
        },
        template: [
            '<div id="wliuDiagCheckboxList" wliu diag maskable movable  style="z-index:8802;" diag-toggle="wliuDiagCheckboxList">',
            '<div head>',
            '{{Words(\'please.select\')}}',
            '</div>',
            '<div body>',
                '<span wliu text ',
                    'ng-attr="{\'input-invalid\': db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).error.HasError()}" ',
                    'ng-init="db.tables[db.checkList.tb].rows[db.tables[db.checkList.tb].GuidIndex(db.checkList.guid)].columns[db.checkList.col].value=db.tables[db.checkList.tb].rows[db.tables[db.checkList.tb].GuidIndex(db.checkList.guid)].columns[db.checkList.col].value || {}" ',
                '>',
                '<span style="display:none;" ng-repeat-start="rdObj in db.collections[db.checkList.collect].items"></span>',
                '<input type="checkbox" wliu ',
                    'id="{{db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).guid + \'-ck-\' + rdObj.value}}" ',
                    'ng-model="db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).value[rdObj.value]" ',
                    'ng-value="rdObj.value" ',
                    'ng-change="db.tables[db.checkList.tb].Change(db.checkList.guid, db.checkList.col)" ',
                    'ng-disabled="!db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col)" ',
                '/>',
                '<label wliu checkbox ',
                    'for="{{db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).guid + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}"',
                '>',
                    '{{rdObj.title}}',
                '</label>',
                '<span style="display:none;" ng-repeat-end></span>',
                '</span>',
            '</div>',
            '<center style="margin-top:6px;">',
                '<a wliu button diag-toggle="wliuDiagCheckboxList" title="Words(\'button.close\')">{{Words(\'button.close\')}}</a>',
            '</center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.db.checkList            = $scope.db.checkList || {};
            $scope.db.checkList.collect    = $scope.db.checkList.collect || "";
            $scope.db.checkList.tb         = $scope.db.checkList.tb || "";
            $scope.db.checkList.guid        = $scope.db.checkList.guid || "";
            $scope.db.checkList.col        = $scope.db.checkList.col || "";
        }
    };
});
WLIU_NG.directive("checkcom.diag", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            maxww:  "@",
            maxhh:  "@"
        },
        template: [
            '<div id="wliuDiagCheckComList" wliu diag maskable movable  style="z-index:8802;" diag-toggle="wliuDiagCheckComList">',
                '<div head>',
                    '{{Words(\'please.select\')}}',
                '</div>',

                '<div body>',
                    '<div wliu checklist style="max-height:{{maxhh}};max-width:{{maxww}}" ',
                        'ng-attr="{\'input-invalid\': db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).error.HasError()}" ',
                        'ng-init="db.tables[db.checkList.tb].rows[db.tables[db.checkList.tb].GuidIndex(db.checkList.guid)].columns[db.checkList.col].value=db.tables[db.checkList.tb].rows[db.tables[db.checkList.tb].GuidIndex(db.checkList.guid)].columns[db.checkList.col].value || {}" ',
                    '>',
                        '<span style="display:none;" ng-repeat-start="rdObj in db.collections[db.checkList.collect].items"></span>',
                            '<input type="checkbox" wliu ',
                                'id="{{db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).guid + \'-ck-\' + rdObj.value}}" ',
                                'ng-model="db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).value[rdObj.value]" ',
                                'ng-value="rdObj.value" ',
                                'ng-change="db.tables[db.checkList.tb].Change(db.checkList.guid, db.checkList.col)" ',
                                'ng-disabled="!db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col)" ',
                            '/>',
                            '<label wliu checkbox ',
                                'for="{{db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).guid + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}"',
                            '>',
                                '{{rdObj.title}}',
                            '</label><br>',
                        '<span style="display:none;" ng-repeat-end></span>',
                    '</div>',
                '</div>',

                '<center style="margin-top:6px;">',
                    '<a wliu button diag-toggle="wliuDiagCheckComList" title="Words(\'button.close\')">{{Words(\'button.close\')}}</a>',
                '</center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.maxww = $scope.maxww || "auto";
            $scope.maxhh = $scope.maxhh || "auto";
            $scope.db.checkList = $scope.db.checkList || {};
            $scope.db.checkList.collect = $scope.db.checkList.collect || "";
            $scope.db.checkList.tb = $scope.db.checkList.tb || "";
            $scope.db.checkList.guid = $scope.db.checkList.guid || "";
            $scope.db.checkList.col = $scope.db.checkList.col || "";
        }
    };
});
WLIU_NG.directive("radio.diag", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "="
        },
        template: [
            '<div id="wliuDiagRadioList" wliu diag maskable movable  style="z-index:8802;" diag-toggle="wliuDiagRadioList">',
            '<div head>',
                '{{Words(\'please.select\')}}',
            '</div>',
            '<div body>',
            '<span wliu text ',
                'ng-attr="{\'input-invalid\': db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).error.HasError()}" ',
            '>',
                '<span style="display:none;" ng-repeat-start="rdObj in db.collections[db.checkList.collect].items"></span>',
                '<input type="radio" wliu ',
                    'id="{{db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).guid + \'-rd-\' + rdObj.value}}" ',
                    'ng-model="db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).value" ',
                    'ng-value="rdObj.value" ',
                    'ng-change="db.tables[db.checkList.tb].Change(db.checkList.guid, db.checkList.col)" ',
                    'ng-disabled="!db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col)" ',
                '/>',
                '<label wliu radio ',
                    'for= "{{db.tables[db.checkList.tb].GuidColumn(db.checkList.guid, db.checkList.col).guid + \'-rd-\' + rdObj.value}}" title="{{rdObj.title}}" > ',
                    '{{rdObj.title}}',
                '</label>',
            '<span style="display:none;" ng-repeat-end></span>',
            '</span>',
            '</div>',
            '<center style="margin-top:6px;">',
                '<a wliu button diag-toggle="wliuDiagRadioList" title="Words(\'button.close\')">{{Words(\'button.close\')}}</a>',
            '</center>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.db.checkList = $scope.db.checkList || {};
            $scope.db.checkList.collect = $scope.db.checkList.collect || "";
            $scope.db.checkList.tb = $scope.db.checkList.tb || "";
            $scope.db.checkList.guid = $scope.db.checkList.guid || "";
            $scope.db.checkList.col = $scope.db.checkList.col || "";
        }
    };
});

var wliuTreeModel = 'ng-model="db.tables[tb].GuidColumn(guid, col).value" ';
var wliuTreeCommon = [
    'ng-change="db.tables[tb].Change(guid, col)" ',
    'ng-disabled="!db.tables[tb].GuidColumn(guid, col)" ',
    'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" '
].join('');
var wliuTreeToolTip = [
    'tooltip-toggle="hover" ',
    'tooltip-body="{{(db.tables[tb].GuidColumnHasError(guid, col)?db.tables[tb].GuidColumn(guid, col).error.Message().nl2br():db.tables[tb].metas[col].title)}}" ',
    'tooltip-target="wliuToolTip" ',
    'tooltip-placement="down" '
].join('');
var wliuTreeBounce = 'ng-model-options="{ updateOn:\'default blur\', debounce:{default: 500, blur:0} }" ';

WLIU_NG.directive("tree.label", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            col: "@"
        },
        template: [
            '<label style="margin:0px; padding:0px;" ',
                'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
                'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
                '{{db.tables[tb].metas[col].title}}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("tree.textbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<input wliu type="text" ',
                wliuTreeModel,
                wliuTreeCommon,
                wliuTreeBounce,
                wliuTreeToolTip,
            '/> '
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                $("ul[wliu][tree][root]").tree({ state: "open" });
            });
        }
    };
});
WLIU_NG.directive("tree.ftextbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            fkey:   "@"
        },
        template: [
            '<input wliu type="text" ',
                'ng-model="db.tables[tb].CurrentColumn(col).value[fkey]" ',  
                'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
                'ng-change="db.tables[tb].Change(db.tables[tb].CurrentGuid(), col)" ',
            '/> '
        ].join(''),
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
        }
    };
});
WLIU_NG.directive("tree.bool", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            guid: "@",
            col: "@"
        },
        template: [
            '<span wliu ',
            'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                wliuTreeToolTip,
            '>',
            '<input wliu type="checkbox" id="{{db.tables[tb].GuidColumn(guid, col).guid}}" ',
                wliuTreeModel,
                wliuTreeCommon,
            '/>',
            '<label wliu checkbox style="margin:0px;" ',
                'for="{{db.tables[tb].GuidColumn(guid, col).guid}}"',
            '>',
            '{{ db.tables[tb].GuidColumn(guid, col).value? db.tables[tb].metas[col].description.labelYes(): db.tables[tb].metas[col].description.labelNo() }}',
            '</label>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("tree.fbool", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            fkey:   "@"
        },
        template: [
            '<span wliu ',
                'ng-attr="{\'input-invalid\': db.tables[tb].CurrentColumnHasError(col)}" ',
                'tooltip-toggle="hover" ',
                'tooltip-body="{{(db.tables[tb].CurrentColumnHasError(col)?db.tables[tb].CurrentColumnErrorMsg(col):db.tables[tb].metas[col].title)}}" ',
                'tooltip-target="wliuToolTip" ',
                'tooltip-placement="down"',
            '>',
            '<input wliu type="checkbox" id="{{db.tables[tb].CurrentColumn(col).guid}}-{{fkey}}" ',
                'ng-model="db.tables[tb].CurrentColumn(col).value[fkey]" ',
                'ng-change="db.tables[tb].Change(db.tables[tb].CurrentGuid(), col)" ',
                'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
            '/>',
            '<label wliu checkbox style="margin:0px;" ',
                'for="{{db.tables[tb].CurrentColumn(col).guid}}-{{fkey}}"',
            '>',
                '{{ db.tables[tb].CurrentColumn(col).value[fkey]? db.tables[tb].metas[col].description.labelYes(): db.tables[tb].metas[col].description.labelNo() }}',
            '</label>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
//booltext,  请将 yes, no 的值放在 Meta description  "有效|无效"
WLIU_NG.directive("tree.booltext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<label wliu style="padding:0px;margin:0px;" ',
            'ng-attr="{\'need\': db.tables[tb].metas[col].required}" ',
            'title="{{db.tables[tb].metas[col].title?db.tables[tb].metas[col].title:col}}"',
            '>',
            '{{ db.tables[tb].GuidColumn(guid, col).value? db.tables[tb].metas[col].description.labelYes(): db.tables[tb].metas[col].description.labelNo() }}',
            '</label>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("tree.select", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<select wliu ',
                'ng-options="sObj.value as sObj.title for sObj in db.GetCollection(tb, col).items" ',
                wliuTreeModel,
                wliuTreeCommon,
                wliuTreeToolTip,
            '>',
            '<option value=""></option>',
            '</select>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("tree.stext", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: '<label wliu style="margin:0px;padding:0px;">{{(db.GetCollection(tb, col).FindItem(db.tables[tb].GuidColumn(guid, col).value)?db.GetCollection(tb, col).FindItem(db.tables[tb].GuidColumn(guid, col).value).title:"")}}</label>',
        controller: function ($scope) {
        }
    };
});

WLIU_NG.directive("tree.checkbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<span wliu ',
                'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                'ng-init="db.tables[tb].rows[db.tables[tb].GuidIndex(guid)].columns[col].value=db.tables[tb].rows[db.tables[tb].GuidIndex(guid)].columns[col].value || {}" ',
                wliuTreeToolTip,
            '>',
                '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(tb, col).items"></span>',
                    '<input type="checkbox" wliu ',
                        'id="{{db.tables[tb].GuidColumn(guid, col).guid + \'-ck-\' + rdObj.value}}" ',
                        'ng-model="db.tables[tb].GuidColumn(guid, col).value[rdObj.value]" ',
                        'ng-value="rdObj.value" ',
                        wliuTreeCommon,
                    '/>',
                    '<label wliu checkbox style="margin:0px;" ',
                        'for= "{{db.tables[tb].GuidColumn(guid, col).guid + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}" > ',
                        '{{rdObj.title}}',
                    '</label>',
                '<span style="display:none;" ng-repeat-end></span>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});
WLIU_NG.directive("tree.checkbox1", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: [
            '<input wliu checklist readonly type="text" ',
                'ng-init="db.tables[tb].rows[db.tables[tb].GuidIndex(guid)].columns[col].value=db.tables[tb].rows[db.tables[tb].GuidIndex(guid)].columns[col].value || {}" ',
                'ng-attr="{\'input-invalid\': db.tables[tb].GuidColumnHasError(guid, col)}" ',
                 wliuTreeToolTip,
                'value="{{valueText()}}" ',
                'title="{{valueText()}}" ',
                'ng-click="triggerDiag(db.tables[tb].metas[col].listRef.collection, tb, guid, col)" ',
                'diag-toggle="wliuDiagCheckboxList" ',
            '/>'
        ].join(''),
        controller: function ($scope) {
            $scope.triggerDiag = function (collect, tb, guid, col) {
                $scope.db.checkList = $scope.db.checkList || {};
                $scope.db.checkList.collect = $scope.db.checkList.collect || "";
                $scope.db.checkList.tb = $scope.db.checkList.tb || "";
                $scope.db.checkList.guid = $scope.db.checkList.guid || "";
                $scope.db.checkList.col = $scope.db.checkList.col || "";

                $scope.db.checkList.collect = collect;
                $scope.db.checkList.tb = tb;
                $scope.db.checkList.guid = guid;
                $scope.db.checkList.col = col;
            };

            $scope.valueText = function (row) {
                var text = "";
                if ($scope.db.tables[$scope.tb])
                    if ($scope.db.tables[$scope.tb].metas)
                        if ($scope.db.tables[$scope.tb].metas[$scope.col])
                            if ($scope.db.GetCollection($scope.tb, $scope.col)) {
                                text = $.map($scope.db.GetCollection($scope.tb, $scope.col).items, function (n) {
                                    if ($scope.db.tables[$scope.tb].GuidColumn($scope.guid, $scope.col)) {
                                        if ($scope.db.tables[$scope.tb].GuidColumn($scope.guid, $scope.col).value[n.value])
                                            return n.title;
                                        else
                                            return null;
                                    } else {
                                        return null;
                                    }

                                }).join("; ");
                            }
                return text;
            }
        },
        link: function (sc, el, attr) {
            $(function () {
                $("#wliuDiagCheckboxList").diag({});
            });
        }
    };
});
WLIU_NG.directive("tree.fcheckbox", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            col:    "@",
            ftb:    "@",
            fguid:  "@",
            fcol:   "@"
        },
        template: [
            '<span wliu>',
                '<span style="display:none;" ng-repeat-start="rdObj in db.GetCollection(ftb, fcol).FilterItems(db.tables[ftb].GuidColumn(fguid, fcol).value)"></span>',
                    '<input type="checkbox" wliu ',
                        'id="{{db.tables[ftb].GuidColumn(fguid, fcol).guid + \'-ck-\' + rdObj.value}}" ',
                        'ng-disabled="!db.tables[tb].CurrentColumn(col)" ',
                        'ng-model="db.tables[tb].CurrentColumn(col).value[db.tables[ftb].GuidRow(fguid).key][rdObj.value]" ',
                        'ng-change="db.tables[tb].Change(db.tables[tb].CurrentGuid(), col)" ',
                        'ng-value="rdObj.value" ',
                    '/>',
                    '<label wliu checkbox style="margin:0px;" ',
                        'for= "{{db.tables[ftb].GuidColumn(fguid, fcol).guid + \'-ck-\' + rdObj.value}}" title="{{rdObj.title}}" > ',
                        '{{rdObj.title}}',
                    '</label>',
                '<span style="display:none;" ng-repeat-end></span>',
            '</span>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});

WLIU_NG.directive("tree.text", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: '<span wliu>{{db.tables[tb].GuidColumn(guid, col).value}}</span>',
        controller: function ($scope) {
        },
        link: function (sc, el, attr) {
            $(function () {
                $("ul[wliu][tree][root]").tree({ state: "open" });
            });
        }
    };
});
WLIU_NG.directive("tree.template", function ($sce) {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            col:    "@"
        },
        template: '<span wliu ng-bind-html="getHTML(db.tables[tb].GuidRow(guid).columns)"></span>',
        controller: function ($scope, $sce) {
            $scope.getHTML = function (columns) {
                let colKeys = ('' + $scope.col).holderArray();
                let values = {};
                for (let i = 0; i < colKeys.length; i++) {
                    let colName = colKeys[i];
                    if (colName.indexOf(",") < 0) {
                        if (columns) if (columns[colName]) {
                            values[colName] = columns[colName].value;
                        }
                    }
                }
                return $sce.trustAsHtml(('' + $scope.col).replaceHolder(values));
            }
        }
    };
});

WLIU_NG.directive("tree.add", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:             "=",
            tb:             "@",
            ftb:            "@",
            kvs:            "=",
            actionadd:      "&"
        },
        template: [
            '<span>',
            '<a wliu btn16 add ',
                'ng-if="addState(db.tables[ftb].state)" ',
                'ng-click="addAction()" ',
                'title="{{Words(\'button.new\')}}"',
            '></a>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.kvs = $scope.kvs || {};
            $scope.addState = function (state) {
                let flag = true;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.add) flag = false;
                if (state !== 0) flag = false;
                return flag;
            };
            $scope.addAction = function () {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.add) {
                    $scope.db.tables[$scope.tb].NewRowB($scope.kvs);
                    if ($scope.actionadd) if ($.isFunction($scope.actionadd)) $scope.actionadd();
                }
                else
                {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.add.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
            };
        }
    };
});
WLIU_NG.directive("tree.bicon", function () {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:             "=",
            tb:             "@",
            guid:           "@",
            icons:          "@",
            actiondetail:   "&",
            actiondelete:   "&",
            actionsave:     "&",
            actioncancel:   "&"
        },
        template: [
            '<span wliu style="white-space:nowrap;">',
                '<a wliu btn16 detail ',
                    'ng-click="detailAction(guid)" ',
                    'ng-if="detailState(db.tables[tb].GuidRow(guid).state)" ',
                    'title="{{Words(\'button.detail\')}}"',
                '></a>',
                '<a wliu btn16 delete ',
                    'ng-click="deleteAction(guid)" ',
                    'ng-if="deleteState(db.tables[tb].GuidRow(guid).state)" ',
                    'title="{{Words(\'button.delete\')}}"',
                '></a>',
                '<a wliu btn16 cancel ',
                    'ng-click="cancelAction(guid)" ',
                    'ng-if="cancelState(db.tables[tb].GuidRow(guid).state)" ',
                    'title="{{Words(\'button.cancel\')}}"',
                '></a>',
                '<a wliu btn16 save ',
                    'ng-click="saveAction(guid)" ',
                    'ng-if="saveState(db.tables[tb].GuidRow(guid).state)" ',
                    'title="{{Words(\'button.save\')}}"',
                '></a>',
            '</span>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.icons = $scope.icons || "";
            $scope.Words = $window.Words;
            $scope.detailState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("detail") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.detail) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state !== 0) flag = false;
                return flag;
            };
            $scope.deleteState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("delete") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.delete) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state !== 0) flag = false;
                return flag;
            };
            $scope.saveState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("save") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.save) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state === 0) flag = false;

                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.add) {
                    if (state === 2) flag = true;
                }
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.delete) {
                    if (state === 3) flag = true;
                }
                return flag;
            };
            $scope.cancelState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("cancel") < 0) flag = false;
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.save) flag = false;
                if (state === undefined || state === null) flag = false;
                if (state === 0) flag = false;

                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.add) {
                    if (state === 2) flag = true;
                }
                if ($scope.db.user && $scope.db.user.rights && !$scope.db.user.rights.delete) {
                    if (state === 3) flag = true;
                }
                return flag;
            };
            $scope.detailAction = function (guid) {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.detail) {
                    $scope.db.tables[$scope.tb].rowGuid = guid;
                    if ($scope.actiondetail) if ($.isFunction($scope.actiondetail)) {
                        $scope.actiondetail();
                    }
                    event.stopPropagation();
                    return false;
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.detail.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }

            };
            $scope.deleteAction = function (guid) {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.delete) {
                    if ($scope.db.tables[$scope.tb].GuidRow(guid)) {
                        switch ($scope.db.tables[$scope.tb].GuidRow(guid).state) {
                            case 0:
                            case 1:
                                $scope.db.tables[$scope.tb].Detach(guid);
                                break;
                            case 2:
                                $scope.db.tables[$scope.tb].Remove(guid);
                                break;
                            case 3:
                                break;
                        }
                        if ($scope.actiondelete) if ($.isFunction($scope.actiondelete)) $scope.actiondelete();
                    }
                    event.stopPropagation();
                    return false;
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.detail.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }
            };
            $scope.saveAction = function (guid) {
                if ($scope.db.user && $scope.db.user.rights && $scope.db.user.rights.save) {
                    $scope.db.tables[$scope.tb].Save(guid)
                        .then(data => {
                            if ($scope.actionsave) if ($.isFunction($scope.actionsave)) $scope.actionsave();
                            $scope.$apply();
                        })
                        .catch(data => {
                            $scope.$apply();
                        });
                    event.stopPropagation();
                    return false;
                } else {
                    $scope.db.tables[$scope.tb].error.Clear();
                    $scope.db.tables[$scope.tb].error.Append(4001, $scope.Words("rights.save.na"));
                    $scope.db.tables[$scope.tb].TableAjaxErrorHandle(0);
                }

            };
            $scope.cancelAction = function (guid) {
                if ($scope.db.tables[$scope.tb].GuidRow(guid)) {
                    switch ($scope.db.tables[$scope.tb].GuidRow(guid).state) {
                        case 0:
                            break;
                        case 1:
                        case 3:
                            $scope.db.tables[$scope.tb].Cancel(guid);
                            break;
                        case 2:
                            $scope.db.tables[$scope.tb].Remove(guid);
                            break;
                    }
                    if ($scope.actioncancel) if ($.isFunction($scope.actioncancel)) $scope.actioncancel();
                }
                event.stopPropagation();
                return false;
            };
        }
    };
});


/*** ng-attr='{ attrName: true}' ***/
WLIU_NG.directive('ngAttr', function () {
    var name = 'ngAttr';
    var selector = true;
    var ATTR_MATCH = /\s*([^=]+)(=\s*(\S+))?/;
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            var oldVal;

            scope.$watch(attr[name], function (value) {
                ngAttrWatchAction(scope.$eval(attr[name]));
            }, true);

            attr.$observe(name, function () {
                ngAttrWatchAction(scope.$eval(attr[name]));
            });

            function ngAttrWatchAction(newVal) {
                if (selector === true || scope.$index % 2 === selector) {
                    if (oldVal && !angular.equals(newVal, oldVal)) {
                        attrWorker(oldVal, removeAttr);
                    }
                    attrWorker(newVal, setAttr);
                }
                oldVal = angular.copy(newVal);
            }


            function splitAttr(value) {
                var m = ATTR_MATCH.exec(value);
                return m && [m[1].replace(/\s+$/, ''), m[3]];
            }


            function setAttr(value) {
                if (value) {
                    if (value[0] === 'undefined' || value[0] === 'null') {
                        return;
                    }
                    element.attr(value[0], angular.isDefined(value[1]) ? value[1] : '');
                }
            }

            function removeAttr(value) {
                if (value) {
                    element.removeAttr(value[0]);
                }
            }

            function attrWorker(attrVal, action, compare) {
                if (angular.isString(attrVal)) {
                    attrVal = attrVal.split(/\s+/);
                }
                if (angular.isArray(attrVal)) {
                    angular.forEach(attrVal, function (v) {
                        v = splitAttr(v);
                        action(v);
                    });
                } else if (angular.isObject(attrVal)) {
                    var attrs = [];
                    angular.forEach(attrVal, function (v, k) {
                        k = splitAttr(k);
                        if (v) {
                            action(k);
                        }
                    });
                }
            }
        }
    };
});