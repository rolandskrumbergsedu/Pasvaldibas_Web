'use strict';

var myApp = angular.module('app', ['chart.js', 'ngDialog']);

myApp.controller('PieCtrl', ['$scope', 'ngDialog', function ($scope, ngDialog) {
    $scope.labels = ['Atvaļinājums', 'Attaisnoti', 'Neattaisnoti'];
    $scope.data = [2, 5, 3];
    $scope.options = {
        legend: { display: false }
    };

    $scope.openModal = function() {
        ngDialog.open({
            template: '../Templates/deputyView.html',
            controller: 'DeputyCtrl'
        });
    }

}]);

myApp.controller('DeputyCtrl', ['$scope', function ($scope) {
    $scope.labels = ['11.11.2016', '11.10.2016', '11.09.2016', '11.08.2016', '11.07.2016', '11.06.2016', '11.05.2016'];
    $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72', '#45b7cd', '#ff6384', '#ff8e72', '#ff8e72'];
    $scope.data = [
      [1, 1, 1, 1, 1, 1, 1]
    ];
}]);
