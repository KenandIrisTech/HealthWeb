
var actionModel;        // actionModel get current permission of ActionModel 
var permissionList;     // permissionList  get current all of defined permission in system 
var unauthorizedPath;   // array path for unauthorized
var authorizedPath;     // array path for authorized
var viewTools = new ViewTools();
function ViewTools() {

    this.toastInstance = null;
    this.showToast = function (params) {
        params = params || {};
        params.toastId = params.toastId || String.format("toast_{0}", new Date().getTime());
        if (!this.toastInstance) {
            var toastId = (new Date()).getTime();
            $('body').append(String.format('<div id="toast_{0}"></div>', toastId));
            var toastInstance = new ej.notifications.Toast({
                position: params.position || { X: 'Right', Y: 'Bottom' },
                showCloseButton: params.showCloseButton || true,
                showProgressBar: params.showProgressBar || true,
                close: function (e) { },
                beforeOpen: function (e) { },
                timeOut: params.timeOut || 500000
            });

            toastInstance.appendTo(String.format('#toast_{0}', toastId));
            this.toastInstance = toastInstance;
        }
        this.toastInstance.show({
            title: params.title || 'Welcome the crazy world',
            content: params.content || 'There is some information for you',
            icon: 'ki-icons-comment',
            position: params.position
        });
    };

    this.dialogInstance = null;
    this.showDialog = function (params) {
        params = params || {};
        if (!this.dialogInstance) {
            var dialogId = (new Date()).getTime();
            $('body').append(String.format('<div id="dialog_{0}"></div>', dialogId));
            this.dialogInstance = new ej.popups.Dialog({
                //target: document.getElementById(dialogId),
                header: params.header || 'Title',
                content: params.content || 'No contents!',
                showCloseIcon: params.showCloseIcon || true,
                //buttons: params.buttons || [
                //    { click: function (e) { dialogInstance.close(); }, buttonModel: { content: 'OK', isPrimary: true } }
                //],
                buttons: params.buttons || [{ click: function (e) { }, buttonModel: { content: 'OK', isPrimary: true } }],
                width: params.width || '60%',
                height: params.height || '400px',
                closeOnEscape: true,
                close: function (e) { /*dialogInstance.destroy(); $(dialogInstance.element).remove();*/ },
                isModal: params.isModal || true,
                position: params.position || { X: 'center', Y: 'center' },
                animationSettings: { effect: 'SlideTop', delay: 50, duration: 500 },
                allowDragging: params.allowDragging || true
            });
            this.dialogInstance.appendTo(String.format('#dialog_{0}', dialogId));
        }
        $(this.dialogInstance.contentEle).html('');
        // https://www.syncfusion.com/forums/139841/grid-and-other-controls-do-not-appear-in-partialview-asp-net-mvc
        params.model = params.model || {};
        params.data = JSON.stringify(params.model);
        if (params.serverUrl) {
            var ajax = new ej.base.Ajax(params.serverUrl, 'POST', true);
            ajax.send(params.data).then((html) => {
                $(this.dialogInstance.contentEle).html(html);
            });
        }
        this.dialogInstance.header = params.header || this.dialogInstance.header;
        this.dialogInstance.width = params.width || this.dialogInstance.width;
        this.dialogInstance.height = params.height || this.dialogInstance.height;
        this.dialogInstance.buttons = params.buttons || this.dialogInstance.buttons;
        this.dialogInstance.show();
    };
}


// Language Localization 
function changeLanguage(language) {
    console.log(language);
    var ajax = new ej.base.Ajax(String.format('/api/{0}Api/ChangeLanguage', 'UserAccess'), 'POST', true);
    ajax.send(JSON.stringify(language)).then(
        function (value) {
            //success here
            window.location.reload();
        },
        function (reason) {
            // failed here 
        }
    );
}
//----------------------- Toolbar prototype declaration ----------------------------------
ej.navigations.Toolbar.prototype.getItemByName = function (names) {
    if (Array.isArray(names)) {
        var nodeList = this.items.map((value, index) => {
            if (names.includes(value.name)) {
                return index;
            }
            else {
                return null;
            }
        }).filter(value => {
            if (value !== null)
                return true;
        });
        return nodeList;
    } else {
        return [];
    }
};

ej.navigations.Toolbar.prototype.disableAllItems = function () {
    var nodeList = this.items.map((value, index) => { return value.id.startsWith('text') ? -1 : index; }).filter(index => { return index > -1; });
    if (nodeList.length > 0)
        this.enableItems(nodeList, false);
};

ej.navigations.Toolbar.prototype.disableItemByName = function (names) {
    var nodeList = this.getItemByName(names);
    if (nodeList.length > 0)
        this.enableItems(nodeList, false);
};

ej.navigations.Toolbar.prototype.enableAllItems = function () {
    var nodeList = this.items.map((value, index) => { return index; });
    this.enableItems(nodeList, true);
};

ej.navigations.Toolbar.prototype.enableItemByName = function (names) {
    var nodeList = this.getItemByName(names);
    if (nodeList.length > 0)
        this.enableItems(nodeList, true);
};

ej.grids.Grid.prototype.newDataSet = function () {
    var dataSet = {};
    this.columns.forEach((column) => {
        dataSet[column.field] = null;
    });
    this.dataSet = dataSet;
    return dataSet;
};

