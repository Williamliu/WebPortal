var WLIU_NG = angular.module("wliu_layout", []);
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
            number: "@",
            start: "@",
            end: "@",
            site: "@",
            address: "@",
            phone: "@",
            email: "@",
            notes: "@"
        },
        template: [
            '<div wliu card1 nohead>',
                '<div wliu card1 title="{{db.tables[tb].GuidColumn(guid, subject).value}}">{{db.tables[tb].GuidColumn(guid, subject).value}} {{db.tables[tb].GuidColumn(guid, number).value}}</div>',
                '<div wliu card1 body>',
                    '<p style="font-size:24px;color:orangered;" title="{{db.tables[tb].metas[start].description}}: {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}">',
                        '<span style="color:blue;">{{db.tables[tb].metas[start].description}}:</span> {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}',
                    '</p > ',
                    '<p title="{{db.tables[tb].metas[site].title}}: {{db.tables[tb].GuidColumn(guid, site).value}}">',
                        '<span style="color:blue;">{{db.tables[tb].metas[site].title}}:</span> {{db.tables[tb].GuidColumn(guid, site).value}}',
                    '</p>',
                    '<p title="{{db.tables[tb].metas[address].title}}: {{db.tables[tb].GuidColumn(guid, address).value}}">',
                        '<span style="color:blue;">{{db.tables[tb].metas[address].title}}:</span> {{db.tables[tb].GuidColumn(guid, address).value}}',
                    '</p>',
                    '<p title="{{db.tables[tb].metas[phone].title}}: {{db.tables[tb].GuidColumn(guid, phone).value}} {{db.tables[tb].GuidColumn(guid, email).value}}">',
                        '<span style="color:blue;">{{db.tables[tb].metas[phone].title}}:</span> {{db.tables[tb].GuidColumn(guid, phone).value}} <a href="{{db.tables[tb].GuidColumn(guid, email).value}}">{{db.tables[tb].GuidColumn(guid, email).value}}</a>',
                    '</p>',
                    '<p ng-if="db.tables[tb].GuidColumn(guid, notes).value" title="{{db.tables[tb].metas[notes].title}}: {{db.tables[tb].GuidColumn(guid, note).value}}">',
                        '<span style="color:orangered;">{{db.tables[tb].metas[notes].title}}</span>: {{db.tables[tb].GuidColumn(guid, notes).value}}',
                    '</p>',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope) {
        }
    };
});

