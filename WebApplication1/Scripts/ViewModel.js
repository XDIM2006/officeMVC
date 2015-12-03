var viewModel =
    {
        CountOfElementForThread: ko.observable(0),
        CountOfImages: ko.observable(0),
        Images: ko.observableArray(),
        SelectedImages: ko.observableArray(),
        SelectImages: function () {
                this.SelectedImages(document.getElementById('uploadFile').files);
        },
        Upload: function(){

            $.ajax({
                type: "POST",
                url: '@Url.Action("Upload", "Home")',
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {                    
                    $('#list').append(result)
                },
                error: function (xhr, status, p3) {
                    alert(xhr.responseText);
                }
            });
        
        }

    }

ko.applyBindings(viewModel);