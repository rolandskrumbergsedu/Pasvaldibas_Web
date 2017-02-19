'use strict';

var myApp = angular.module('app', ['chart.js', 'ngDialog']);

myApp.controller('PasvaldibaCtrl', ['$scope', 'ngDialog', '$http', function ($scope, ngDialog, $http) {

    $scope.loadingData = true;
    $scope.isError = false;

    $scope.allColors = ['#09f935', '#cb511b', '#dfd701', '#93d3d3',
        '#451722', '#564f3b', '#efd489',
        '#82a667', '#bfadd0', '#db322e'];

    $scope.reasonsToColor = [];
    $scope.colorsForReasons = [];

    function inArray(needle, haystack) {
        var count = haystack.length;
        for (var i = 0; i < count; i++) {
            if (haystack[i] === needle) { return true; }
        }
        return false;
    }

    $http({
        method: 'GET',
        url: '/api/PasvaldibasApmeklejumi/ALSUNGA' //TO DO: change as parameter
    }).then(function (response) {

        $scope.municipalityName = response.data.PasvaldibaName;
        $scope.deputies = response.data.Deputies;
        $scope.loadingData = false;

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
        });

    }, function () {
        $scope.isError = true;
        $scope.loadingData = false;
    });

    $scope.options = {
        legend: { display: false }
    };

    $scope.openModal = function () {
        ngDialog.open({
            template: '../Templates/deputyView.html',
            controller: 'DeputyCtrl'
        });
    }

    $scope.getLabels = function (deputy) {

        var reasons = Object.getOwnPropertyNames(deputy.NotAttendedCountReasons).sort();

        return reasons;

    }

    $scope.getData = function (deputy) {

        var data = [];
        var labels = Object.getOwnPropertyNames(deputy.NotAttendedCountReasons).sort();
        labels.forEach(function (val) {
            data.push(deputy.NotAttendedCountReasons[val]);
        });

        return data;

    }

    $scope.getChartColors = function (deputy) {

        if (deputy) {
            var reasons = Object.getOwnPropertyNames(deputy.NotAttendedCountReasons).sort();
            var colors = [];

            reasons.forEach(function (reason) {
                var index = $scope.reasonsToColor.indexOf(reason);

                colors.push($scope.colorsForReasons[index]);
            });

            return colors;
        }

        return null;
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

myApp.controller('DeputyCtrl', ['$scope', function ($scope) {
    $scope.labels = ['11.11.2016', '11.10.2016', '11.09.2016', '11.08.2016', '11.07.2016', '11.06.2016', '11.05.2016'];
    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72', '#45b7cd', '#ff6384', '#ff8e72', '#ff8e72'];
    $scope.data = [
      [1, 1, 1, 1, 1, 1, 1]
    ];
}]);