WLIU_NG.directive("layout.card2", function ($sce) {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            guid: "@",
            imgsrc: "@",
            subject: "@",
            number: "@",
            start: "@",
            end: "@",
            site: "@",
            address: "@",
            phone: "@",
            email: "@",
            notes: "@",
            isfree: "@",
            amount: "@",
            payment: "@",
            currency: "@",
            discount: "@",
            discountamount: "@",
            discounttext: "@"
        },
        template: [
            '<div wliu card2>',
                '<div wliu card2 head>',
                    '<img src="{{db.tables[tb].GuidColumn(guid, imgsrc).value?db.tables[tb].GuidColumn(guid, imgsrc).value:\'/Resource/style/Images/ShaolinBackground/baduanjing.jpg\'}}">',
                '</div>',
                '<div wliu card2 class-subject title="{{db.tables[tb].GuidColumn(guid, subject).value}} {{db.tables[tb].GuidColumn(guid, number).value}}">{{db.tables[tb].GuidColumn(guid, subject).value}} {{db.tables[tb].GuidColumn(guid, number).value}}</div>',
                '<div wliu card2 body>',
                    '<p class-title title="{{db.tables[tb].metas[start].description}}: {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}">',
                        '<span>{{db.tables[tb].metas[start].description}}:</span> {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}',
                    '</p > ',
                    '<p class-site title="{{db.tables[tb].metas[site].title}}: {{db.tables[tb].GuidColumn(guid, site).value}}">',
                        '<span>{{db.tables[tb].metas[site].title}}:</span> {{db.tables[tb].GuidColumn(guid, site).value}}',
                    '</p>',
                    '<p class-address title="{{db.tables[tb].metas[address].title}}: {{db.tables[tb].GuidColumn(guid, address).value}}">',
                        '<span>{{db.tables[tb].metas[address].title}}:</span> {{db.tables[tb].GuidColumn(guid, address).value}}',
                    '</p>',
                    '<p class-contact title="{{db.tables[tb].metas[phone].title}}: {{db.tables[tb].GuidColumn(guid, phone).value}} {{db.tables[tb].GuidColumn(guid, email).value}}">',
                        '<span>{{db.tables[tb].metas[phone].title}}:</span> {{db.tables[tb].GuidColumn(guid, phone).value}} <a href="{{db.tables[tb].GuidColumn(guid, email).value}}">{{db.tables[tb].GuidColumn(guid, email).value}}</a>',
                    '</p>',
                    '<p class-notes ng-if="db.tables[tb].GuidColumn(guid, notes).value" title="{{db.tables[tb].metas[notes].title}}: {{db.tables[tb].GuidColumn(guid, notes).value}}">',
                        '<span class-notes>{{db.tables[tb].metas[notes].title}}</span>: {{db.tables[tb].GuidColumn(guid, notes).value}}',
                    '</p>',
                    '<p class-free ng-if="db.tables[tb].GuidColumn(guid, isfree).value" title="{{Words(\'col.feeamount\')}}: {{Words(\'col.free\')}}">',
                        '<span>{{Words(\'col.feeamount\')}}:</span> {{Words(\'col.free\')}}',
                    '</p>',
                    '<p class-tuition ng-if="!db.tables[tb].GuidColumn(guid, isfree).value" ',
                        'title="{{Words(\'col.feeamount\')}}: ${{db.tables[tb].GuidColumn(guid, amount).value}} {{db.tables[tb].GuidColumn(guid, currency).value}}">',
                        '<span>{{Words(\'col.feeamount\')}}: </span>',
                        '<span style="color:orangered;" ng-class="{\'break-through\': db.tables[tb].GuidColumn(guid, amount).value!=db.tables[tb].GuidColumn(guid, payment).value && db.tables[tb].GuidColumn(guid, payment).value>0}">${{db.tables[tb].GuidColumn(guid, amount).value}} {{db.tables[tb].GuidColumn(guid, currency).value}}</span>',
                    '</p>',
                    '<p class-discount',
                        ' ng-if="!db.tables[tb].GuidColumn(guid, isfree).value && db.tables[tb].GuidColumn(guid, discountamount).value>0" ',
                        'title="{{db.tables[tb].GuidColumn(guid, discounttext).value}}: ${{db.tables[tb].GuidColumn(guid, discountamount).value}} {{db.tables[tb].GuidColumn(guid, currency).value}}">',
                        '<span>{{db.tables[tb].GuidColumn(guid, discounttext).value}}({{db.tables[tb].GuidColumn(guid, discount).value}}%): </span>',
                        '<span class-discount>-${{db.tables[tb].GuidColumn(guid, discountamount).value}} {{db.tables[tb].GuidColumn(guid, currency).value}}</span>',
                    '</p>',
                    '<p class-pay ng-if="!db.tables[tb].GuidColumn(guid, isfree).value && db.tables[tb].GuidColumn(guid, amount).value!=db.tables[tb].GuidColumn(guid, payment).value && db.tables[tb].GuidColumn(guid, payment).value>0" title="{{Words(\'col.payment.amount\')}}: ${{db.tables[tb].GuidColumn(guid, payment).value}} {{db.tables[tb].GuidColumn(guid, currency).value}}">',
                        '<span>{{Words(\'col.payment.amount\')}}:</span> ${{db.tables[tb].GuidColumn(guid, payment).value}} {{db.tables[tb].GuidColumn(guid, currency).value}}</span>',
                    '</p>',
                '</div>',
            '</div>'
        ].join(''),
        controller: function ($scope, $window) {
            $scope.Words = $window.Words;
        }
    };
});



