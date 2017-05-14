Microsoft.WebPortal.MigrationBatchPagePresenter = function (webPortal, feature, context) {
    /// <summary>
    /// Manages the migration batch page experience. 
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    this.base.constructor.call(this, webPortal, feature, "Home", "/Template/MigrationBatch/");

    var self = this;

    this.viewModel = {
        EnvironmentId: context.EnvironmentId,
        Id: context.Id,
        IsSet: ko.observable(false),
        Mailboxes: ko.observableArray([]),
        ShowProgress: ko.observable(true)
    };

    this.onRefreshClicked = function () {

    };

    var migrationBatchesNavigation = function () {
        var info = { Id: self.viewModel.EnvironmentId }
        self.webPortal.Journey.advance(Microsoft.WebPortal.Feature.MigrationBatches, info);
    };

    webPortal.Services.HeaderBar.resetBreadcrumbs();
    webPortal.Services.HeaderBar.addBreadcrumb(false, migrationBatchesNavigation, 'Migration Batches');
    webPortal.Services.HeaderBar.addBreadcrumb(true, null, context.Name);
};

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.MigrationBatchPagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.MigrationBatchPagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is rendered but not shown yet.
    /// </summary>

    var self = this;
    ko.applyBindings(self, $("#MigrationBatchContainer")[0]);

    var getMigrationBatch = function () {
        var getMigrationBatchServerCall = self.webPortal.ServerCallManager.create(
            self.feature, self.webPortal.Helpers.ajaxCall(
                "api/migrationbatch/details?environmentId=" + self.viewModel.EnvironmentId + "&batchId=" + self.viewModel.Id,
                Microsoft.WebPortal.HttpMethod.Get), "GetMigrationBatches");

        self.viewModel.IsSet(false);
        self.viewModel.ShowProgress(true);

        getMigrationBatchServerCall.execute().done(function (value) {
            self.viewModel.Mailboxes(value.Mailboxes);

            self.viewModel.IsSet(true);
        }).fail(function (result, status, error) {
            var notification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Error,
                "Failed to retrieve the migration batch details...");

            notification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.RETRY, self.webPortal.Resources.Strings.Retry, function () {
                    notification.dismiss();

                    // retry
                    getMigrationBatch();
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

    getMigrationBatch();
};

//@ sourceURL=MigrationBatchPagePresenter.js