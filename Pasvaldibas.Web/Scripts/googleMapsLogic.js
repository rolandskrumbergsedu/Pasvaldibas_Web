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
                
                var contentString = '<h3>' + value.Name + '</h3><br/><a href="' + link + '">Skatīt domes sēžu apmeklējumu</a>';
                var infowindow = new google.maps.InfoWindow({
                    content: contentString
                });
                
                marker.addListener('click',
                    function() {
                        infowindow.open(map, this);
                    });
            }
        });

    });

    
}