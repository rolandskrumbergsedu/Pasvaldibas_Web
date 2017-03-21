'use strict';

var myApp = angular.module('app', ['chart.js', 'ngDialog']);

myApp.controller('PasvaldibaBasicCtrl', ['$scope', 'ngDialog', '$http',
    function ($scope, ngDialog, $http) {
        $scope.loadingData = true;
        $scope.isError = false;

        $scope.reasonsToColor = ['Ieradās', 'Neieradās'];
        $scope.colorsForReasons = ['#82a667', '#db322e'];

        $scope.labels = ['Ieradās', 'Neieradās'];
        $scope.deputyData = [];
        $scope.colors = ['#82a667', '#db322e'];

        var currentUrlSplitted = window.location.href.split("/");

        var currentMunicipality = currentUrlSplitted[currentUrlSplitted.length - 1];
        var urlToCall = '/api/PasvaldibasApmeklejumiBasic/' + currentMunicipality;

        $http({
            method: 'GET',
            url: urlToCall
        }).then(function (response) {

            $scope.municipalityName = response.data.PasvaldibaName;
            $scope.municipalityCode = response.data.PasvaldibaCode;
            $scope.deputies = response.data.Deputies;

            // go through all deputies
            $scope.deputies.forEach(function (deputy) {

                var data = [];
                data.push(deputy.AttendedCount);
                data.push(deputy.NotAttendedCount);
                $scope.deputyData.push({
                    data: data
                });
            });

            $scope.loadingData = false;

        }, function () {
            $scope.isError = true;
            $scope.loadingData = false;
        });

        $scope.options = {
            legend: { display: false },
            tooltips: {
                callbacks: {
                    label: function (tooltipItem, data) {
                        
                        var currentValue = data.datasets[0].data[tooltipItem.index];
                        var label = data.labels[tooltipItem.index];
                        var allSum = 0;
                        data.datasets[0].data.forEach(function(value) {
                            allSum += value;
                        });
                        return label + ": " + Math.round(currentValue / allSum * 100) + "% sēžu";

                    }
                }
            }
        };

        $scope.openDeputy = function (dptyId) {

            var data = {
                deputyId: dptyId
            }

            ngDialog.open({
                template: '../Templates/deputyView.html',
                controller: 'DeputyCtrl',
                data: data,
                width: 700
        });
        }

        $scope.getData = function (deputy) {
            return $scope.deputyData[deputy].data;
        }

        $scope.getColor = function (index) {
            return {
                "background-color": $scope.colorsForReasons[index]
            }
        }

        $scope.getGoodness = function (deputy) {

            var percentage = deputy.AttendedCount / (deputy.AttendedCount + deputy.NotAttendedCount);

            var color = "";

            if (percentage >= 0.8) {
                color = "#009900";
            } else if (percentage >= 0.5 && percentage < 0.8) {
                color = "#ffcc00";
            } else if (percentage < 0.5) {
                color = "#ff0000";
            }

            return {
                "color": color
            }
        }
    }]);

