Microsoft.WebPortal.MigrationBatchesPagePresenter = function (webPortal, feature, context) {
    /// <summary>
    /// Manages the migration batches page experience. 
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    this.base.constructor.call(this, webPortal, feature, "Home", "/Template/MigrationBatches/");

    var self = this;

    this.viewModel = {
        EnvironmentId: context.Id,
        IsSet: ko.observable(false),
        MigrationBatches: ko.observableArray([]),
        ShowProgress: ko.observable(true)
    };

    this.onAddClicked = function () {
        webPortal.Journey.advance(Microsoft.WebPortal.Feature.MigrationBatchAddNew, context);
    };

    this.onDeleteClicked = function (data) {
        var deletionPrompt = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Warning,
            webPortal.Resources.Strings.Plugins.HomePage.MigrationBatchDeleteMessage, [
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.YES, webPortal.Resources.Strings.Yes, function () {
                    deletionPrompt.dismiss();
                    self._deleteMigrationBatch(data);
                }),
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.NO, webPortal.Resources.Strings.No, function () {
                    deletionPrompt.dismiss();
                    deletionPrompt = null;
                })
            ]);

        webPortal.Services.Notifications.add(deletionPrompt);
    };

    this.onViewClicked = function (data) {
        var batchInfo = { EnvironmentId: data.EnvironmentId, Id: data.Id, Name: data.Name };
        webPortal.Journey.advance(Microsoft.WebPortal.Feature.MigrationBatch, batchInfo);
    };

    webPortal.Services.HeaderBar.resetBreadcrumbs();
    webPortal.Services.HeaderBar.addBreadcrumb(true, null, 'Migration Batches');
};

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.MigrationBatchesPagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.MigrationBatchesPagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is rendered but not shown yet.
    /// </summary>

    var self = this;
    ko.applyBindings(self, $("#MigrationBatchesContainer")[0]);

    var getMigrationBatches = function () {
        var getMigrationBatchesServerCall = self.webPortal.ServerCallManager.create(
            self.feature, self.webPortal.Helpers.ajaxCall("api/migrationbatch?environmentId=" + self.viewModel.EnvironmentId, Microsoft.WebPortal.HttpMethod.Get), "GetMigrationBatches");

        self.viewModel.IsSet(false);
        self.viewModel.ShowProgress(true);

        getMigrationBatchesServerCall.execute().done(function (value) {
            self.viewModel.MigrationBatches(value.MigrationBatches);

            self.viewModel.IsSet(true);
        }).fail(function (result, status, error) {
            var notification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Error,
                "Failed to retrieve migration batches...");

            notification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.RETRY, self.webPortal.Resources.Strings.Retry, function () {
                    notification.dismiss();

                    // retry
                    getMigrationBatches();
                }),
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.CANCEL, self.webPortal.Resources.Strings.Cancel, function () {
                    notification.dismiss();
                })
            ]);

            self.webPortal.Services.Notifications.add(notification);
        }).always(function () {
            // stop showing progress
            self.viewModel.ShowProgress(false);
        });
    };

    getMigrationBatches();
};

Microsoft.WebPortal.MigrationBatchesPagePresenter.prototype._deleteMigrationBatch = function (data) {
    /// <summary>
    /// Deletes the migration batch represented by the data parameter.
    /// </summary>

    var self = this;

    var deleteMigrationBatchServerCall = self.webPortal.ServerCallManager.create(self.feature,
        self.webPortal.Helpers.ajaxCall("api/migrationbatch/delete", Microsoft.WebPortal.HttpMethod.Post, data, Microsoft.WebPortal.ContentType.Json), "DeleteMigrationBatch");

    var deletionNotification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Progress,
        self.webPortal.Resources.Strings.Plugins.HomePage.DeletingMigrationBatchMessage);

    self.webPortal.Services.Notifications.add(deletionNotification);

    var deleteMigrationBatch = function () {
        deleteMigrationBatchServerCall.execute().done(function (value) {
            self.viewModel.MigrationBatches(value.MigrationBatches);

            deletionNotification.type(Microsoft.WebPortal.Services.Notification.NotificationType.Success);
            deletionNotification.message(self.webPortal.Resources.Strings.Plugins.HomePage.MigrationDeletionConfirmation);
            deletionNotification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                    deletionNotification.dismiss();
                })
            ]);
        }).fail(function (result, status, error) {
            self.webPortal.Helpers.displayRetryCancelErrorNotification(deletionNotification, self.webPortal.Resources.Strings.Plugins.HomePage.MigrationBatchDeletionFailure,
                self.webPortal.Resources.Strings.Plugins.HomePage.DeletingEnvironment, deleteMigrationBatch, function () {
                    //
                });
        });
    };

    deleteMigrationBatch();
};

//@ sourceURL=MigrationBatchesPagePresenter.js