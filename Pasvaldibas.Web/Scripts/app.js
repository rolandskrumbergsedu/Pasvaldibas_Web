'use strict';

var myApp = angular.module('app', ['chart.js', 'ngDialog']);

myApp.controller('PasvaldibaCtrl', ['$scope', 'ngDialog', '$http', '$controller', function ($scope, ngDialog, $http, $controller) {

    $scope.loadingData = true;
    $scope.isError = false;

    $scope.allColors = ['#09f935', '#cb511b', '#dfd701', '#93d3d3',
        '#451722', '#564f3b', '#efd489',
        '#82a667', '#bfadd0', '#db322e'];

    $scope.reasonsToColor = [];
    $scope.colorsForReasons = [];

    $scope.deputyLabels = [];
    $scope.deputyData = [];
    $scope.deputyChartColors = [];

    function inArray(needle, haystack) {
        var count = haystack.length;
        for (var i = 0; i < count; i++) {
            if (haystack[i] === needle) { return true; }
        }
        return false;
    }

    var currentUrlSplitted = window.location.href.split("/");

    var currentMunicipality = currentUrlSplitted[currentUrlSplitted.length - 1];
    var urlToCall = '/api/PasvaldibasApmeklejumi/' + currentMunicipality;
    
    $http({
        method: 'GET',
        url: urlToCall
    }).then(function (response) {

        $scope.municipalityName = response.data.PasvaldibaName;
        $scope.municipalityCode = response.data.PasvaldibaCode;
        $scope.deputies = response.data.Deputies;

        // go through all deputies
        $scope.deputies.forEach(function (deputy) {

            // get all reasons
            var reasons = Object.getOwnPropertyNames(deputy.NotAttendedCountReasons);

            // if there are any reason
            if (reasons.length > 0) {
                // go through all reasons
                reasons.forEach(function (reason) {
                    // check if exists in already colored reasons
                    var exists = inArray(reason, $scope.reasonsToColor);

                    // if does not exist, add to colored reasons
                    if (!exists) {
                        var newColor = $scope.allColors.pop();
                        $scope.reasonsToColor.push(reason);
                        $scope.colorsForReasons.push(newColor);
                    }
                });
            }

            // get labels
            $scope.deputyLabels.push({
                labels: reasons.sort()
            });

            // get data
            var data = [];
            reasons.sort().forEach(function (val) {
                data.push(deputy.NotAttendedCountReasons[val]);
            });
            $scope.deputyData.push({
                data: data
            });

            // get chart colors
            var colors = [];
            reasons.sort().forEach(function (reason) {
                var index = $scope.reasonsToColor.indexOf(reason);

                colors.push($scope.colorsForReasons[index]);
            });
            $scope.deputyChartColors.push({
                colors: colors
            });
        });

        $scope.loadingData = false;

    }, function () {
        $scope.isError = true;
        $scope.loadingData = false;
    });

    $scope.options = {
        legend: { display: false }
    };

    $scope.openDeputy = function (dptyId) {
        
        var data = {
            deputyId: dptyId
        }

        ngDialog.open({
            template: '../Templates/deputyView.html',
            controller: 'DeputyCtrl',
            data: data
        });
    }

    $scope.getLabels = function (deputy) {

        return $scope.deputyLabels[deputy].labels;

    }

    $scope.getData = function (deputy) {

        return $scope.deputyData[deputy].data;
    }

    $scope.getChartColors = function (deputy) {

        return $scope.deputyChartColors[deputy].colors;
    }

    $scope.getColor = function (index) {
        return {
            "background-color": $scope.colorsForReasons[index]
        }
    }

    // 80 - 100 - green
    // 60 - 69 - orange
    // 0 - 59 - red
    $scope.getGoodness = function(deputy) {

        var percentage = deputy.AttendedCount / (deputy.AttendedCount + deputy.NotAttendedCount);

        var color = "";

        if (percentage >= 0.8) {
            color = "#82a667";
        } else if (percentage >= 0.6 && percentage < 0.8) {
            color = "#cb511b";
        } else if (percentage < 0.6) {
            color = "#db322e";
        }

        return {
            "color": color
        }
    }

}]);

myApp.controller('DeputyCtrl', ['$scope','$http', function ($scope, $http) {

    $scope.deputyName = "";
    $scope.deputyMunicipality = "";

    $scope.data2013labels = [];
    $scope.data2014labels = [];
    $scope.data2015labels = [];
    $scope.data2016labels = [];

    $scope.data2013data = [];
    $scope.data2014data = [];
    $scope.data2015data = [];
    $scope.data2016data = [];

    $scope.options = {
        scales: {
            yAxes: [{
                display: false,
                barPercentage: 1,
                ticks: {
                    min: 0,
                    max: 1.2,
                    stepSize: 0.2
                }
            }]
        }
    }

    $scope.dataset2013Override = [{}];

    $scope.dataset2014Override = [{}];

    $scope.dataset2015Override = [{}];

    $scope.dataset2016Override = [{}];

    var urlToCall = '/api/Deputati/' + $scope.ngDialogData.deputyId;

    function populate(apmeklejumi, labels, data, override) {
        var d = [];
        var c = [];
        apmeklejumi.forEach(function (val) {
            labels.push(val.Date);

            if (val.Attended === "1") {
                c.push("#458B00");
            } else {
                c.push("#FF0000");
            }
            d.push(1);
        });
        data.push(d);
        override[0].backgroundColor = c;
    }

    $http({
        method: 'GET',
        url: urlToCall
    })
        .then(function (response) {
            $scope.deputyName = response.data.Name;
            $scope.deputyMunicipality = response.data.Municipality;

            populate(response.data.Apmeklejumi2013, $scope.data2013labels, $scope.data2013data, $scope.dataset2013Override);
            populate(response.data.Apmeklejumi2014, $scope.data2014labels, $scope.data2014data, $scope.dataset2014Override);
            populate(response.data.Apmeklejumi2015, $scope.data2015labels, $scope.data2015data, $scope.dataset2015Override);
            populate(response.data.Apmeklejumi2016, $scope.data2016labels, $scope.data2016data, $scope.dataset2016Override);
        });

}]);

myApp.controller('OverviewCtrl', ['$scope', '$http', function ($scope, $http) {
    
    $scope.loadingData = true;

    $scope.municipalities = null;

    $http({
            method: 'GET',
            url: '/api/Pasvaldiba'
        })
        .then(function(response) {

            $scope.municipalities = response.data;

            $scope.loadingData = false;
        });

}]);