myApp.controller('DeputyCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.deputyName = "";
    $scope.deputyMunicipality = "";

    $scope.iemesluKrasas = {};
    $scope.iemesluKrasas["Ieradās"] = "#82a667";
    $scope.iemesluKrasas["Neieradās"] = "#db322e";
    $scope.iemesluKrasas["Darbā"] = "#09f935";
    $scope.iemesluKrasas["Slimība"] = "#cb511b";
    $scope.iemesluKrasas["Atvaļinājums"] = "#dfd701";

    $scope.allColors = ['#93d3d3', '#451722', '#564f3b', '#efd489', '#bfadd0', '#db322e'];

    $scope.pieLabels = [];
    $scope.pieColors = [];
    $scope.pieData = [];

    $scope.data2013labels = [];
    $scope.data2014labels = [];
    $scope.data2015labels = [];
    $scope.data2016labels = [];

    $scope.data2013data = [];
    $scope.data2014data = [];
    $scope.data2015data = [];
    $scope.data2016data = [];

    $scope.pieOptions = {
        legend: { display: false }
    };

    $scope.options2013 = {
        responsive: false,
        scales: {
            yAxes: [{
                display: false,
                barPercentage: 1,
                ticks: {
                    min: 0,
                    max: 1.2,
                    stepSize: 0.2
                },
                barThickness: 50,
                gridLines: {
                    display: false
                }
            }],
            xAxes: [
            {
                stacked: true,
                barPercentage: 1,
                categoryPercentage: 1,
                barThickness: 20,
                gridLines: {
                    display: false
                }
            }]
        },
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    var label = data.labels[tooltipItem.index];
                    var newValue = $scope.tooltipLabel2013[label];
                    return newValue;
                }
            }
        }
    };
    $scope.options2014 = {
        responsive: false,
        scales: {
            yAxes: [{
                display: false,
                barPercentage: 1,
                ticks: {
                    min: 0,
                    max: 1.2,
                    stepSize: 0.2
                },
                barThickness: 50,
                gridLines: {
                    display: false
                }
            }],
            xAxes: [
            {
                stacked: true,
                barPercentage: 1,
                barThickness: 20,
                gridLines: {
                    display: false
                }
            }]
        },
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    var label = data.labels[tooltipItem.index];
                    var newValue = $scope.tooltipLabel2014[label];
                    return newValue;
                }
            }
        }
    };
    $scope.options2015 = {
        responsive: false,
        scales: {
            yAxes: [{
                display: false,
                barPercentage: 1,
                ticks: {
                    min: 0,
                    max: 1.2,
                    stepSize: 0.2
                },
                barThickness: 50,
                gridLines: {
                    display: false
                }
            }],
            xAxes: [
            {
                stacked: true,
                barPercentage: 1,
                barThickness: 20,
                gridLines: {
                    display: false
                }
            }]
        },
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    var label = data.labels[tooltipItem.index];
                    var newValue = $scope.tooltipLabel2015[label];
                    return newValue;
                }
            }
        }
    };
    $scope.options2016 = {
        responsive: false,
        scales: {
            yAxes: [{
                display: false,
                barPercentage: 1,
                ticks: {
                    min: 0,
                    max: 1.2,
                    stepSize: 0.2
                },
                barThickness: 50,
                gridLines: {
                    display: false
                }
            }],
            xAxes: [
            {
                stacked: true,
                barPercentage: 1,
                barThickness: 20,
                gridLines: {
                    display: false
                }
            }]
        },
        tooltips: {
            callbacks: {
                label: function (tooltipItem, data) {
                    var label = data.labels[tooltipItem.index];
                    var newValue = $scope.tooltipLabel2016[label];
                    return newValue;
                }
            }
        }
    };

    $scope.dataset2013Override = [{}];
    $scope.dataset2014Override = [{}];
    $scope.dataset2015Override = [{}];
    $scope.dataset2016Override = [{}];

    $scope.tooltipLabel2013 = {}
    $scope.tooltipLabel2014 = {}
    $scope.tooltipLabel2015 = {}
    $scope.tooltipLabel2016 = {}

    var urlToCall = '/api/Deputati/' + $scope.ngDialogData.deputyId;

    function populate(apmeklejumi, labels, data, override, newLabels) {
        var d = [];
        var c = [];
        apmeklejumi.forEach(function (val) {
            labels.push(val.Date);

            if (val.Attended === "1") {
                c.push($scope.iemesluKrasas["Ieradās"]);
                newLabels[val.Date] = "Ieradās";
            } else {
                c.push($scope.iemesluKrasas[val.Reason]);
                newLabels[val.Date] = val.Reason;
            }

            d.push(1);
        });
        data.push(d);
        override[0].backgroundColor = c;
        override[0].borderWidth = 0;
    }

    function inArray(needle, haystack) {
        var count = haystack.length;
        for (var i = 0; i < count; i++) {
            if (haystack[i] === needle) { return true; }
        }
        return false;
    }

    $http({
        method: 'GET',
        url: urlToCall
    })
        .then(function (response) {
            $scope.deputyName = response.data.Name;
            $scope.deputyMunicipality = response.data.Municipality;

            // get all reasons
            var reasons = Object.getOwnPropertyNames(response.data.NotAttendedCountReasons);

            // if there are any reason
            if (reasons.length > 0) {

                // go through all reasons
                reasons.sort().forEach(function (reason) {

                    // check if exists in already colored reasons
                    var exists = inArray(reason, $scope.pieLabels);

                    // if does not exist, add to colored reasons
                    if (!exists) {

                        var newColor;
                        if ($scope.iemesluKrasas[reason]) {
                            newColor = $scope.iemesluKrasas[reason];
                        } else {
                            newColor = $scope.allColors.pop();
                            $scope.iemesluKrasas[reason] = newColor;
                        }

                        $scope.pieLabels.push(reason);
                        $scope.pieColors.push(newColor);
                    }
                });
            }

            // get data
            reasons.sort().forEach(function (val) {
                $scope.pieData.push(response.data.NotAttendedCountReasons[val]);
            });

            populate(response.data.Apmeklejumi2013, $scope.data2013labels, $scope.data2013data, $scope.dataset2013Override, $scope.tooltipLabel2013);
            populate(response.data.Apmeklejumi2014, $scope.data2014labels, $scope.data2014data, $scope.dataset2014Override, $scope.tooltipLabel2014);
            populate(response.data.Apmeklejumi2015, $scope.data2015labels, $scope.data2015data, $scope.dataset2015Override, $scope.tooltipLabel2015);
            populate(response.data.Apmeklejumi2016, $scope.data2016labels, $scope.data2016data, $scope.dataset2016Override, $scope.tooltipLabel2016);

            var width2013 = $scope.data2013labels.length * 20;
            $scope.style2013 = {
                height: '100px',
                width: width2013 + 'px'
            }

            var width2014 = $scope.data2014labels.length * 20;
            $scope.style2014 = {
                height: '100px',
                width: width2014 + 'px'
            }

            var width2015 = $scope.data2015labels.length * 20;
            $scope.style2015 = {
                height: '100px',
                width: width2015 + 'px'
            }

            var width2016 = $scope.data2016labels.length * 20;
            $scope.style2016 = {
                height: '100px',
                width: width2016 + 'px'
            }

        });

}]);

myApp.controller('OverviewCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.loadingData = true;

    $scope.municipalities = null;

    $http({
        method: 'GET',
        url: '/api/Pasvaldiba'
    })
        .then(function (response) {

            $scope.municipalities = response.data;

            $scope.loadingData = false;
        });

}]);