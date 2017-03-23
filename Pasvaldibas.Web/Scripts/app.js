'use strict';

var myApp = angular.module('app', ['chart.js', 'ngDialog']);

myApp.controller('PasvaldibaBasicCtrl', ['$scope', 'ngDialog', '$http',
    function ($scope, ngDialog, $http) {
        $scope.loadingData = true;
        $scope.isError = false;

        $scope.reasonsToColor = ['Ieradās', 'Neieradās'];
        $scope.colorsForReasons = ['#47a447', '#ff0000'];

        $scope.labels = ['Ieradās', 'Neieradās'];
        $scope.deputyData = [];
        $scope.colors = ['#47a447', '#ff0000'];

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
                bodyFontSize: 10,
                titleFontSize: 10,
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
                width: 600
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

//https://github.com/chartjs/Chart.js/blob/master/samples/tooltips/custom-pie.html
myApp.controller('DeputyCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.deputyName = "";
    $scope.deputyMunicipality = "";

    $scope.show2013 = false;
    $scope.show2014 = false;
    $scope.show2015 = false;
    $scope.show2016 = false;

    $scope.unattendedCount = 0;
    $scope.allCount = 0;

    $scope.iemesluKrasas = {};
    $scope.iemesluKrasas["Ieradās"] = "#47a447";
    $scope.iemesluKrasas["Neieradās"] = "#ff0000";

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
        legend: { display: false },
        tooltips: {
            bodyFontSize: 11,
            titleFontSize: 11
        }
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
            bodyFontSize: 11,
            titleFontSize: 11,
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
            bodyFontSize: 11,
            titleFontSize: 11,
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
            bodyFontSize: 11,
            titleFontSize: 11,
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
            bodyFontSize: 11,
            titleFontSize: 11,
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

        $scope.allCount = $scope.allCount + apmeklejumi.length;

        apmeklejumi.forEach(function (val) {
            labels.push(val.Date);

            if (val.Attended === "1") {
                c.push($scope.iemesluKrasas["Ieradās"]);
                newLabels[val.Date] = "Ieradās";
            } else {
                c.push($scope.iemesluKrasas["Neieradās"]);
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

                        $scope.pieLabels.push(reason);
                        if (reason === "Ieradās") {
                            $scope.pieColors.push($scope.iemesluKrasas["Ieradās"]);
                        } else {
                            $scope.pieColors.push($scope.iemesluKrasas["Neieradās"]);
                            $scope.unattendedCount = $scope.unattendedCount + response.data.NotAttendedCountReasons[reason];
                        }
                        
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

            var baseLength = 20;
            if (response.data.Apmeklejumi2013.length > 25 || response.data.Apmeklejumi2014.length > 25 || response.data.Apmeklejumi2015.length > 25 || response.data.Apmeklejumi2016.length > 25) {
                baseLength = 15;
            }

            if (response.data.Apmeklejumi2013.length > 0) {
                var width2013;
                if ($scope.data2013labels.length < 5) {
                    width2013 = $scope.data2013labels.length * 30;
                } else {
                    width2013 = $scope.data2013labels.length * baseLength;
                }
                
                $scope.style2013 = {
                    height: '110px',
                    width: width2013 + 'px'
                }
                $scope.show2013 = true;
            }

            if (response.data.Apmeklejumi2014.length > 0) {
                var width2014;
                if ($scope.data2014labels.length < 5) {
                    width2014 = $scope.data2014labels.length * 30;
                } else {
                    width2014 = $scope.data2014labels.length * baseLength;
                }

                $scope.style2014 = {
                    height: '100px',
                    width: width2014 + 'px'
                }
                $scope.show2014 = true;
            }

            if (response.data.Apmeklejumi2015.length > 0) {
                var width2015;
                if ($scope.data2015labels.length < 5) {
                    width2015 = $scope.data2015labels.length * 30;
                } else {
                    width2015 = $scope.data2015labels.length * baseLength;
                }

                $scope.style2015 = {
                    height: '100px',
                    width: width2015 + 'px'
                }
                $scope.show2015 = true;
            }

            if (response.data.Apmeklejumi2016.length > 0) {
                var width2016;
                if ($scope.data2016labels.length < 5) {
                    width2016 = $scope.data2016labels.length * 30;
                } else {
                    width2016 = $scope.data2016labels.length * baseLength;
                }

                $scope.style2016 = {
                    height: '100px',
                    width: width2016 + 'px'
                }
                $scope.show2016 = true;
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