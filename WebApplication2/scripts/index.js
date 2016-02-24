
var app = angular.module('myApp', ['ngSanitize']);

app.controller('MyCtrl', function ($scope, $http) {
    $scope.isValid = 0;
    $scope.totalIssue = 0;
    //github repository url
    $scope.url = '';

    //list declaration to hold git repository details
    $scope.issueCntFor24HrsList = [];
    $scope.issueCnt24HrsTo7DaysList = [];
    $scope.issueCntMoreThan7DaysList = [];
    //end of list declaration

    //get repository details function
    $scope.getDetails = function () {
        //displays blockUI while fetching git repository
        $.blockUI({
            message: '<h1>Fetching github repository.</h1>',
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .5,
                color: '#fff'
            }
        });
        //end of blockUI

        //call web api to get repository details
        $http.get("/api/Shippable/GetDetails?url=" + $scope.url)
            .then(function (response) {
                //mark as valid response
                $scope.isValid = 1;
                $scope.totalIssue = 0;
                var data = response.data;
                //count total issues
                for (var i = 0; i < data.length; i++) {
                    $scope.totalIssue += data[i].length;
                }

                //assign list returned by web api call
                $scope.issueCntFor24HrsList = data[0];
                $scope.issueCnt24HrsTo7DaysList = data[1];
                $scope.issueCntMoreThan7DaysList = data[2];
                //end of assign list

                //unblock blockUI after fetching github repository
                $.unblockUI();
            }, function error(response) {
                //mark as invalid response
                $scope.isValid = 0;

                //clear list
                $scope.issueCntFor24HrsList = [];
                $scope.issueCnt24HrsTo7DaysList = [];
                $scope.issueCntMoreThan7DaysList = [];
                //end clear list

                //unblock blockUI
                $.unblockUI();

                //displays blockUI displaying message as Invalid URL
                $.blockUI({
                    message: '<h1>Invalid URL</h1>',
                    css: {
                        border: 'none',
                        padding: '15px',
                        backgroundColor: '#000',
                        '-webkit-border-radius': '10px',
                        '-moz-border-radius': '10px',
                        opacity: .5,
                        color: '#fff'
                    }
                });
                //unblockUI blockUI after 2s
                setTimeout($.unblockUI, 2000);

            });
    }

});

