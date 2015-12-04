function ResizeSettingsClass(Name, Width, Height) {
    var self = this;
    self.Name = Name;
    self.Width = Width;
    self.Height = Height;
}

var viewModel =
    {
        // size of set of images for server processing
        CountOfImages: ko.observable(10),
        //images are chose on client 
        Images: ko.observableArray(),
        Name : ko.observable('large'),
        Width : ko.observable(1000),
        Height : ko.observable(1000),
        CustomResizeSettings: ko.observableArray([new ResizeSettingsClass("_thumb", 20, 20)]),       

        AddCustomResizeSetting: function () {
            var self = this;
            var ResizeSettingsModel = new ResizeSettingsClass(self.Name(), self.Width(), self.Height());
            $.ajax({
                url: 'Home/AddSetting',
                type: "POST",
                data: ko.toJSON(ResizeSettingsModel),
                contentType: 'application/json; charset=utf-8',
                success: function (ResizeSettingsModel) {
                    self.CustomResizeSettings.push(ResizeSettingsModel);
                },
                error: function (xhr, status, p3) {
                    alert(xhr.responseText);
                }
            });
        },

        IsImageExsist: function (image) {
            var result = ko.utils.arrayFirst(this.Images(), function (item) {
                return item.name === image.name && item.size === image.size && item.type === image.type;
            });
            return (result!=null)?true:false
        },
        //images are sent to server 
        ProcessingImages: ko.observableArray(),
        //images are processed on server 
        ProcessedImages: ko.observableArray(),
        
        // select images in input field
        SelectImages: function () {
            var files = document.getElementById('uploadFile').files;
            for (var i = 0; i < files.length; i++) {
                if (!this.IsImageExsist(files[i])) {
                    this.Images.push(files[i]);
                }
                else { alert(files[i].name + " is already there");}
            }

        },

        SentToServer: function () {
            var self = this;
            var formData = new FormData();
            var localCountOfImages = (self.Images().length > self.CountOfImages()) ? self.CountOfImages() : self.Images().length;
            for (var i = 0; i < localCountOfImages; i++) {
                var item = self.Images.shift();
                self.ProcessingImages.push(item);
                formData.append("file" + i, item)
            }
            self.save = function () {
                $.ajax("/echo/json/", {
                    data: {
                        json: ko.toJSON({
                            tasks: this.tasks
                        })
                    },
                    type: "POST",
                    dataType: 'json',
                    success: function (result) {
                        alert(ko.toJSON(result))
                    }
                });
            };
            $.ajax({
                url: 'Home/Upload',
                type: "POST",
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data) {                
                     if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            self.ProcessingImages.remove(function (item) { return item.name == data[i].OriginalFile.fileName; });
                            self.ProcessedImages.push(data[i]);
                        }
                    }

                },
                error: function (xhr, status, p3) {
                    alert(xhr.responseText);
                    var length = self.ProcessingImages().length;
                    for (var i = 0; i < length; i++) {
                        self.Images.push(self.ProcessingImages.shift());
                    }
                }
            });
        }
    }

ko.applyBindings(viewModel);


