
var ____debug_grid = false;
var ____show_validation = false;
var OneViewControl = {
    WebView: function (params) {
        'use strict';
        params = params || {};
        params.viewType = params.viewType || OneViewControl.ViewType.GRID_GRID;
        params.relations = params.relations || {};
        params.queryMode = params.queryMode || OneViewControl.QueryMode.SERVER;
        params.initQueryParams = params.initQueryParams || null;

        var self = this;
        //var dataSet = {};
        this.relationModel;
        this.dataProcessModel;
        this.viewType = params.viewType;
        this.queryMode = params.queryMode;
        this.eventTarget = new OneViewControl.EventTarget();
        this.addEventListener = function (type, listener, useCapture) {
            //console.log('addEventListener', listener, self.eventTarget); toolbarStandard
            self.eventTarget.addEventListener(type, listener, useCapture);
        };
        function _init() {
            self.relationModel = new OneViewControl.RelationModel(params.relations, params.queryMode, params.viewType);
            self.relationModel.viewType = params.viewType;
            self.relationModel.eventTarget = self.eventTarget;
            self.dataProcessModel = new OneViewControl.DataProcessModel(self);

            // Start Data Processing
            self.dataProcessModel.start(params.initQueryParams);

            // Set View Title
            if (params.title)
                setViewTitle(params.title);
        }
        window.addEventListener("load", _init(), false);
    },

    DataProcessModel: function (view) {
        var self = this;
        this.view = view;
        var viewType = view.viewType;
        var queryMode = view.queryMode;
        var startupMartName = self.view.relationModel.startupMartName;
        // Declare click event of toolbar item 
        for (var [key, value] of self.view.relationModel.dataMarts.entries()) {
            value.toolbarInstance.addEventListener('clicked', toolbarItemClick);
        }

        this.start = function (initQueryParams) {
            self.initQueryParams = initQueryParams;
            self.view.relationModel.startDataBinding(initQueryParams);
        };

        function toolbarItemClick(e) {
            var dataMart = self.view.relationModel.getDataMart(this.dataMartName);
            //console.log(this.dataMartName, startupMartName, dataMart.elementInstance.dataSet);
            var commandItemName = e.item.id.replace(String.format('{0}Cmd', dataMart.serverApi.toLowerCase()), '');
            if (commandItemName.startsWith('text')) return;
            var isStartupDataMartToolbar = (this.dataMartName === startupMartName);
            var primaryKeyFieldNames = dataMart.elementInstance.getPrimaryKeyFieldNames();
            var primaryKeyValue;
            if (dataMart.elementInstance.dataSet) {
                primaryKeyValue = dataMart.elementInstance.dataSet[primaryKeyFieldNames[0]];
            }
            // -------------------------- Data Process at Client side --------------------------
            if (queryMode === OneViewControl.QueryMode.CLIENT && isStartupDataMartToolbar) {
                //console.log(self.view);
                switch (commandItemName) {
                    case 'Add':
                        if (viewType === OneViewControl.ViewType.SINGLE_FORM || viewType === OneViewControl.ViewType.FORM_GRID) {
                            self.view.relationModel.newForm();
                        }
                        break;
                    case 'Update':
                        if (viewType === OneViewControl.ViewType.SINGLE_FORM || viewType === OneViewControl.ViewType.FORM_GRID) {
                            self.view.relationModel.updateForm();
                        }
                        else if (viewType === OneViewControl.ViewType.SINGLE_GRID || viewType === OneViewControl.ViewType.GRID_GRID) {
                            self.view.relationModel.updateAll();
                        }
                        break;
                    case 'Delete':
                        if (viewType === OneViewControl.ViewType.SINGLE_FORM || viewType === OneViewControl.ViewType.FORM_GRID) {
                            self.view.relationModel.deleteForm();
                        }
                        break;
                    case 'Cancel':
                        if (viewType === OneViewControl.ViewType.SINGLE_FORM || viewType === OneViewControl.ViewType.FORM_GRID) {
                            self.view.relationModel.cancelForm();
                        }
                        else if (viewType === OneViewControl.ViewType.SINGLE_GRID || viewType === OneViewControl.ViewType.GRID_GRID) {
                            self.view.relationModel.cancelAll();
                        }
                        break;
                    default:
                        return;
                }
                dataMart.toolbarInstance.disableAllItems();
            }

            // -------------------------- Data Process at Server side --------------------------
            if (queryMode === OneViewControl.QueryMode.SERVER) {
                var rowIndex = dataMart.elementInstance.selectedRowIndex;
                console.log(e.item.id, String.format('{0}Cmd', dataMart.serverApi.toLowerCase()), commandItemName);
                switch (commandItemName) {
                    case 'Add':
                        var ajax = new ej.base.Ajax(String.format('/api/{0}Api/NewForm', dataMart.serverApi), 'POST', true);
                        ajax.send(JSON.stringify({}))
                            .then(function (result) {
                                dataMart.elementInstance.dataSet = JSON.parse(result).result[0];
                                dataMart.elementInstance.dataSet.STATE = OneViewControl.EntityState.Added;
                                console.log(dataMart.elementInstance.dataSet);
                                self.view.relationModel.setFormDataBinding(dataMart, -1);
                                dataMart.toolbarInstance.disableItemByName(['Add']);
                                dataMart.toolbarInstance.enableItemByName(['Update', 'Cancel']);
                            }).catch(function (error) {
                                console.log(error);
                                self.view.relationModel.showUpdateError(error);
                            });
                        break;
                    case 'Update':

                        if (dataMart.targetFormElement && dataMart.elementInstance.dataSet) {
                            if (dataMart.formValidator) {
                                //var isValid = dataMart.formValidator.validate();
                                //console.log('validate', dataMart, isValid);
                                if (!dataMart.formValidator.validate()) {
                                    console.log('Failed Data Validation!');
                                    return;
                                }
                            }

                            //if (dataMart.elementInstance.dataSet.STATE !== OneViewControl.EntityState.Added && rowIndex <= -1) {
                            //    dataMart.toolbarInstance.disableAllItems();
                            //    viewTools.showToast({ title: 'Update', content: 'No selected record!' });
                            //    return;
                            //}

                            if (dataMart.elementInstance.dataSet.STATE === undefined) {
                                console.error('Property STATE undefined');
                                return;
                            }

                            var query = new ej.data.Query();
                            switch (dataMart.elementInstance.dataSet.STATE) {
                                case OneViewControl.EntityState.Added:
                                    self.view.relationModel.setFormDataValue(dataMart, -1);
                                    //dataMart.elementInstance.addRecord(dataMart.elementInstance.dataSet, 0);
                                    //console.log(dataMart.elementInstance.dataSet, dataMart.elementInstance.currentViewData);
                                    dataMart.elementInstance.dataSource.insert(
                                        dataMart.elementInstance.dataSet,
                                        dataMart.entityName,
                                        query, 0)
                                        .then(function (result) {
                                            dataMart.elementInstance.dataSet.STATE = OneViewControl.EntityState.Unchanged;
                                            dataMart.elementInstance.refresh();
                                            dataMart.toolbarInstance.enableItemByName(['Add']);
                                            dataMart.toolbarInstance.disableItemByName(['Update', 'Cancel']);
                                        }).catch(function (error) {
                                            //console.log(error);
                                            self.view.relationModel.showUpdateError(error);
                                            return;
                                        });
                                    break;
                                case OneViewControl.EntityState.Modified:
                                    console.log(primaryKeyValue.toString(), dataMart.elementInstance.getColumns(), dataMart.elementInstance.currentViewData, dataMart.elementInstance.dataSet);
                                    self.view.relationModel.setFormDataValue(dataMart, rowIndex);
                                    dataMart.elementInstance.setRowData(primaryKeyValue.toString(), dataMart.elementInstance.dataSet);
                                    //dataMart.elementInstance.updateRow(rowIndex, dataMart.elementInstance.dataSet);
                                    dataMart.elementInstance.dataSource.update(
                                        primaryKeyFieldNames[0],
                                        dataMart.elementInstance.dataSet,
                                        dataMart.entityName,
                                        query)
                                        .then(function (result) {
                                            dataMart.elementInstance.dataSet.STATE = OneViewControl.EntityState.Unchanged;
                                            dataMart.elementInstance.setRowData(primaryKeyValue, dataMart.elementInstance.dataSet);
                                            //console.log(result.result.entity);
                                            dataMart.toolbarInstance.enableItemByName(['Add']);
                                            dataMart.toolbarInstance.disableItemByName(['Update', 'Cancel']);
                                        }).catch(function (error) {
                                            //console.log(error);
                                            self.view.relationModel.showUpdateError(error);
                                            return;
                                        });
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        //this.changeButtonState = function (buttonStates) {
        //    if (!hasToolbar) return;
        //    switch (buttonStates) {
        //        case OneViewControl.ButtonState.New:
        //            toolbarInstance.enableItems([0], true);
        //            break;
        //        case OneViewControl.ButtonState.Update:
        //            toolbarInstance.enableItems([1], true);
        //            break;
        //        case OneViewControl.ButtonState.Delete:
        //            toolbarInstance.enableItems([2], true);
        //            break;
        //        case OneViewControl.ButtonState.Cancel:
        //            toolbarInstance.enableItems([3], true);
        //            break;
        //        case OneViewControl.ButtonState.EnableAll:
        //        case OneViewControl.ButtonState.Initial:
        //            toolbarInstance.enableItems([0, 1, 2, 3], true);
        //            break;
        //        case OneViewControl.ButtonState.Modifiled:
        //            toolbarInstance.enableItems([0, 2], false);
        //            break;
        //        case OneViewControl.ButtonState.DisableAll:
        //            toolbarInstance.enableItems([0, 1, 2, 3], false);
        //            break;
        //    }
        //};
    },

    QueryModel: function (query) {
        var self = this;
        this.dataManager;
        this.adaptor;
        this.serverApi;
        this.dataResult;
        this.recordCount = 0;

        function initialize() {
            if (query) {
                this.queryMode = query.queryMode;
                this.serverApi = query.serverApi;
                createDataManager();
                //console.info(self.dataManager);
            }
        }

        function createDataManager() {
            self.adaptor = new ej.data.UrlAdaptor();
            if (query.queryMode === OneViewControl.QueryMode.CLIENT &&
                (query.viewType === OneViewControl.ViewType.FORM_GRID || query.viewType === OneViewControl.ViewType.SINGLE_FORM)) {
                self.dataManager = new ej.data.DataManager({
                    url: String.format('/api/{0}Api/NewForm', query.serverApi),
                    //insertUrl: String.format('/api/{0}Api/Insert', self.serverApi),
                    //updateUrl: String.format('/api/{0}Api/Update', self.serverApi),
                    //removeUrl: String.format('/api/{0}Api/Remove', self.serverApi),
                    crudUrl: String.format('/api/{0}Api/Crud', self.serverApi),
                    crossDomain: true,
                    adaptor: self.adaptor,
                    jsonp: 'jsonCallback'
                });
            }
            else {
                self.dataManager = new ej.data.DataManager({
                    url: String.format('/api/{0}Api/GetAll', query.serverApi),
                    //insertUrl: String.format('/api/{0}Api/Insert', self.serverApi),
                    //updateUrl: String.format('/api/{0}Api/Update', self.serverApi),
                    //removeUrl: String.format('/api/{0}Api/Remove', self.serverApi),
                    crudUrl: String.format('/api/{0}Api/Crud', query.serverApi),
                    crossDomain: true,
                    adaptor: self.adaptor,
                    jsonp: 'jsonCallback'
                });
            }
        }

        this.execQuery = function (query) {
            if (query === undefined)
                query = new ej.data.Query();
            var promise = query.execute(self.dataManager,
                done => {
                    return done;
                },
                fail => {
                    console.info(fail);
                });
            return promise;

        };
        initialize();
    },

    RowFilterModel: function () {

    },

    ColumnFilterModel: function () {

    },

    CommandModel: function () {

    },

    SearchModel: function () {

    },

    RelationModel: function (relations, queryMode, viewType) {
        var pkValue = 1;
        var self = this;
        this.dataMarts = new Map();
        this.queryMode = queryMode;
        this.startupMartName = relations.serverApi || relations.entity;
        function _initialize() {
            if (relations && relations.id !== undefined) {
                getInstance(relations, null);
            }
        }

        function getInstance(r, p) {
            var element = document.getElementById(r.id);
            if (element && element.ej2_instances !== undefined) {   // its a selector html control
                var instance = element.ej2_instances[0];
                // Add to data mart
                self.dataMarts.set(r.serverApi, r);
                // Set Primary Key & Grid Instance
                r.elementInstance = instance;
                r.elementInstance.dataMartName = r.serverApi;
                r.elementInstance.parentMartName = p === null ? null : p.serverApi || null;
                r.primaryKey = instance.columns.filter(c => { return c.isPrimaryKey; }).map(c => { return { name: c.field }; });
                r.foreignKey = p === null ? null : p.primaryKey || null;
                r.parentMartName = p === null ? null : p.serverApi || null;
                r.queryParam = r.queryParam || {};

                // Create QueryModel
                r.queryModel = new OneViewControl.QueryModel({ viewType: viewType, queryMode: queryMode, serverApi: r.serverApi });
                // 宣告 Grid Event Binding
                if (instance) {
                    instance.addEventListener('beginEdit', grid_beginEdit);
                    instance.addEventListener('actionBegin', grid_actionBegin);
                    instance.addEventListener('actionComplete', grid_actionComplete);
                    instance.addEventListener('dataSourceChanged', grid_dataSourceChanged);
                    instance.addEventListener('dataBound', grid_dataBound);
                    instance.addEventListener('rowDataBound', grid_rowDataBound);
                    instance.addEventListener('rowSelecting', grid_rowSelecting);
                    instance.addEventListener('rowSelected', grid_rowSelected);
                    instance.addEventListener('rowDeselected', grid_rowDeselected);
                    instance.addEventListener('actionFailure', grid_actionFailure);
                }
            }
            // WebView Form Control
            else if (queryMode === OneViewControl.QueryMode.CLIENT &&
                (viewType === OneViewControl.ViewType.FORM_GRID || viewType === OneViewControl.ViewType.SINGLE_FORM)) {
                self.dataMarts.set(r.serverApi, r);
                r.elementInstance = {};
                r.elementInstance.dataMartName = r.serverApi;
                r.elementInstance.parentMartName = p === null ? null : p.serverApi || null;
                //r.primaryKey = 
                r.foreignKey = p === null ? null : p.primaryKey || null;
                r.parentMartName = p === null ? null : p.serverApi || null;
                r.queryParam = r.queryParam || {};
                // Create QueryModel
                r.queryModel = new OneViewControl.QueryModel({ viewType: viewType, queryMode: queryMode, serverApi: r.serverApi });
            }

            // -------------- toolbar setting --------------
            r.showToolbar = r.showToolbar === undefined ? true : r.showToolbar;
            r.toolbar = r.toolbar || ['Add', 'Update', 'Cancel'];
            r.targetFormId = /*r.targetFormId || */String.format('{0}FormContainer', r.serverApi.toLowerCase());
            r.targetFormElement = document.getElementById(r.targetFormId);
            r.toolbarTargetId = String.format('{0}FormContainerToolbar', r.serverApi.toLowerCase());
            r.toolbarTargetElement = document.getElementById(r.toolbarTargetId);
            setToolbarInstance(r);
            // ----------------------------------------------
            // ---Set field instance of Data Form if exists--
            setFormFieldElement(r);
            // ----------------------------------------------

            if (r.children && Array.isArray(r.children)) {
                r.children.forEach(child => {
                    getInstance(child, r);
                });
            }
        }

        function setToolbarInstance(r) {
            r.toolbarInstance = new ej.navigations.Toolbar({
                id: String.format('{0}Toolbar', r.serverApi.toLowerCase(), r.serverApi.toLowerCase()),
                items: r.toolbar.map(function (item) {
                    return {
                        prefixIcon: String.format('ki-icons ki-icon-{0}', item.toLowerCase()),
                        id: String.format('{0}Cmd{1}', r.serverApi.toLowerCase(), item),
                        name: item,
                        align: 'Left',
                        text: item
                    };
                }),
                dataMartName: r.serverApi
            });

            if (r.toolbarTargetElement && r.showToolbar) {
                r.toolbarInstance.appendTo(String.format('#{0}Toolbar', r.targetFormId));

                // -------------- setup toolbar title text ---------------
                if (r.title) {
                    r.toolbarInstance.addItems([
                        { id: 'textTitle', aligh: 'Left', template: String.format('<div class="e-control ki-toolbar-header">{0}</div>', r.title) }
                    ]);
                }
                r.toolbarInstance.disableAllItems();
                // -------------------------------------------------------
            }
            else {
                r.showToolbar = false;
            }

            //r.toolbarInstance.enableItemByName(['Update']);
            //console.log(r.toolbarInstance.getItemByName(['Update']));
        }

        function setFormFieldElement(r, force) {
            force = force || false;
            if (force || (r.targetFormElement && r.formFieldInstances === undefined)) {
                var formElements = $(r.targetFormElement).find('input, input + textarea, textarea, select');
                //console.log(formElements);
                r.formFieldInstances = new Map();
                formElements.each((index, element) => {
                    //console.log(element.id);
                    try {
                        ej2_instances = document.getElementById(element.id).ej2_instances;
                        if (ej2_instances) {
                            instance = ej2_instances[0];
                            r.formFieldInstances.set(element.id, instance);
                            instance.addEventListener('change', formFieldValueChanged);
                        }
                    }
                    catch (e) { /*console.info(e.message);*/ }
                });
                // set foem validation for form element
                setFormValidator(r);
            }
        }

        this.setFormFieldElement = function (r, force) {
            setFormFieldElement(r, force);
        };

        this.startDataBinding = function (initQueryParams) {
            var startupDataMart = self.getDataMart(self.startupMartName);
            switch (self.queryMode) {
                case OneViewControl.QueryMode.CLIENT:
                    switch (self.viewType) {
                        case OneViewControl.ViewType.FORM_GRID:
                        case OneViewControl.ViewType.SINGLE_FORM:
                            break;
                    }
                    break;
            }
            setDataSource(startupDataMart, initQueryParams);
        };

        function setDataSource(dataMart, initQueryParams) {
            var query = new ej.data.Query();
            switch (self.queryMode) {
                case OneViewControl.QueryMode.SERVER:
                    switch (self.viewType) {
                        case OneViewControl.ViewType.SINGLE_GRID:
                        case OneViewControl.ViewType.GRID_GRID:
                        case OneViewControl.ViewType.GRID_FORM:
                            // user queryParam
                            dataMart.elementInstance.query.params = [];
                            dataMart.elementInstance.query.addParams('queryParams', dataMart.queryParam);
                            self.eventTarget.dispatchEvent({
                                type: 'beforeQuery',
                                query: dataMart.elementInstance.query,
                                name: dataMart.elementInstance.dataMartName,
                                instance: dataMart.elementInstance
                            });
                            // initialize data source of Grid or  refreshing Grid
                            if (Array.isArray(dataMart.elementInstance.dataSource) && dataMart.elementInstance.dataSource.length === 0) {
                                // initialize data source
                                dataMart.elementInstance.dataSource = dataMart.queryModel.dataManager;
                            }
                            else {
                                // re-execute query
                                dataMart.elementInstance.refresh();

                                //https://ej2.syncfusion.com/documentation/api/data/query/#execute
                                //dataMart.elementInstance.query.execute();
                            }
                            break;
                    }
                    break;
                case OneViewControl.QueryMode.CLIENT:
                    switch (self.viewType) {
                        case OneViewControl.ViewType.SINGLE_GRID:
                        case OneViewControl.ViewType.GRID_GRID:
                        case OneViewControl.ViewType.GRID_FORM:
                            self.eventTarget.dispatchEvent({
                                type: 'beforeQuery',
                                query: dataMart.elementInstance.query,
                                name: dataMart.elementInstance.dataMartName
                                //instance: dataMart.elementInstance
                            });
                            var promise = dataMart.queryModel.execQuery(query);
                            promise.then(function (data) {
                                dataMart.elementInstance.dataSource = data.result.result;
                            });
                            break;
                        case OneViewControl.ViewType.FORM_GRID:
                        case OneViewControl.ViewType.SINGLE_FORM:
                            if (initQueryParams !== null && typeof initQueryParams === 'object')
                                self.getForm(initQueryParams);
                            else
                                self.newForm();
                    }
                    break;
            }
        }

        function setChildrenDataSource(childrenMart, parentData) {
            if (childrenMart === undefined) return;
            switch (self.queryMode) {
                case OneViewControl.QueryMode.SERVER:
                    childrenMart.forEach(dm => {
                        if (dm.foreignKey) {
                            dm.elementInstance.query.params = [];
                            dm.elementInstance.query.queries = [];
                            //dm.elementInstance.query.addParams(dm.foreignKey[0].name, this.currentViewData[0][dm.foreignKey[0].name]);
                            dm.elementInstance.parentData = parentData;
                            dm.elementInstance.query.where(dm.foreignKey[0].name, 'equal', parentData[dm.foreignKey[0].name]);
                        }
                        setDataSource(dm);
                    });
                    break;
                case OneViewControl.QueryMode.CLIENT:
                    childrenMart.forEach(dm => {
                        dm.elementInstance.parentData = parentData;
                        dm.elementInstance.dataSource = parentData[dm.entity];
                    });
                    break;
            }
        }

        // json callback 
        function _jsonCallback(e) {
            console.log('dataSourceCallback'.d);
        }

        this.getDataMart = function (name) {
            for (var [key, value] of self.dataMarts.entries()) {
                if (value.serverApi === name || value.entity === name)
                    return value;
            }
            return null;
        };

        this.executeQuery = function (params) {
            params = params || {};
        };

        this.newForm = function () {
            var query = new ej.data.Query();
            var dataMart = self.getDataMart(self.startupMartName);
            dataMart.queryModel.dataManager.dataSource.url = String.format("/api/{0}Api/NewForm", dataMart.serverApi);
            var promise = dataMart.queryModel.execQuery(query);
            promise.then(
                function (data) {   // resolve
                    if (Array.isArray(data.result.result)) {
                        dataMart.elementInstance.dataSource = data.result.result;
                        dataMart.elementInstance.dataSet = data.result.result[0];
                    }
                    setFormDataBinding(dataMart);
                    setChildrenDataSource(dataMart.children, dataMart.elementInstance.dataSet);
                    if (dataMart.formValidator)
                        dataMart.formValidator.validate();

                    self.eventTarget.dispatchEvent({
                        callFrom: 'newForm',
                        type: 'afterQuery',
                        dataMart: dataMart,
                        query: query
                    });
                },
                function (e) {      // reject

                }
            );
        };

        this.getForm = function (queryParam) {  // { column: 'COLLECTION_ID', operator: 'equal', value: 10,  }
            var query = new ej.data.Query();
            queryParam.operator = queryParam.operator || 'equal';
            query.where(queryParam.column, queryParam.operator, queryParam.value);
            var dataMart = self.getDataMart(self.startupMartName);
            dataMart.queryModel.dataManager.dataSource.url = String.format("/api/{0}Api/GetForm", dataMart.serverApi);
            var promise = dataMart.queryModel.execQuery(query);
            //console.log('getForm', promise, queryParam);
            promise.then(
                function (data) {               // resolve
                    if (Array.isArray(data.result.result)) {
                        dataMart.elementInstance.dataSource = data.result.result;
                        dataMart.elementInstance.dataSet = data.result.result[0];
                    }
                    setFormDataBinding(dataMart);
                    setChildrenDataSource(dataMart.children, dataMart.elementInstance.dataSet);

                    // validate form if exists
                    if (dataMart.formValidator)
                        dataMart.formValidator.validate();

                    // fire event afterQuery
                    self.eventTarget.dispatchEvent({
                        callFrom: 'getForm',
                        type: 'afterQuery',
                        dataMart: dataMart,
                        query: query
                    });
                },
                function (e) {                    // reject
                    console.log('getForm - reject', e);
                }
            );
        };

        this.updateForm = function (params) {
            params = params || {};
            params.action = params.action || 'update';
            var dataMart = self.getDataMart(self.startupMartName);
            var changed = dataMart.elementInstance.dataSource
                .filter(data => data.STATE === OneViewControl.EntityState.Modified || data.STATE === OneViewControl.EntityState.Added);

            //if (changed.length === 0) {
            //    viewTools.showToast({ title: 'Update Form', content: 'Nothing to update' });
            //    return;
            //}
            var isValid = dataMart.formValidator === undefined ? true : dataMart.formValidator.validate();
            if (isValid) {
                if (dataMart.formFieldInstances)
                    setFormDataValue(dataMart);
                dataMart.elementInstance.dataSource.forEach(data => {
                    switch (params.action) {
                        case 'insert':
                            data.STATE = OneViewControl.EntityState.Added;
                            break;
                        case 'update':
                        default:
                            //data.STATE = OneViewControl.EntityState.Modified;
                            //console.log('update form - update', data.STATE, OneViewControl.EntityState.Modified);
                            break;
                    }
                });
                var postData = {};
                postData.action = params.action;
                switch (params.action) {
                    case 'insert':
                        postData.added = dataMart.elementInstance.dataSource;
                        break;
                    case 'update':
                    default:
                        postData.changed = dataMart.elementInstance.dataSource;
                        break;
                }

                var ajax = new ej.base.Ajax(String.format('/api/{0}Api/UpdateForm', dataMart.serverApi), 'POST', true);
                var result;
                ajax.onSuccess = function (value) {
                    result = JSON.parse(value);
                    resumeChangedData(dataMart.elementInstance.dataSource);
                };
                ajax.onFailure = function (reason) {
                    console.log(reason);
                    result = JSON.parse(reason.responseText);
                    console.log('failure', result, dataMart.elementInstance.dataSource);
                    viewTools.showToast({
                        title: result.entityName || result.Message,     // unhandle webapi exception will rerturn result.Message etc...
                        content: result.fieldName !== undefined
                            ? String.format('[{0}] : {1}', result.fieldName, result.returnMessage)
                            : result.ExceptionMessage
                    });
                };
                ajax.beforeSend = function (e) {
                    // show spinner here 
                };
                ajax.send(JSON.stringify(postData)).then(
                    function (value) {
                        //success here
                    },
                    function (reason) {
                        // failed here 
                    }
                );

                // use syncfusion ajax instead
                //$.post(String.format('/api/{0}Api/UpdateForm', dataMart.serverApi),
                //    postData
                //).then(function (result) {
                //    console.log('update form return result', result);
                //    resumeChangedData(dataMart.elementInstance.dataSource);
                //}).fail(function (error) {
                //    console.trace('Error', error);
                //});
            }
            else {
                console.trace('Not validated');
            }
        };

        this.resetForm = function () {
            var dataMart = self.getDataMart(self.startupMartName);
            self.newForm();
            setFormDataBinding(dataMart, 0);
            dataMart.formValidator.validate();
        };

        this.addFormFieldRule = function (dataMartName, fieldName, ruleObject) {
            dataMartName = dataMartName || self.startupMartName;
            var dataMart = self.getDataMart(dataMartName);
            //console.log(dataMart);
            dataMart.formValidator.addRules(fieldName, ruleObject);
        };

        this.deleteForm = function () {
            console.log('deleteForm');
        };

        this.cancelForm = function () {
            var query = new ej.data.Query();
            query.where('COLLECTION_ID', 'equal', 179);
            var dataMart = self.getDataMart(self.startupMartName);
            dataMart.queryModel.dataManager.dataSource.url = String.format("/api/{0}Api/GetForm", dataMart.serverApi);

            var promise = dataMart.queryModel.execQuery(query);
            promise.then(
                function (data) {   // resolve
                    if (Array.isArray(data.result.result)) {
                        dataMart.elementInstance.dataSource = data.result.result;
                        dataMart.elementInstance.dataSet = data.result.result[0];
                    }
                    setFormDataBinding(dataMart);
                    setChildrenDataSource(dataMart.children, dataMart.elementInstance.dataSet);
                    //dataMart.formValidator.validate();
                },
                function (e) {      // reject

                }
            );
        };

        this.updateAll = function () {
            console.log('updateAll');
            var rootDataMart = self.dataMarts.get(relations.serverApi);
            var changed = rootDataMart.elementInstance.dataSource
                .filter(data => data.STATE === OneViewControl.EntityState.Modified || data.STATE === OneViewControl.EntityState.Added);

            if (rootDataMart.elementInstance.deleted)
                changed = [...changed, ...rootDataMart.elementInstance.deleted];

            changed = mergeChangedData(changed);
            delete rootDataMart.elementInstance.deleted;
            if (changed.length > 0) {
                var ajax = new ej.base.Ajax(String.format('/api/{0}Api/UpdateAll', rootDataMart.serverApi), 'POST', true, 'application/json');
                var result;
                ajax.onSuccess = function (value) {
                    result = JSON.parse(value);
                    resumeChangedData(changed);
                };
                ajax.onFailure = function (reason) {
                    result = JSON.parse(reason.responseText);
                    viewTools.showToast({
                        title: result.entityName,
                        content: String.format('[{0}] : {1}', result.fieldName, result.returnMessage)
                    });
                };
                ajax.beforeSend = function (e) {
                    // show spinner here 
                };

                var postData = { action: 'update', changed: changed };
                ajax.send(JSON.stringify(postData)).then(
                    function (value) {
                        //success here
                    },
                    function (reason) {
                        // failed here 
                    }
                );
            }
        };

        this.cancelAll = function () {
            console.log('cancelAll');
            var dataMart = self.getDataMart(self.startupMartName);
            dataMart.queryModel.dataManager.dataSource.url = String.format("/api/{0}Api/GetForm", dataMart.serverApi);
            var promise = dataMart.queryModel.execQuery();
            self.startDataBinding();
        };

        // Grid event implementation
        function grid_beginEdit(e) {
            if (____debug_grid) console.log('beginEdit', e);

        }

        function grid_actionBegin(e) {
            if (____debug_grid) console.log('actionBegin', e);
            var dataMart = self.getDataMart(this.dataMartName);
            var primaryKey, foreignKey;
            if (Array.isArray(dataMart.foreignKey))
                foreignKey = dataMart.foreignKey[0].name;
            if (Array.isArray(dataMart.primaryKey) && dataMart.primaryKey.length)
                primaryKey = dataMart.primaryKey[0].name;
            else
                console.error('Undefined Primary Key in Grid.Columns');
            switch (e.requestType) {
                case 'refresh':
                    // implement predict here
                    break;
                case 'beginEdit':
                    break;
                case 'add':
                    //console.log(e);
                    break;
                case 'delete':
                    if (self.queryMode === OneViewControl.QueryMode.CLIENT) {
                        e.data.forEach(data => {
                            modifyParentCascade(this.dataMartName, data);
                        });
                    }
                    break;
            }
        }

        function grid_actionComplete(e) {
            if (____debug_grid) console.log('actionComplete', e);
            var dataMart = self.getDataMart(this.dataMartName);
            var primaryKey, foreignKey;
            if (Array.isArray(dataMart.foreignKey))
                foreignKey = dataMart.foreignKey[0].name;
            if (Array.isArray(dataMart.primaryKey))
                primaryKey = dataMart.primaryKey[0].name;
            switch (self.queryMode) {
                case OneViewControl.QueryMode.SERVER:
                case OneViewControl.QueryMode.CLIENT:
                    switch (e.requestType) {
                        case 'refresh':
                            // implement predict here
                            break;
                        case 'add':  // before append
                            if (foreignKey) {
                                e.data[foreignKey] = this.parentData[foreignKey];
                            }
                            e.data.STATE = OneViewControl.EntityState.Added;
                            console.log('before add (client)', e.data);
                            break;
                        case 'save':
                            if (e.type === 'actionComplete' && e.name === 'actionComplete') {
                                switch (e.action) {
                                    case 'add':
                                        if (primaryKey) {
                                            e.data[primaryKey] = pkValue++ * -1;    //new Date().getTime() * -1;
                                            console.log('after add(client)', e.data);
                                        }
                                        break;
                                    case 'edit':
                                        break;
                                }
                                if (self.queryMode === OneViewControl.QueryMode.CLIENT) {
                                    modifyParentCascade(this.dataMartName, e.data);
                                    if (dataMart.showToolbar) {
                                        dataMart.toolbarInstance.enableItemByName(['Update', 'Cancel']);
                                    }
                                }
                            }
                            break;
                        case 'delete':
                            if (self.queryMode === OneViewControl.QueryMode.CLIENT) {
                                e.data.forEach(data => {
                                    removeCascade(this, data);
                                });
                            }
                            break;
                    }
                    break;
            }

            self.eventTarget.dispatchEvent({
                type: 'gridActionComplete',
                instance: this,
                evt: e
            });
        }

        function grid_dataSourceChanged(e) {
            if (____debug_grid) console.log('dataSourceChanged', e);

        }

        function grid_dataBound(e) {
            if (____debug_grid) console.log('dataBound', e);
            var dataMart = self.getDataMart(this.dataMartName);
            var childrenMart = getChildrenMart(this.dataMartName);

            //console.log(dataMart);
            switch (self.queryMode) {
                case OneViewControl.QueryMode.SERVER:
                    if (dataMart.toolbarInstance && dataMart.showToolbar) {
                        if (Array.isArray(childrenMart) && this.currentViewData.length > 0) {
                            //setChildrenDataSource(childrenMart, this.currentViewData[0]);
                        }
                        else {
                            dataMart.toolbarInstance.enableItemByName(['Add']);
                        }
                    }
                    break;
            }


            // Iris insist to remark below code 
            //switch (self.queryMode) {
            //    case OneViewControl.QueryMode.SERVER:
            //        if (Array.isArray(childrenMart) && this.currentViewData.length > 0) {
            //            setChildrenDataSource(childrenMart, this.currentViewData[0]);
            //        }
            //        break;
            //    case OneViewControl.QueryMode.CLIENT:
            //        if (Array.isArray(childrenMart) && this.currentViewData.length > 0) {
            //            setChildrenDataSource(childrenMart, this.currentViewData[0]);
            //        }
            //        break;
            //}
            //if (dataMart.targetFormElement !== null)
            //     setFormDataBinding(dataMart, 0); 
        }

        function grid_rowDataBound(e) {
            if (____debug_grid) console.log('rowDataBound', e);

        }

        function grid_rowSelecting(e) {
            if (____debug_grid) console.log('rowSelecting', e);

        }

        function grid_rowSelected(e) {
            if (____debug_grid) console.log('rowSelected', e);
            var childrenMart = getChildrenMart(this.dataMartName);
            var dataMart = self.getDataMart(this.dataMartName);
            if (Array.isArray(childrenMart)) {
                setChildrenDataSource(childrenMart, e.data);
                enableAdding(childrenMart);
            }

            //console.log('rowSelected', dataMart, view);
            // Bind field value if there is declared form id
            setFormDataBinding(dataMart, this.selectedRowIndex);

            if (queryMode === OneViewControl.QueryMode.SERVER) {
                try {
                    if (dataMart.toolbarInstance && dataMart.showToolbar) {
                        if (dataMart.elementInstance.dataSet.STATE !== OneViewControl.EntityState.Modified) {
                            dataMart.toolbarInstance.disableItemByName(['Update', 'Cancel']);
                            dataMart.toolbarInstance.enableItemByName(['Add']);
                        }
                        else {
                            dataMart.toolbarInstance.enableItemByName(['Update', 'Cancel']);
                            dataMart.toolbarInstance.disableItemByName(['Add']);
                        }
                    }
                }
                catch (error) {
                    console.log(dataMart.toolbarInstance.items, dataMart.toolbarInstance.getItemByName(['Update', 'Cancel']));
                }
            }
        }

        function grid_rowDeselected(e) {
            //console.log('rowDeselected', e, this.selectedRowIndex);
            var childrenMart = getChildrenMart(this.dataMartName);
            disableAdding(childrenMart);
        }

        function disableAdding(childrenMart) {
            if (childrenMart) {
                childrenMart.forEach(dm => {
                    dm.elementInstance.editSettings.allowAdding = false;
                });
            }
        }

        function enableAdding(childrenMart) {
            if (childrenMart) {
                childrenMart.forEach(dm => {
                    dm.elementInstance.editSettings.allowAdding = true;
                });
            }
        }

        function grid_actionFailure(e) {
            if (____debug_grid) console.log('actionFailure', e);
            var result;
            if (Array.isArray(e)) {
                result = JSON.parse(e[0].error.responseText);
            }
            else if (e.returnMessage) {
                result = { entityName: e.entityName, fieldName: e.fieldName, returnMessage: e.returnMessage };
            }
            else {
                result = { fieldName: 'Unknown', returnMessage: 'Unknown' };
            }
            //console.log(result);
            viewTools.showToast({
                title: result.entityName,
                content: String.format('[{0}] : {1}', result.fieldName, result.returnMessage)
            });
        }

        this.showUpdateError = function (e) {
            var result;
            if (Array.isArray(e)) {
                result = JSON.parse(e[0].error.responseText);
            }
            viewTools.showToast({
                title: result.entityName,
                content: String.format('[{0}] : {1}', result.fieldName, result.returnMessage)
            });
        };

        function getChildrenMart(name) {
            return self.dataMarts.get(name).children;
        }

        _initialize();

        function removeCascade(elementInstance, data) {
            var dataMart = self.dataMarts.get(elementInstance.dataMartName);
            data.STATE = OneViewControl.EntityState.Deleted;

            //console.log(dataMart, elementInstance, elementInstance.parentData);
            var parentData = elementInstance.parentData;
            if (parentData !== undefined) {
                if (parentData[String.format('deleted_{0}', dataMart.entity)] === undefined)
                    parentData[String.format('deleted_{0}', dataMart.entity)] = [];
                parentData[String.format('deleted_{0}', dataMart.entity)].push(data);
            }
            else {
                if (elementInstance.deleted === undefined)
                    elementInstance.deleted = [];
                elementInstance.deleted.push(data);
            }
            if (dataMart) {
                if (dataMart.children) {
                    // set all dataRows state = deleted
                    dataMart.children.forEach(child => {
                        if (data[child.entity]) {
                            //console.log('children', data[child.entity]);
                            data[child.entity].forEach(childData => {
                                // recursive deleting 
                                removeCascade(child.elementInstance, childData);
                            });
                        }
                    });
                }
            }
        }

        function modifyParentCascade(dataMartName, data) {
            var dataMart = self.dataMarts.get(dataMartName);
            var primaryKey = dataMart.primaryKey[0].name;
            var primaryKeyValue = data[primaryKey];
            var currentData = dataMart.elementInstance.dataSource.find(current => current[primaryKey] === primaryKeyValue);
            if (currentData && currentData.STATE === OneViewControl.EntityState.Unchanged)
                currentData.STATE = OneViewControl.EntityState.Modified;

            convertIsoDate(currentData);
            //console.log(data, currentData, primaryKey, primaryKeyValue, dataMart.elementInstance.dataSource);
            if (self.queryMode === OneViewControl.QueryMode.CLIENT) {
                if (dataMart.showToolbar) {
                    dataMart.toolbarInstance.enableItemByName(['Update', 'Cancel']);
                }
            }

            if (dataMart.parentMartName) {
                var foreignKey = (dataMart.foreignKey) ? dataMart.foreignKey[0].name : null;
                var foreignKeyValue = (foreignKey) ? data[foreignKey] : null;
                var parentDataMart = self.dataMarts.get(dataMart.parentMartName);
                var parentData = parentDataMart.elementInstance.dataSource.find(child => child[foreignKey] === foreignKeyValue);
                //console.log(dataMart.elementInstance, { foreignKey: foreignKey, foreignKeyValue: foreignKeyValue }, parentData);
                modifyParentCascade(dataMart.parentMartName, parentData);
            }
        }

        function mergeChangedData(changed) {
            //console.log(self.dataMarts, [...self.dataMarts.entries()], relations);
            changed.forEach(data => {
                var entityNames = Object.getOwnPropertyNames(data).filter(f => Array.isArray(data[f]) && f.startsWith('deleted_'));
                //console.log(entityNames);
                entityNames.forEach(entityName => {
                    var propName = entityName.substring(8);
                    if (data[propName].length > 0)
                        mergeChangedData(data[propName]);

                    data[propName] = [...data[propName], ...data[entityName]];
                    delete data[entityName];
                });
            });
            return changed;
        }

        function resumeChangedData(changed, entityName) {
            var deleted = [...changed.filter(d => d.STATE === OneViewControl.EntityState.Deleted)];
            for (var i = 0; i < deleted.length; i++) {
                var index = changed.indexOf(deleted[i]);
                changed.splice(index, 1);
            }
            //console.log(entityName, changed);
            changed.forEach(data => {
                data.STATE = OneViewControl.EntityState.Unchanged;
                var entityNames = Object.getOwnPropertyNames(data).filter(f => Array.isArray(data[f]));
                entityNames.forEach(entityName => {
                    if (data[entityName].length > 0)
                        resumeChangedData(data[entityName], entityName);
                });
            });
        }

        function convertIsoDate(data) {
            if (data instanceof Object) {
                for (let [key, value] of Object.entries(data)) {
                    if (value instanceof Date) {
                        data[key] = value.toISOString();
                    }
                }
            }
        }

        // Form Data Handle
        // -----------------------------------------------------------------------------
        // refer to formvalidation https://ej2.syncfusion.com/documentation/form-validator/validation-rules/?no-cache=1
        function setFormValidator(relation) {
            //console.log('form validator', relation);
            relation.toolTip = new ej.popups.Tooltip({
                //opensOn: 'custom',
                target: '.e-input, .e-input-group, .e-check',
                position: 'TopCenter',
                beforeRender: function (e) {
                    if (e.target.title === '') {
                        e.cancel = true;
                    }
                }
            });
            relation.toolTip.appendTo('#' + relation.targetFormId);
            relation.formValidator = new ej.inputs.FormValidator('#' + relation.targetFormId, {
                //locale : 'zh-Hant',
                validationBegin: e => { //formElement, param (error message), value
                    if (____show_validation) console.log('validation begin', e);
                },
                customPlacement: (inputElement, errorElement) => {
                    if (____show_validation) console.log('custom placement', inputElement, errorElement);
                },
                validationComplete: e => {  //errorElement, inputName, message, status
                    if (____show_validation) console.log('validation complete', e);
                    // workround https://ej2.syncfusion.com/documentation/tooltip/getting-started/?no-cache=1#initialize-tooltip-within-a-container
                    if (e.status === 'failure') {
                        if (e.element.classList.contains('e-ddl-hidden')) {
                            e.element.parentElement.title = e.message;
                            e.element.parentElement.classList.add('e-error');
                        }
                        else {
                            //console.log(e.element);
                            e.element.title = e.message;
                            e.element.classList.add('e-error');
                        }
                    }
                    else {
                        if (e.element.classList.contains('e-ddl-hidden')) {
                            e.element.parentElement.title = '';
                            e.element.parentElement.classList.remove('e-error');
                        }
                        else {
                            e.element.classList.remove('e-error');
                            e.element.title = '';
                        }
                    }
                },
                change: e => {
                    if (____show_validation) console.log('dropdown list changed', e);
                    if (relation.elementInstance.dataSet === undefined) return;
                    var fieldValue = relation.elementInstance.dataSet[e.target.name];
                    var isValid = true;
                    if (fieldValue !== e.target.value) {
                        isValid = relation.formValidator.validate(e.target.name);
                        updateEntityState(relation);
                        self.eventTarget.dispatchEvent({
                            type: 'fieldValueChanged',
                            name: 'selectChanged',
                            fieldName: e.target.name,
                            changedValue: e.target.value,
                            originalValue: fieldValue,
                            dataMart: relation,
                            isValid: isValid,
                            eventArgs: e
                        });
                    }
                },
                click: e => {
                    if (____show_validation) console.log('checkbox clicked');
                    if (relation.elementInstance.dataSet === undefined) return;
                    var fieldValue = relation.elementInstance.dataSet[e.target.name];
                    var isValid = true;
                    if (fieldValue !== e.target.checked) {
                        isValid = relation.formValidator.validate(e.target.name);
                        updateEntityState(relation);
                        self.eventTarget.dispatchEvent({
                            type: 'fieldValueChanged',
                            name: 'checkBoxChanged',
                            fieldName: e.target.name,
                            changedValue: e.target.checked,
                            originalValue: fieldValue,
                            dataMart: relation,
                            isValid: isValid,
                            eventArgs: e
                        });
                    }
                },
                keyup: e => {
                    if (____show_validation) console.log('form element keyup', e);
                    if (relation.elementInstance.dataSet === undefined) return;
                    var fieldValue = relation.elementInstance.dataSet[e.target.name];
                    var isValid = true;
                    //console.log(fieldValue, e.target.value, fieldValue !== e.target.value, e);
                    if (fieldValue !== e.target.value) {
                        isValid = relation.formValidator.validate('#' + e.target.name);
                        updateEntityState(relation);
                        self.eventTarget.dispatchEvent({
                            type: 'fieldValueChanged',
                            name: 'textBoxChanged',
                            fieldName: e.target.name,
                            changedValue: e.target.value,
                            originalValue: fieldValue,
                            dataMart: relation,
                            isValid: isValid,
                            eventArgs: e
                        });
                    }
                },
                focusout: e => {
                    if (____show_validation) console.log('form element focusout', e);
                    if (relation.elementInstance.dataSet === undefined) return;
                    var fieldValue = relation.elementInstance.dataSet[e.target.name];
                    var elementValueType = typeof fieldValue;
                    var isValid = true;
                    var isChanged = false;
                    if (fieldValue instanceof Date) {
                        isChanged = fieldValue.getTime() !== new Date(e.target.value).getTime();
                    }
                    if (isChanged) {
                        isValid = relation.formValidator.validate(e.target.name);
                        updateEntityState(relation);
                        self.eventTarget.dispatchEvent({
                            type: 'fieldValueChanged',
                            name: 'controlFocusOut',
                            fieldName: e.target.name,
                            changedValue: e.target.value,
                            originalValue: fieldValue,
                            dataMart: relation,
                            isValid: isValid,
                            eventArgs: e
                        });
                    }
                }
            });

            function updateEntityState(dataMart) {
                if (dataMart.elementInstance.dataSet.STATE === OneViewControl.EntityState.Unchanged) {
                    dataMart.elementInstance.dataSet.STATE = OneViewControl.EntityState.Modified;
                }
                if (dataMart.toolbarInstance && dataMart.showToolbar) {
                    dataMart.toolbarInstance.enableItemByName(['Update', 'Cancel']);
                    dataMart.toolbarInstance.disableItemByName(['Add']);
                }
            }

            //relation.formValidator.reset = function () {
            //    console.log(this);
            //};
            relation.formValidator.dataBind();
        }

        function setFormDataBinding(dataMart, rowIndex) {
            if (rowIndex === undefined)
                rowIndex = rowIndex || 0;
            var dataSet;
            if (rowIndex > -1) {
                if (Array.isArray(dataMart.elementInstance.dataSource)) {           // queryMode==CLIENT
                    dataSet = dataMart.elementInstance.dataSource[rowIndex];
                }
                else if (Array.isArray(dataMart.elementInstance.currentViewData)) { // queryMode==SERVER
                    dataSet = dataMart.elementInstance.currentViewData[rowIndex];
                }
                dataMart.elementInstance.dataSet = dataSet;
            } else {
                dataSet = dataMart.elementInstance.dataSet;
            }

            if (dataMart.formFieldInstances && dataSet) {
                for (const [field, elementInstance] of dataMart.formFieldInstances) {
                    try {
                        if (dataSet[field] !== undefined) {
                            elementInstance.value = dataSet[field];
                            document.getElementById(field).value = dataSet[field];
                            if (elementInstance.element.classList.contains('e-dropdownlist')) {
                                //elementInstance.dataBind();

                            }
                            if (elementInstance.element.classList.contains('e-checkbox')) {
                                elementInstance.checked = dataSet[field];
                            }
                            elementInstance.dataBind();
                            // textbox 另外處理
                        }
                    }
                    catch (e) { console.log(e.message); }
                }
                if (dataMart.formValidator) {
                    dataMart.formValidator.validate();
                }
            }
        }

        this.setFormDataBinding = function (dataMart, rowIndex) {
            setFormDataBinding(dataMart, rowIndex);
        };

        function setFormDataValue(dataMart, rowIndex) {
            if (rowIndex === undefined)
                rowIndex = rowIndex || 0;
            var dataSet;

            // rowIndex == -1 menas addding row
            if (rowIndex > -1) {
                if (Array.isArray(dataMart.elementInstance.dataSource)) {
                    dataSet = dataMart.elementInstance.dataSource[rowIndex];
                }
                else if (Array.isArray(dataMart.elementInstance.currentViewData)) {
                    dataSet = dataMart.elementInstance.currentViewData[rowIndex];
                }
            }
            else {
                dataSet = dataMart.elementInstance.dataSet;
            }
            for (const [field, elementInstance] of dataMart.formFieldInstances) {
                //console.log(field, elementInstance.value);
                dataSet[field] = elementInstance.value instanceof Date ? elementInstance.value.toISOString() : elementInstance.value;
                if (elementInstance.element.classList.contains('e-checkbox')) {
                    dataSet[field] = elementInstance.checked;
                }
                if (elementInstance.element.classList.contains('e-dropdownlist')) {
                    //elementInstance.dataBind();
                }
            }
            if (dataSet.STATE === OneViewControl.EntityState.Unchanged) {
                dataSet.STATE = OneViewControl.EntityState.Modified;
            }
            convertIsoDate(dataSet);
            dataMart.elementInstance.dataSet = dataSet;
            return dataMart.elementInstance.dataSet;
        }

        this.setFormDataValue = function (dataMart, rowIndex) {
            //console.log('setFormDataValue', dataMart, rowIndex);
            return setFormDataValue(dataMart, rowIndex);
        };

        function formFieldValueChanged(e) {
            if (this.element.classList.contains('e-checkbox')) {
                this.value = this.checked;
            }
        }

        this.extFormDataBinding = function (dataMart, formId, rowIndex) {
            //console.log('extBinding', dataMart);
            rowIndex = rowIndex || 0;
            var ej2_instances;
            if (dataMart) {
                dataMart.targetFormId = formId;
                dataMart.targetFormElement = document.getElementById(dataMart.targetFormId);
                if (dataMart.targetFormElement) {
                    var formElements = $(dataMart.targetFormElement).find('input, input + textarea, textarea, select');
                    dataMart.formFieldInstances = new Map();
                    formElements.each((index, element) => {
                        try {
                            ej2_instances = document.getElementById(element.id).ej2_instances;
                            if (ej2_instances) {
                                instance = ej2_instances[0];
                                dataMart.formFieldInstances.set(element.id, instance);
                                instance.addEventListener('change', formFieldValueChanged);
                            }
                        }
                        catch (e) { /*console.info(e.message);*/ }
                    });

                    // set foem field value 
                    if (Array.isArray(dataMart.elementInstance.dataSource))
                        dataMart.elementInstance.dataSet = dataMart.elementInstance.dataSource[rowIndex];
                    setFormDataBinding(dataMart, rowIndex);

                    // set foem validation for form element
                    setFormValidator(dataMart);

                    // validate form if exists
                    if (dataMart.formValidator)
                        dataMart.formValidator.validate();
                }
            }
        };

        this.extUpdateForm = function (dataMart, rowIndex) {
            rowIndex = rowIndex || 0;
            setFormDataValue(dataMart, rowIndex);
            //console.log(dataMart.elementInstance.dataSet);
            self.updateForm();
        };
        // -----------------------------------------------------------------------------

    },

    EventTarget: function () {

        this._init = function () {
            this._registrations = {};
        };

        this._getListeners = function (type, useCapture) {
            var captype = (useCapture ? '1' : '0') + type;
            if (!(captype in this._registrations))
                this._registrations[captype] = [];
            return this._registrations[captype];
        };

        this.addEventListener = function (type, listener, useCapture) {
            var listeners = this._getListeners(type, useCapture);
            var ix = listeners.indexOf(listener);
            if (ix === -1)
                listeners.push(listener);
        };

        this.removeEventListener = function (type, listener, useCapture) {
            var listeners = this._getListeners(type, useCapture);
            var ix = listeners.indexOf(listener);
            if (ix !== -1)
                listeners.splice(ix, 1);
        };

        this.dispatchEvent = function (evt) {
            var listeners = this._getListeners(evt.type, false).slice();
            for (var i = 0; i < listeners.length; i++)
                listeners[i].call(this, evt);
            return !evt.defaultPrevented;
        };

        this._init();
    },

    DataEventArgs: function () {

    },

    ViewType: {
        SINGLE_FORM: 1,
        SINGLE_GRID: 2,
        FORM_GRID: 3,
        GRID_FORM: 4,
        GRID_GRID: 5,
        properties: {
            1: { name: "SINGLE_FORM", value: 1, desc: "SINGLE_FORM" },
            2: { name: "SINGLE_GRID", value: 2, desc: "SINGLE_GRID" },
            3: { name: "FORM_GRID", value: 3, desc: "FORM_GRID" },
            4: { name: "GRID_FORM", value: 3, desc: "GRID_FORM" },
            5: { name: "GRID_GRID", value: 3, desc: "GRID_GRID" }
        }
    },

    QueryMode: {
        SERVER: 1,
        CLIENT: 2,
        properties: {
            1: { name: "Server", value: 1, desc: "Server" },
            2: { name: "Client", value: 2, desc: "Client" }
        }
    },

    ButtonState: {
        New: 1,
        Update: 2,
        Delete: 3,
        Cancel: 4,
        Initial: 81,
        Modifiled: 82,
        DisableAll: 91,
        EnableAll: 92
    },

    EntityState: {
        Unchanged: 2,
        Added: 4,
        Deleted: 8,
        Modified: 16
    }
};


function setViewTitle(title) {
    var element = document.querySelector('.ki-view-caption');
    if (element) {
        element.setAttribute('data-content', title);
    }
}

String.format = function (src) {
    if (arguments.length === 0) return null;
    var args = Array.prototype.slice.call(arguments, 1);
    return src.replace(/\{(\d+)\}/g, function (m, i) {
        return args[i];
    });
};

