var WLIU_NG = angular.module("wliu_layout", []);
WLIU_NG.directive("layout.card", function ($sce) {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:         "=",
            tb:         "@",
            guid:       "@",
            imgsrc:     "@",
            subject:    "@",
            start:      "@",
            end:        "@",
            site:       "@",
            address:    "@",
            phone:      "@",
            email:      "@",
            actionenroll:   "&",
            actiondetail:   "&",
            actionclose:    "&",
            icons:          "@"
        },
        template: [
            '<div wliu card>',
            '<div wliu card head>',
                    '<img src="{{db.tables[tb].GuidColumn(guid, imgsrc).value?db.tables[tb].GuidColumn(guid, imgsrc).value:\'/Resource/style/Images/ShaolinBackground/baduanjing.jpg\'}}">',
                '</div>',
                '<div wliu card title="{{db.tables[tb].GuidColumn(guid, subject).value}}">{{db.tables[tb].GuidColumn(guid, subject).value}}</div>',
                '<div wliu card body>',
                    '<p style="font-size:20px;color:orangered;" title="{{db.tables[tb].metas[start].description}}: {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}">',
                        '{{db.tables[tb].metas[start].description}}: {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}',
                    '</p > ',
                    '<p title="{{db.tables[tb].metas[site].title}}: {{db.tables[tb].GuidColumn(guid, site).value}}">',
                        '{{db.tables[tb].metas[site].title}}: {{db.tables[tb].GuidColumn(guid, site).value}}',
                    '</p>',
                    '<p title="{{db.tables[tb].metas[address].title}}: {{db.tables[tb].GuidColumn(guid, address).value}}">',
                    '{{db.tables[tb].metas[address].title}}: {{db.tables[tb].GuidColumn(guid, address).value}}',
                    '</p>',
                    '<p title="{{db.tables[tb].metas[phone].title}}: {{db.tables[tb].GuidColumn(guid, phone).value}} {{db.tables[tb].GuidColumn(guid, email).value}}">',
                        '{{db.tables[tb].metas[phone].title}}: {{db.tables[tb].GuidColumn(guid, phone).value}} <a href="{{db.tables[tb].GuidColumn(guid, email).value}}">{{db.tables[tb].GuidColumn(guid, email).value}}</a>',
                    '</p>',
                '</div>',
                '<div wliu card foot>',
                    '<a href="javascript:void(0)" ng-if="enrollState()" ng-click="clickEnroll(guid)" class="btn btn-primary" style="margin:0px 6px;">{{Words(\'button.enroll\')}}</a>',
                    '<a href="javascript:void(0)" ng-if="detailState()" ng-click="clickDetail(guid)" class="btn btn-primary" style="margin:0px 6px;">{{Words(\'button.detail\')}}</a>',
                    '<a href="javascript:void(0)" ng-if="closeState()"  ng-click="clickClose(guid)"  class="btn btn-primary" style="margin:0px 6px;">{{Words(\'button.close\')}}</a>',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
            $scope.clickEnroll = function (guid) {
                 $scope.db.tables[$scope.tb].rowGuid = guid;
                if ($scope.actionenroll) if ($.isFunction($scope.actionenroll)) 
                    $scope.actionenroll();
            };
            $scope.clickDetail = function (guid) {
                 $scope.db.tables[$scope.tb].rowGuid = guid;
                if ($scope.actiondetail) if ($.isFunction($scope.actiondetail)) 
                    $scope.actiondetail();
            };
            $scope.clickClose = function (guid) {
                $scope.db.tables[$scope.tb].rowGuid = guid;
                if ($scope.actionclose) if ($.isFunction($scope.actionclose))
                    $scope.actionclose();
            };
            $scope.enrollState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("enroll") < 0) flag = false;
                return flag;
            };
            $scope.detailState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("detail") < 0) flag = false;
                return flag;
            };
            $scope.closeState = function (state) {
                let flag = true;
                if ($scope.icons.indexOf("close") < 0) flag = false;
                return flag;
            };
        }
    };
});

WLIU_NG.directive("layout.card1", function ($sce) {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            guid: "@",
            subject: "@",
            start: "@",
            end: "@",
            site: "@",
            address: "@",
            phone: "@",
            email: "@"
        },
        template: [
            '<div wliu card nohead>',
                '<div wliu card title="{{db.tables[tb].GuidColumn(guid, subject).value}}">{{db.tables[tb].GuidColumn(guid, subject).value}}</div>',
                '<div wliu card body>',
                    '<p style="font-size:20px;color:orangered;" title="{{db.tables[tb].metas[start].description}}: {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}">',
                        '{{db.tables[tb].metas[start].description}}: {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}',
                    '</p > ',
                    '<p title="{{db.tables[tb].metas[site].title}}: {{db.tables[tb].GuidColumn(guid, site).value}}">',
                        '{{db.tables[tb].metas[site].title}}: {{db.tables[tb].GuidColumn(guid, site).value}}',
                    '</p>',
                    '<p title="{{db.tables[tb].metas[address].title}}: {{db.tables[tb].GuidColumn(guid, address).value}}">',
                        '{{db.tables[tb].metas[address].title}}: {{db.tables[tb].GuidColumn(guid, address).value}}',
                    '</p>',
                    '<p title="{{db.tables[tb].metas[phone].title}}: {{db.tables[tb].GuidColumn(guid, phone).value}} {{db.tables[tb].GuidColumn(guid, email).value}}">',
                        '{{db.tables[tb].metas[phone].title}}: {{db.tables[tb].GuidColumn(guid, phone).value}} <a href="{{db.tables[tb].GuidColumn(guid, email).value}}">{{db.tables[tb].GuidColumn(guid, email).value}}</a>',
                    '</p>',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});



WLIU_NG.directive("layout.cardimg", function ($sce) {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db:     "=",
            tb:     "@",
            guid:   "@",
            imgsrc: "@",
            hh:     "@"
        },
        template: [
            '<img style="display:block;ww:100%;height:{{hh}};object-fit:contain;" src="{{db.tables[tb].GuidColumn(guid, imgsrc).value?db.tables[tb].GuidColumn(guid, imgsrc).value:\'/Resource/style/Images/ShaolinBackground/baduanjing.jpg\'}}">'
        ].join(''),
        controller: function ($scope) {
            $scope.hh = $scope.hh || "100%";
        }
    };
});
