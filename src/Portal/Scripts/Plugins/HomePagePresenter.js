Microsoft.WebPortal.HomePagePresenter = function (webPortal, feature) {
    /// <summary>
    /// Manages the home page experience. 
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    this.base.constructor.call(this, webPortal, feature, "Home", "/Template/Home/");

    var self = this;

    this.viewModel = {
        Environments: ko.observableArray([]),
        ShowProgress: ko.observable(true),
        IsSet: ko.observable(false)
    };

    this.onAddEnvrionmentClicked = function () {
        webPortal.Journey.advance(Microsoft.WebPortal.Feature.EnvironmentAddNew);
    };

    this.onBatchesClicked = function (data) {
        var environmentInfo = { Id: data.Id };
        // Activate the migration batch page presenter and pass it the selected environment.
        webPortal.Journey.advance(Microsoft.WebPortal.Feature.MigrationBatches, environmentInfo);
    };

    this.onDeleteClicked = function (data) {
        var deletionPrompt = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Warning,
            webPortal.Resources.Strings.Plugins.HomePage.DeleteEnvrionmentMessage, [
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.YES, webPortal.Resources.Strings.Yes, function () {
                    deletionPrompt.dismiss();
                    self._deleteEnvironment(data);
                }),
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.NO, webPortal.Resources.Strings.No, function () {
                    deletionPrompt.dismiss();
                    deletionPrompt = null;
                })
            ]);

        webPortal.Services.Notifications.add(deletionPrompt);
    };

    this.onRefreshClicked = function (data) {
        self._refreshEnvironment(data);
    };

    webPortal.Services.HeaderBar.resetBreadcrumbs();
    webPortal.Services.HeaderBar.addBreadcrumb(true, null, 'Home');
};

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.HomePagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.HomePagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is rendered but not shown yet.
    /// </summary>

    var self = this;
    ko.applyBindings(self, $("#EnvironmentsContainer")[0]);

    var getEnvironments = function () {
        var getEnvironmentsServerCall = self.webPortal.ServerCallManager.create(
            self.feature, self.webPortal.Helpers.ajaxCall("api/environment", Microsoft.WebPortal.HttpMethod.Get), "GetEnvironments");

        self.viewModel.IsSet(false);
        self.viewModel.ShowProgress(true);

        getEnvironmentsServerCall.execute().done(function (value) {
            self.viewModel.Environments(value.Environments);

            self.viewModel.IsSet(true);
        }).fail(function (result, status, error) {
            var notification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Error,
                self.webPortal.Resources.Strings.Plugins.HomePage.EnvironmentRetrievalFailure);

            notification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.RETRY, self.webPortal.Resources.Strings.Retry, function () {
                    notification.dismiss();

                    // retry
                    getEnvironments();
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

    getEnvironments();
};

Microsoft.WebPortal.HomePagePresenter.prototype._deleteEnvironment = function (data) {
    /// <summary>
    /// Deletes the environment represented by the data parameter.
    /// </summary>

    var self = this;

    var deleteEnvironmentServerCall = self.webPortal.ServerCallManager.create(self.feature,
        self.webPortal.Helpers.ajaxCall("api/environment/delete", Microsoft.WebPortal.HttpMethod.Post, data, Microsoft.WebPortal.ContentType.Json), "DeleteEnvironment");

    var deletionNotification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Progress,
        self.webPortal.Resources.Strings.Plugins.HomePage.DeletingEnvironmentMessage);

    self.webPortal.Services.Notifications.add(deletionNotification);

    var deleteEnvironment = function () {
        deleteEnvironmentServerCall.execute().done(function (value) {
            self.viewModel.Environments(value.Environments);

            deletionNotification.type(Microsoft.WebPortal.Services.Notification.NotificationType.Success);
            deletionNotification.message(self.webPortal.Resources.Strings.Plugins.HomePage.EnvironmentDeletionConfirmation);
            deletionNotification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                    deletionNotification.dismiss();
                })
            ]);
        }).fail(function (result, status, error) {
            self.webPortal.Helpers.displayRetryCancelErrorNotification(deletionNotification, self.webPortal.Resources.Strings.Plugins.HomePage.EnvironmentDeletionFailure,
                self.webPortal.Resources.Strings.Plugins.HomePage.DeletingEnvironment, deleteEnvironment, function () {
                    //
                });
        });
    };

    deleteEnvironment();
};

Microsoft.WebPortal.HomePagePresenter.prototype._refreshEnvironment = function (data) {
    var self = this;

    var message = self.webPortal.Resources.Strings.Plugins.HomePage.EnvironmentRefreshingMessage + " " + data.Name + "...";

    var notification = new Microsoft.WebPortal.Services.Notification(
        Microsoft.WebPortal.Services.Notification.NotificationType.Progress,
        message);

    var refreshEnvironmentServerCall = self.webPortal.ServerCallManager.create(
        self.feature, self.webPortal.Helpers.ajaxCall("api/environment/refresh?environmentId=" + data.Id, Microsoft.WebPortal.HttpMethod.Get), "RefreshEvironment");

    self.webPortal.Services.Notifications.add(notification);

    var refreshEnvironment = function () {
        refreshEnvironmentServerCall.execute().done(function () {
            notification.type(Microsoft.WebPortal.Services.Notification.NotificationType.Success);
            notification.message(self.webPortal.Resources.Strings.Plugins.HomePage.EnvironmentRefreshConfirmation);
            notification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                    notification.dismiss();
                })
            ]);
        }).fail(function (result, status, error) {
            self.webPortal.Helpers.displayRetryCancelErrorNotification(
                notification,
                self.webPortal.Resources.Strings.Plugins.HomePage.EnvironmentRefreshFailure,
                message, refreshEnvironment, function () {
                    //
                });
        });
    };

    refreshEnvironment();
};

//@ sourceURL=HomePagePresenter.js