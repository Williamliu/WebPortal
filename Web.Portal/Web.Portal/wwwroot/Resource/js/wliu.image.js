var WLIU = WLIU || {};
WLIU.Gallery = function (gallery) {
    if ($.isPlainObject(gallery)) {
        this.scope          = null;
        this.method         = gallery.method || "none";
        this.getUrl         = gallery.getUrl || "";
        this.saveUrl        = gallery.saveUrl || "";
        this.view           = gallery.view || "small";
        this.edit           = gallery.edit || "large";
        this.size           = {};
        this.size           = gallery.size;
        this.navi           = new WLIU.Navi(gallery.navi);
        this.error          = new WLIU.Error(gallery.error);
        this.key            = gallery.key || 0;
        this.maxCount       = gallery.maxCount || 0;
        this.images         = [];
        this.scanner        = null;
    } else {
        this.scope          = null;
        this.method         = "none";
        this.getUrl         = "";
        this.saveUrl        = "";
        this.view           = "small";
        this.edit           = "large";
        this.size           = {};
        this.navi           = new WLIU.Navi();
        this.error          = new WLIU.Error();
        this.key            = 0;
        this.maxCount       = 0;
        this.images         = [];
        this.scanner        = null;
    }
};
WLIU.Gallery.prototype = {
    Print: function (html) {
        var w1 = window.open("output.html");
        var html_str = '<html style="margin:0px;padding:0px;"><head><style type="text/css" media="print">@page { margin: 0mm; } html{margin:0px;} body{margin:0px;font-family:FangSong,Arial;} img[src=""]{display:none;margin:0px;}</style><head><body><center>';
        w1.document.open();
        w1.document.write(html_str);
        w1.document.write(html);
        w1.document.write('</center></html>');
        w1.document.close();
        w1.print();
    },
    startScanner: function (callback) {
        var self = this;
        if (self.scanner) return;
        self.scanner = new Instascan.Scanner({ video: document.getElementById('scannerElement') });
        self.scanner.addListener('scan', function (content) {
            // close diag should be in callback, more flexible
            //$("#wliuDiagScanner").diag("hide");
            // callback send QR Code to callback handler
            if (callback) if ($.isFunction(callback)) callback(content);
            // place to callback 
            //self.scanner.stop();
            //self.scanner = null;
        });

        Instascan.Camera.getCameras().then(function (cameras) {
            if (cameras.length > 0) {
                self.scanner.start(cameras[0]);
                //if (self.scanner) show scanner
            } else {
                self.ShowCameraError();
            }
        }).catch(function (e) {
            self.ShowCameraError();
        });
    },
    stopScanner: function () {
        if (this.scanner) {
            this.scanner.stop();
            this.scanner = null;
            $("#wliuDiagScanner").float("hide");
        }
    },
    InitCamera: function () {
        let self = this;
        if(this.GalleryAjaxErrorHandle()) return;
        if ($("#videoElement").length > 0) {
            var video = document.querySelector("#videoElement");
            if (navigator.mediaDevices.getUserMedia) {
                navigator.mediaDevices.getUserMedia({ video: true })
                    .then(stream=>{
                        video.srcObject = stream;
                        video.play();
                    })
                    .catch(e => {
                        self.ShowCameraError();
                    });
            }
        }
    },
    StopCamera: function () {
        try {
            if (this.GalleryAjaxErrorHandle()) return;
            if ($("#videoElement").length > 0) {
                let video = document.querySelector("#videoElement");
                if (video.srcObject) {
                    let stream = video.srcObject;
                    let tracks = stream.getTracks();
                    for (let i = 0; i < tracks.length; i++) {
                        let track = tracks[i];
                        track.stop();
                    }
                    video.srcObject = null;
                }
            }
        }
        catch (e)
        {
            console.log(e);
        }
    },
    SnapShot: function () {
        let self = this;
        try {
            if (self.CurrentImage()) {
                if (this.GalleryAjaxErrorHandle()) return;
                if ($("#videoElement").length > 0) {
                    let video = document.querySelector("#videoElement");
                    let canvas = document.createElement("canvas");
                    let context = canvas.getContext('2d');
                    canvas.width = video.videoWidth;
                    canvas.height = video.videoHeight;
                
                    context.drawImage(video, 0, 0, video.videoWidth, video.videoHeight);
                    let imgDataURL = canvas.toDataURL(self.CurrentImage().mime_type);
                    self.CurrentImage().data["origin"].w        = canvas.width;
                    self.CurrentImage().data["origin"].h        = canvas.height;
                    self.CurrentImage().data["origin"].size     = imgDataURL.length;
                    self.CurrentImage().data["origin"].content  = imgDataURL;
                    canvas = null;
                    self.StopCamera();
                    self.CurrentImage().resetOrigin().then(d => {
                        if (self.CurrentImage().state <= 1) self.CurrentImage().state = 1;
                        if (self.CurrentImage().state === 3) self.CurrentImage().state = 1; 
                        self.Apply();
                        $("#wliuDiagCamera").diag("hide");
                        self.ShowEditor();
                    }).catch(e => {
                        if (self.CurrentImage().state !== 2) self.CurrentImage().state = 3; 
                        self.Apply();
                    });
                }
            }
        }
        catch (e) {
            console.log(e);
        }
    },
    ShowEditor: function () {
        let self = this;
        $("#img_editor", "#wliuDiagImageEditor").attr("src", self.CurrentContent(self.edit));
        $("div.wliu-image-crop", "#wliuDiagImageEditor").css({ left: "5%", top: "5%", width: "90%", height: "90%" });
    },
    ShowCameraError: function () {
        $("#wliuDiagTableError").diag({
            content: [
                '<center>' + Words("error.remind") + '</center>',
                '<span style="color:orange;">' + Words("error.code") + '</span> : <span style="color:orange;">101</span><br>',
                Words("image.camera.error")
            ].join('')
        }).diag("show");
    },
    ShowComment: function () {
        if (this.GalleryAjaxErrorHandle()) return;
        if ($("#wliuDiagImageText").length > 0) {
            $("#wliuDiagImageText").diag("show");
            return false;
        }
    },
    ShowMainComment: function () {
        if (this.GalleryAjaxErrorHandle()) return;
        if ($("#wliuDiagImageMainText").length > 0) {
            $("#wliuDiagImageMainText").diag("show");
            return false;
        }
    },
    Clear: function () {
        this.key        = 0;
        this.method     = "none";
        this.error      = new WLIU.Error();
        this.navi       = new WLIU.Navi();
        this.images     = [];
    },
    NewRowB: function () {
        if (this.images.length >= this.maxCount) {
            return;
        }
        if (this.GalleryAjaxErrorHandle()) return;
        let nimg = new WLIU.Image();
        nimg.refKey = this.key || 0;
        nimg.state = 2;
        nimg.size = this.size;
        nimg.data = {};
        for (let sizeName in nimg.size) {
            nimg.data[sizeName] = {};
            nimg.data[sizeName].content = "";
        }
        this.images.unshift(nimg);
        this.navi.guid = nimg.guid;
        return nimg;
    },
    NewRowA: function () {
        if (this.images.length >= this.maxCount) {
            return;
        }
        if (this.GalleryAjaxErrorHandle()) return;
        let nimg = new WLIU.Image();
        nimg.refKey = this.key || 0;
        nimg.state = 2;
        nimg.size = this.size;
        nimg.data = {};
        for (let sizeName in nimg.size) {
            nimg.data[sizeName] = {};
            nimg.data[sizeName].content = "";
        }
        this.images.push(nimg);
        this.navi.current = nimg.guid;
        return nimg;
    },
    CurrentGuid: function () {
        return this.navi.guid;
    },
    CurrentIndex: function () {
        return $.indexInList(this.images, { guid: this.navi.guid });
    },
    CurrentState: function () {
        if (this.images[this.CurrentIndex()]) {
            return this.images[this.CurrentIndex()].state;
        }
        return 0;
    },
    CurrentImage: function () {
        if (this.images[this.CurrentIndex()]) {
            return this.images[this.CurrentIndex()];
        }
        return null;
    },
    GuidImage: function (p_guid) {
        return $.findInArray(this.images, { guid: p_guid });
    },
    GuidIndex: function (p_guid) {
        return $.indexInList(this.images, { guid: p_guid });
    },
    CurrentContent: function (sizeName) {
        if (this.GalleryAjaxErrorHandle()) return;
        if (this.CurrentImage())
            if (this.CurrentImage().data[sizeName])
                return this.CurrentImage().data[sizeName].content;

        return "";
    },
    GuidContent: function (guid, sizeName) {
        if (this.GalleryAjaxErrorHandle()) return;
        if (this.GuidImage(guid))
            if (this.GuidImage(guid).data[sizeName])
                return this.GuidImage(guid).data[sizeName].content;
        return "";
    },
    SetKey: function (refKey) {
        this.key = refKey;
        for (let idx in this.images) {
            this.images[idx].refKey = this.key;  // important
        }
    }, 
    SetCurrent: function (guid) {
        this.navi.guid = guid;
    },
    Init: function (url) {
        let defer = $.Deferred();

        let self = this;
        self.method = "init";
        self.navi.isLoading = 1;

        AJAX.Get(url, "get")
            .then(data => {
                self.navi.isLoading = 0;
                self.SyncInitGallery(data);
                self.GalleryAjaxErrorHandle();
                if (self.error.HasError() === false)
                    defer.resolve(self);
                else
                    defer.reject(self);
                self.Apply();
            })
            .catch(data => {
                self.navi.isLoading = 0;
                self.error = new WLIU.Error(data.error);
                self.GalleryAjaxErrorHandle();
                defer.reject(self);
                self.Apply();
            });

        return defer.promise();
    },
    Get: function (main) {
        let defer = $.Deferred();
        main = main || 0;
        let self = this;
        self.method = "get";
        self.navi.isLoading = 1;
        let url = `${self.getUrl}/${this.key}?main=${main}&view=${self.view}&edit=${self.edit}`;
        AJAX.Get(url)
            .then(data => {
                self.navi.isLoading = 0;
                self.SyncGetGallery(data);
                self.GalleryAjaxErrorHandle();
                if (self.error.HasError() === false)
                    defer.resolve(self);
                else
                    defer.reject(self);
                self.Apply();
            })
            .catch(data => {
                self.navi.isLoading = 0;
                self.error = new WLIU.Error(data.error);
                self.GalleryAjaxErrorHandle();
                defer.reject(self);
                self.Apply();
            });
        return defer.promise();
    },
    GalleryAjaxErrorHandle: function () {
        let self = this;
        if (self.method === "save") $("div[wliu][loading][method='save']").loading("hide");
        if (self.method === "get") $("div[wliu][loading][method='get']").loading("hide");

        if (self.error.HasError()) {
            $("#wliuHint").hint("show", "error", (self.method === "save" ? Words("save.error") : Words("get.error")));
            $("#wliuDiagTableError").diag({
                close: function () {
                    if (self.error.code >= 9999) {
                        if (self.error.memo !== "") window.location.href = self.error.memo;
                    }
                },
                content: [
                    '<center>' + Words("error.remind") + '</center>',
                    '<span style="color:orange;">' + Words("error.code") + '</span> : <span style="color:orange;">' + self.error.code + '</span><br>',
                    self.error.Message().nl2br()
                ].join('')
            }).diag("show");
            self.method = "none";
            return true;
        }
        else {
            if (self.method === "save") $("#wliuHint").hint("show", "success", Words("save.success"));
            self.method = "none";
            return false;
        }
    },
    LinkScope: function (p_scope) {
        p_scope.Gallery = this;
        this.scope = p_scope;
    },
    AddImage: function (image) {
        this.images.push(image);
    },
    AddOImage: function (oimage) {
        let nimg = new WLIU.Image(oimage);
        nimg.size = this.size;
        nimg.data["origin"].content = nimg.data[this.edit].content; 
        this.images.push(nimg);
    },
    AddOImages: function (oimages) {
        this.images = [];
        if ($.isArray(oimages)) {
            for (let idx in oimages) {
                this.AddOImage(oimages[idx]);
            }
        }
        return this;
    },
    // Image Operation 
    FromFile: function (event) {
        if (this.CurrentImage()) {
            if (this.GalleryAjaxErrorHandle()) return;
            files = (event.srcElement || event.target).files;
            if (!files) return;
            if (files.length <= 0) return;
            let self = this;

            this.CurrentImage().fromFile(files[0]).then(d => {
                self.Apply();
                self.ShowEditor();
            }).catch(e => {
                self.Apply();
            });
        }
    },
    RotateImage: function () {
        if (this.GalleryAjaxErrorHandle()) return;
        let self = this;
        if (this.CurrentImage()) {
            this.CurrentImage().rotateAll().then(d => {
                self.Apply();
                self.ShowEditor();
            }).catch(e => {
                self.Apply();
            });
        }
    },
    CropImage: function () {
        if (this.GalleryAjaxErrorHandle()) return;
        let self = this;
        if (this.CurrentImage()) {
            let ww = $("img#img_editor", "#wliuDiagImageEditor").width();
            let hh = $("img#img_editor", "#wliuDiagImageEditor").height();
            let x = $("div.wliu-image-crop", "#wliuDiagImageEditor").position().left;
            let y = $("div.wliu-image-crop", "#wliuDiagImageEditor").position().top;
            let nw = $("div.wliu-image-crop", "#wliuDiagImageEditor").outerWidth();
            let nh = $("div.wliu-image-crop", "#wliuDiagImageEditor").outerHeight();
            //console.log(ww + " : " + hh + " : " + x + " : " + y + " : " + nw + " : " + nh);
            this.CurrentImage().cropAll(ww, hh, x, y, nw, nh).then(d => {
                self.Apply();
                self.ShowEditor();
            }).catch(e => {
                self.Apply();
            });
        }
    },
    ResetImage: function () {
        if (this.GalleryAjaxErrorHandle()) return;
        let self = this;
        if (this.CurrentImage()) {
            this.CurrentImage().resetOrigin().then(d => {
                self.Apply();
                self.ShowEditor();
            }).catch(e => {
                self.Apply();
            });
        }
    },
    Remove: function (guid) {
        if (this.GalleryAjaxErrorHandle()) return;
        if (this.GuidImage(guid)) {
            this.Detach(guid);
            if (this.GuidImage(guid).state===2) $.removeInList(this.images, { guid: guid });
            this.navi.guid = this.images.length > 0 ? this.images[0].guid : "";
        }
    },
    Empty: function (guid) {
        if (this.GalleryAjaxErrorHandle()) return;
        if (this.GuidImage(guid))
                this.Remove(guid);

        if (this.images.length > 0)
            this.navi.guid = this.rows[0].guid;
        else
            this.NewRowB();
    },
    Detach: function (guid) {
        if (this.GalleryAjaxErrorHandle()) return;
        if (this.GuidImage(guid)) {
            this.GuidImage(guid).Detach();
            return this.GuidImage(guid);
        }
    },
    Cancel: function (guid) {
        if (this.GuidImage(guid)) {
            let self = this;
            this.GuidImage(guid).Cancel().then(d => {
                self.Apply();
            }).catch(e => {
                self.Apply();
            });
        }
    },
    CurrentError: function () {
        if (this.CurrentImage())
            return this.CurrentImage().error;
        else
            return new WLIU.Error();
    },
    GuidError: function (guid) {
        if (this.GuidImage(guid))
            return this.GuidImage(guid).error;
        else
            return new WLIU.Error();
    },
    Apply: function () {
        if (this.scope) {
            if (!this.scope.$root.$$phase) this.scope.$apply();
        }
    },

    SyncInitGallery: function (gallery) {
        if (gallery) {
            let getError = false;
            // Sync Error
            this.error = new WLIU.Error(gallery.error);
            if (this.error.HasError()) getError = true;
            if (getError === false) {
                this.view = gallery.view;
                this.edit = gallery.edit;
                this.getUrl = gallery.getUrl;
                this.saveUrl = gallery.saveUrl;
                this.key = gallery.key;
                this.maxCount = gallery.maxCount;
                // If get data no error,  sync navi and add rows
                this.rowGuid = gallery.rowGuid;
                this.navi = new WLIU.Navi(gallery.navi);
                this.size = gallery.size;
                this.AddOImages(gallery.images);
            }
        }
    },
    SyncGetGallery: function (gallery) {
        if (gallery) {
            let getError = false;
            // Sync Error
            this.error = new WLIU.Error(gallery.error);
            if (this.error.HasError()) getError = true;
            if (getError === false) {
                // If get data no error,  sync navi and add rows
                this.rowGuid = gallery.rowGuid;
                this.navi = new WLIU.Navi(gallery.navi);
                this.AddOImages(gallery.images);
            }
        }
    },
    SaveImages: function () {
        let self = this;
        let defer = $.Deferred();
        if ($.isArray(self.images)) {
            let tasks = [];
            for (let idx = 0; idx < self.images.length; idx++) {
                self.images[idx].refKey = this.key; //important
                let task = self.images[idx].SaveAjax(self.saveUrl, self.images[idx].Progress);
                task.then(d => {
                    for (let idx = self.images.length - 1; idx >= 0; idx--) {
                        if (self.images[idx].state===3)
                            $.removeByIndex(self.images, idx);
                    }
                });
                tasks.push(task);
            }
            self.method = "save";
            $("div[wliu][loading][method='save']").loading("show");
            Promise.all(tasks).then(d => {
                self.GalleryAjaxErrorHandle();
                defer.resolve(self);
                self.Apply();
            }).catch(e => {
                self.GalleryAjaxErrorHandle();
                defer.reject(self);
                self.Apply();
            });
        }
        return defer.promise();
    },
    SaveImage: function (guid, main) {
        let self = this;
        let defer = $.Deferred();
        if (self.GuidImage(guid)) {
            if (self.GuidImage(guid).refKey > 0) {
                self.method = "save";
                $("div[wliu][loading][method='save']").loading("show");
                self.GuidImage(guid).SaveAjax(self.saveUrl, self.GuidImage(guid).Progress)
                    .then(d => {
                        self.error = Clone(d.error);
                        self.GalleryAjaxErrorHandle();
                        if (self.GuidImage(d.guid)) {
                            if (self.GuidImage(d.guid).state === 3) {
                                $.removeInList(self.images, { guid: d.guid });
                                if (main) self.NewRowB();
                            }
                        }
                        defer.resolve(self);
                        self.Apply();
                    })
                    .catch(e => {
                        self.GalleryAjaxErrorHandle();
                        defer.reject(self);
                        self.Apply();
                    });
            }
        }
        return defer.promise();
    },
    SaveCurrentText: function () {
        let self = this;
        let defer = $.Deferred();
        $("div[wliu][loading][method='save']").loading("show");
        if (self.CurrentImage()) {
            self.method = "save";
            self.CurrentImage().TextAjax(self.saveUrl).then(d => {
                self.GalleryAjaxErrorHandle();
                defer.resolve(self);
                self.Apply();
            }).catch(e => {
                self.GalleryAjaxErrorHandle();
                defer.reject(self);
                self.Apply();
            });
        }
        return defer.promise();
    }
};