WLIU_NG.directive("layout.card3", function ($sce) {
    return {
        restrict: "E",
        replace: true,
        transclude: true,
        scope: {
            db: "=",
            tb: "@",
            guid: "@",
            imgsrc: "@",
            subject: "@",
            number: "@",
            start: "@",
            end: "@",
            site: "@",
            address: "@",
            phone: "@",
            email: "@",
            notes: "@",
            actionenroll: "&",
            actiondetail: "&",
            actionclose: "&",
            icons: "@"
        },
        template: [
            '<div wliu card3>',
            '<div wliu card3 body>',
                '<div wliu card3 left desktop>',
                    '<img src="{{db.tables[tb].GuidColumn(guid, imgsrc).value?db.tables[tb].GuidColumn(guid, imgsrc).value:\'/Resource/style/Images/ShaolinBackground/baduanjing.jpg\'}}">',
                '</div>',
                '<div wliu card3 center>',
                    '<p class-subject title="{{db.tables[tb].GuidColumn(guid, subject).value}} {{db.tables[tb].GuidColumn(guid, number).value}}">',
                        '{{ db.tables[tb].GuidColumn(guid, subject).value }} {{ db.tables[tb].GuidColumn(guid, number).value }}',
                    '</p>',
                    '<p class-title style="font-size:20px;color:orangered;" title="{{db.tables[tb].metas[start].description}}: {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}">',
                        '<span>{{db.tables[tb].metas[start].description}}:</span> {{db.tables[tb].GuidColumn(guid, start).value}} ~ {{db.tables[tb].GuidColumn(guid, end).value}}',
                    '</p > ',
                    '<p class-site title="{{db.tables[tb].metas[site].title}}: {{db.tables[tb].GuidColumn(guid, site).value}}">',
                        '<span>{{db.tables[tb].metas[site].title}}:</span> {{db.tables[tb].GuidColumn(guid, site).value}}',
                    '</p>',
                    '<p class-address title="{{db.tables[tb].metas[address].title}}: {{db.tables[tb].GuidColumn(guid, address).value}}">',
                        '<span>{{db.tables[tb].metas[address].title}}:</span> {{db.tables[tb].GuidColumn(guid, address).value}}',
                    '</p>',
                    '<p class-contact title="{{db.tables[tb].metas[phone].title}}: {{db.tables[tb].GuidColumn(guid, phone).value}} {{db.tables[tb].GuidColumn(guid, email).value}}">',
                        '<span>{{db.tables[tb].metas[phone].title}}:</span> {{db.tables[tb].GuidColumn(guid, phone).value}} <a href="{{db.tables[tb].GuidColumn(guid, email).value}}">{{db.tables[tb].GuidColumn(guid, email).value}}</a>',
                    '</p>',
                    '<p class-notes ng-if="db.tables[tb].GuidColumn(guid, notes).value" title="{{db.tables[tb].metas[notes].title}}: {{db.tables[tb].GuidColumn(guid, notes).value}}">',
                        '<span class-notes>{{db.tables[tb].metas[notes].title}}</span>: {{db.tables[tb].GuidColumn(guid, notes).value}}',
                    '</p>',
                '</div>',
                '<div wliu card3 right>',
                    '<a href="javascript:void(0)" ng-if="enrollState()" ng-click="clickEnroll(guid)" class="btn btn-primary" style="display:block;margin-top:20px;">{{Words(\'button.enroll\')}}</a>',
                    '<a href="javascript:void(0)" ng-if="detailState()" ng-click="clickDetail(guid)" class="btn btn-primary" style="display:block;margin-top:20px;">{{Words(\'button.detail\')}}</a>',
                '</div>',
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
