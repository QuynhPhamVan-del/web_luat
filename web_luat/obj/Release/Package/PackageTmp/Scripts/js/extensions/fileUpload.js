class FileUpload {
    constructor(fileQueue, uploaded, key) {
        this.fileQueue = fileQueue;
        this.uploaded = uploaded;
        this.fileKey = key;
    }
    //#region properties
    getFileQueue() {
        return this.fileQueue;
    }
    setFileQueue(fileQueue) {
        this.fileQueue = fileQueue;
    }
    getUploaded() {
        return this.uploaded;
    }
    setUploaded(uploaded) {
        this.uploaded = uploaded;
    }
    getFileKey() {
        return this.fileKey;
    }
    setFileKey(fileKey) {
        this.fileKey = fileKey;
    }
    getListFiles() {
        return this.listFiles;
    }
    setListFiles(list) {
        this.listFiles = list;
    }
    getFileSelected() {
        return this.fileSelected;
    }
    setFileSelected(file) {
        this.fileSelected = file;
    }
    //#endregion properties

    //#region methods
    //add file upload
    addFile(files) {
        let arr = this.fileQueue;
        for (var i = 0; i < files.length; i++) {
            let id = $.fn.generateID();
            let check = false;
            while (check == false) {
                if (arr.find(x => x.id == id)) {
                    id = $.fn.generateID();
                }
                else {
                    check = true;
                }
            }
            files[i].Id = id;
            files[i].FileKey = this.getFileKey();
            files[i].FileName = files[i].name;
            arr.push(files[i]);
        }
    }
    //add file upload
    removeFile(id) {
        let arr = this.fileQueue.filter(x => x.Id != id);
        let arrUploaded = this.uploaded.filter(x => x.Id != id);
        this.setFileQueue(arr);
        this.setUploaded(arrUploaded);
    }
    //view list files
    viewListFiles(elm) {
        let str = '';
        let arr = this.fileQueue;
        let arrUploaded = this.uploaded;
        let listFiles = arrUploaded.concat(arr);
        let arrViewExt = ['.doc', '.docx', '.pdf'];
        listFiles.map(item => {
            str += `<tr data-id="${item.Id}">
                <td style="word-break: break-all;">${item.FileName}</td>
                <td class="text-right" width="50">
                    ${item.FilePath != null && item.FilePath != "" ?
                        `<a href="javascript:void(0)" class="text-primary btn-download-file"><i class="fas fa-download"></i></a>`
                        :
                        ""
                    }
                    ${item.FilePath != null && item.FilePath != "" && arrViewExt.filter(x => item.FileName.includes(x)).length ?
                        `<a href="javascript:void(0)" class="text-primary btn-view-file"><i class="fas fa-eye"></i></a>`
                        :
                        ""
                    }
                    <a href="javascript:void(0)" class="text-danger btn-remove-file"><i class="fas fa-trash-alt"></i></a>
                </td>
            </tr>`
        })
        $("#list_" + elm + " tbody").html(str);
        this.initEvent();
    }
    //init event
    initEvent() {
        let state = this;
        $(`#list_${this.fileKey} > tbody > tr > td .btn-download-file`).off().on("click", function () {
            let id = $(this).closest("tr").data("id");
            let file = state.getUploaded().find(x => x.Id == id);
            if (file != null) {
                $.fn.downloadFile(file.FilePath);
            }
            else {
                $.fn.toastrMessage("Không tìm thấy file", "Thông báo", "error");
            }
        })

        $(`#list_${this.fileKey} > tbody > tr > td .btn-remove-file`).off().on("click", function () {
            let id = $(this).closest("tr").data("id");
            removeFile(id);
        })

        $(".btn-view-file").off().on('click', function () {
            let id = $(this).closest("tr").data("id");
            let file = state.getUploaded().find(x => x.Id == id);
            state.setFileSelected(file);
            state.openPreviewFile();
        })
    }
    //open view list file
    openViewListFiles(listFiles, callback) {
        $.fn.loading();
        $.fn.postData(ACT_FILEMANAGE_VIEWLISTFILES, {listFiles}, res => {
            $.fn.showModal({
                title: "DANH SÁCH TỆP TIN", 
                bodyContent: res.content,
                dialogClass: "modal-lg",
                buttonSave: !1
            });
            this.setListFiles(res.listFiles);
            if (callback) callback();
            $.fn.offLoading();
        })
    }
    //preview file
    openPreviewFile(callback) {
        $.fn.loading();
        $.fn.postData(ACT_FILEMANAGE_VIEWPREVIEWFILE, {}, res => {
            $.fn.showSubModal({
                title: "XEM TỆP TIN", 
                bodyContent: res,
                dialogClass: "modal-xl",
                buttonSave: !1
            });
            if (callback) callback();
            $.fn.offLoading();
        })
    }
    //#endregion methods
}