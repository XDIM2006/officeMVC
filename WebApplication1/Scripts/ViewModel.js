function ResizeSettingsClass(Name, Width, Height) {
    var self = this;
    self.Name = Name;
    self.Width = Width;
    self.Height = Height;
}
function ImageModel(o) {
    var self = this;
    self.filePath = o.filePath;
    self.fileName = o.fileName;
    self.StartResize = o.StartTime;
    self.FinishResize = o.FinishTime;
}
function WorkModel(o) {
    var self = this;
    self.OriginalFile = new ImageModel(o.OriginalFile);
    self.PreviewFile = new ImageModel(o.PreviewFile);
    self.Files = ko.observableArray();
    for (var i = 0; i < o.Files.length; i++) {
        self.Files.push(new ImageModel(o.Files[i]));
    }
}

function generateUUID() {
    var d = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
};

var viewModel =
    {
        uuid : generateUUID(),
        // size of set of images for server processing
        CountOfImages: ko.observable(10),
        //images are chose on client 
        Images: ko.observableArray(),
        Name : ko.observable('_large'),
        Width : ko.observable(1000),
        Height : ko.observable(1000),
        CustomResizeSettings: ko.observableArray(),       

        AddCustomResizeSetting: function () {
            var self = this;
            var ResizeSettingsModel = new ResizeSettingsClass(self.Name(), self.Width(), self.Height());
            $.ajax({
                url: 'Home/AddSetting' + '?uuid=' + self.uuid,
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
            $.ajax({
                url: 'Home/Upload' + '?uuid=' + self.uuid,
                type: "POST",
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data) {                
                     if (data.length > 0) {
                         for (var i = 0; i < data.length; i++) {
                             var model = new WorkModel(data[i]);
                             self.ProcessingImages.remove(function (item) { return item.name == model.OriginalFile.fileName; });
                             self.ProcessedImages.push(model);
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