WLIU.Image = function (image) {
    if (image) {
        this.id = image.id || 0;
        this.refKey = image.refKey || 0;
        this.guid = image.guid || CreateUUID();
        this.state = image.state || 0;
        this.isLoading = false;
        this.title_en = image.title_en || "";
        this.title_cn = image.title_cn || "";
        this.detail_en = image.detail_en || "";
        this.detail_cn = image.detail_cn || "";
        this.full_name = image.full_name || "";
        this.short_name = image.short_name || "";
        this.ext_name = image.ext_name || "";
        this.mime_type = image.mime_type || "image/jpeg";
        this.size = image.size || {};
        this.data = image.data || {};
        this.current = this.data["large"]?this.data["large"].content:"";
        this.status = image.status || false;
        this.main = image.main || false;
        this.sort = image.sort || 0;
        this.error = new WLIU.Error(image.error);
    } else {
        this.id = 0;
        this.refKey = 0;
        this.guid = CreateUUID();
        this.state = 0;
        this.isLoading = false;
        this.title_en = "";
        this.title_cn = "";
        this.detail_en = "";
        this.detail_cn = "";
        this.full_name = "";
        this.short_name = "";
        this.ext_name = "";
        this.mime_type = "image/jpeg";
        this.size = {};
        this.data = {};
        this.current = "";
        this.status = false;
        this.main = false;
        this.sort = 0;
        this.error = new WLIU.Error();
    }
};

