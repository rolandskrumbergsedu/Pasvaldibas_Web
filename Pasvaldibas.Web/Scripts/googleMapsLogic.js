function initMap() {

    var pasvaldibas;
    $.get("https://pasvaldibas.azurewebsites.net/api/pasvaldiba/", function (data, status) {
        pasvaldibas = data;

        var latvia = { lat: 56.8107518, lng: 24.5707024 };

        var map = new google.maps.Map(document.getElementById('map'), {
            zoom: 7,
            center: latvia
        });

        pasvaldibas.forEach(function (value) {

            if (value.Latitude && value.Longtitude) {

                var link = "/Overview/Index/" + value.Code;

                var marker = new google.maps.Marker({
                    position: {
                        lat: value.Latitude,
                        lng: value.Longtitude
                    },
                    map: map,
                    title: value.Name,
                    url: link
                });
                
                var contentString = '<h3>' + value.Name + '</h3><br/><p>Nospied uz marķiera, lai apskatītu pašvaldības sēžu apmeklējumu</p>';
                var infowindow = new google.maps.InfoWindow({
                    content: contentString
                });

                google.maps.event.addListener(marker, 'click', function () {
                    window.location.href = this.url;
                });

                marker.addListener('mouseover', function () {
                    infowindow.open(map, this);
                });

                marker.addListener('mouseout', function () {
                    infowindow.close();
                });
            }
        });

    });

    
}