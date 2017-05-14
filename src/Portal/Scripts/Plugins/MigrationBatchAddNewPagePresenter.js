Microsoft.WebPortal.MigrationBatchAddNewPagePresenter = function (webPortal, feature, context) {
    /// <summary>
    /// Provides a mechanism for adding new environments. 
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    this.base.constructor.call(this, webPortal, feature, "Home", "/Template/MigrationBatchAddNew/");

    this.addMailboxesView = new Microsoft.WebPortal.Views.AddMailboxesView(webPortal, "#AddMailboxesViewContainer", context);
    this.migrationBatchProfileView = new Microsoft.WebPortal.Views.NewMigrationBatchProfileView(webPortal, "#MigrationBatchProfileContainer");

    this.onCancelClicked = function () {
        webPortal.Journey.retract();
    };

    this.context = context;
    this.isPosting = false;

    var self = this;

    this.getMailboxes = function () {
        var mailboxes = [];

        for (var i in this.addMailboxesView.mailboxesList.rows()) {
            mailboxes.push({
                DisplayName: this.addMailboxesView.mailboxesList.rows()[i].mailbox.DisplayName,
                Guid: this.addMailboxesView.mailboxesList.rows()[i].mailbox.Guid,
                Name: this.addMailboxesView.mailboxesList.rows()[i].mailbox.Name,
                PrimarySmtpAddress: this.addMailboxesView.mailboxesList.rows()[i].mailbox.PrimarySmtpAddress,
                SamAccountName: this.addMailboxesView.mailboxesList.rows()[i].mailbox.SamAccountName,
                UserPrincipalName: this.addMailboxesView.mailboxesList.rows()[i].mailbox.UserPrincipalName
            });
        }
        return mailboxes;
    };

    this.getMigrationBatchInformation = function () {
        var migrationBatchInformation = {
            EnvironmentId: this.context.Id,
            Mailboxes: this.getMailboxes(),
            Name: this.migrationBatchProfileView.viewModel.Name(),
            StartTime: this.migrationBatchProfileView.viewModel.StartTime(),
            TargetDeliveryDomain: this.migrationBatchProfileView.viewModel.TargetDeliveryDomain()
        };

        return migrationBatchInformation;
    };

    this.onFormSubmit = function () {
        if (self.isPosting) {
            return;
        }

        if ($("#Form").valid()) {
            if (self.addMailboxesView.mailboxesList.rows().length <= 0) {
                self.webPortal.Services.Dialog.show("emptyMailboxesErrorMessage-template", {}, [
                    Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, "ok-Button", function () {
                        self.webPortal.Services.Dialog.hide();
                    })
                ]);

                return;
            }

            self.isPosting = true;

            var notification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Progress, self.webPortal.Resources.Strings.Plugins.AddMigrationBatch.PreparingBatchMessage);
            self.webPortal.Services.Notifications.add(notification);

            new Microsoft.WebPortal.Utilities.RetryableServerCall(self.webPortal.Helpers.ajaxCall("api/migrationbatch",
                Microsoft.WebPortal.HttpMethod.Post,
                self.getMigrationBatchInformation(),
                Microsoft.WebPortal.ContentType.Json, 120000),
                "CreateMigrationBatch", []).execute()
                .done(function (result) {

                    // turn the notification into a success
                    notification.type(Microsoft.WebPortal.Services.Notification.NotificationType.Success);
                    notificationMessage = self.webPortal.Resources.Strings.Plugins.AddMigrationBatch.BatchRegistrationSuccessMessage + " - " + result.Name + " (" + result.Id + ")";
                    notification.message(notificationMessage);
                    notification.buttons([
                        Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                            notification.dismiss();
                        })
                    ]);

                    self.webPortal.Journey.start(Microsoft.WebPortal.Feature.MigrationBatch, result);
                })
                .fail(function (result, status, error) {
                    notification.type(Microsoft.WebPortal.Services.Notification.NotificationType.Error);
                    notification.buttons([
                        Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                            notification.dismiss();
                        })
                    ]);

                    var errorPayload = JSON.parse(result.responseText);

                    if (errorPayload) {
                        switch (errorPayload.ErrorCode) {
                            case Microsoft.WebPortal.ErrorCode.InvalidInput:
                                notification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.InvalidInputErrorPrefix + errorPayload.Details.ErrorMessage);
                                break;
                            case Microsoft.WebPortal.ErrorCode.DownstreamServiceError:
                                notification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.DownstreamErrorPrefix + errorPayload.Details.ErrorMessage);
                                break;
                            default:
                                notification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.CustomerRegistrationFailureMessage);
                                break;
                        }
                    } else {
                        notification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.CustomerRegistrationFailureMessage);
                    }
                })
                .always(function () {
                    self.isPosting = false;
                });
        }
    };
};

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.MigrationBatchAddNewPagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.MigrationBatchAddNewPagePresenter.prototype.onActivate = function () {
    /// <summary>
    /// Called when the presenter is activated.
    /// </summary>
};

Microsoft.WebPortal.MigrationBatchAddNewPagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is about to be rendered.
    /// </summary>

    ko.applyBindings(this, $("#Form")[0]);

    this.addMailboxesView.render();
    this.migrationBatchProfileView.render();
};

Microsoft.WebPortal.MigrationBatchAddNewPagePresenter.prototype.onShow = function () {
    /// <summary>
    /// Called when content is shown.
    /// </summary>

    this.addMailboxesView.show();
    this.migrationBatchProfileView.show();
};

//@ sourceURL=MigrationBatchAddNewPagePresenter.js