WLIU.Image.prototype = {
    fromFile: function (file) {
        let self = this;
        let defer = $.Deferred();
        if (this.size["origin"]) {
            this.full_name = file.name.fileName();
            this.short_name = file.name.shortName();
            this.ext_name = file.name.extName();
            this.mime_type = file.type;

            this.file2Origin(file).then(d => {
                self.error.Reset();
                if (self.state <= 1) self.state = 1; 
                if (self.state===3) self.state = 1;
                defer.resolve(d);
            }).catch(e => {
                self.error.Reset();
                if (self.state !== 2) self.state = 3;
                self.error.Append(101, Words("image.invalid"));
                self.ImageErrorHandle();
                defer.reject(e);
            });
        }
        else {
            self.error.Reset();
            self.error.Append(101, Words("image.invalid"));
            self.ImageErrorHandle();
            defer.reject("");
        }
        return defer.promise();
    },
    file2DataURL: function (file) {
        let defer = $.Deferred();
        let fs = new FileReader();
        fs.onload = function (ev1) {
            let data = ev1.target.result;
            defer.resolve(data);
        };
        fs.onerror = function () {
            defer.reject("");
        };
        fs.readAsDataURL(file);
        return defer.promise();
    },
    file2Origin: function (file) {
        let self = this;
        let defer = $.Deferred();
        let fs = new FileReader();
        fs.onload = function (ev) {
            let data = ev.target.result;
            self.initOrigin(data).then(d => {
                self.resizeAllFromOrigin().then(d => {
                    defer.resolve(self);
                }).catch(e => {
                    defer.reject("");
                });
            }).catch(e => {
                defer.reject("");
            });
        };
        fs.onerror = function (ev) {
            let data = ev.target.result;
            defer.reject("");
        };
        fs.readAsDataURL(file);
        return defer.promise();
    },
    resizeAllFromOrigin: function () {
        let self = this;
        let defer = $.Deferred();
        if (this.size["origin"]) {
            if (this.data["origin"].content) {
                let d_large = this.resizeImage("origin", "large");
                let d_medium = this.resizeImage("origin", "medium");
                let d_small = this.resizeImage("origin", "small");
                let d_tiny = this.resizeImage("origin", "tiny");

                $.when(d_large, d_medium, d_small, d_tiny).then((d1, d2, d3, d4) => {
                    defer.resolve(self);
                }, (e1, e2, e3, e4) => {
                    self.error.Reset();
                    self.error.Append(101, Words("image.invalid"));
                    self.ImageErrorHandle();
                    defer.reject("");
                });
            }
            else
            {
                this.data["large"].content = "";
                this.data["medium"].content = "";
                this.data["small"].content = "";
                this.data["tiny"].content = "";
                defer.resolve(self);
            }
        }
        else
        {
            self.error.Reset();
            self.error.Append(101, Words("image.invalid"));
            self.ImageErrorHandle();
            defer.reject("");
        }
        return defer.promise();
    },
    resizeAllFromLarge: function () {
        let self = this;
        let defer = $.Deferred();
        let d_medium = this.resizeImage("large",    "medium");
        let d_small = this.resizeImage("large",     "small");
        let d_tiny = this.resizeImage("large",      "tiny");

        $.when(d_medium, d_small, d_tiny).then((d1, d2, d3, d4) => {
            defer.resolve(self);
        }, (e1, e2, e3, e4) => {
            defer.reject("");
        });
        return defer.promise();
    },
    resizeImage: function (fromSize, toSize) {
        let self = this;
        let defer = $.Deferred();
        let img = new Image();
        img.onload = function () {
            let size = self.size[toSize];
            let canvas = document.createElement("canvas");
            let ctx = canvas.getContext("2d");
            let ratio_ww = 1;
            let ratio_hh = 1;
            if (self.scale) {
                if (size.w > 0) ratio_ww = size.w / img.width;
                if (size.h > 0) ratio_hh = size.h / img.height;
            } else {
                if (size.w > 0 && img.width > size.w) ratio_ww = size.w / img.width;
                if (size.h > 0 && img.height > size.h) ratio_hh = size.h / img.height;
            }
            let ratio = Math.min(ratio_ww, ratio_hh);
            canvas.width = img.width * ratio;
            canvas.height = img.height * ratio;
            ctx.drawImage(img, 0, 0, img.width, img.height, 0, 0, canvas.width, canvas.height);

            let imgDataURL = canvas.toDataURL(self.mime_type);
            self.data[toSize].content = imgDataURL;
            canvas = null;
            defer.resolve(self.data[toSize].content);
        };
        img.onerror = function () {
            defer.reject("");
        };
        img.src = self.data[fromSize].content;
        return defer.promise();
    },

    // sizeName: "origin", "large","medium","small","tiny"; size: {w, h}
    initOrigin: function (dataUrl) {
        let self = this;
        let defer = $.Deferred();
        let img = new Image();
        img.onload = function () {
            let size = self.size["origin"];
            let canvas = document.createElement("canvas");
            let ctx = canvas.getContext("2d");
            let ratio_ww = 1;
            let ratio_hh = 1;
            if (self.scale) {
                if (size.w > 0) ratio_ww = size.w / img.width;
                if (size.h > 0) ratio_hh = size.h / img.height;
            } else {
                if (size.w > 0 && img.width > size.w) ratio_ww = size.w / img.width;
                if (size.h > 0 && img.height > size.h) ratio_hh = size.h / img.height;
            }
            let ratio = Math.min(ratio_ww, ratio_hh);
            canvas.width    = img.width * ratio;
            canvas.height   = img.height * ratio;
            ctx.drawImage(img, 0, 0, img.width, img.height, 0, 0, canvas.width, canvas.height);

            let imgDataURL = canvas.toDataURL(self.mime_type);
            self.data["origin"].content = imgDataURL;
            canvas = null;
            defer.resolve(self.data["origin"].content);
        };
        img.onerror = function () {
            defer.reject("");
        };
        img.src = dataUrl;
        return defer.promise();
    },
    rotateAll: function () {
        let self = this;
        let defer = $.Deferred();
        if (this.size["origin"]) {
            let d_large = this.rotateImage("large");
            let d_medium = this.rotateImage("medium");
            let d_small = this.rotateImage("small");
            let d_tiny = this.rotateImage("tiny");

            $.when(d_large, d_medium, d_small, d_tiny).then((d1, d2, d3, d4) => {
                if (self.state <= 1) self.state = 1;
                defer.resolve(self);
            }, (e1, e2, e3, e4) => {
                self.error.Reset();
                self.error.Append(101, Words("image.invalid"));
                self.ImageErrorHandle();
                defer.reject("");
            });
        }
        else
        {
            self.error.Reset();
            self.error.Append(101, Words("image.invalid"));
            self.ImageErrorHandle();
            defer.reject("");
        }
        return defer.promise();
    },
    rotateImage: function (size) {
        let self = this;
        let defer = $.Deferred();

        let degree = 90;
        let fromImg = self.data[size];

        let t_img = new Image();
        t_img.onload = function () {
            let canvas = document.createElement("canvas");
            let ctx = canvas.getContext("2d");
            // important: different 180 and 90
            if (degree % 180 === 0) {
                canvas.width = t_img.width;
                canvas.height = t_img.height;
            } else {
                canvas.width = t_img.height;
                canvas.height = t_img.width;
            }
            ctx.translate(canvas.width / 2, canvas.height / 2);
            ctx.rotate(degree * Math.PI / 180);
            ctx.drawImage(t_img, - t_img.width / 2, -t_img.height / 2);
            let imgDataURL = canvas.toDataURL(self.mime_type);
            fromImg.content = imgDataURL;
            canvas = null;
            defer.resolve(fromImg.content);
        };
        t_img.onerror = function () {
            defer.reject("");
        };
        t_img.src = fromImg.content;
        return defer.promise();
    },
    cropAll: function (ww, hh, x, y, nw, nh) {
        let self = this;
        let defer = $.Deferred();
        if (this.size["origin"]) {
            let largeImg = self.data["large"];
            if (largeImg.content !== "") {
                let t_img = new Image();
                t_img.onload = function () {
                    let ratio_ww = 1;
                    let ratio_hh = 1;
                    ratio_ww = t_img.width / ww;
                    ratio_hh = t_img.height / hh;

                    x = x * ratio_ww;
                    y = y * ratio_hh;
                    nw = nw * ratio_ww;
                    nh = nh * ratio_hh;
                    if (x < 0) x = 0;
                    if (y < 0) y = 0;
                    if (x + nw > t_img.width) nw = t_img.width - x;
                    if (y + nh > t_img.height) nh = t_img.height - y;

                    let canvas = document.createElement("canvas");
                    let ctx = canvas.getContext("2d");
                    canvas.width = nw;
                    canvas.height = nh;

                    ctx.drawImage(t_img, x, y, nw, nh, 0, 0, canvas.width, canvas.height);
                    let imgDataURL = canvas.toDataURL(self.mime_type);
                    largeImg.content = imgDataURL;

                    self.resizeAllFromLarge().then(d => {
                        if (self.state <= 1) self.state = 1;
                        defer.resolve(largeImg.content);
                    }).catch(e => {
                        self.error.Reset();
                        self.error.Append(101, Words("image.invalid"));
                        self.ImageErrorHandle();
                        defer.reject("");
                    });
                };
                t_img.onerror = function () {
                    self.error.Reset();
                    self.error.Append(101, Words("image.invalid"));
                    self.ImageErrorHandle();
                    defer.reject("");
                };
                t_img.src = largeImg.content;
            }
            else {
                self.error.Reset();
                self.error.Append(101, Words("image.invalid"));
                self.ImageErrorHandle();
                defer.reject("");
            }
        }
        else
        {
            self.error.Reset();
            self.error.Append(101, Words("image.invalid"));
            self.ImageErrorHandle();
            defer.reject("");
        }
        return defer.promise();
    },
    resetOrigin: function () {
        return this.resizeAllFromOrigin();
    },
    Cancel: function () {
        this.error.Reset();
        if(this.state!==2) this.state = 0;
        this.data["origin"].content = this.current;
        return this.resizeAllFromOrigin();
    },
    Detach: function () {
        if (this.state<=1) this.state = 3;
        if (this.data["origin"]) this.data["origin"].content = "";
        if (this.data["large"]) this.data["large"].content = "";
        if (this.data["medium"]) this.data["medium"].content = "";
        if (this.data["small"]) this.data["small"].content = "";
        if (this.data["tiny"]) this.data["tiny"].content = "";
    },
    Progress: function (load, total, percent) {
        //console.log("load: " + load + "  total: " + total + "  percent: " + percent);
    },
    SaveAjax: function (url, callback) {
        let self = this;
        let defer = $.Deferred();
        let send = false;
        if (self.state === 1 && self.error.HasError() === false && self.data["large"] && self.data["large"].content !== "") {
            send = true;
        }
        if (self.state === 2 && self.error.HasError() === false && self.data["large"] && self.data["large"].content !== "") {
            send = true;
        }
        if (self.state === 3) { // if deatch include new image, should be replace image
            send = true;
        }
        if (send) {
            let nimage = new WLIU.GImage(self);
            self.isLoading = true;
            this.ImageAjax(url, nimage, callback).then(data => {
                self.isLoading = false;
                self.SyncImage(data);
                self.ImageErrorHandle();
                defer.resolve(self);
            }).catch(data => {
                self.isLoading = false;
                self.error = new WLIU.Error(data.error);
                self.ImageErrorHandle();
                defer.reject(self);
            });
        }
        else
        {
            defer.resolve(self);
        }
        return defer.promise();
    },
    TextAjax: function (url, callback) {
        let self = this;
        let defer = $.Deferred();
        let send = false;
        if (self.state === 0) {
            send = true;
        }
        if (self.state === 1) {
            send = true;
        }
        if (self.state === 2) {
            send = false;
        }
        if (self.state === 3) {
            send = true;
        }
        if (send) {
            let nimage = new WLIU.GImageText(self);
            self.isLoading = true;
            this.ImageAjax(url, nimage, callback).then(data => {
                self.isLoading = false;
                self.error = new WLIU.Error(data.error);
                //self.SyncImage(data);
                self.ImageErrorHandle();
                defer.resolve(self);
            }).catch(data => {
                self.isLoading = false;
                self.error = new WLIU.Error(data.error);
                self.ImageErrorHandle();
                defer.reject(self);
            });
        }
        else {
            defer.resolve(self);
        }
        return defer.promise();
    },
    ImageAjax: function (url, nimage, callback) {
        if (url !== "") {
            return AJAX.Post(url, nimage, callback);
        } else {
            let defer = $.Deferred();
            let resp = {};
            resp.error = {};
            resp.error.code = 404;
            resp.error.messages = [Words("save.url.notfound")];
            defer.reject(resp);
            return defer.promise();
        }
    },
    SyncImage: function (gimage) {
        let self = this;
        self.error = new WLIU.Error(gimage.error);
        if (self.error.HasError() === false) {
            self.id = gimage.id;
            self.refKey = gimage.refKey;
            self.state = gimage.state;
            self.isLoading = false;
            self.data["origin"].content = self.data["large"].content;
            self.current = self.data["large"].content;
        }
    },
    ImageErrorHandle: function () {
        let self = this;
        if (self.error.HasError()) {
            $("#wliuDiagTableError").diag({
                close: function () {
                    if (self.error.code >= 9999) {
                        if (self.error.memo !== "") window.location.href = self.error.memo;
                    }
                },
                content: [
                    '<center>' + Words("error.remind") + '</center>',
                    '<span style="color:orange;">' + Words("error.code") + '</span> : <span style="color:orange;">' + self.error.code + '</span><br>',
                    self.error.Message().nl2br()
                ].join('')
            }).diag("show");
        }
    }
};

WLIU.GImage = function (image) {
    if (image) {
        this.id = image.id;
        this.state = image.state;
        this.refKey = image.refKey;
        this.guid = image.guid;
        this.state = image.state;
        this.title_en = image.title_en;
        this.title_cn = image.title_cn;
        this.detail_en = image.detail_en;
        this.detail_cn = image.detail_cn;
        this.full_name = image.full_name;
        this.short_name = image.short_name;
        this.ext_name = image.ext_name;
        this.mime_type = image.mime_type;
        this.main = image.main;
        this.sort = image.sort;
        this.data = {};
        for (let sizeName in image.data) {
            if (sizeName !== "origin") {
                this.data[sizeName] = image.data[sizeName];
            }
        }
        return this;
    }
    else
    {
        return null;
    }
};

WLIU.GImageText = function (image) {
    if (image) {
        this.id = image.id;
        this.refKey = image.refKey;
        this.guid = image.guid;
        this.state = image.state;
        this.title_en = image.title_en;
        this.title_cn = image.title_cn;
        this.detail_en = image.detail_en;
        this.detail_cn = image.detail_cn;
        this.main = image.main;
        this.sort = image.sort;
        return this;
    }
    else {
        return null;
    }
};
